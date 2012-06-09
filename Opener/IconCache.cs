using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace FileOpener.Opener
{
    public sealed class IconCache
    {
        readonly ImageList imageList = new ImageList { ColorDepth = ColorDepth.Depth24Bit, ImageSize = new Size(16, 16) };
        readonly Dictionary<int, int> lists = new Dictionary<int, int>();

        public ImageList ImageList { get { return imageList; } }

        public int Add(IVsHierarchy hierarchy, uint itemId)
        {
            var imgList = hierarchy.GetProp<int>(VSConstants.VSITEMID_ROOT, __VSHPROPID.VSHPROPID_IconImgList);
            int startFrom;
            if (!lists.TryGetValue(imgList, out startFrom)) {
                startFrom = imageList.Images.Count;
                lists.Add(imgList, startFrom);
                ImageListAppend(imageList.Handle, (IntPtr)imgList);
            }

            var idx = hierarchy.GetProp<int>(itemId, __VSHPROPID.VSHPROPID_IconIndex);
            return (idx < 0) ? -1 : startFrom + idx;
        }

        private static void ImageListAppend(IntPtr destination, IntPtr source)
        {
            var count1 = ImageList_GetImageCount(destination);
            var count2 = ImageList_GetImageCount(source);
            if (count1 < 0 || count2 <= 0) {
                return;
            }

            ImageList_SetImageCount(destination, (uint)(count1 + count2));
            while (count2-- > 0) {
                var hIcon = ImageList_GetIcon(source, count2, 0);
                ImageList_ReplaceIcon(destination, count1 + count2, hIcon);
                DestroyIcon(hIcon);
            }
        }

        const string Comctl32 = "comctl32.dll";
        const string User32 = "user32.dll";

        [DllImport(Comctl32)]
        private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

        [DllImport(Comctl32)]
        private static extern int ImageList_GetImageCount(IntPtr himl);

        [DllImport(Comctl32)]
        private static extern bool ImageList_SetImageCount(IntPtr himl, uint uNewCount);

        [DllImport(Comctl32)]
        private static extern int ImageList_ReplaceIcon(IntPtr himl, int i, IntPtr hicon);

        [DllImport(User32, SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        readonly Dictionary<string, int> extCache = new Dictionary<string, int>();
        public int AddFile(string f)
        {
            var ext = Path.GetExtension(f).ToLowerInvariant();
            int result;
            if (!extCache.TryGetValue(ext, out result)) {
                // if we dont call ToBitmap() here, image will be corrupted
                using (var ico = FileIconLoader.GetFileIcon("*" + ext, false))
                using (var bmp = ico.ToBitmap()) {
                    result = ImageList.Images.Count;
                    ImageList.Images.Add(bmp);
                }
                extCache.Add(ext, result);
            }

            return result;
        }

        public void SerializeTo(Stream fs)
        {
            new BinaryFormatter().Serialize(fs, ImageList.ImageStream);
        }

        public void Deserialize(FileStream fs)
        {
            ImageList.ImageStream = (ImageListStreamer)new BinaryFormatter().Deserialize(fs);
        }
    }
}
