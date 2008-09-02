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
        /// Efficient algorithm that fails on complicated concave shapes.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        private static Bitmap GetMask_SimpleScan(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = (backgroundColor.GetBrightness() > 0.20 ? BlurDistance : 0);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));
            int xMin = 0, xMax = image.Width - 1;
            int yMin = 0, yMax = image.Height - 1;

#if true
            // Don't process pixels at the very border.
            xMin += 1; xMax -= 1; yMin += 1; xMin -= 1;
#endif

            #region Find the outermost image pixel on every vertical and horizontal line.

            int[] yTop = new int[image.Width];
            int[] yBottom = new int[image.Width];
            int[] xLeft = new int[image.Height];
            int[] xRight = new int[image.Height];

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                    {
                        yTop[x] = y;
                        break;
                    }
                    yTop[x] = yMax + 1; yBottom[x] = -1;
                }
                for (int y = yMax; y >= yTop[x]; y--)
                {
                    if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                    {
                        yBottom[x] = y;
                        break;
                    }
                }
            }
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                    {
                        xLeft[y] = x;
                        break;
                    }
                    xLeft[y] = xMax + 1; xRight[y] = -1;
                }
                for (int x = xMax; x >= xLeft[y]; x--)
                {
                    if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                    {
                        xRight[y] = x;
                        break;
                    }
                }
            }

            #endregion

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

            for (int x = xMin; x <= xMax; x++)
            {
                int y1 = yTop[x];
                int y2 = yBottom[x];

                if (y1 > y2)
                {
                    continue;
                }

                for (int i = Math.Max(xMin, x - contourDist - blurDist); i <= Math.Min(xMax, x + contourDist + blurDist); i++)
                {
                    // Top half circle.
                    for (int j = y1; j >= Math.Max(yMin, y1 - contourDist - blurDist); j--)
                    {
                        dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], (x - i) * (x - i) + (y1 - j) * (y1 - j));
                    }

                    // Bottom half circle.
                    for (int j = y2; j <= Math.Min(yMax, y2 + contourDist + blurDist); j++)
                    {
                        dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], (x - i) * (x - i) + (y2 - j) * (y2 - j));
                    }

                    // Column inbetween.
                    int d = (x - i) * (x - i);
                    for (int j = y1 + 1; j <= y2 - 1; j++)
                    {
                        if (xLeft[j] <= x && x <= xRight[j])
                        {
                            dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], d);
                        }
                    }
                }
            }

            for (int y = yMin; y <= yMax; y++)
            {
                int x1 = xLeft[y];
                int x2 = xRight[y];

                if (x1 > x2)
                {
                    continue;
                }

                for (int j = Math.Max(yMin, y - contourDist - blurDist); j <= Math.Min(yMax, y + contourDist + blurDist); j++)
                {
                    // Top half circle.
                    for (int i = x1; i >= Math.Max(xMin, x1 - contourDist - blurDist); i--)
                    {
                        dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], (x1 - i) * (x1 - i) + (y - j) * (y - j));
                    }

                    // Bottom half circle.
                    for (int i = x2; i <= Math.Min(xMax, x2 + contourDist + blurDist); i++)
                    {
                        dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], (x2 - i) * (x2 - i) + (y - j) * (y - j));
                    }

                    // Bar inbetween.
                    int d = (y - j) * (y - j);
                    for (int i = x1 + 1; i <= x2 - 1; i++)
                    {
                        if (yTop[i] <= y && y <= yBottom[i])
                        {
                            dist2ToImage[i, j] = Math.Min(dist2ToImage[i, j], d);
                        }
                    }
                }
            }

            #endregion

            #region Set the color of all mask pixels to black with an appropriate transparency; determine the bounding box.

            // Coordinates of the bounding box.
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            Color black = Color.Black;
            black = Color.Magenta; // TODO: delete this line
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
