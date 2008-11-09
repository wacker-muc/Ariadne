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

        public static bool GetBoolSetting(string name, bool defaultValue)
        {
            Int32 value = (defaultValue == false ? 0 : 1);

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return (value != 0);
        }

        public static bool GetBoolSetting(string name)
        {
            return GetBoolSetting(name, true);
        }

        public static int GetIntSetting(string name, int defaultValue)
        {
            Int32 value = defaultValue;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return value;
        }

        public static string GetStringSetting(string name, string defaultValue)
        {
            string value = defaultValue;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (string)key.GetValue(name, value);
            }

            return value;
        }

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