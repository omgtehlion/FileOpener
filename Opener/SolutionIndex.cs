using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace FileOpener.Opener
{
    using SubItem = Tuple<string, int, ItemInfo>;

    public sealed partial class SolutionIndex
    {
        private List<ItemInfo> Index = new List<ItemInfo>();
        public string SolutionName { get; private set; }
        public readonly IconCache IconCache = new IconCache();

        public int Count
        {
            get { return Index.Count; }
        }

        public bool Ready { get; private set; }

        public SolutionIndex(string slnName)
        {
            Ready = false;
            SolutionName = slnName;
            var hash = CalcHash(slnName.ToLowerInvariant());
            CachePath = Path.Combine(SettingsPath, hash + ".bin");
        }

        public void Reindex(IVsSolution solution, Action<int> onIndexed)
        {
            var hierarchy = solution as IVsHierarchy;

            if (hierarchy != null) {
                var index = new List<ItemInfo>();

                Func<ItemInfo, bool> condition = item => !string.IsNullOrEmpty(item.Name)
                                                      && !string.IsNullOrEmpty(item.Path)
                                                      && !item.Path.EndsWith(@"\")
                                                      && !item.Path.EndsWith("/");

                foreach (var w in EnumHierarchyItems(hierarchy, VSConstants.VSITEMID_ROOT, 0, "", null).Where(condition)) {
                    index.Add(w);
                    onIndexed(index.Count);
                }
                Index = index;
                CreateSubIndexes();
                Ready = true;
            }

            SaveCache();
        }

        private IEnumerable<ItemInfo> EnumHierarchyItems(IVsHierarchy hierarchy, uint itemId, int level, string project, TinyList<string> logicalPath)
        {
            var currentName = hierarchy.GetProp<string>(itemId, __VSHPROPID.VSHPROPID_Name);

            // Enumerate nested
            var nested = hierarchy.GetNested(itemId);
            if (nested != null) {
                if (level == 1) {
                    project = currentName;
                }
                foreach (var item in EnumHierarchyItems(nested.Item1, nested.Item2, level, project, logicalPath)) {
                    yield return item;
                }
                yield break;
            }

            if (SkipItem(currentName)) {
                yield break;
            }

            yield return new ItemInfo {
                Name = currentName,
                ProjectName = project,
                IconIndex = IconCache.Add(hierarchy, itemId),
                LogicalPath = logicalPath.Foldr(new StringBuilder(), (s, b) => b.Append('/')
                    .Append(s)).Append('/').Append(currentName).ToString(),
                Path = (level > 0 && itemId != VSConstants.VSITEMID_NIL && itemId != VSConstants.VSITEMID_ROOT) 
                    ? hierarchy.GetCanonicalName(itemId) : null,
            };

            // Enumerate normal children
            var hasSideEffects = hierarchy.GetProp<object>(itemId, __VSHPROPID.VSHPROPID_HasEnumerationSideEffects);
            if (hasSideEffects as bool? == true) {
                // Enumerate slow
                var path = hierarchy.GetCanonicalName(itemId).TrimEnd('/', '\\');
                if (Directory.Exists(path)) {
                    if (!SkipItem(currentName)) {
                        foreach (var item in EnumDirectory(path, project, logicalPath)) {
                            yield return item;
                        }
                    }
                } else {
                    // TODO: notify user somehow
                }
            } else {
                // Enumerate fast
                // maybe use __VSHPROPID.VSHPROPID_FirstVisibleChild ?
                var node = hierarchy.GetProp<object>(itemId, __VSHPROPID.VSHPROPID_FirstChild);
                if (level > 1) {
                    logicalPath = logicalPath.Cons(currentName);
                }

                while (node != null) {
                    var nodeId = (uint)(int)node;
                    if (nodeId == VSConstants.VSITEMID_NIL) {
                        break;
                    }
                    foreach (var item in EnumHierarchyItems(hierarchy, nodeId, level + 1, project, logicalPath)) {
                        yield return item;
                    }
                    node = hierarchy.GetProp<object>(nodeId, __VSHPROPID.VSHPROPID_NextSibling);
                }
            }
        }

        private IEnumerable<ItemInfo> EnumDirectory(string path, string project, TinyList<string> logicalPath)
        {
            logicalPath = logicalPath.Cons(Path.GetFileName(path));
            foreach (var f in Directory.GetFiles(path)) {
                var name = Path.GetFileName(f);
                yield return new ItemInfo {
                    Name = name,
                    Path = f,
                    ProjectName = project,
                    IconIndex = IconCache.AddFile(f),
                    LogicalPath = logicalPath.Foldr(new StringBuilder(), (s, b) => b.Append('/').Append(s)).Append('/').Append(name).ToString(),
                };
            }
            foreach (var item in Directory.GetDirectories(path).Where(d => !SkipItem(Path.GetFileName(path)))
                .SelectMany(d => EnumDirectory(d, project, logicalPath))) {
                yield return item;
            }
        }

        public List<ItemInfo> Query(string query)
        {
            if (Index == null) {
                return new List<ItemInfo>();
            }

            if (!query.All(Char.IsLetter)) {
                return PlainQuery(query, false);
            }

            var regex = new Regex("^" + Regex.Escape(query), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var result = AbbrIndex.Where(i => regex.IsMatch(i.Item1))
                .Concat(WordIndex.Where(i => regex.IsMatch(i.Item1)))
                .OrderBy(i => i.Item2)
                .ThenBy(i => i.Item3.Name, MyPathComparer)
                .ThenBy(i => i.Item3.Path, MyPathComparer)
                .Select(i => i.Item3)
                .ToList();

            if (result.Count > 0) {
                return result;
            }

            return PlainQuery(query, true);
        }

        public List<ItemInfo> PlainQuery(string query, bool sortByPos)
        {
            var tmp = new StringBuilder();
            foreach (var t in query) {
                switch (t) {
                    case '?':
                        tmp.Append('.');
                        break;
                    case '*':
                        tmp.Append(".*");
                        break;
                    default:
                        tmp.Append(Regex.Escape(t.ToString()));
                        break;
                }
            }
            query = tmp.ToString();

            var regex = new Regex(query, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var filtered = Index.Select(i => Tuple.Create(regex.Match(i.Name), i))
                .Where(i => i.Item1.Success);

            filtered = sortByPos
                ? filtered.OrderBy(i => i.Item1.Index).ThenBy(i => i.Item2.Name, MyPathComparer).ThenBy(i => i.Item2.Path, MyPathComparer)
                : filtered.OrderBy(i => i.Item2.Name, MyPathComparer).ThenBy(i => i.Item2.Path, MyPathComparer);

            return filtered.Select(i => i.Item2).ToList();
        }

        private sealed class PathComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x.Length == y.Length) {
                    return x.CompareTo(y);
                }
                return x.Length - y.Length;
            }
        }

        private readonly PathComparer MyPathComparer = new PathComparer();

        private static bool SkipItem(string name)
        {
            return name == null
                || name == ".git"
                || name == ".svn"
                || name == ".plastic"
                || name.EndsWith(".user", StringComparison.OrdinalIgnoreCase)
                ;
        }

        public static string GetSlnFileName(IVsSolution solution)
        {
            return solution.GetProp<string>(__VSPROPID.VSPROPID_SolutionFileName);
        }

        #region some magic

        List<SubItem> AbbrIndex = new List<SubItem>();
        List<SubItem> WordIndex = new List<SubItem>();

        private static string GetAbbr(string name)
        {
            return new string(GetWordBegins(name).Select(i => name[i]).ToArray());
        }

        private static IList<int> GetWordBegins(string name)
        {
            var result = new List<int>();
            var mustBeStart = true;
            for (var i = 0; i < name.Length; i++) {
                var c = name[i];
                if (mustBeStart) {
                    if (Char.IsLetter(c)) {
                        result.Add(i);
                        mustBeStart = false;
                    }
                } else {
                    if (Char.IsLetter(c)) {
                        if (Char.IsUpper(c)) {
                            result.Add(i);
                        }
                    } else {
                        mustBeStart = true;
                    }
                }
            }
            return result;
        }

        private static IEnumerable<string> GetWords(string name)
        {
            var begins = GetWordBegins(name);
            var ends = begins.Skip(1).Concat(new[] { name.Length }).ToList();

            return begins.Zip(ends, (begin, end) => {
                var i = begin;
                for (; i < end; i++) {
                    if (!Char.IsLetter(name[i]))
                        break;
                }
                return name.Substring(begin, i - begin);
            }).ToList();
        }

        private static int Priority(int type, int topPriority, int lowPriority)
        {
            return (type << 16) +
                (Math.Min(0xFF, topPriority) << 8) +
                (Math.Min(0xFF, lowPriority));
        }

        private void CreateSubIndexes()
        {
            Func<string, string> trimExt = Path.GetFileNameWithoutExtension;

            AbbrIndex = Index.Select(i => {
                var abbr = GetAbbr(trimExt(i.Name));
                return new SubItem(abbr.ToUpperInvariant(), Priority(0, abbr.Length, i.Name.Length), i);
            }).Where(i => i.Item1.Length >= 2).ToList();

            WordIndex = Index.SelectMany(i => {
                var idx = 0;
                var name = trimExt(i.Name);
                return GetWords(name).Select(w => new SubItem(w, Priority(1, idx++, name.Length), i));
            }).ToList();
        }

        #endregion
    }
}