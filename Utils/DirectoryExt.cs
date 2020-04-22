using System.IO;

namespace Iterum.Utils
{
    internal static class DirectoryExt
    {
        public static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void DeleteIfExit(string path, bool recursive)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }
    }
}