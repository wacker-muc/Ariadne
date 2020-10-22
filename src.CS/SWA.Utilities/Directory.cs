using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SWA.Utilities
{
    public static class Directory
    {
        /// <summary>
        /// When positive, the results of the Find() method will be cached
        /// and reused for that number of seconds.
        /// When negative, they will be cached forever.
        /// </summary>
        /// <value>The result valid for seconds.</value>
        public static int ResultValidForSeconds { get; set; }

        /// <summary>
        /// Key: a combination of directoryPath and filePattern.
        /// Value.Key: a DateTime when the cached result was created.
        /// Value.Value: the resulting list of file names.
        /// </summary>
        private static readonly Dictionary<string, KeyValuePair<DateTime, List<string>>> Cache
            = new Dictionary<string, KeyValuePair<DateTime, List<string>>>(20);

        public static List<string> Find(string directoryPath, string filePattern, bool withSubdirectories)
        {
            var key = directoryPath + ":" + filePattern;
            lock (Cache)
            {
                if (Cache.ContainsKey(key))
                {
                    TimeSpan age = System.DateTime.Now - Cache[key].Key;
                    if (age.TotalSeconds <= ResultValidForSeconds || ResultValidForSeconds < 0)
                    {
                        Console.WriteLine("{0} {1} -- {2}", DateTime.Now.ToLongTimeString(), filePattern, "using cached result");
                        return Cache[key].Value;
                    }
                }
            }

            List<string> result = new List<string>();

            try
            {
                // Create a reference to the given directory.
                DirectoryInfo di = new DirectoryInfo(directoryPath);

                // Create an array representing the files in the given directory.
                FileInfo[] fis = di.GetFiles(filePattern, (withSubdirectories
                    ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

                foreach (FileInfo fi in fis)
                {
                    result.Add(fi.FullName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                // leave result empty
            }

            if (ResultValidForSeconds != 0)
            {
                lock (Cache)
                {
                    Console.WriteLine("{0} {1} -- {2}", DateTime.Now.ToLongTimeString(), filePattern, "adding to cache");
                    Cache[key] = new KeyValuePair<DateTime, List<string>>(DateTime.Now, result);
                }
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
