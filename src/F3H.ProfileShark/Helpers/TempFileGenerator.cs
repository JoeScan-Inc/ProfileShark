using System;
using System.IO;

public static class TempFileGenerator
{
    public static string CreateTempBinaryFileName(string prefix = null, string directory = null)
    {
        try
        {
            // Use system temp path if no directory specified
            string tempPath = directory ?? Path.GetTempPath();
            
            // Ensure directory exists
            if (!Directory.Exists(tempPath))
            {
                throw new DirectoryNotFoundException($"Specified directory does not exist: {tempPath}");
            }

            // Generate unique filename
            string fileName;
            do
            {
                string randomPart = Path.GetRandomFileName();
                string nameWithoutExt = prefix != null ? $"{prefix}_{randomPart}" : randomPart;
                fileName = Path.Combine(tempPath, $"{nameWithoutExt}.bin");
            }
            while (File.Exists(fileName));

            return fileName;
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to generate temporary file name: {ex.Message}", ex);
        }
    }
}