using System.IO;
using System.Runtime.InteropServices;

namespace exArchivos.Services
{
    public static class FileTypeProvider
    {
        public static string GetTypeDescription(string path)
        {
            try
            {
                var fileInfo = new SHFILEINFO();
                var flags = SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES;
                var attributes = Path.HasExtension(path) 
                    ? FILE_ATTRIBUTE_NORMAL 
                    : FILE_ATTRIBUTE_DIRECTORY;

                SHGetFileInfo(path, attributes, ref fileInfo, 
                    (uint)Marshal.SizeOf(fileInfo), flags);

                return string.IsNullOrWhiteSpace(fileInfo.szTypeName) 
                    ? "Archivo" 
                    : fileInfo.szTypeName;
            }
            catch
            {
                return "Archivo";
            }
        }

        #region Win32 Interop

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_TYPENAME = 0x000000400;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern nint SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        #endregion
    }
}