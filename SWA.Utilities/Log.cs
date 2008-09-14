using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SWA.Utilities
{
    public class Log
    {
        #region Static members.

        private static bool enabled = false;

        private static string logFileName = "Ariadne.log";
        private static Log instance;

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
                Instance.logFile.WriteLine(text);
                Instance.logFile.Flush();
            }
        }

        #endregion
    }
}
