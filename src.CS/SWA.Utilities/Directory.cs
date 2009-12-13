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
            List<string> result = new List<string>();

            try
            {
                // Create a reference to the given directory.
                DirectoryInfo di = new DirectoryInfo(directoryPath);

                // Create an array representing the files in the given directory.
                FileInfo[] fis = di.GetFiles(filePattern, (withSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

                foreach (FileInfo fi in fis)
                {
                    result.Add(fi.FullName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                // leave result empty
            }

            return result;
        }

        /// <summary>
        /// Returns the full path to the application directory.
        /// </summary>
        /// <returns></returns>
        public static string ApplicationDirectory
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            }
        }
    }
}
