using System;
using System.Collections.Generic;
using System.Text;
using SWA.Utilities;

namespace SWA.Ariadne.Outlines
{
    internal class FractalOutlineShape : FunctionOutlineShape
    {
        #region Geometric functions, tagged with the TDF attribute

        /// <summary>
        /// Creates a Mandelbrot set fractal.
        /// </summary>
        /// <param name="cr">parameter c, real part</param>
        /// <param name="ci">parameter c, imaginary part</param>
        /// <returns>
        /// +1 for the inside of the Mandelbrot set;
        /// -k for the outside, where k is the number of iterations until escaping
        /// </returns>
        /// Symmetry = 4: Will not be rotated.
        /// Scale ~ 5: Compensates for the scaling applied in the FunctionOutlineShape constructor.
        [TDF(4, 5.0, 7.0)]
        private static double Mandelbrot(double cr, double ci)
        {
            double zr = 0, zi = 0;

            int n = 200;
            for (int k = 1; k <= n; k++)
            {
                // In complex coordinates: x is mapped to x^2 - c.

                double xr2 = zr * zr - zi * zi;
                double xi2 = 2 * zr * zi;

                // Apply the cr and ci parameters.  cr is shifted to bring the resulting figure into the shape center.
                zr = xr2 + (cr - 0.5);
                zi = xi2 + ci;

                if (zr * zr + zi * zi > 2.0)
                {
                    return -k;
                }
            }

            return +1;
        }

        /// <summary>
        /// Creates a Julia set fractal.
        /// </summary>
        /// <param name="cr">parameter c, real part</param>
        /// <param name="ci">parameter c, imaginary part</param>
        /// <returns>
        /// +1 for the inside of the Julia set;
        /// -k for the outside, where k is the number of iterations until escaping
        /// </returns>
        /// Symmetry = 4: Will not be rotated.
        /// Scale ~ 4: Compensates for the scaling applied in the FunctionOutlineShape constructor.
        [TDF(4, 4.0, 5.0)]
        private static double Julia(double zr, double zi)
        {
            // The escape limit is rather low to get a thicker shape.
            // Thus, we avoid that the shape breaks up into several not connected parts.
            int n = 20;
            for (int k = 1; k <= n; k++)
            {
                // In complex coordinates: x is mapped to x^2 - c.

                double zr2 = zr * zr - zi * zi;
                double zi2 = 2 * zr * zi;

                // Apply the Mandelbrot coordinate parameters.
                zr = zr2 + t1;
                zi = zi2 + t2;

                if (zr * zr + zi * zi > 2.0)
                {
                    // k < n
                    return ((double)(k - n) / (double)n);
                }
            }

            return +1;
        }

        /// <summary>
        /// Chooses a value for the parameters t1 and t2.
        /// (t1, t2) is a point close to the border of the Mandelbrot set.
        /// </summary>
        /// <param name="r"></param>
        internal static void JuliaConfigurator(Random r, double squareWidth)
        {
            #region Find a point close to the Mandelbrot border.

            // Start with two random points, one inside and one outside of the Mandelbrot set.
            double cr0, ci0; // inside
            double cr1, ci1; // outside
            double phi;
            phi = 2.0 * Math.PI * r.NextDouble();
            Geometry.PolarToRect(0.099, phi, out cr0, out ci0);
            phi = 2.0 * Math.PI * r.NextDouble();
            Geometry.PolarToRect(2.001, phi, out cr1, out ci1);
            cr0 += 0.5;
            cr1 += 0.5;

            // Bisect the interval [c0..c1] until the distance is less than 10^-6.
            for (int k = 0; k < 20; k++)
            {
                // Determine whether the interval's midpoint is inside or outside.
                double cr2 = 0.5 * (cr0 + cr1), ci2 = 0.5 * (ci0 + ci1);
                double m0 = Mandelbrot(cr0, ci1);
                double m1 = Mandelbrot(cr1, ci1);
                double m = Mandelbrot(cr2, ci2);

                if (m > 0) // inside
                {
                    cr0 = cr2;
                    ci0 = ci2;
                }
                else
                {
                    // Stop when we are close enough to the border.
                    if (m < -20 && k > 6)
                    {
                        break;
                    }

                    cr1 = cr2;
                    ci1 = ci2;
                }
            }

            #endregion

            // Set the Julia set parameters t1 and t2 to the selected (inside) Mandelbrot point coordinates.
            t1 = cr0 - 0.5; // see the coordinate translation in Mandelbrot()
            t2 = ci0;
        }

        #endregion
    }
}
