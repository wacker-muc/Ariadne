using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    internal class FractalOutlineShape : FunctionOutlineShape
    {
        #region Geometric functions, tagged with the TDF attribute

        /// <summary>
        /// Creates a Mandelbrot set fractal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// Symmetry = 4: Will not be rotated.
        /// Scale ~ 5: Compensates the scaling applied in the FunctionOutlineShape constructor.
        [TDF(4, 5.0, 7.0)]
        private static double TDF_01_Mandelbrot(double cr, double ci)
        {
            double xr = 0, xi = 0;

            for (int n = 0; n < 200; n++)
            {
                // In complex coordinates: x is mapped to x^2 - c.

                double xr2 = xr * xr - xi * xi;
                double xi2 = 2 * xr * xi;

                // Apply the cr and ci parameters.  cr is shifted to bring the resulting figure into the shape center.
                xr = xr2 - (cr + 0.5);
                xi = xi2 - ci;

                if (xr * xr + xi * xi > 2.0)
                {
                    return -1;
                }
            }

            return +1;
        }

        #endregion
    }
}
