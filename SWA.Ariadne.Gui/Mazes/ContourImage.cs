using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Mazes
{
    /// <summary>
    /// Represents the actually painted area of an image, subtracting the background.
    /// The contour of the image is extended by 16 pixels.
    /// If the background is not black (or very dark), the contour edge is further extended by a blurred region.
    /// </summary>
    /// TODO: Extend template so that contour scans cannot leave the range.
    /// TODO: Eliminate tests with iLimit and jLimit.
    /// TODO: Apply bounding box to template in an early stage.
    /// TODO: Collect pixels in lists of identical patterns, then process these lists uniformly.
    public partial class ContourImage
    {
        private const int ContourDistance = 16;
        private const int BlurDistance = 12;

        /// <summary>
        /// Returns the width of a region around the image that should be blurred gradually
        /// from the background color to complete black.
        /// </summary>
        /// <param name="backgroundColor">The darker the background, the smaller the result.</param>
        /// <returns></returns>
        private static int GetBlurDistance(Color backgroundColor)
        {
            float b = backgroundColor.GetBrightness();
            return (int)(Math.Sqrt(b) * BlurDistance);
        }

        /// <summary>
        /// Returns the width of a region around the image that should not be completely black.
        /// That is the ContourDistance plus the BlurDistance for the given background color.
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        private static int GetFrameWidth(Color backgroundColor)
        {
            return ContourDistance + GetBlurDistance(backgroundColor);
        }

        /// <summary>
        /// Returns an image with the background color set to black.
        /// The background is guessed from the pixels on the image border.
        /// The contour of the image (without its background) is extended by ContourDistance and BlurDistance.
        /// </summary>
        /// <param name="template">the original image</param>
        /// <param name="mask">will be set to the mask that was applied to the template</param>
        /// <returns></returns>
        public static Image CreateFrom(Image template, out Bitmap mask, int algorithm)
        {
            float fuzziness = 0.05F;
            float share;

            Bitmap bitmapTemplate = template as Bitmap;
            if (bitmapTemplate == null)
            {
                bitmapTemplate = new Bitmap(template);
            }

            Color backgroundColor = GuessBackgroundColor(bitmapTemplate, fuzziness, out share);

            if (share < 0.8F)
            {
                mask = null;
                return template;
            }

            Bitmap result = Copy(template, backgroundColor);

            Rectangle bbox;
            fuzziness = 0.10F;
            mask = GetMask(result, backgroundColor, fuzziness, out bbox, algorithm);
            ApplyMask(result, mask);

#if false // TODO: true
            result = Crop(result, bbox);
            mask = Crop(mask, bbox);            
#endif

            return result;
        }

        /// <summary>
        /// Returns the color that is dominant in the image border (width: 2 pixels).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fuzziness">maximum color distance that is considered equal to the dominant color</param>
        /// <param name="share">out: ratio of background color pixels among all pixels (0.0 .. 1.0)</param>
        /// <returns></returns>
        private static Color GuessBackgroundColor(Bitmap image, float fuzziness, out float share)
        {
            int borderWidth = 2;
            int xMin = 0, xMax = image.Width - 1;
            int yMin = 0, yMax = image.Height - 1;

            #region Collect a sample of pixels near the image border.

            List<Color> pixels = new List<Color>(borderWidth * (image.Width + image.Height));

            for (int x = 0; x < image.Width; x += 1 + x % 7)
            {
                pixels.Add(image.GetPixel(x, yMin + (x + 0) % borderWidth));
                pixels.Add(image.GetPixel(x, yMax - (x + 1) % borderWidth));
            }
            for (int y = 0 + borderWidth; y < image.Height - borderWidth; y += 1 + y % 5)
            {
                pixels.Add(image.GetPixel(xMin + (y + 0) % borderWidth, y));
                pixels.Add(image.GetPixel(xMax - (y + 1) % borderWidth, y));
            }

            #endregion

            #region Determine average color of the pixels on the border.

            // Sum of RGB pixel values.
            int rSum = 0, gSum = 0, bSum = 0;

            foreach (Color px in pixels)
            {
                rSum += px.R;
                gSum += px.G;
                bSum += px.B;
            }

            int n = pixels.Count;
            Color result = Color.FromArgb(rSum / n, gSum / n, bSum / n);

            #endregion

            #region Determine number of pixels that are within the fuzziness range.

            int nShare = 0;

            foreach (Color px in pixels)
            {
                if (ColorDistance(result, px) <= fuzziness)
                {
                    ++nShare;
                }
            }

            share = (float)nShare / (float)pixels.Count;

            #endregion

            return result;
        }

        /// <summary>
        /// Copies the given image into a Bitmap that is augmented with a background color frame.
        /// Thus, all image pixels are located at least GetFrameWidth() from the result's border.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        private static Bitmap Copy(Image image, Color backgroundColor)
        {
            int width = image.Width, height = image.Height;

            // Extend the image with a frame of the background color.
            // Thus, all parts of the image are at least that far away from the border.
            int frameWidth = GetFrameWidth(backgroundColor);
            width += 2 * frameWidth;
            height += 2 * frameWidth;

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(width, height, Graphics.FromImage(image));
            Graphics g = Graphics.FromImage(result);
            g.FillRectangle(new SolidBrush(backgroundColor), new Rectangle(0, 0, result.Width, result.Height));
            g.DrawImageUnscaled(image, frameWidth, frameWidth);

            return result;
        }

        /// <summary>
        /// Returns a bitmap to be applied to the given image.
        /// Areas dominated by the background color will be black.
        /// Other areas will be transparent.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        private static Bitmap GetMask(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox, int algorithm)
        {
            switch (algorithm)
            {
                case 0:
                    return GetMask_CompleteScan(image, backgroundColor, fuzziness, out boundingBox);
                case 1:
                    return GetMask_SimpleScan(image, backgroundColor, fuzziness, out boundingBox);
                case 2:
                    return GetMask_OutlineScanA(image, backgroundColor, fuzziness, out boundingBox);
                case 3:
                    return GetMask_OutlineScanB(image, backgroundColor, fuzziness, out boundingBox);
                case 4:
                    return GetMask_BlockScan(image, backgroundColor, fuzziness, out boundingBox);
                default:
                case 5:
                    return GetMask_ContourScan(image, backgroundColor, fuzziness, out boundingBox);
            }
        }

        /// <summary>
        /// Returns a value between 0.0 (identical colors) and 1.0 (opposite colors).
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        /// TODO: Make this an object method; compare with backgroundColor and test against fuzziness.
        private static float ColorDistance(Color col1, Color col2)
        {
#if false
            float d, result = 0;

            float h1 = col1.GetHue(), s1 = col1.GetSaturation(), b1 = col1.GetBrightness();
            float h2 = col2.GetHue(), s2 = col2.GetSaturation(), b2 = col1.GetBrightness();

            d = (s1 - s2) / 1.0F;
            result += d * d;

            d = (b1 - b2) / 1.0F;
            result += d * d;

            d = (h1 - h2) / 180.0F;
            if (d > 1.0F) { d = 2.0F - d; }

            // The hue is less relevant for dim and dark colors.
            if (d > 0.01F && (s1 < 0.5F || b1 < 0.5F || s2 < 0.5F || b2 < 0.5F))
            {
                d *= (s1 * b1);
                d *= (s2 * b1);
            }

            result += d * d;

            return (float)Math.Min(1.0, Math.Sqrt(result));
#else
            int dr = Math.Abs(col1.R - col2.R);
            int dg = Math.Abs(col1.G - col2.G);
            int db = Math.Abs(col1.B - col2.B);

            return (float)Math.Max(dr, Math.Max(dg, db)) / 255.0F;
#endif
        }

        /// <summary>
        /// Draws the given mask over the given image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="mask"></param>
        private static void ApplyMask(Bitmap image, Bitmap mask)
        {
            Graphics g = Graphics.FromImage(image);
            g.DrawImageUnscaled(mask, 0, 0);
        }

        /// <summary>
        /// Returns the image part defined by srcRect.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="srcRect"></param>
        /// <returns></returns>
        private static Bitmap Crop(Bitmap image, Rectangle srcRect)
        {
            if (srcRect.Width < image.Width - 2 || srcRect.Height < image.Height - 2)
            {
                // Create a new Bitmap with the same resolution as the original image.
                Bitmap result = new Bitmap(srcRect.Width, srcRect.Height, Graphics.FromImage(image));
                Graphics g = Graphics.FromImage(result);
                Rectangle destRect = new Rectangle(0, 0, result.Width, result.Height);

                g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
                
                return result;
            }
            else
            {
                return image;
            }
        }
    }
}
