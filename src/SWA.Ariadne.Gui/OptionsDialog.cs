using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
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

        private void selectImageFolderButton_Click(object sender, EventArgs e)
        {
            if (this.imageFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.imageFolderTextBox.Text = this.imageFolderBrowserDialog.SelectedPath;
            }
        }

        private void LoadSettings()
        {
            // General tab.
            checkBoxDetailsBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX);
            checkBoxBlinking.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING);
            checkBoxEfficientSolvers.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS);
            textBoxStepsPerSecond.Text = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND, 200).ToString();

            // Images tab.
            imageNumberNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER, 0);
            imageMinSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MIN_SIZE, 120);
            imageMaxSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE, 180);
            imageFolderTextBox.Text = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
        }

        private void SaveSettings()
        {
            RegistryKey key = RegisteredOptions.AppRegistryKey(true);

            // General tab.
            key.SetValue(RegisteredOptions.OPT_SHOW_DETAILS_BOX, (checkBoxDetailsBox.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_BLINKING, (checkBoxBlinking.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_EFFICIENT_SOLVERS, (checkBoxEfficientSolvers.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_STEPS_PER_SECOND, Int32.Parse(textBoxStepsPerSecond.Text), RegistryValueKind.DWord);

            // Images tab.
            key.SetValue(RegisteredOptions.OPT_IMAGE_NUMBER, imageNumberNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MIN_SIZE, imageMinSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MAX_SIZE, imageMaxSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_FOLDER, imageFolderTextBox.Text, RegistryValueKind.String);
        }
    }
}
