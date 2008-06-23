using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    public class ImageOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        private Bitmap img;
        private int imgXOffset, imgYOffset;
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
                int xi = x - imgXOffset, yi = y - imgYOffset;

                if (xi < 0 || yi < 0 || xi >= img.Width || yi >= img.Height)
                {
                    return hasBlackBackground;
                }
                else
                {
                    return (img.GetPixel(xi, yi).GetBrightness() <= 0.5);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an OutlineShape based on a Bitmap image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xImg">X location of the image in shape coordinates</param>
        /// <param name="yImg">Y location of the image in shape coordinates</param>
        private ImageOutlineShape(Bitmap img, int xImg, int yImg)
        {
            this.img = img;
            this.imgXOffset = xImg;
            this.imgYOffset = yImg;
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape SouthAmerica(int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);

            Bitmap img = Properties.Resources.SouthAmerica;
            double scale = 2 * sz / Math.Max(img.Width, img.Height);
            img = new Bitmap(img, new Size((int)(img.Width * scale), (int)(img.Height * scale)));

            int xImg = (int)(xc - img.Width / 2.0);
            int yImg = (int)(yc - img.Height / 2.0);

            return new ImageOutlineShape(img, xImg, yImg);
        }

        #endregion
    }
}
