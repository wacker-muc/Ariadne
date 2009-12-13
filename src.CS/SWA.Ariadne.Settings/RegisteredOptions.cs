using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace SWA.Ariadne.Settings
{
    public static class RegisteredOptions
    {
        private const string REGISTRY_KEY = "SOFTWARE\\SWA_Ariadne";

        public const string OPT_SHOW_DETAILS_BOX = "show details box";
        public const string OPT_BLINKING = "paint blinking end square";
        public const string OPT_EFFICIENT_SOLVERS = "use efficient solvers";
        public const string OPT_STEPS_PER_SECOND = "steps per second";
        public const string OPT_IMAGE_NUMBER = "image number";
        public const string OPT_IMAGE_MIN_SIZE = "image minimum size";
        public const string OPT_IMAGE_MAX_SIZE = "image maximum size";
        public const string OPT_IMAGE_MIN_SIZE_PCT = "image minimum size percent";
        public const string OPT_IMAGE_MAX_SIZE_PCT = "image maximum size percent";
        public const string OPT_IMAGE_FOLDER = "image folder";
        public const string OPT_IMAGE_SUBTRACT_BACKGROUND = "subtract uniform image background color";
        public const string OPT_BACKGROUND_IMAGES = "display background images";
        public const string OPT_BACKGROUND_IMAGE_FOLDER = "background image folder";
        public const string OPT_OUTLINE_SHAPES = "add outline shapes";
        public const string OPT_IRREGULAR_MAZES = "build irregular mazes";
        public const string OPT_PAINT_ALL_WALLS = "paint all maze walls";
        public const string OPT_MULTIPLE_MAZES = "create multiple mazes";
        public const string OPT_LOG_SOLVER_STATISTICS = "log solver statistics";

        // Note: The Screensaver options dialog will recreate the registry entry and store only the OPT_ values.
        //       All other values will be lost.
        public const string SAVE_IMAGE_PATHS = "immediate image paths";

        private static bool GetBoolSetting(string name, bool defaultValue)
        {
            Int32 value = (defaultValue == false ? 0 : 1);

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return (value != 0);
        }

        /// <summary>
        /// Returns a boolean value from the Windows Registry.
        /// If no value has been registered, the application default for the given option name is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetBoolSetting(string name)
        {
            bool defaultValue = false;

            switch (name)
            {
                case OPT_BACKGROUND_IMAGES:
                case OPT_PAINT_ALL_WALLS:
                case OPT_LOG_SOLVER_STATISTICS:
                    defaultValue = false;
                    break;

                case OPT_SHOW_DETAILS_BOX:
                case OPT_BLINKING:
                case OPT_EFFICIENT_SOLVERS:
                case OPT_IMAGE_SUBTRACT_BACKGROUND:
                case OPT_OUTLINE_SHAPES:
                case OPT_IRREGULAR_MAZES:
                case OPT_MULTIPLE_MAZES:
                    defaultValue = true;
                    break;
            }

            return GetBoolSetting(name, defaultValue);
        }

        private static int GetIntSetting(string name, int defaultValue)
        {
            Int32 value = defaultValue;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return value;
        }

        /// <summary>
        /// Returns an integer value from the Windows Registry.
        /// If no value has been registered, the application default for the given option name is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetIntSetting(string name)
        {
            int defaultValue = 0;

            switch (name)
            {
                case OPT_STEPS_PER_SECOND:
                    defaultValue = 200;
                    break;

                case OPT_IMAGE_NUMBER:
                    defaultValue = 0;
                    break;

                case OPT_IMAGE_MIN_SIZE:
                    if (GetIntSetting(OPT_IMAGE_MIN_SIZE_PCT, -1) < 0)
                    {
                        // only if the new option is not set
                        defaultValue = 300;
                    }
                    break;

                case OPT_IMAGE_MAX_SIZE:
                    if (GetIntSetting(OPT_IMAGE_MAX_SIZE_PCT, -1) < 0)
                    {
                        // only if the new option is not set
                        defaultValue = 400;
                    }
                    break;

                case OPT_IMAGE_MIN_SIZE_PCT:
                    if (GetIntSetting(OPT_IMAGE_MIN_SIZE, -1) < 0)
                    {
                        // only if the now obsolete option is not set
                        defaultValue = 30;
                    }
                    break;

                case OPT_IMAGE_MAX_SIZE_PCT:
                    if (GetIntSetting(OPT_IMAGE_MAX_SIZE, -1) < 0)
                    {
                        // only if the now obsolete option is not set
                        defaultValue = 60;
                    }
                    break;
            }

            return GetIntSetting(name, defaultValue);
        }

        private static string GetStringSetting(string name, string defaultValue)
        {
            string value = defaultValue;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (string)key.GetValue(name, value);
            }

            return value;
        }

        /// <summary>
        /// Returns a string value from the Windows Registry.
        /// If no value has been registered, an empty string is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetStringSetting(string name)
        {
            return GetStringSetting(name, "");
        }

        /// <summary>
        /// Returns our RegistryKey or null if it does not exist in the Windows registry.
        /// </summary>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public static RegistryKey AppRegistryKey(bool initialize)
        {
            RegistryKey rootKey = Registry.CurrentUser;

            RegistryKey result;
            try
            {
                result = rootKey.OpenSubKey(REGISTRY_KEY, true);
            }
            catch
            {
                result = null;
            }

            if (initialize)
            {
                if (result != null)
                {
                    rootKey.DeleteSubKeyTree(REGISTRY_KEY);
                }

                result = rootKey.CreateSubKey(REGISTRY_KEY);
            }

            return result;
        }

        /// <summary>
        /// Returns our RegistryKey or null if it does not exist in the Windows registry.
        /// </summary>
        /// <param name="initialize"></param>
        /// <returns></returns>
        public static RegistryKey AppRegistryKey()
        {
            return AppRegistryKey(false);
        }
    }
}