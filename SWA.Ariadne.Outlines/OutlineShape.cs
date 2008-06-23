using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    public class OutlineShape
    {
        #region Member variables and Properties

        public virtual bool this[int x, int y]
        {
            get { return squares[x, y]; }
        }
        private bool[,] squares;

        #endregion

        #region Constructor

        private OutlineShape(int xSize, int ySize)
        {
            this.squares = new bool[xSize, ySize];
        }

        /// <summary>
        /// Constructor for derived classes.
        /// </summary>
        protected OutlineShape()
        {
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Circle(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
#if true
            return ImageOutlineShape.SouthAmerica(xSize, ySize, centerX, centerX, shapeSize);
#else
            OutlineShape result = new OutlineShape(xSize, ySize);

            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    double dx = x - xc, dy = y - yc;
                    result.squares[x, y] = (dx * dx + dy * dy <= sz * sz);
                }
            }

            return result;
#endif
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Diamond(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            OutlineShape result = new OutlineShape(xSize, ySize);

            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    double dx = Math.Abs(x - xc), dy = Math.Abs(y - yc);
                    result.squares[x, y] = (dx + dy <= sz);
                }
            }

            return result;
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <returns></returns>
        public static OutlineShape Char(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            FontFamily fontFamily = new FontFamily("Helvetica");
            char[] shapeCharacters = { 'C', 'O', 'S', 'V', 'X', '3', '6', '8', '9', '?', };
            char ch = shapeCharacters[r.Next(shapeCharacters.Length)];

            return Char(xSize, ySize, centerX, centerY, shapeSize, ch, fontFamily);
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="r">a source of random numbers</param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <returns></returns>
        public static OutlineShape Symbol(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            FontFamily fontFamily = new FontFamily("Times New Roman");
            char[] shapeCharacters = {
                //'\u0040',   // @
                '\u03C0',   // pi
                '\u05D0',   // aleph
                '\u263B',   // smiley
                '\u2660',   // spades
                '\u2663',   // clubs
                '\u2665',   // hearts
                '\u2666',   // diamonds
                '\u266A',   // musical note
            };
            char ch = shapeCharacters[r.Next(shapeCharacters.Length)];

            return Char(xSize, ySize, centerX, centerY, shapeSize, ch, fontFamily);
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Calculate (absolute) [xSize x ySize] coordinates from the given (relative) values.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="shapeSize"></param>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="sz"></param>
        protected static void ConvertParameters(int xSize, int ySize, double centerX, double centerY, double shapeSize, out double xc, out double yc, out double sz)
        {
            // Determine center coordinates in the shape coordinate system.
            xc = xSize * centerX;
            yc = ySize * centerY;

            // Determine sz so that a circle would touch the nearest border.
            // If the center is beyond a border, that border is not considered.
            sz = double.MaxValue;
            if (xc > 0) sz = Math.Min(sz, xc);
            if (yc > 0) sz = Math.Min(sz, yc);
            if (xc < xSize) sz = Math.Min(sz, xSize - xc);
            if (yc < ySize) sz = Math.Min(sz, ySize - yc);

            // Multiply with the requested ratio.
            sz *= shapeSize;
        }

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
        private static OutlineShape Char(int xSize, int ySize, double centerX, double centerY, double shapeSize, char ch, FontFamily fontFamily)
        {
            OutlineShape result = new OutlineShape(xSize, ySize);

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
                        // result.squares[x, y] = false;
                    }
                    else if (img.GetPixel(imgX, imgY).GetBrightness() > 0.5)
                    {
                        result.squares[x, y] = true;
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
