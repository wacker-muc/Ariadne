using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape defined by the contour of a black and white bitmap image.
    /// </summary>
    internal class BitmapOutlineShape : OutlineShape
    {
        #region Class variables

        /// <summary>
        /// List of Property Methods that return bitmap images from the Resources file.
        /// </summary>
        private static List<System.Reflection.MethodInfo> BitmapProperties = SWA.Utilities.Resources.BitmapProperties(typeof(Resources.Bitmaps));

        #endregion

        #region Member variables and Properties

        private Bitmap map;
        private int mapXOffset, mapYOffset;
        private bool hasBlackBackground = false;

        /// <summary>
        /// Returns true if the image's pixel value is darker than 50%.
        /// Images are assumed to be painted black on a white background.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool this[int x, int y]
        {
            get
            {
                int xi = x - mapXOffset, yi = y - mapYOffset;

                if (xi < 0 || yi < 0 || xi >= map.Width || yi >= map.Height)
                {
                    return hasBlackBackground;
                }
                else
                {
                    return (map.GetPixel(xi, yi).GetBrightness() <= 0.5);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an OutlineShape based on a Bitmap image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        private BitmapOutlineShape(Bitmap img, int xSize, int ySize, double centerX, double centerY, double shapeSize)
            : base(xSize, ySize)
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);
            double scale = 2 * sz / Math.Max(img.Width, img.Height);

            this.map = new Bitmap(img, new Size((int)(img.Width * scale), (int)(img.Height * scale)));
            this.mapXOffset = (int)(xc - this.map.Width / 2.0);
            this.mapYOffset = (int)(yc - this.map.Height / 2.0);

            #region Determine the background color: black or white.

            int w = this.map.Width - 1, h = this.map.Height - 1;
            float cornerBrightness = 0;
            cornerBrightness += this.map.GetPixel(0, 0).GetBrightness();
            cornerBrightness += this.map.GetPixel(w, 0).GetBrightness();
            cornerBrightness += this.map.GetPixel(0, h).GetBrightness();
            cornerBrightness += this.map.GetPixel(w, h).GetBrightness();
            this.hasBlackBackground = (cornerBrightness < 0.5F * 4);

            #endregion
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape Random(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            System.Reflection.MethodInfo method = BitmapProperties[r.Next(BitmapProperties.Count)];
            Bitmap img = (Bitmap) method.Invoke(null, null);
            return new BitmapOutlineShape(img, xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
