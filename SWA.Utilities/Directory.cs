using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SWA.Utilities
{
    public static class Directory
    {
        public static List<string> Find(string directoryPath, string filePattern, bool withSubdirectories)
        {
            // Create a reference to the given directory.
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            
            // Create an array representing the files in the given directory.
            FileInfo[] fis = di.GetFiles(filePattern, (withSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

            List<string> result = new List<string>(fis.Length);

            foreach (FileInfo fi in fis)
            {
                result.Add(fi.FullName);
            }

            return result;
        }
    }
}
