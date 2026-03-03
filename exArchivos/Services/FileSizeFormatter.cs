namespace exArchivos.Services
{
    public static class FileSizeFormatter
    {
        private const long BytesInKilobyte = 1024;
        private const long BytesInMegabyte = BytesInKilobyte * 1024;
        private const long BytesInGigabyte = BytesInMegabyte * 1024;

        public static string Format(long bytes)
        {
            if (bytes >= BytesInGigabyte)
                return $"{bytes / (double)BytesInGigabyte:0.##} GB";

            if (bytes >= BytesInMegabyte)
                return $"{bytes / (double)BytesInMegabyte:0.##} MB";

            if (bytes >= BytesInKilobyte)
                return $"{bytes / (double)BytesInKilobyte:0.##} KB";

            return $"{bytes} B";
        }
    }
}