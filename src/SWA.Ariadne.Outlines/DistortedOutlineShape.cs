using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    internal class DistortedOutlineShape : SmoothOutlineShape
    {
        #region Member variables and Properties

        private SmoothOutlineShape baseShape;

        public delegate void Distortion(ref double x, ref double y);

        private Distortion distortion;

        public override bool this[double x, double y]
        {
            get 
            {
                this.distortion(ref x, ref y);
                return baseShape[x, y];
            }
        }

        #endregion

        #region Constructor

        internal DistortedOutlineShape(int xSize, int ySize, SmoothOutlineShape baseShape, Distortion distortion)
            : base(xSize, ySize)
        {
            this.baseShape = baseShape;
            this.distortion = distortion;
        }

        #endregion

        #region Static methods returning Distortion delegates

        public static Distortion SpiralDistortion(Random r, double xCenter, double yCenter, double size, double maxWindings)
        {
            double winding = (0.33 + 0.66 * r.NextDouble()) * maxWindings;
            winding = maxWindings;
            if (r.Next(2) == 0)
            {
                winding *= -1;
            }

            return SpiralDistortion(xCenter, yCenter, size, winding);
        }

        public static Distortion SpiralDistortion(double xCenter, double yCenter, double size, double winding)
        {
#if true
            System.Console.WriteLine(string.Format("SpiralDistortion(@({0:0.##},{1:0.##}), ={2:0.##})", xCenter, yCenter, size));
#endif

            return delegate(ref double x, ref double y)
            {
                double radius, phi;

                x -= xCenter;
                y -= yCenter;

                SWA.Utilities.Geometry.RectToPolar(x, y, out radius, out phi);
                phi += radius * winding / size * 2.0 * Math.PI;
                SWA.Utilities.Geometry.PolarToRect(radius, phi, out x, out y);

                x += xCenter;
                y += yCenter;
            };
        }

        #endregion
    }
}
