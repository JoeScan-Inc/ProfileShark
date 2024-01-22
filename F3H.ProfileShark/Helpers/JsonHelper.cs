using System.IO;

namespace F3H.ProfileShark.Helpers;

public static class JsonHelper
{
    public static void CreateEmptyFileIfNotExisting(string path)
    {
        if (File.Exists(path))
        {
            return;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        using var f = File.CreateText(path);
        f.WriteLine("{}");
    }
}