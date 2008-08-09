using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape that is based on a two dimensional array of boolean values.
    /// The values can be set and cleared explicitely.
    /// </summary>
    internal class ExplicitOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        public override bool this[int x, int y]
        {
            get { return (squares[x, y] != 0); }
        }
        public void SetValue(int x, int y, bool value)
        {
            this.squares[x, y] = (value == true ? (byte)1 : (byte)0);
        }
        private byte[,] squares;

        #endregion

        #region Constructor

        public ExplicitOutlineShape(int xSize, int ySize)
            : base(xSize, ySize)
        {
            this.squares = new byte[xSize, ySize];
        }

        public ExplicitOutlineShape(OutlineShape template)
            : this(template.XSize, template.YSize)
        {
            for (int x = 0; x < this.XSize; x++)
            {
                for (int y = 0; y < this.YSize; y++)
                {
                    this.SetValue(x, y, template[x, y]);
                }
            }
        }

        public ExplicitOutlineShape(OutlineShape template, InsideShapeDelegate isReserved)
            : this(template.XSize, template.YSize)
        {
            for (int x = 0; x < this.XSize; x++)
            {
                for (int y = 0; y < this.YSize; y++)
                {
                    this.SetValue(x, y, (template[x, y] && (isReserved == null || !isReserved(x, y))));
                }
            }
        }

        public ExplicitOutlineShape(Bitmap template)
            : this(template.Width, template.Height)
        {
            for (int x = 0; x < this.XSize; x++)
            {
                for (int y = 0; y < this.YSize; y++)
                {
                    // black -> true, white -> false
                    this.SetValue(x, y, template.GetPixel(x, y).GetBrightness() <= 0.5);
                }
            }
        }

        #endregion

        #region OutlineShape implementation

        /// <summary>
        /// Returns the largest subset of the template shape whose squares are all connected to each other.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="isReserved">defines the maze's reserved areas</param>
        /// <returns></returns>
        public static OutlineShape ConnectedSubset(OutlineShape template, InsideShapeDelegate isReserved)
        {
            ExplicitOutlineShape result = new ExplicitOutlineShape(template, isReserved);

            #region Scan the shape for connected areas.

            byte subsetId = 1;
            int largestAreaSize = 0;
            byte largestAreaId = 0;

            for (int x = 0; x < result.XSize; x++)
            {
                for (int y = 0; y < result.YSize; y++)
                {
                    if (result.squares[x, y] == 1 && subsetId < byte.MaxValue)
                    {
                        int areaSize = result.FillSubset(x, y, ++subsetId);
                        if (areaSize > largestAreaSize)
                        {
                            largestAreaSize = areaSize;
                            largestAreaId = subsetId;
                        }
                    }
                }
            }

            #endregion

            #region Leave only the largest subset, eliminate all others.

            for (int x = 0; x < result.XSize; x++)
            {
                for (int y = 0; y < result.YSize; y++)
                {
                    result.SetValue(x, y, (result.squares[x, y] == largestAreaId));
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Returns the template shape, augmented by all totally enclosed areas.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="isReserved">defines the maze's reserved areas</param>
        /// <returns></returns>
        public static OutlineShape Closure(OutlineShape template, InsideShapeDelegate isReserved)
        {
            ExplicitOutlineShape result = new ExplicitOutlineShape(template.Inverse());

            #region Scan and mark the reserved areas.

            if (isReserved != null)
            {
                byte reservedId = 3;

                for (int x = 0; x < result.XSize; x++)
                {
                    for (int y = 0; y < result.YSize; y++)
                    {
                        if (isReserved(x, y))
                        {
                            result.squares[x, y] = reservedId;
                        }
                    }
                }
            }

            #endregion

            #region Scan all outside areas.

            byte outsideId = 2;
            int x0 = 0, x1 = result.XSize - 1, y0 = 0, y1 = result.YSize - 1;

            for (int x = 0; x < result.XSize; x++)
            {
                if (result.squares[x, y0] == 1)
                {
                    result.FillSubset(x, y0, outsideId);
                }
                if (result.squares[x, y1] == 1)
                {
                    result.FillSubset(x, y1, outsideId);
                }
            }
            for (int y = 0; y < result.YSize; y++)
            {
                if (result.squares[x0, y] == 1)
                {
                    result.FillSubset(x0, y, outsideId);
                }
                if (result.squares[x1, y] == 1)
                {
                    result.FillSubset(x1, y, outsideId);
                }
            }

            #endregion

            #region Add the areas which were not reached.

            for (int x = 0; x < result.XSize; x++)
            {
                for (int y = 0; y < result.YSize; y++)
                {
                    // 0: square is part of the template (not part of its inverse)
                    // 1: square is not part of the template, but was not reached
                    result.SetValue(x, y, (result.squares[x, y] <= 1));
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Mark all squares connected to (x, y) with the given ID.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="id">a unique subset id greater than 1</param>
        /// <returns>number of squares in the subset</returns>
        private int FillSubset(int x, int y, byte id)
        {
            int result = 0;

            // Use an array of (x, y) coordinates large enough to add all squares of the shape.
            // Only squares with value == 1 are added, the value is then changed to the given ID.
            // Thus, no square will be added twice.
            int[] xp = new int[XSize * YSize], yp = new int[XSize * YSize];
            int k = 0, n = 0;

            // Add the given square to the list.
            if (squares[x, y] == 1)
            {
                xp[n] = x; yp[n] = y; n++;
                squares[x, y] = id; result++;
            }

            // Scan all squares in the list.
            while (k < n)
            {
                x = xp[k]; y = yp[k]; k++;

                if (x > 0 && squares[x - 1, y] == 1)
                {
                    xp[n] = x - 1; yp[n] = y; n++;
                    squares[x - 1, y] = id; result++;
                }
                if (x + 1 < XSize && squares[x + 1, y] == 1)
                {
                    xp[n] = x + 1; yp[n] = y; n++;
                    squares[x + 1, y] = id; result++;
                }
                if (y > 0 && squares[x, y - 1] == 1)
                {
                    xp[n] = x; yp[n] = y - 1; n++;
                    squares[x, y - 1] = id; result++;
                }
                if (y + 1 < YSize && squares[x, y + 1] == 1)
                {
                    xp[n] = x; yp[n] = y + 1; n++;
                    squares[x, y + 1] = id; result++;
                }
            }

            return result;
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
