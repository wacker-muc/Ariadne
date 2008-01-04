using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
{
    #region class DetailsDialog

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

            // Create a data object, fill its contents from the target and add it to the BindingSource.
            this.data = new AriadneSettingsData();
            data.FillFrom(target);
            CalculateResultingArea();
            dataBindingSource.Add(data);
        }

        #endregion

        /// <summary>
        /// Activates the modified settings.
        /// Will generate a new maze with the modified settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// TODO: Should only draw the current maze with different attributes.
        private void OnLayoutSet(object sender, EventArgs e)
        {
            // Pass the modified data to the target.
            target.TakeParametersFrom(data);
        }

        /// <summary>
        /// Reacts to a Click event and raises following events that trigger the DataBinding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickAutoCheckbox(object sender, EventArgs e)
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
        private void OnLayoutDataChanged(object sender, EventArgs e)
        {
            // Avoid an infinite recursion
            if (_runningOnLayoutDataChanged)
            {
                return;
            }

            try
            {
                _runningOnLayoutDataChanged = true;

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

                if (data.GridWidthModified)
                {
                    data.AutoGridWidth = false;
                    data.WallWidth = Math.Max(1, (int)(0.3 * data.GridWidth));
                    data.SquareWidth = data.GridWidth - data.WallWidth;
                }
                if (data.PathWidthModified)
                {
                    data.AutoPathWidth = false;
                    data.SquareWidth = Math.Max(data.PathWidth, data.SquareWidth);
                }
                if (data.SquareWidthModified)
                {
                    data.AutoSquareWidth = false;
                    data.GridWidth = data.SquareWidth + data.WallWidth;
                }
                if (data.WallWidthModified)
                {
                    data.AutoWallWidth = false;
                    data.GridWidth = data.SquareWidth + data.WallWidth;
                }
                if (data.SquareWidthModified)
                {
                    data.PathWidth = Math.Min(data.PathWidth, data.SquareWidth);
                }

                if (!data.AutoGridWidth)
                {
                    data.AutoSquareWidth = true;
                    data.AutoPathWidth = true;
                    data.AutoWallWidth = true;
                }
                if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
                {
                    data.AutoGridWidth = true;
                }

                CalculateResultingArea();
                data.ClearModifedFlags();
                dataBindingSource.ResetCurrentItem();
            }
            finally
            {
                _runningOnLayoutDataChanged = false;
            }
        }
        private bool _runningOnLayoutDataChanged = false;

        private void CalculateResultingArea()
        {
            // TODO: xSize and ySize
            int width = 1 * data.SquareWidth + data.WallWidth;
            int height = 1 * data.SquareWidth + data.WallWidth;
            data.ResultingArea = width.ToString() + " x " + height.ToString();
        }
    }

    #endregion

    #region class AriadneSettingsData


    #endregion
}