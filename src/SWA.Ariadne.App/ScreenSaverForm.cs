using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SWA.Ariadne.App
{
    public partial class ScreenSaverForm : MazeForm
    {
        #region Constructor

        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Switch auto repeat mode on.
            this.repeatMode = true;

            // Let the MazeUserControl cover the whole form.
            this.mazeUserControl.Location = new Point(0, 0);
            this.mazeUserControl.Size = this.Size;
            this.mazeUserControl.BringToFront();

            // Select the strategyComboBox's item that chooses a random strategy.
            strategyComboBox.SelectedItem = "(any)";

            this.OnNew(null, null);
            this.OnStart(null, null);
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void ScreenSaverForm_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        #endregion
    }
}