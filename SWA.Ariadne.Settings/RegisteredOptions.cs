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

        public static bool GetBoolSetting(string name)
        {
            Int32 value = 1;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return (value != 0);
        }

        public static int GetIntSetting(string name)
        {
            Int32 value = 200;

            RegistryKey key = AppRegistryKey();
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return value;
        }

        public static RegistryKey AppRegistryKey(bool initialize)
        {
            RegistryKey result = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
            
            if (result != null)
            {
                Registry.LocalMachine.DeleteSubKeyTree(REGISTRY_KEY);
            }

            result = Registry.LocalMachine.CreateSubKey(REGISTRY_KEY);

            return result;
        }

        public static RegistryKey AppRegistryKey()
        {
            return AppRegistryKey(false);
        }
    }
}