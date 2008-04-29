using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Model
{
    public class OutlineShape
    {
        #region Member variables and Properties

        public bool this[int x, int y]
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

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Circle(int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
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
        }

        /// <summary>
        /// Create an outline shape.
        /// </summary>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border </param>
        /// <returns></returns>
        public static OutlineShape Diamond(int xSize, int ySize, double centerX, double centerY, double shapeSize)
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
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        /// <param name="ch">character</param>
        /// <returns></returns>
        public static OutlineShape Char(int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            char ch = 'X';
            FontFamily fontFamily = new FontFamily("Helvetica");

            OutlineShape result = new OutlineShape(xSize, ySize);

            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            #region Find a font with the desired height.
            
            Font font = new Font(fontFamily, (float)(2 * sz), FontStyle.Bold);
            int desiredHeight = (int)(2.5 * sz);
            for (float a = (float)sz, b = (float)(3 * sz); b - a > 0.5; )
            {
                float m = (a + b) / 2;
                font = new Font(fontFamily, m);
                if (font.Height > desiredHeight)
                {
                    b = m;
                }
                else
                {
                    a = m;
                }
            }
            
            #endregion

            #region Draw the given character into an image (white on black).

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Bitmap img = new Bitmap(xSize, ySize);
            Graphics g = Graphics.FromImage(img);
            g.DrawRectangle(Pens.Black, 0, 0, xSize, ySize);
            g.DrawString(new string(ch, 1), font, Brushes.White, new RectangleF(0, 0, xSize, ySize), stringFormat);

            #endregion

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    double dx = Math.Abs(x - xc), dy = Math.Abs(y - yc);
                    result.squares[x, y] = (img.GetPixel(x,y).GetBrightness() > 0.5);
                }
            }

            return result;
        }

        #endregion

        #region Auxiliary methods

        private static void ConvertParameters(int xSize, int ySize, double centerX, double centerY, double shapeSize, out double xc, out double yc, out double sz)
        {
            // Determine center coordinates in the shape coordinate system.
            xc = xSize * centerX;
            yc = ySize * centerY;

            // Determine r so that a circle would touch the nearest border.
            // If the center is beyond a border, that border is not considered.
            sz = double.MaxValue;
            if (xc > 0) sz = Math.Min(sz, xc);
            if (yc > 0) sz = Math.Min(sz, yc);
            if (xc < xSize) sz = Math.Min(sz, xSize - xc);
            if (yc < ySize) sz = Math.Min(sz, ySize - yc);

            // Multiply with the requested ratio.
            sz *= shapeSize;
        }

        #endregion
    }
}
