using System;
using System.IO;

namespace exArchivos.Models
{
    public class FileSystemItemInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public bool IsDirectory { get; set; }
        public FileSystemInfo FileSystemInfo { get; set; } = null!;
        public int IconIndex { get; set; }
    }
}