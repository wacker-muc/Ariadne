using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// Applies a continuous transformation to the coordinate system of an underlying shape.
    /// </summary>
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

        /// <summary>
        /// Affects a straight line through the given center so that it forms a spiral.
        /// </summary>
        /// <param name="xCenter">Center of distortion.</param>
        /// <param name="yCenter">Center of Distortion.</param>
        /// <param name="size">Radius of the reference circle.</param>
        /// <param name="winding">Points on the reference circle are wound by winding*2*PI radians.</param>
        /// <returns></returns>
        public static Distortion SpiralDistortion(double xCenter, double yCenter, double size, double winding)
        {
            return delegate(ref double x, ref double y)
            {
                double r, phi;
                RectToLocalPolar(xCenter, yCenter, x, y, out r, out phi);

                phi += r * winding / size * 2.0 * Math.PI;
                
                LocalPolarToRect(xCenter, yCenter, r, phi, out x, out y);
            };
        }

        /// <summary>
        /// Affects a circle around the given center so that it is indented towards the center.
        /// </summary>
        /// <param name="xCenter">Center of distortion.</param>
        /// <param name="yCenter">Center of Distortion.</param>
        /// <param name="waveCount">There will be waveCount "corners" that are not distorted.</param>
        /// <param name="waveShift">When 0, the points at (2*PI)/(n*waveCount) radians will not be distorted.</param>
        /// <param name="minRatio">The distance from the center will be multiplied by a value between minRatio and 1.</param>
        /// <returns></returns>
        public static Distortion RadialWaveDistortion(double xCenter, double yCenter, int waveCount, double waveShift, double minRatio)
        {
            return delegate(ref double x, ref double y)
            {
                double r, phi;
                RectToLocalPolar(xCenter, yCenter, x, y, out r, out phi);
                double phi0 = phi;

                phi -= waveShift * 2.0 * Math.PI;
                phi *= waveCount;
                double k = Math.Max(-0.5, Math.Min(0.999, (1 - minRatio)));
                double f = Math.Cos(phi);       // -1 .. +1, f(0) = +1
                double g = 0.5 * k * (1 - f);   // 0 .. k, g(0) = 0
                double h = 1 - g;               // 1-k .. 1, h(0) = 1
                r /= h;

                LocalPolarToRect(xCenter, yCenter, r, phi0, out x, out y);
            };
        }

        #endregion

        #region Auxiliary methods

        private static void RectToLocalPolar(double xCenter, double yCenter, double x, double y, out double r, out double phi)
        {
            SWA.Utilities.Geometry.RectToPolar(x - xCenter, y - yCenter, out r, out phi);
        }

        private static void LocalPolarToRect(double xCenter, double yCenter, double r, double phi, out double x, out double y)
        {
            SWA.Utilities.Geometry.PolarToRect(r, phi, out x, out y);

            x += xCenter;
            y += yCenter;
        }

        #endregion
    }
}
