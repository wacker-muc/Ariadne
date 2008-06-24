using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace SWA.Ariadne.Outlines
{
    public class ImageOutlineShape : OutlineShape
    {
        #region Class variables

        static List<System.Reflection.MethodInfo> BitmapProperties;

        #endregion

        #region Class Constructor

        static ImageOutlineShape()
        {
            BitmapProperties = new List<System.Reflection.MethodInfo>();
#if false
            // This fails in the class constructor and returns null.  :-(
            Type bitmapType = System.Type.GetType("System.Drawing.Bitmap");
#else
            Bitmap bitmap = new Bitmap(1, 1);
            Type bitmapType = bitmap.GetType();
#endif

            System.Type resourcesType = System.Type.GetType("SWA.Ariadne.Outlines.Properties.Resources");
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            foreach (System.Reflection.PropertyInfo info in resourcesType.GetProperties(flags))
            {
                Type propertyType = info.PropertyType;
                if (bitmapType.IsAssignableFrom(propertyType))
                {
                    BitmapProperties.Add(info.GetGetMethod(true));
                }
            }
        }

        #endregion

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
        /// Create an OutlineShape based on a Bitmap image.
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

        /// <summary>
        /// Create an OutlineShape based on a Bitmap image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="xSize">width of the created shape</param>
        /// <param name="ySize">height of the created shape</param>
        /// <param name="centerX">X coordinate, relative to total width; 0.0 = top, 1.0 = bottom</param>
        /// <param name="centerY">Y coordinate, relative to total height; 0.0 = left, 1.0 = right</param>
        /// <param name="shapeSize">size, relative to distance of center from the border; 1.0 will touch the border</param>
        private ImageOutlineShape(Bitmap img, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xc, out yc, out sz);
            double scale = 2 * sz / Math.Max(img.Width, img.Height);

            this.img = new Bitmap(img, new Size((int)(img.Width * scale), (int)(img.Height * scale)));
            this.imgXOffset = (int)(xc - this.img.Width / 2.0);
            this.imgYOffset = (int)(yc - this.img.Height / 2.0);
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape Random(Random r, int xSize, int ySize, double centerX, double centerY, double shapeSize)
        {
            System.Reflection.MethodInfo method = BitmapProperties[r.Next(BitmapProperties.Count)];
            Bitmap img = (Bitmap) method.Invoke(null, null);
            return new ImageOutlineShape(img, xSize, ySize, centerX, centerY, shapeSize);
        }

        #endregion
    }
}
