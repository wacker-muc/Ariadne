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

        private void selectBackgroundImageFolderButton_Click(object sender, EventArgs e)
        {
            // Start at the path found in the registered options.
            this.imageFolderBrowserDialog.SelectedPath = this.backgroundImageFolderTextBox.Text;

            if (this.imageFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.backgroundImageFolderTextBox.Text = this.imageFolderBrowserDialog.SelectedPath;
            }
        }

        private void checkBoxDifferentBackgroundImageFolder_CheckedChanged(object sender, EventArgs e)
        {
            this.selectBackgroundImageFolderButton.Enabled = this.backgroundImageFolderTextBox.Enabled = this.checkBoxDifferentBackgroundImageFolder.Checked;
        }

        private void LoadSettings()
        {
            // General tab.
            checkBoxDetailsBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_SHOW_DETAILS_BOX);
            checkBoxBlinking.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING);
            checkBoxEfficientSolvers.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS);
            textBoxStepsPerSecond.Text = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND).ToString();
            checkBoxLogSolverStatistics.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_LOG_SOLVER_STATISTICS);

            // Images tab.
            imageNumberNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
            imageMinSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MIN_SIZE);
            imageMaxSizeNumericUpDown.Value = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE);
            imageFolderTextBox.Text = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
            subtractImagesBackgroundCheckBox.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND);

            // Background tab.
            checkBoxBackgroundImage.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BACKGROUND_IMAGES);
            backgroundImageFolderTextBox.Text = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_BACKGROUND_IMAGE_FOLDER);

            if (backgroundImageFolderTextBox.Text == "")
            {
                checkBoxDifferentBackgroundImageFolder.Checked = false;
                backgroundImageFolderTextBox.Enabled = false;
                selectBackgroundImageFolderButton.Enabled = false;
                backgroundImageFolderTextBox.Text = imageFolderTextBox.Text;
            }
            else
            {
                checkBoxDifferentBackgroundImageFolder.Checked = true;
                backgroundImageFolderTextBox.Enabled = true;
                selectBackgroundImageFolderButton.Enabled = true;
            }

            // Extras tab.
            checkBoxPaintAllWalls.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_PAINT_ALL_WALLS);
            checkBoxOutlineShapes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_OUTLINE_SHAPES);
            checkBoxIrregularMazes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_IRREGULAR_MAZES);
            checkBoxMultipleMazes.Checked = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_MULTIPLE_MAZES);
        }

        private void SaveSettings()
        {
            RegistryKey key = RegisteredOptions.AppRegistryKey(true);

            // General tab.
            key.SetValue(RegisteredOptions.OPT_SHOW_DETAILS_BOX, (Int32)(checkBoxDetailsBox.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_BLINKING, (Int32)(checkBoxBlinking.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_EFFICIENT_SOLVERS, (Int32)(checkBoxEfficientSolvers.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_STEPS_PER_SECOND, Int32.Parse(textBoxStepsPerSecond.Text), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_LOG_SOLVER_STATISTICS, (Int32)(checkBoxLogSolverStatistics.Checked ? 1 : 0), RegistryValueKind.DWord);

            // Images tab.
            key.SetValue(RegisteredOptions.OPT_IMAGE_NUMBER, (Int32)imageNumberNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MIN_SIZE, (Int32)imageMinSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MAX_SIZE, (Int32)imageMaxSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_FOLDER, imageFolderTextBox.Text, RegistryValueKind.String);
            key.SetValue(RegisteredOptions.OPT_IMAGE_SUBTRACT_BACKGROUND, (Int32)(subtractImagesBackgroundCheckBox.Checked ? 1 : 0), RegistryValueKind.DWord);

            // Background tab.
            key.SetValue(RegisteredOptions.OPT_BACKGROUND_IMAGES, (Int32)(checkBoxBackgroundImage.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_BACKGROUND_IMAGE_FOLDER, (checkBoxDifferentBackgroundImageFolder.Checked ? backgroundImageFolderTextBox.Text : ""), RegistryValueKind.String);

            // Extras tab.
            key.SetValue(RegisteredOptions.OPT_PAINT_ALL_WALLS, (Int32)(checkBoxPaintAllWalls.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_OUTLINE_SHAPES, (Int32)(checkBoxOutlineShapes.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IRREGULAR_MAZES, (Int32)(checkBoxIrregularMazes.Checked ? 1 : 0), RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_MULTIPLE_MAZES, (Int32)(checkBoxMultipleMazes.Checked ? 1 : 0), RegistryValueKind.DWord);
        }
    }
}
