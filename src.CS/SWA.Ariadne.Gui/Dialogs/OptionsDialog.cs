using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using SWA.Ariadne.Ctrl;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
            InitializeToolTip();

            #region Adjust horizontal alignment of the controls.

            int x = this.imageNumberNumericUpDown.Location.X;
            this.imageNumberNumericUpDown.Location = new Point(x, this.imageNumberNumericUpDown.Location.Y);

            #endregion
        }

        /// <summary>
        /// Add a tool tip text to all controls.
        /// </summary>
        private void InitializeToolTip()
        {
            #region General

            toolTip.SetToolTip(checkBoxDetailsBox, string.Join("\n", new string[] {
                "If selected, the screen saver will display a window ",
                "with information on the current maze solver and ",
                "run-time statistics."}));
            toolTip.SetToolTip(checkBoxBlinking, string.Join("\n", new string[] {
                "If selected, the end point of the solution path is ",
                "displayed as a blinking square."}));
            toolTip.SetToolTip(checkBoxEfficientSolvers, string.Join("\n", new string[] {
                "If selected, the maze solver strategies may detect ",
                "areas (dead ends) that are completely surrounded ",
                "and will not lead to the target square."}));
            toolTip.SetToolTip(textBoxStepsPerSecond, string.Join("\n", new string[] {
                "Speed of the solver algorithm. Reasonable values are ",
                "50 (very slow) to 800 (very fast)."}));
            toolTip.SetToolTip(checkBoxLogSolverStatistics, string.Join("\n", new string[] {
                "If selected, for each solver run one line of run-time ",
                "statistics is written to a log file in the application ",
                "directory:",
                SolverController.SolverLogPath()}));

            toolTip.SetToolTip(labelStepsPerSecond, toolTip.GetToolTip(textBoxStepsPerSecond));

            #endregion

            #region Images

            toolTip.SetToolTip(imageNumberNumericUpDown, string.Join("\n", new string[] {
                "Number of (foreground) images that should be displayed ",
                "in the screen saver (provided there is enough room)."}));
            toolTip.SetToolTip(imageMinSizeNumericUpDown, string.Join("\n", new string[] {
                "When an image needs to be reduced in size (because ",
                "it is larger than " + labelImagesMaxSize.Text + "), this is the minimum size it ",
                "will be scaled to.",
                "Note: The images will not be enlarged, even if they ",
                "are smaller than " + labelImagesMinSize.Text + "."}));
            toolTip.SetToolTip(imageMaxSizeNumericUpDown, string.Join("\n", new string[] {
                "Images that are larger than " + labelImagesMaxSize.Text + " (in width or ",
                "height) will be reduced in size."}));
            toolTip.SetToolTip(labelImagesMinSizePct, string.Join("\n", new string[] {
                "Image sizes are specified as a percentage of the screen dimension."}));
            toolTip.SetToolTip(imageFolderTextBox, string.Join("\n", new string[] {
                "Path to a directory with images (JPG, PNG, GIF) that ",
                "will be displayed in the screen saver. Images will ",
                "also be searched in all subdirectories."}));

            toolTip.SetToolTip(labelImagesNumber, toolTip.GetToolTip(imageNumberNumericUpDown));
            toolTip.SetToolTip(labelImagesMinSize, toolTip.GetToolTip(imageMinSizeNumericUpDown));
            toolTip.SetToolTip(labelImagesMaxSize, toolTip.GetToolTip(imageMaxSizeNumericUpDown));
            toolTip.SetToolTip(labelImagesMaxSizePct, toolTip.GetToolTip(labelImagesMinSizePct));
            toolTip.SetToolTip(selectImageFolderButton, toolTip.GetToolTip(imageFolderTextBox));

            #endregion

            #region Background

            toolTip.SetToolTip(checkBoxBackgroundImage, string.Join("\n", new string[] {
                "If selected, a background image will be displayed ",
                "behind the maze.  The image is initially hidden and ",
                "slowly uncovered as the solver passes over it."}));
            toolTip.SetToolTip(checkBoxDifferentBackgroundImageFolder, string.Join("\n", new string[] {
                "When selected, you may choose a background image folder. ",
                "Otherwise, background and foreground images are ",
                "selected from the same folder."}));
            toolTip.SetToolTip(backgroundImageFolderTextBox, string.Join("\n", new string[] {
                "Path to a directory (including its subdirectories) ",
                "with images (JPG, PNG, GIF) that will be displayed ",
                "as screen saver background images."}));
            toolTip.SetToolTip(subtractImagesBackgroundCheckBox, string.Join("\n", new string[] {
                "If selected and an image has a uniformly colored ",
                "background (e.g. all white or all black), the area ",
                "that this image covers on the maze is reduced to ",
                "an outline shape."}));

            toolTip.SetToolTip(selectBackgroundImageFolderButton, toolTip.GetToolTip(backgroundImageFolderTextBox));

            #endregion

            #region Extras

            toolTip.SetToolTip(checkBoxPaintAllWalls, string.Join("\n", new string[] {
                "If selected, the screen saver will always paint a complete ",
                "maze. Otherwise, it may also paint walls only along the ",
                "examined path or paint no walls at all (which provides ",
                "for a larger number of squares)."}));
            toolTip.SetToolTip(checkBoxOutlineShapes, string.Join("\n", new string[] {
                "If selected, certain continuous walls may be built ",
                "around the outline of natural or geometrical shapes. ",
                "Some shapes are easily recognizable and others are not."}));
            toolTip.SetToolTip(checkBoxIrregularMazes, string.Join("\n", new string[] {
                "If selected, the maze paths may follow certain ",
                "preferred patterns. Otherwise, all mazes are ",
                "uniformly random."}));
            toolTip.SetToolTip(checkBoxMultipleMazes, string.Join("\n", new string[] {
                "If selected, an outline shape (see above) may ",
                "contain a second separate maze that is solved ",
                "independently of the main maze."}));

            #endregion
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {
            #region Set the copyright text.

            try // If running in a UnitTest environment, there will be no usable assembly info.
            {
                labelCopyright.Text = AboutBox.AssemblyCopyright + ", " + AboutBox.AssemblyVersion;
            }
            catch (NullReferenceException) { }

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

            try // bad options might cause an exception
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                string msg = "cannot load saved options: " + ex.Message;
                Log.WriteLine(msg, true);
            }
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

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            Form form = new AboutDetailsForm();
            form.ShowDialog();
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
            InitImageSizeCtrl(imageMinSizeNumericUpDown, RegisteredOptions.OPT_IMAGE_MIN_SIZE, RegisteredOptions.OPT_IMAGE_MIN_SIZE_PCT);
            InitImageSizeCtrl(imageMaxSizeNumericUpDown, RegisteredOptions.OPT_IMAGE_MAX_SIZE, RegisteredOptions.OPT_IMAGE_MAX_SIZE_PCT);
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

        private void InitImageSizeCtrl(NumericUpDown ctrl, string optNamePx, string optNamePct)
        {
            // Since version 3.5: image sizes are given as a percentage of the screen size
            int valuePct = RegisteredOptions.GetIntSetting(optNamePct);

            // Before version 3.5: image sizes are given in pixels
            int valuePx = RegisteredOptions.GetIntSetting(optNamePx);
            if (valuePct <= 0 && valuePx > 0)
            {
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                int screenSize = Math.Min(screenBounds.Width, screenBounds.Height);
                valuePct = 100 * valuePx / screenSize;
            }

            ctrl.Value = Math.Min(Math.Max(valuePct, ctrl.Minimum), ctrl.Maximum);
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
            key.SetValue(RegisteredOptions.OPT_IMAGE_MIN_SIZE_PCT, (Int32)imageMinSizeNumericUpDown.Value, RegistryValueKind.DWord);
            key.SetValue(RegisteredOptions.OPT_IMAGE_MAX_SIZE_PCT, (Int32)imageMaxSizeNumericUpDown.Value, RegistryValueKind.DWord);
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
