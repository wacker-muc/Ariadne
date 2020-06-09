using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SWA.Ariadne.Settings
{
    public partial class ColorLabel : Label
    {
        public ColorLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            this.UpdateText();

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void UpdateText()
        {
            StringBuilder text = new StringBuilder(80);

            Color c = this.BackColor;

            text.Append("h = " + c.GetHue().ToString("000"));
            text.Append(", ");
            text.Append("s = " + c.GetSaturation().ToString("0.00"));
            text.Append(", ");
            text.Append("b = " + c.GetBrightness().ToString("0.00"));

            this.Text = text.ToString();

            if (c.GetBrightness() < 0.48)
            {
                this.ForeColor = Color.White;
            }
            else
            {
                this.ForeColor = Color.Black;
            }
        }
    }
}
