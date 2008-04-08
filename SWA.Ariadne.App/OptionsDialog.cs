using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SWA.Ariadne.Settings;

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

        private void LoadSettings()
        {
            checkBoxDetailsBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX);
            checkBoxBlinking.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING);
            checkBoxEfficientSolvers.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS);
            textBoxStepsPerSecond.Text = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND).ToString();
        }

        private void SaveSettings()
        {
            RegistryKey key = RegisteredOptions.AppRegistryKey(true);

            key.SetValue(RegisteredOptions.OPT_SHOW_DETAILS_BOX, (checkBoxDetailsBox.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_BLINKING, (checkBoxBlinking.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_EFFICIENT_SOLVERS, (checkBoxEfficientSolvers.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_STEPS_PER_SECOND, Int32.Parse(textBoxStepsPerSecond.Text), RegistryValueKind.DWord);
        }
    }
}