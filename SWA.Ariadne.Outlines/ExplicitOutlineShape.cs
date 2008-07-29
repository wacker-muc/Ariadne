using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    class ExplicitOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        public override bool this[int x, int y]
        {
            get { return squares[x, y]; }
        }
        public void SetValue(int x, int y, bool value)
        {
            this.squares[x, y] = value;
        }
        private bool[,] squares;

        #endregion

        #region Constructor

        public ExplicitOutlineShape(int xSize, int ySize)
            : base(xSize, ySize)
        {
            this.squares = new bool[xSize, ySize];
        }

        public ExplicitOutlineShape(int xSize, int ySize, OutlineShape template)
            : this(xSize, ySize)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    this.squares[x, y] = template[x, y];
                }
            }
        }

        #endregion

        #region Character shape implementation

        /// <summary>
        /// Create an outline shape from the given character and font family.
        /// </summary>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <param name="ch"></param>
        /// <param name="fontFamily"></param>
        /// <returns></returns>
        internal static OutlineShape Char(int xSize, int ySize, double centerX, double centerY, double shapeSize, char ch, FontFamily fontFamily)
        {
            ExplicitOutlineShape result = new ExplicitOutlineShape(xSize, ySize);

            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);
            sz *= 2; // sz is not used as a radius but as the character height

            #region Draw the given character into an image (white on black).

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            int enlargement = 3;
            int imgXSize = enlargement * xSize, imgYSize = enlargement * ySize;
            Bitmap img = new Bitmap(imgXSize, imgYSize);
            Graphics g = Graphics.FromImage(img);
            Font font = new Font(fontFamily, (float)(enlargement * sz), FontStyle.Bold);
            g.DrawRectangle(Pens.Black, 0, 0, imgXSize, imgXSize);
            g.DrawString(new string(ch, 1), font, Brushes.White, new RectangleF(0, 0, imgXSize, (int)(1.20 * imgYSize)), stringFormat);

            #endregion

            #region Scale the image so that the covered area is of the requested size.

            int imgXCenter, imgYCenter;
            ScaleImage(ref img, sz, out imgXCenter, out imgYCenter);

            int imgXOffset = imgXCenter - (int)xc;
            int imgYOffset = imgYCenter - (int)yc;

            #endregion

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    int imgX = x + imgXOffset, imgY = y + imgYOffset;

                    if (imgX < 0 || imgX >= img.Width || imgY < 0 || imgY >= img.Height)
                    {
                        // result.SetValue(x, y, false);
                    }
                    else if (img.GetPixel(imgX, imgY).GetBrightness() > 0.5)
                    {
                        result.SetValue(x, y, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Scale the given image so that the painted area's larger dimension is equal to size.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="sz">desired size of the covered area</param>
        /// <param name="imgXCenter">center of the covered area</param>
        /// <param name="imgYCenter">center of the covered area</param>
        private static void ScaleImage(ref Bitmap img, double sz, out int imgXCenter, out int imgYCenter)
        {
            int imgXMin, imgXMax, imgYMin, imgYMax;
            int x, y;
            bool found;

            #region Find imgXMin
            for (x = 0, found = false; x < img.Width && !found; x++)
            {
                for (y = 0; y < img.Height; y++)
                {
                    if (img.GetPixel(x, y).GetBrightness() > 0.5)
                    {
                        found = true;
                        break;
                    }
                }
            }
            imgXMin = x;
            #endregion

            if (!found)
            {
                imgXCenter = imgYCenter = 0;
                return;
            }

            #region Find imgXMax
            for (x = img.Width - 1, found = false; x > imgXMin && !found; x--)
            {
                for (y = 0; y < img.Height; y++)
                {
                    if (img.GetPixel(x, y).GetBrightness() > 0.5)
                    {
                        found = true;
                        break;
                    }
                }
            }
            imgXMax = x;
            #endregion

            #region Find imgYMin
            for (y = 0, found = false; y < img.Height && !found; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    if (img.GetPixel(x, y).GetBrightness() > 0.5)
                    {
                        found = true;
                        break;
                    }
                }
            }
            imgYMin = y;
            #endregion

            #region Find imgYMax
            for (y = img.Height - 1, found = false; y > imgYMin && !found; y--)
            {
                for (x = 0; x < img.Width; x++)
                {
                    if (img.GetPixel(x, y).GetBrightness() > 0.5)
                    {
                        found = true;
                        break;
                    }
                }
            }
            imgYMax = y;
            #endregion

            #region Scale image so that the larger dimension of the painted area is equal to sz

            double scale = sz / Math.Max(imgXMax - imgXMin, imgYMax - imgYMin);
            img = new Bitmap(img, new Size((int)(img.Width * scale), (int)(img.Height * scale)));

            imgXCenter = (int)((imgXMin + imgXMax) * scale / 2.0);
            imgYCenter = (int)((imgYMin + imgYMax) * scale / 2.0);

            #endregion
        }

        #endregion
    }
}
