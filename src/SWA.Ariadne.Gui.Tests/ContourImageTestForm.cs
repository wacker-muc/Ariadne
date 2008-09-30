using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Gui.Mazes;

namespace SWA.Ariadne.Gui.Tests
{
    public partial class ContourImageTestForm : ImageTestForm
    {
        Image template, processed;

        public ContourImageTestForm()
        {
            InitializeComponent();
            timeLabel.Text = "";
        }

        protected override void newImageButton_Click(object sender, EventArgs e)
        {
            base.newImageButton_Click(sender, e);
            template = processed = null;
        }

        private void showContourButton_Click(object sender, EventArgs e)
        {
            if (this.imageButton.Image == null)
            {
                return;
            }

            if (template == null)
            {
                template = this.imageButton.Image;
            }

            DateTime start = DateTime.Now;

            ContourImage ci = new ContourImage(template);
            ci.ProcessImage();
            processed = ci.ProcessedImage;

            TimeSpan t = DateTime.Now - start;
            int ms = (int)t.TotalMilliseconds;
            this.timeLabel.Text = ms.ToString();

            this.imageButton.Image = processed;
        }
    }
}