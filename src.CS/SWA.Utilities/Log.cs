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

        private static bool enabled = false;

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
            this.logFile = new StreamWriter(logFileName, false);
        }

        #endregion

        #region Public methods

        public static void WriteLine(string text)
        {
            if (enabled)
            {
                string threadName = Thread.CurrentThread.Name;
                if (threadName == null) { threadName = "MAIN"; }

                sema.WaitOne();
                Instance.logFile.Write(threadName);
                Instance.logFile.Write(" | ");
                Instance.logFile.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
                Instance.logFile.Write(" | ");
                Instance.logFile.WriteLine(text);
                Instance.logFile.Flush();
                sema.Release();
            }
        }

        #endregion
    }
}
