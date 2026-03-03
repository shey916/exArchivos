using System.IO;

namespace exArchivos.Services
{
    public static class DirectorySizeCalculator
    {
        public static long Calculate(DirectoryInfo directory)
        {
            if (directory == null)
                return 0;

            long totalSize = 0;

            try
            {
                totalSize += CalculateFilesSize(directory);
                totalSize += CalculateSubdirectoriesSize(directory);
            }
            catch
            {
                // Ignorar errores de acceso durante el cálculo
            }

            return totalSize;
        }

        private static long CalculateFilesSize(DirectoryInfo directory)
        {
            long size = 0;
            
            try
            {
                foreach (var file in directory.GetFiles())
                {
                    size += file.Length;
                }
            }
            catch
            {
                // Ignorar errores de acceso a archivos individuales
            }

            return size;
        }

        private static long CalculateSubdirectoriesSize(DirectoryInfo directory)
        {
            long size = 0;

            try
            {
                foreach (var subdirectory in directory.GetDirectories())
                {
                    size += Calculate(subdirectory);
                }
            }
            catch
            {
                // Ignorar errores de acceso a subdirectorios
            }

            return size;
        }
    }
}