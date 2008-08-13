using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    internal class RectanglesOutlineShape : OutlineShape
    {
        /// <summary>
        /// An OutlineShape formed of concentric rectangles.
        /// </summary>
        #region Member variables and Properties

        private double xCenter, yCenter;
        private double nStripes;
        private bool crossShaped;
        private double extendCenter;

        public override bool this[int x, int y]
        {
            get
            {
                double sx, sy, s;

                #region Calculate the distance from the center, relative to the distance (center..border).

                if (x <= xCenter)
                {
                    sx = (x - xCenter) / (0 - xCenter);
                }
                else
                {
                    sx = (x - xCenter) / (XSize - xCenter);
                }
                if (y <= yCenter)
                {
                    sy = (y - yCenter) / (0 - yCenter);
                }
                else
                {
                    sy = (y - yCenter) / (YSize - yCenter);
                }

                #endregion

                if (crossShaped)
                {
                    s = Math.Min(sx, sy);
                }
                else
                {
                    s = Math.Max(sx, sy);
                }
                int k = (int)(s * nStripes + extendCenter);

                return (k % 2 == 0);
            }
        }

        #endregion

        #region Constructor

        private RectanglesOutlineShape(int xSize, int ySize, double xCenter, double yCenter, double nStripes, bool crossShaped, double extendCenter)
            : base(xSize, ySize)
        {
            this.xCenter = xCenter;
            this.yCenter = yCenter;
            this.nStripes = nStripes;
            this.crossShaped = crossShaped;
            this.extendCenter = extendCenter;
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape CreateInstance(Random r, int xSize, int ySize, double centerX, double centerY)
        {
            double xc, yc, sz;
            ConvertParameters(xSize, ySize, centerX, centerY, 1.0, out xc, out yc, out sz);
            double dc = Math.Min(Math.Min(xc, xSize - 1 - xc), Math.Min(yc, ySize - 1 - yc));

            int stripeWidth = r.Next(2, 7);

            bool crossShaped = (r.Next(2) == 0);
            double extendCenter = (crossShaped ? 0.5 : 0.25);

            double nStripes = Math.Truncate(dc / stripeWidth);

            // Adjust nStripes to make the center rectangle a little larger.
            nStripes -= extendCenter;
            // Avoid single line artefact at x = 0 or y = 0.
            nStripes *= 0.999;

            return new RectanglesOutlineShape(xSize, ySize, xc, yc, nStripes, crossShaped, extendCenter);
        }

        #endregion
    }
}
