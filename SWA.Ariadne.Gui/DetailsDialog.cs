using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
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
        }

        public DetailsDialog(IAriadneSettingsSource target)
            : this()
        {
            this.target = target;

            #region Set the minimum and maximum values of NumericUpDownControls.

            this.squareWidthNumericUpDown.Minimum = MazeUserControl.MinSquareWidth;
            this.squareWidthNumericUpDown.Maximum = MazeUserControl.MaxSquareWidth;
            this.pathWidthNumericUpDown.Minimum = MazeUserControl.MinPathWidth;
            this.pathWidthNumericUpDown.Maximum = MazeUserControl.MaxPathWidth;
            this.wallWidthNumericUpDown.Minimum = MazeUserControl.MinWallWidth;
            this.wallWidthNumericUpDown.Maximum = MazeUserControl.MaxWallWidth;
            this.gridWidthNumericUpDown.Minimum = MazeUserControl.MinGridWidth;
            this.gridWidthNumericUpDown.Maximum = MazeUserControl.MaxGridWidth;

            #endregion

            #region Create a data object, fill its contents from the target and add it to the BindingSource.

            this.data = new AriadneSettingsData();

            #region Put all Outline panel settings into the data object.

            data.ImageNumber = (int) this.imageNumberNumericUpDown.Value;
            data.ImageMinSize = (int) this.imageMinSizeNumericUpDown.Value;
            data.ImageMaxSize = (int) this.imageMaxSizeNumericUpDown.Value;
            data.ImageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);

            data.CircleNumber = (int)this.circleNumberNumericUpDown.Value;
            data.CircleOffCenter = (int)this.circleOffCenterNumericUpDown.Value;
            data.CircleSize = (int)this.circleSizeNumericUpDown.Value;

            data.DiamondNumber = (int)this.diamondNumberNumericUpDown.Value;
            data.DiamondOffCenter = (int)this.diamondOffCenterNumericUpDown.Value;
            data.DiamondSize = (int)this.diamondSizeNumericUpDown.Value;

            data.CharNumber = (int)this.charNumberNumericUpDown.Value;
            data.CharOffCenter = (int)this.charOffCenterNumericUpDown.Value;
            data.CharSize = (int)this.charSizeNumericUpDown.Value;

            data.SymbolNumber = (int)this.symbolNumberNumericUpDown.Value;
            data.SymbolOffCenter = (int)this.symbolOffCenterNumericUpDown.Value;
            data.SymbolSize = (int)this.symbolSizeNumericUpDown.Value;

            data.PolygonNumber = (int)this.polygonNumberNumericUpDown.Value;
            data.PolygonOffCenter = (int)this.polygonOffCenterNumericUpDown.Value;
            data.PolygonSize = (int)this.polygonSizeNumericUpDown.Value;

            data.BitmapNumber = (int)this.bitmapNumberNumericUpDown.Value;
            data.BitmapOffCenter = (int)this.bitmapOffCenterNumericUpDown.Value;
            data.BitmapSize = (int)this.bitmapSizeNumericUpDown.Value;

            data.VisibleOutlines = this.visibleOutlinesCheckBox.Checked;

            #endregion

            target.FillParametersInto(data);
            
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
                    MazeUserControl.SuggestWidths(data.GridWidth, out squareWidth, out pathWidth, out wallWidth);

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
                    data.GridWidth = Math.Min(data.SquareWidth + data.WallWidth, MazeUserControl.MaxGridWidth);
                    data.WallWidth = data.GridWidth - data.SquareWidth;
                }
                if (data.WallWidthModified)
                {
                    data.AutoWallWidth = gridWithModifiedManually;
                    data.GridWidth = Math.Min(data.SquareWidth + data.WallWidth, MazeUserControl.MaxGridWidth);
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