using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Outlines
{
    internal class RectanglesOutlineShape : OutlineShape
    {
        /// <summary>
        /// An OutlineShape formed of concentric rectangles.
        /// </summary>
        #region Member variables and Properties

        private RectangleF nucleus;
        private float nStripes;
        private bool crossShaped;

        public override bool this[int x, int y]
        {
            get
            {
                float sx, sy, s;

                #region Calculate the distance from the center, relative to the distance (center..border).

                if (x < nucleus.Left)
                {
                    sx = (x - nucleus.Left) / (0 - nucleus.Left);
                }
                else if (x > nucleus.Right)
                {
                    sx = (x - nucleus.Right) / (XSize - nucleus.Right);
                }
                else
                {
                    sx = -0.9F / nStripes;
                }

                if (y < nucleus.Top)
                {
                    sy = (y - nucleus.Top) / (0 - nucleus.Top);
                }
                else if (y > nucleus.Bottom)
                {
                    sy = (y - nucleus.Bottom) / (YSize - nucleus.Bottom);
                }
                else
                {
                    sy = -0.9F / nStripes;
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
                int k = (int)(1.0F + s * nStripes);

                return (k % 2 == 0);
            }
        }

        #endregion

        #region Constructor

        private RectanglesOutlineShape(int xSize, int ySize, RectangleF nucleus, float nStripes, bool crossShaped)
            : base(xSize, ySize)
        {
            this.nucleus = nucleus;
            this.nStripes = nStripes;
            this.crossShaped = crossShaped;
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
            bool elongatedNucleus = (!crossShaped && (r.Next(2) == 0));

            float nStripes = (float)Math.Truncate(dc / stripeWidth);

            // The center rectangle.
            RectangleF nucleus = new RectangleF();

            // Start with a square of size stripeWidth.
            nucleus.X = -0.5F * stripeWidth;
            nucleus.Y = -0.5F * stripeWidth;
            nucleus.Width = stripeWidth;
            nucleus.Height = stripeWidth;

            // Extend the edge parallel to the larger shape dimension.
            float aspect = (float)xSize / (float)ySize;

            if (aspect > 1)
            {
                if (elongatedNucleus)
                {
                    nucleus.Width += (xSize - ySize);
                    nucleus.X -= 0.5F * (xSize - ySize);
                }
                else
                {
                    nucleus.X *= aspect;
                    nucleus.Width *= aspect;
                }
            }
            else
            {
                if (elongatedNucleus)
                {
                    nucleus.Height += (ySize - xSize);
                    nucleus.Y -= 0.5F * (ySize - xSize);
                }
                else
                {
                    nucleus.Y /= aspect;
                    nucleus.Height /= aspect;
                }
            }

            if (!crossShaped)
            {
                // Make the center rectangle a little larger.
                nucleus.X -= 0.25F * stripeWidth;
                nucleus.Y -= 0.25F * stripeWidth;
                nucleus.Width += 0.5F * stripeWidth;
                nucleus.Height += 0.5F * stripeWidth;
            }

#if false
            if (crossShaped && r.Next(2) == 0)
            {
                // Use a zero-size nucleus.
                nucleus = new RectangleF(0F, 0F, 0F, 0F);
            }
#endif

            // Move the center rectangle to the given center location.
            nucleus.X += (float)xc;
            nucleus.Y += (float)yc;

            // Avoid single line artefact at x = 0 or y = 0.
            nStripes *= 0.999F;

            return new RectanglesOutlineShape(xSize, ySize, nucleus, nStripes, crossShaped);
        }

        #endregion
    }
}
