using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui.Mazes
{
    public class InfoPanelPainter
    {
        #region Member variables

        private readonly MazePainter mazePainter;
        private Graphics TargetGraphics
        {
            get
            {
                if (mazePainter.Buffer == null) return null;
                return mazePainter.Buffer.Graphics;
            }
        }

        private readonly Panel outerInfoPanel;
        private readonly Panel innerInfoPanel;
        private readonly Label infoLabelCaption;
        private readonly Label infoLabelStatus;

        private readonly Bitmap bitmap;
        private readonly Graphics bitmapGraphics;

        #endregion

        #region Constructor

        public InfoPanelPainter(MazePainter mazePainter)
        {
            this.mazePainter = mazePainter;

            #region Create Control objects

            outerInfoPanel = new Panel
            {
                Location = new Point(0, 0), // will be relocated later 
                Size = new Size(444, 44),
                Padding = new Padding(3),
                BackColor = Control.DefaultBackColor,
            };

            innerInfoPanel = new Panel
            {
                Location = new Point(3, 3),
                Size = new Size(438, 38),
                BorderStyle = BorderStyle.Fixed3D,
                Padding = new Padding(2),
                //BackColor = Color.Beige,
            };

            infoLabelCaption = new Label
            {
                Location = new Point(2, 1),
                Size = new Size(430, 16),
                Padding = new Padding(0),
                Text = "Caption",
                TextAlign = ContentAlignment.MiddleCenter,
            };

            infoLabelStatus = new Label
            {
                Location = new Point(2, 17),
                Size = new Size(430, 16),
                Padding = new Padding(0),
                Text = "Status",
                TextAlign = ContentAlignment.MiddleCenter,
            };

            outerInfoPanel.Controls.Add(innerInfoPanel);
            innerInfoPanel.Controls.Add(infoLabelCaption);
            innerInfoPanel.Controls.Add(infoLabelStatus);

            innerInfoPanel.ResumeLayout(false);
            outerInfoPanel.ResumeLayout(true);

            #endregion

            #region Create a bitmap for rendering the Controls

            //targetGraphics = graphics;
            bitmap = new Bitmap(outerInfoPanel.Size.Width, outerInfoPanel.Size.Height);
            bitmapGraphics = Graphics.FromImage(bitmap);

            // Approximate a standard label's settings
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            bitmapGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            #endregion

            Paint();
        }

        #endregion

        #region Public properties and methods

        /// <summary>
        /// The Control that this Painter will be painting.
        /// </summary>
        public Control Panel => outerInfoPanel;

        public void ChooseLocation(Size areaSize, Random random)
        {
            outerInfoPanel.Location = SuggestLocation(outerInfoPanel.Size, areaSize, random);
        }

        public void SetCaption(string text)
        {
            infoLabelCaption.Text = text;
            PaintLabel(infoLabelCaption);
        }

        public void SetStatus(string text)
        {
            infoLabelStatus.Text = text;
            PaintLabel(infoLabelStatus);
        }

        public void Paint()
        {
            PaintPanel(bitmapGraphics, outerInfoPanel, outerInfoPanel);
            PaintPanel(bitmapGraphics, innerInfoPanel, outerInfoPanel);
            PaintLabel(bitmapGraphics, infoLabelCaption, outerInfoPanel);
            PaintLabel(bitmapGraphics, infoLabelStatus, outerInfoPanel);

            PaintBitmap();
            if (mazePainter.Buffer != null)
            {
                mazePainter.Buffer.Render();
            }
        }

        /// <summary>
        /// Suggests where to place a control of the given size within the given area.
        /// </summary>
        public static Point SuggestLocation(Size controlSize, Size areaSize, Random random)
        {
            int x, y;
            int xMin = areaSize.Width / 20;
            int yMin = areaSize.Height / 20;
            int xMax = areaSize.Width - xMin - controlSize.Width;
            int yMax = areaSize.Height - yMin - controlSize.Height;
            x = random.Next(xMin, xMax);
            y = random.Next(yMin, yMax);

            #region Try to find a Y coordinate that leaves enough room for an image.

            int imgCount = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
            int imgSize = 20 + RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE);

            for (int i = 0; i < 8; i++)
            {
                if (imgCount < 1 || y > imgSize || y < areaSize.Height - imgSize - controlSize.Height)
                {
                    break;
                }

                y = random.Next(yMin, yMax);
            }

            #endregion

            return new Point(x, y);
        }

        #endregion

        #region Private methods

        private void PaintLabel(Label label)
        {
            PaintLabel(bitmapGraphics, label, outerInfoPanel);
            PaintBitmap();
        }

        private void PaintBitmap()
        {
            if (TargetGraphics == null) return;

            bitmapGraphics.Flush();

            TargetGraphics.DrawImage(bitmap,
                outerInfoPanel.Location.X,
                outerInfoPanel.Location.Y,
                new Rectangle(0, 0, outerInfoPanel.Width, outerInfoPanel.Height),
                GraphicsUnit.Pixel);
            TargetGraphics.Flush();
        }

        private void PaintPanel(Graphics g, Panel panel, Control container)
        {
            var rect = new Rectangle(panel.Location, panel.Size);
            var pZero = new Point(0, 0);
            var pBase = container.PointToScreen(pZero);
            var pThis = panel.PointToScreen(pZero);
            rect.X = (pThis.X - pBase.X);
            rect.Y = (pThis.Y - pBase.Y);
            if (panel.BorderStyle == BorderStyle.Fixed3D)
            {
                rect.X -= 2; rect.Y -= 2;
            }

            var b = new SolidBrush(panel.BackColor);
            g.FillRectangle(b, rect);

            if (panel.BorderStyle == BorderStyle.Fixed3D)
            {
                Pen p = Pens.DarkGray;
                p = new Pen(Color.FromArgb(255, 0x8e, 0x8e, 0x8e));
                g.DrawLine(p, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);
                g.DrawLine(p, rect.Left, rect.Top, rect.Right - 1, rect.Top);

                p = Pens.White;
                g.DrawLine(p, rect.Right - 1, rect.Bottom - 1, rect.Right - 1, rect.Top);
                g.DrawLine(p, rect.Right - 1, rect.Bottom - 1, rect.Left, rect.Bottom - 1);

                p = Pens.DarkSlateGray;
                p = new Pen(Color.FromArgb(255, 0x2e, 0x2e, 0x2e));
                g.DrawLine(p, rect.Left + 1, rect.Top + 1, rect.Left + 1, rect.Bottom - 2);
                g.DrawLine(p, rect.Left + 1, rect.Top + 1, rect.Right - 2, rect.Top + 1);

                p = Pens.LightGray;
                p = new Pen(Color.FromArgb(255, 0xd3, 0xd3, 0xd3));
                g.DrawLine(p, rect.Right - 2, rect.Bottom - 2, rect.Right - 2, rect.Top + 1);
                g.DrawLine(p, rect.Right - 2, rect.Bottom - 2, rect.Left + 1, rect.Bottom - 2);
            }
        }

        private void PaintLabel(Graphics g, Label label, Panel container)
        {
            var rect = new Rectangle(label.Location, label.Size);
            var pZero = new Point(0, 0);
            var pBase = container.PointToScreen(pZero);
            var pThis = label.PointToScreen(pZero);
            rect.X = (pThis.X - pBase.X);
            rect.Y = (pThis.Y - pBase.Y);

            var b = new SolidBrush(label.BackColor);
            g.FillRectangle(b, rect);

            var format = new StringFormat(StringFormat.GenericDefault);
            switch (label.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    break;
            }

            g.DrawString(label.Text, label.Font, Brushes.Black, rect, format);
        }

#endregion
    }
}
