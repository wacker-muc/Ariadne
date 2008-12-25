using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Utilities
{
    public static class Geometry
    {
        public static int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                int t = a % b;
                a = b;
                b = t;
            }

            return a;
        }

        public static void RectToPolar(double x, double y, out double r, out double phi)
        {
            r = Math.Sqrt(x * x + y * y);
            phi = Math.Atan2(y, x);
        }

        public static void PolarToRect(double r, double phi, out double x, out double y)
        {
            x = r * Math.Cos(phi);
            y = r * Math.Sin(phi);
        }
    }
}
