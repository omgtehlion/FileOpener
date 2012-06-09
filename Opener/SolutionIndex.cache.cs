using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileOpener.Opener
{
    partial class SolutionIndex
    {
        const string SerializationKey = "{BFBD11A1-FBF4-43DD-AADA-813268481BFE}";
        private static readonly string SettingsPath = Path.Combine(
           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
           Path.Combine("xmm", "QuickOpenFile"));

        private readonly string CachePath;

        #region caching

        private static string CalcHash(string path)
        {
            var result = new StringBuilder();
            using (var sha = new SHA1CryptoServiceProvider()) {
                foreach (var b in sha.ComputeHash(Encoding.Unicode.GetBytes(path)))
                    result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }

        private void DeleteCacheFile()
        {
            File.Delete(CachePath);
            if (File.Exists(CachePath)) {
                System.Threading.Thread.Sleep(10);
                File.Delete(CachePath);
            }
        }

        public bool LoadFromCache()
        {
            if (File.Exists(CachePath)) {
                try {
                    //var watch = Stopwatch.StartNew();
                    Deserialize(CachePath);
                    //Index = null;
                    //System.Windows.Forms.MessageBox.Show("Cache " + watch.ElapsedMilliseconds);
                    if (Index == null) {
                        DeleteCacheFile();
                    }
                    CreateSubIndexes();
                    Ready = Index != null;
                    return Ready;
                } catch {
                    DeleteCacheFile();
                }
            }

            return false;
        }

        private void SaveCache()
        {
            TraceDirecotry(CachePath);
            DeleteCacheFile();
            // save cache only for big projects
            if (Index.Count > 1000) {
                SerializeTo(CachePath);
            }
        }

        private static void TraceDirecotry(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                TraceDirecotry(dir);
                Directory.CreateDirectory(dir);
            }
        }

        private void SerializeTo(string path)
        {
            var data = Index;
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) {
                fs.SetLength(0);
                using (var writer = new BinaryWriter(fs, Encoding.UTF8)) {
                    writer.Write(SerializationKey);
                    writer.Write(data.Count);
                    foreach (var item in data) {
                        item.Serialize(writer);
                    }
                    writer.Flush();
                    IconCache.SerializeTo(fs);
                }
            }
        }

        private void Deserialize(string path)
        {
            var idx = new List<ItemInfo>();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(fs, Encoding.UTF8)) {
                var suuid = reader.ReadString();
                if (suuid != SerializationKey) {
                    throw new NotImplementedException();
                }
                var count = reader.ReadInt32();
                for (var i = 0; i < count; i++) {
                    idx.Add(ItemInfo.Deserialize(reader));
                }
                IconCache.Deserialize(fs);
            }
            Index = idx;
        }

        #endregion
    }
}