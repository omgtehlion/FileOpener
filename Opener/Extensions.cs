using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace FileOpener.Opener
{
    public static class Extensions
    {
        public static T GetProp<T>(this IVsHierarchy hierarchy, uint itemid, __VSHPROPID propid)
        {
            object tmp;
            hierarchy.GetProperty(itemid, (int)propid, out tmp);
            if (tmp != null) {
                return (T)tmp;
            }
            return default(T);
        }

        public static T GetProp<T>(this IVsSolution hierarchy, __VSPROPID propid)
        {
            object tmp;
            hierarchy.GetProperty((int)propid, out tmp);
            if (tmp != null) {
                return (T)tmp;
            }
            return default(T);
        }

        public static string GetCanonicalName(this IVsHierarchy hierarchy, uint itemId)
        {
            try {
                string pbstrName;
                if (hierarchy.GetCanonicalName(itemId, out pbstrName) == 0) {
                    return pbstrName;
                }
            } catch (Exception) {
            }
            return "";
        }

        public static Tuple<IVsHierarchy, uint> GetNested(this IVsHierarchy hierarchy, uint itemid)
        {
            IntPtr ptr;
            uint hierId;
            var guid = typeof(IVsHierarchy).GUID;
            var hr = hierarchy.GetNestedHierarchy(itemid, ref guid, out ptr, out hierId);

            if (hr == 0 && ptr != IntPtr.Zero) {
                var tmp = Marshal.GetObjectForIUnknown(ptr) as IVsHierarchy;
                Marshal.Release(ptr);
                if (tmp != null) {
                    return new Tuple<IVsHierarchy, uint>(tmp, hierId);
                }
            }
            return null;
        }
    }
}
