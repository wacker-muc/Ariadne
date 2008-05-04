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
        public const string OPT_OUTLINE_SHAPES = "add outline shapes";

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

        public static RegistryKey AppRegistryKey(bool initialize)
        {
            RegistryKey rootKey = Registry.CurrentUser;

            RegistryKey result;
            try
            {
                result = rootKey.OpenSubKey(REGISTRY_KEY);
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

        public static RegistryKey AppRegistryKey()
        {
            return AppRegistryKey(false);
        }
    }
}