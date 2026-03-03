using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace exArchivos.Services
{
    public class SystemIconProvider : IDisposable
    {
        private readonly ImageList _imageList;
        private readonly Dictionary<string, int> _iconIndexCache = new();
        private const string FolderIconKey = "__FOLDER__";

        public ImageList ImageList => _imageList;

        public SystemIconProvider(ImageList imageList)
        {
            _imageList = imageList ?? throw new ArgumentNullException(nameof(imageList));
            ConfigureImageList();
        }

        private void ConfigureImageList()
        {
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.ImageSize = new Size(16, 16);
        }

        public int GetIconIndex(string path, bool isDirectory)
        {
            var cacheKey = BuildCacheKey(path, isDirectory);

            if (_iconIndexCache.TryGetValue(cacheKey, out var cachedIndex))
                return cachedIndex;

            return CacheNewIcon(cacheKey, path, isDirectory);
        }

        private string BuildCacheKey(string path, bool isDirectory)
        {
            if (isDirectory)
                return FolderIconKey;

            var extension = Path.GetExtension(path).ToLowerInvariant();
            return string.IsNullOrEmpty(extension) ? path.ToLowerInvariant() : extension;
        }

        private int CacheNewIcon(string key, string path, bool isDirectory)
        {
            var icon = ExtractSystemIcon(path, isDirectory) ?? SystemIcons.WinLogo;
            _imageList.Images.Add(icon);
            
            var index = _imageList.Images.Count - 1;
            _iconIndexCache[key] = index;
            
            return index;
        }

        public void ClearCache()
        {
            _iconIndexCache.Clear();
            _imageList.Images.Clear();
        }

        private static Icon? ExtractSystemIcon(string path, bool isDirectory)
        {
            try
            {
                var fileInfo = new SHFILEINFO();
                var flags = SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES;
                var attributes = isDirectory ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;

                var result = SHGetFileInfo(path, attributes, ref fileInfo, 
                    (uint)Marshal.SizeOf(fileInfo), flags);

                if (fileInfo.hIcon == IntPtr.Zero)
                    return null;

                var icon = Icon.FromHandle(fileInfo.hIcon).Clone() as Icon;
                DestroyIcon(fileInfo.hIcon);
                
                return icon;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _imageList?.Dispose();
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

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, 
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        #endregion
    }
}