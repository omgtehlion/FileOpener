using System;
using System.Drawing;
using System.Runtime.InteropServices;

// For loading an icon from a file. 
// see: http://www.atakala.com/Browser/Item.aspx?user_id=amos&dict_id=1955

public static class FileIconLoader
{
    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_LARGEICON = 0x0;
    private const uint SHGFI_SMALLICON = 0x1;
    private const uint SHGFI_USEFILEATTRIBUTES = 0x10;

    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

    [DllImport("shell32.dll")]
    private static extern IntPtr SHGetFileInfo(string pszPath,
        uint dwFileAttributes,
        ref SHFILEINFO psfi,
        uint cbSizeFileInfo,
        uint uFlags);

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public static Icon GetFileIcon(string fileName, bool largeIcon)
    {
        var shinfo = new SHFILEINFO();
        var sizeFlag = largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON;
        SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            SHGFI_ICON |
            sizeFlag |
            SHGFI_USEFILEATTRIBUTES);
        try {
            return Icon.FromHandle(shinfo.hIcon);
        } catch { 
            return null;
        }
    }
}
