using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Utilities
{
    /// <summary>
    /// Provides methods for defining colors in the HSB color model.
    /// </summary>
    public static class ColorBuilder
    {
        public const float MaxHue = 360.0F;
        public const float MaxSaturation = 1.0F;
        public const float MaxBrightness = 1.0F;

        /// <summary>
        /// Convert from HSB color coefficients to a .NET Color.
        /// </summary>
        /// <param name="h">hue, 0.0 .. 360.0</param>
        /// <param name="s">saturation, 0.0 .. 1.0</param>
        /// <param name="b">brightness, 0.0 .. 1.0</param>
        /// <returns></returns>
        /// Code copied and adapted from http://blogs.msdn.com/cjacks/archive/2006/04/12/575476.aspx
        public static Color ConvertHSBToColor(float h, float s, float b)
        {
            int a = 255;

            if (!(0 <= h && h <= MaxHue))
            {
                throw new ArgumentOutOfRangeException("h", h,
                  "invalid hue");
            }
            if (!(0 <= s && s <= MaxSaturation))
            {
                throw new ArgumentOutOfRangeException("s", s,
                  "invalid saturation");
            }
            if (!(0 <= b && b <= MaxBrightness))
            {
                throw new ArgumentOutOfRangeException("b", b,
                  "invalid brightness");
            }

            if (0 == s)
            {
                return Color.FromArgb(a, Convert.ToInt32(b * 255),
                  Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(a, iMax, iMid, iMin);
            }
        }

        /// <summary>
        /// Returns two random colors.
        /// Both saturation and brightness values will be in the range given by the two reference colors.
        /// The hues will have a minimum distance (one quadrant).
        /// </summary>
        /// <param name="ref1"></param>
        /// <param name="ref2"></param>
        /// <param name="forwardColor">brighter color</param>
        /// <param name="backwardColor">darker color</param>
        public static void SuggestColors(Color ref1, Color ref2, out Color forwardColor, out Color backwardColor)
        {
            Random r = RandomFactory.CreateRandom();

            float hDist = 0.25F * MaxHue;
            float sDist = 0.50F * MaxSaturation;
            float bDist = 0.50F * MaxBrightness;

            float sMin = Math.Min(ref1.GetSaturation(), ref2.GetSaturation());
            float sMax = Math.Max(ref1.GetSaturation(), ref2.GetSaturation());
            float bMin = Math.Min(ref1.GetBrightness(), ref2.GetBrightness());
            float bMax = Math.Max(ref1.GetBrightness(), ref2.GetBrightness());

            float sRange = sMax - sMin;
            float bRange = bMax - bMin;

            #region Choose a random backward color, using the HSB model

            float h = ColorBuilder.MaxHue;
            float s = sMin;
            float b = bMin;

            h *= (float)r.NextDouble();
            s += (float)(((1.0F - sDist) * sRange) * r.NextDouble());
            b += (float)(((1.0F - bDist) * bRange) * r.NextDouble());

            backwardColor = ColorBuilder.ConvertHSBToColor(h, s, b);

            #endregion

            #region Choose a random forward color: at a distance and brighter than the backward color

            // Minimum hue distance: 1/4 of the hue range.
            float hueMin = h + hDist;
            float hueMax = h - hDist + ColorBuilder.MaxHue;

            h = hueMin + (hueMax - hueMin) * (float)r.NextDouble();
            if (h > ColorBuilder.MaxHue) { h -= ColorBuilder.MaxHue; }

            s += sDist * sRange;
            b += bDist * bRange;

            //saturation += (float)((maxSaturation - saturation) * r.NextDouble());
            //brightness += (float)((maxBrightness - brightness) * r.NextDouble());

            forwardColor = ColorBuilder.ConvertHSBToColor(h, s, b);

            #endregion
        }

        /// <summary>
        /// Returns the (absolute) difference of two hue values.
        /// The result is between 0.0 and 0.5 * MaxHue.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static float HueDifference(float a, float b)
        {
            float result = Math.Abs(a - b);
            if (result > ColorBuilder.MaxHue / 2)
            {
                result = ColorBuilder.MaxHue - result;
            }
            return result;
        }
    }
}
