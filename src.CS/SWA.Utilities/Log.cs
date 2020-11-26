using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace SWA.Utilities
{
    public class Log
    {
        #region Static members.

#if DEBUG
        private static readonly bool enabled = true;
#else
        private static readonly bool enabled = false;
#endif

        private static string logFileName = "Ariadne.log";
        private static Log instance;

        private static readonly Semaphore sema = new Semaphore(1, 1);

        private static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
        }

        #endregion

        #region Constructor and Destructor

        private StreamWriter logFile;

        private Log()
        {
            string path = Path.Combine(Directory.ApplicationDirectory, logFileName);
            try
            {
                this.logFile = new StreamWriter(path, false);
            }
            catch (Exception) // access violation
            {
                this.logFile = null;
            }
        }

        #endregion

        #region Public methods

        public static void WriteLine(string text, bool alsoOnConsole = false)
        {
            if (enabled)
            {
                string threadName = Thread.CurrentThread.Name;
                if (threadName == null) { threadName = "MAIN"; }

                sema.WaitOne();
                var f = Instance.logFile;
                if (f == null) // i.e., the logfile is not writable
                {
                    alsoOnConsole = true;
                }
                else
                {
                    f.Write(threadName);
                    f.Write(" | ");
                    f.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
                    f.Write(" | ");
                    f.WriteLine(text);
                    f.Flush();
                }
                sema.Release();
            }
            if (alsoOnConsole)
            {
                System.Console.Out.WriteLine(text);
            }
        }

        #endregion
    }
}
