using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Mazes
{
    public partial class ContourImage
    {
        /// <summary>
        /// Returns a bitmap to be applied to the given image.
        /// Inefficient algorithm, scans every single pixel.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        private static Bitmap GetMask_CompleteScan(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = (backgroundColor.GetBrightness() > 0.20 ? BlurDistance : 0);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));
            int xMin = 0, xMax = image.Width - 1;
            int yMin = 0, yMax = image.Height - 1;

            // For every pixel: distance to closest non-background pixel.
            // Actually, the value is the squared length of the diagonal distance.
            int[,] dist2ToImage = new int[image.Width, image.Height];

            #region Initialize dist2ToImage.
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    dist2ToImage[x, y] = int.MaxValue;
                }
            }
            #endregion

            #region Determine the distance of all mask pixels to an image pixel.

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (ColorDistance(pixel, backgroundColor) > fuzziness)
                    {
                        for (int i = Math.Max(xMin, x - contourDist - blurDist); i <= Math.Min(xMax, x + contourDist + blurDist); i++)
                        {
                            for (int j = Math.Max(yMin, y - contourDist - blurDist); j <= Math.Min(yMax, y + contourDist + blurDist); j++)
                            {
                                dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], (x - i) * (x - i) + (y - j) * (y - j));
                            }
                        }
                    }
                }
            }
            #endregion

            #region Set the color of all mask pixels to black with an appropriate transparency; determine the bounding box.

            // Coordinates of the bounding box.
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            Color black = Color.Black;
            black = Color.Pink; // TODO: delete this line
            Color transparent = Color.FromArgb(0, black);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    double dist = Math.Sqrt(dist2ToImage[x, y]);
                    Color maskColor;

                    if (dist > contourDist + blurDist)
                    {
                        maskColor = black;
                    }
                    else
                    {
                        bbxMin = Math.Min(bbxMin, x);
                        bbxMax = Math.Max(bbxMax, x);
                        bbyMin = Math.Min(bbyMin, y);
                        bbyMax = Math.Max(bbyMax, y);

                        if (dist <= contourDist)
                        {
                            maskColor = transparent;
                        }
                        else
                        {
                            int a = (int)(255 * (dist - contourDist) / blurDist);
                            maskColor = Color.FromArgb(a, black);
                        }
                    }

                    result.SetPixel(x, y, maskColor);
                }
            }

            boundingBox = new Rectangle(bbxMin, bbyMin, bbxMax - bbxMin + 1, bbyMax - bbyMin + 1);

            #endregion

            return result;
        }
    }
}
