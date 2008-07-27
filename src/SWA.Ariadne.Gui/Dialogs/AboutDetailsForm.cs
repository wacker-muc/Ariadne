using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SWA.Ariadne.Gui.Dialogs
{
    public partial class AboutDetailsForm : Form
    {
        public AboutDetailsForm()
        {
            InitializeComponent();

            this.Size = new Size(400, 480);
            this.textBoxFeatureLog.Text = "";
            this.textBoxFeatureLog.Text += Properties.Resources.Overview;
            this.textBoxFeatureLog.Text += Properties.Resources.FeatureLog;
        }
    }
}