using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SWA.Ariadne.App
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {
            #region Set the copyright text.

            labelCopyright.Text = AboutBox.AssemblyCopyright + ", " + AboutBox.AssemblyVersion;

            // Remove the text before the copyright sign: "Copyright "
            int p = Math.Max(labelCopyright.Text.IndexOf('©'),
                             labelCopyright.Text.IndexOf("(c)"));
            if (p > 0)
            {
                labelCopyright.Text = labelCopyright.Text.Substring(p);
            }

            labelCopyright.Location = new Point((this.Width - labelCopyright.Width) / 2, 
                                                labelCopyright.Top);

            #endregion

            LoadSettings();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private const string REGISTRY_KEY = "SOFTWARE\\SWA_Ariadne";
        public const string OPT_SHOW_DETAILS_BOX = "show details box";
        public const string OPT_BLINKING = "blinking";
        public const string OPT_STEPS_PER_SECOND = "steps per second";

        private void LoadSettings()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
            if (key == null)
            {
                checkBoxDetailsBox.Checked = true;
                checkBoxBlinking.Checked = true;
                textBoxStepsPerSecond.Text = "200";
            }
            else
            {
                checkBoxDetailsBox.Checked = ((Int32)key.GetValue(OPT_SHOW_DETAILS_BOX, 1) != 0);
                checkBoxBlinking.Checked = ((Int32)key.GetValue(OPT_BLINKING, 1) != 0);
                textBoxStepsPerSecond.Text = ((Int32)key.GetValue(OPT_STEPS_PER_SECOND, 200)).ToString();
            }
        }

        public static bool GetBoolSetting(string name)
        {
            Int32 value = 1;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return (value != 0);
        }

        public static int GetIntSetting(string name)
        {
            Int32 value = 200;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
            if (key != null)
            {
                value = (Int32)key.GetValue(name, value);
            }

            return value;
        }

        private void SaveSettings()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
            if (key != null)
            {
                Registry.LocalMachine.DeleteSubKeyTree(REGISTRY_KEY);
            }

            key = Registry.LocalMachine.CreateSubKey(REGISTRY_KEY);

            key.SetValue(OPT_BLINKING, (checkBoxBlinking.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(OPT_SHOW_DETAILS_BOX, (checkBoxDetailsBox.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(OPT_STEPS_PER_SECOND, Int32.Parse(textBoxStepsPerSecond.Text), RegistryValueKind.DWord);
        }
    }
}