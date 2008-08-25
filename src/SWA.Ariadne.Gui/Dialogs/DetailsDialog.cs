using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using SWA.Ariadne.Gui.Mazes;
using SWA.Ariadne.Gui.Painters;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class DetailsDialog : Form
    {
        #region Member variables

        private AriadneSettingsData data;
        private IAriadneSettingsSource target;

        #endregion

        #region Constructor

        public DetailsDialog()
        {
            InitializeComponent();

            // Give every radio button its specific Tag value.
            this.outlineRadioButtonNone.Tag = AriadneSettingsData.OutlineKindEnum.None;
            this.outlineRadioButtonRandom.Tag = AriadneSettingsData.OutlineKindEnum.Random;
            this.outlineRadioButtonCircle.Tag = AriadneSettingsData.OutlineKindEnum.Circle;
            this.outlineRadioButtonDiamond.Tag = AriadneSettingsData.OutlineKindEnum.Diamond;
            this.outlineRadioButtonCharacter.Tag = AriadneSettingsData.OutlineKindEnum.Character;
            this.outlineRadioButtonSymbol.Tag = AriadneSettingsData.OutlineKindEnum.Symbol;
            this.outlineRadioButtonPolygon.Tag = AriadneSettingsData.OutlineKindEnum.Polygon;
            this.outlineRadioButtonFunction.Tag = AriadneSettingsData.OutlineKindEnum.Function;
            this.outlineRadioButtonBitmap.Tag = AriadneSettingsData.OutlineKindEnum.Bitmap;
            this.outlineRadioButtonTiles.Tag = AriadneSettingsData.OutlineKindEnum.Tiles;
            this.outlineRadioButtonRectangles.Tag = AriadneSettingsData.OutlineKindEnum.Rectangles;
            this.outlineRadioButtonGrid.Tag = AriadneSettingsData.OutlineKindEnum.Grid;
            this.outlineRadioButtonGridElement.Tag = AriadneSettingsData.OutlineKindEnum.GridElement;
            this.outlineRadioButtonNone.Checked = true;

            this.wallVisibilityRadioButtonAlways.Tag = AriadneSettingsData.WallVisibilityEnum.Always;
            this.wallVisibilityRadioButtonNever.Tag = AriadneSettingsData.WallVisibilityEnum.Never;
            this.wallVisibilityRadioButtonWhenVisited.Tag = AriadneSettingsData.WallVisibilityEnum.WhenVisited;
            this.wallVisibilityRadioButtonAlways.Checked = true;

            // Set the relevant action delegates of all OutlineKind radio buttons.
            foreach (Control control in this.outlineKindPanel.Controls)
            {
                if (typeof(RadioButton).IsAssignableFrom(control.GetType()))
                {
                    RadioButton button = (RadioButton)control;
                    button.CheckedChanged += new System.EventHandler(this.OnOutlineKindChanged);
                }
            }

            // Set the relevant action delegates of all WallVisibility radio buttons.
            foreach (Control control in this.wallVisibilityGroupBox.Controls)
            {
                if (typeof(RadioButton).IsAssignableFrom(control.GetType()))
                {
                    RadioButton button = (RadioButton)control;
                    button.CheckedChanged += new System.EventHandler(this.OnWallVisibilityChanged);
                }
            }
        }

        public DetailsDialog(IAriadneSettingsSource target)
            : this()
        {
            this.target = target;

            #region Set the minimum and maximum values of NumericUpDownControls.

            this.squareWidthNumericUpDown.Minimum = MazePainter.MinSquareWidth;
            this.squareWidthNumericUpDown.Maximum = MazePainter.MaxSquareWidth;
            this.pathWidthNumericUpDown.Minimum = MazePainter.MinPathWidth;
            this.pathWidthNumericUpDown.Maximum = MazePainter.MaxPathWidth;
            this.wallWidthNumericUpDown.Minimum = MazePainter.MinWallWidth;
            this.wallWidthNumericUpDown.Maximum = MazePainter.MaxWallWidth;
            this.gridWidthNumericUpDown.Minimum = MazePainter.MinGridWidth;
            this.gridWidthNumericUpDown.Maximum = MazePainter.MaxGridWidth;

            #endregion

            #region Create a data object, fill its contents from the target and add it to the BindingSource.

            this.data = new AriadneSettingsData();

            #region Put the panel settings into the data object.

            data.IrregularMaze = this.irregularMazeCheckBox.Checked;
            data.Irregularity = (int)this.irregularityNumericUpDown.Value;

            data.WallVisibility = AriadneSettingsData.WallVisibilityEnum.Always;

            data.AutoColors = true;

            data.ImageNumber = (int) this.imageNumberNumericUpDown.Value;
            data.ImageMinSize = (int) this.imageMinSizeNumericUpDown.Value;
            data.ImageMaxSize = (int) this.imageMaxSizeNumericUpDown.Value;
            data.ImageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);

            data.OutlineKind = AriadneSettingsData.OutlineKindEnum.None;
            data.OutlineOffCenter = (int)this.offCenterNumericUpDown.Value;
            data.OutlineSize = (int)this.sizeNumericUpDown.Value;
            data.AsEmbeddedMaze = this.asEmbeddedMazeCheckBox.Checked;
            data.VisibleOutlines = this.visibleOutlinesCheckBox.Checked;
            data.DistortedOutlines = this.distortedOutlinesCheckBox.Checked;

            #endregion

            target.FillParametersInto(data);

            // Select the current OutlineKind radio button.
            foreach (Control control in this.outlineKindPanel.Controls)
            {
                if (typeof(RadioButton).IsAssignableFrom(control.GetType()))
                {
                    RadioButton button = (RadioButton)control;
                    button.Checked = ((AriadneSettingsData.OutlineKindEnum)button.Tag == data.OutlineKind);
                }
            }

            // Select the current WallVisibility radio button.
            foreach (Control control in this.wallVisibilityGroupBox.Controls)
            {
                if (typeof(RadioButton).IsAssignableFrom(control.GetType()))
                {
                    RadioButton button = (RadioButton)control;
                    button.Checked = ((AriadneSettingsData.WallVisibilityEnum)button.Tag == data.WallVisibility);
                }
            }

            CalculateResultingArea();
            
            data.AutoSquareWidth = data.AutoPathWidth = data.AutoWallWidth = data.AutoGridWidth = true;
            data.AutoMazeWidth = data.AutoMazeHeight = data.AutoSeed = true;
            data.ClearModifedFlags();
            
            dataBindingSource.DataSource = data;

            #endregion
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Activates the modified settings.
        /// Will generate a new maze with the modified settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSet(object sender, EventArgs e)
        {
            // Pass the modified data to the target.
            target.TakeParametersFrom(data);

            // Reread the parameters, in case the target has not accepted every setting.
            target.FillParametersInto(data);
            CalculateResultingArea();
            data.ClearModifedFlags();
            
            dataBindingSource.ResetCurrentItem();
        }

        /// <summary>
        /// Called from the RadioButtons in the outlineKindPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOutlineKindChanged(object sender, EventArgs e)
        {
            RadioButton b = (RadioButton)sender;
            if (b.Checked)
            {
                data.OutlineKind = (AriadneSettingsData.OutlineKindEnum)b.Tag;
            }
        }

        /// <summary>
        /// Called from the RadioButtons in the wallVisibilityGroupBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWallVisibilityChanged(object sender, EventArgs e)
        {
            RadioButton b = (RadioButton)sender;
            if (b.Checked)
            {
                data.WallVisibility = (AriadneSettingsData.WallVisibilityEnum)b.Tag;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSuggestColors(object sender, EventArgs e)
        {
            Color forwardColor, backwardColor;
            ColorBuilder.SuggestColors(data.ReferenceColor1, data.ReferenceColor2, out forwardColor, out backwardColor);
            
            data.ForwardColor = forwardColor;
            data.BackwardColor = backwardColor;
            data.AutoColors = false;

            dataBindingSource.ResetCurrentItem();
        }

        private void OnSelectImageFolder(object sender, EventArgs e)
        {
            // Start at the path found in the registered options.
            this.imageFolderBrowserDialog.SelectedPath = this.data.ImageFolder;

            if (this.imageFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.imageFolderTextBox.Text = this.imageFolderBrowserDialog.SelectedPath;
                this.data.ImageFolder = this.imageFolderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Reacts to a Click event and raises following events that trigger the DataBinding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickImmediateUpdate(object sender, EventArgs e)
        {
            // temporarily change the focus to another control
            this.setLayoutButton.Select();
            ((Control)sender).Select();
        }

        /// <summary>
        /// Update the other controls to reflect the derived changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataChanged(object sender, EventArgs e)
        {
            // Avoid an infinite recursion
            if (_busyOnDataChanged)
            {
                return;
            }

            try
            {
                _busyOnDataChanged = true;

                #region Layout data

                if (data.AutoGridWidthModified)
                {
                    if (!data.AutoGridWidth)
                    {
                        data.AutoSquareWidth = true;
                        data.AutoPathWidth = true;
                        data.AutoWallWidth = true;
                    }
                }
                if (data.AutoSquareWidthModified || data.AutoPathWidthModified || data.AutoWallWidthModified)
                {
                    if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
                    {
                        data.AutoGridWidth = true;
                    }
                }

                bool gridWithModifiedManually = data.GridWidthModified;

                if (data.GridWidthModified)
                {
                    data.AutoGridWidth = false;
                    
                    int squareWidth, pathWidth, wallWidth;
                    bool visibleWalls = (data.WallVisibility != AriadneSettingsData.WallVisibilityEnum.Never);
                    MazePainter.SuggestWidths(data.GridWidth, visibleWalls, out squareWidth, out pathWidth, out wallWidth);

                    data.WallWidth = wallWidth;
                    data.SquareWidth = squareWidth;
                    data.PathWidth = pathWidth;
                }
                if (data.PathWidthModified)
                {
                    data.AutoPathWidth = gridWithModifiedManually;
                    data.SquareWidth = Math.Max(data.PathWidth, data.SquareWidth);
                }
                if (data.SquareWidthModified)
                {
                    data.AutoSquareWidth = gridWithModifiedManually;
                    data.PathWidth = Math.Min(data.PathWidth, data.SquareWidth);
                    data.GridWidth = Math.Min(data.SquareWidth + data.WallWidth, MazePainter.MaxGridWidth);
                    data.WallWidth = data.GridWidth - data.SquareWidth;
                }
                if (data.WallWidthModified)
                {
                    data.AutoWallWidth = gridWithModifiedManually;
                    data.GridWidth = Math.Min(data.SquareWidth + data.WallWidth, MazePainter.MaxGridWidth);
                    data.SquareWidth = data.GridWidth - data.WallWidth;
                }

                if (gridWithModifiedManually || (data.AutoGridWidthModified && !data.AutoGridWidth))
                {
                    data.AutoSquareWidth = true;
                    data.AutoPathWidth = true;
                    data.AutoWallWidth = true;
                }
                if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
                {
                    data.AutoGridWidth = true;
                }

                #endregion

                #region Shape data

                if (data.MazeWidthModified)
                {
                    data.AutoMazeWidth = false;
                }
                if (data.MazeHeightModified)
                {
                    data.AutoMazeHeight = false;
                }
                if (data.SeedModified)
                {
                    data.AutoSeed = false;
                }

                #endregion

                CalculateResultingArea();
                data.ClearModifedFlags();
                dataBindingSource.ResetCurrentItem();
            }
            finally
            {
                _busyOnDataChanged = false;
            }
        }
        private bool _busyOnDataChanged = false;

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Updates the maze dimensions (pixels) value.
        /// </summary>
        private void CalculateResultingArea()
        {
            int width = data.MazeWidth * data.GridWidth + data.WallWidth;
            int height = data.MazeHeight * data.GridWidth + data.WallWidth;
            data.ResultingArea = width.ToString() + " x " + height.ToString();
        }

        #endregion
    }
}