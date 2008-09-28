using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();

            #region Adjust horizontal alignment of the controls.

            int x = this.imageNumberNumericUpDown.Location.X;
            this.imageNumberNumericUpDown.Location = new Point(x, this.imageNumberNumericUpDown.Location.Y);

            #endregion
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
            // Start at the path found in the registered options.
            this.imageFolderBrowserDialog.SelectedPath = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);

            if (this.imageFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.imageFolderTextBox.Text = this.imageFolderBrowserDialog.SelectedPath;
            }
        }

        private void LoadSettings()
        {
            // General tab.
            checkBoxDetailsBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX, true);
            checkBoxBlinking.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING, true);
            checkBoxEfficientSolvers.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS, true);
            textBoxStepsPerSecond.Text = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND, 200).ToString();

            // Images tab.
            imageNumberNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER, 0);
            imageMinSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MIN_SIZE, 120);
            imageMaxSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE, 180);
            imageFolderTextBox.Text = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
            subtractImagesBackgroundCheckBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND, true);

            // Extras tab.
            checkBoxPaintAllWalls.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_PAINT_ALL_WALLS, false);
            checkBoxOutlineShapes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_OUTLINE_SHAPES, true);
            checkBoxIrregularMazes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IRREGULAR_MAZES, true);
            checkBoxMultipleMazes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_MULTIPLE_MAZES, true);
        }

        private void SaveSettings()
        {
            RegistryKey key = RegisteredOptions.AppRegistryKey(true);

            // General tab.
            key.SetValue(RegisteredOptions.OPT_SHOW_DETAILS_BOX, (Int32)(checkBoxDetailsBox.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_BLINKING, (Int32)(checkBoxBlinking.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_EFFICIENT_SOLVERS, (Int32)(checkBoxEfficientSolvers.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_STEPS_PER_SECOND, Int32.Parse(textBoxStepsPerSecond.Text), RegistryValueKind.DWord);

            // Images tab.
            key.SetValue(RegisteredOptions.OPT_IMAGE_NUMBER, (Int32)imageNumberNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MIN_SIZE, (Int32)imageMinSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MAX_SIZE, (Int32)imageMaxSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_FOLDER, imageFolderTextBox.Text, RegistryValueKind.String);
            key.SetValue(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND, (Int32)(subtractImagesBackgroundCheckBox.Checked ? 1 : 0), RegistryValueKind.DWord);

            // Extras tab.
            key.SetValue(RegisteredOptions.OPT_PAINT_ALL_WALLS, (Int32)(checkBoxPaintAllWalls.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_OUTLINE_SHAPES, (Int32)(checkBoxOutlineShapes.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IRREGULAR_MAZES, (Int32)(checkBoxIrregularMazes.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_MULTIPLE_MAZES, (Int32)(checkBoxMultipleMazes.Checked ? 1 : 0), RegistryValueKind.DWord);
        }
    }
}
