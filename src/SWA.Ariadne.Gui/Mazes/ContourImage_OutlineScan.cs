using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Mazes
{
    public partial class ContourImage
    {
        // Single position patterns.
        const UInt16 E = 1, N = 2, W = 4, S = 8, NE = 16, NW = 32, SW = 64, SE = 128, C = 256;

        // Quadrant patterns.
        const UInt16 QNW = C + N + W + NW;
        const UInt16 QNE = C + N + E + NE;
        const UInt16 QSW = C + S + W + SW;
        const UInt16 QSE = C + S + E + SE;

        // Horizontal and vertical line patterns.
        const UInt16 LH = W + C + E, LV = N + C + S;

        // Hemisphere patterns.
        const UInt16 HN = LH + N, HS = LH + S, HE = LV + E, HW = LV + W;

        /// <summary>
        /// Returns a bitmap to be applied to the given image.
        /// Rather efficient algorithm; leaves small areas uncovered.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        private static Bitmap GetMask_OutlineScanA(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = (backgroundColor.GetBrightness() > 0.20 ? BlurDistance : 0);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));

            // A bit pattern describing a pixel and its immediate surroundings.
            // Image pixels are represented as set bits, background pixels are cleared bits.
            UInt16[,] pattern = new UInt16[image.Width, image.Height];


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

            // Derive the shape patterns for every image pixel.
            ExtractPattern(image, backgroundColor, fuzziness, pattern, dist2ToImage);
            NormalizePattern(pattern);

            #region Determine the distance of all mask pixels to an image pixel.

            int x0 = 0, x1 = image.Width - 1;
            int y0 = 0, y1 = image.Height - 1;

            #region Scan for pixels that influence a horizontal or vertical line.

            ScanHemisphere(pattern, HW, LV, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            ScanHemisphere(pattern, HE, LV, x1, x0, y0, y1, dist2ToImage, contourDist + blurDist);
            ScanHemisphere(pattern, HN, LH, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            ScanHemisphere(pattern, HS, LH, x0, x1, y1, y0, dist2ToImage, contourDist + blurDist);

            #endregion

            #region Scan for pixels that influence one of the four quadrants.

            ScanOctantsAndDiagonal(pattern, QNW, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            ScanOctantsAndDiagonal(pattern, QNE, x1, x0, y0, y1, dist2ToImage, contourDist + blurDist);
            ScanOctantsAndDiagonal(pattern, QSW, x0, x1, y1, y0, dist2ToImage, contourDist + blurDist);
            ScanOctantsAndDiagonal(pattern, QSE, x1, x0, y1, y0, dist2ToImage, contourDist + blurDist);

            #endregion

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

        /// <summary>
        /// Returns a bitmap to be applied to the given image.
        /// Rather efficient algorithm (hopefully).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        private static Bitmap GetMask_OutlineScanB(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = GetBlurDistance(backgroundColor);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));

            // A bit pattern describing a pixel and its immediate surroundings.
            // Image pixels are represented as set bits, background pixels are cleared bits.
            UInt16[,] pattern = new UInt16[image.Width, image.Height];


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

            // Derive the shape patterns for every image pixel.
            ExtractPattern(image, backgroundColor, fuzziness, pattern, dist2ToImage);
            NormalizePattern(pattern);

            #region Determine the distance of all mask pixels to an image pixel.

            int x0 = 0, x1 = image.Width - 1;
            int y0 = 0, y1 = image.Height - 1;

            #region Scan for pixels that influence a horizontal or vertical line.

            // ScanHemisphere(pattern, HW, LV, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            // ScanHemisphere(pattern, HE, LV, x1, x0, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanHemisphere(pattern, HN, LH, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanHemisphere(pattern, HS, LH, x0, x1, y1, y0, dist2ToImage, contourDist + blurDist);

            #endregion

            #region Scan for pixels that influence one of the four quadrants.

            ScanQuadrant(pattern, QNW, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanQuadrant(pattern, QNE, x1, x0, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanQuadrant(pattern, QSW, x0, x1, y1, y0, dist2ToImage, contourDist + blurDist);
            //ScanQuadrant(pattern, QSE, x1, x0, y1, y0, dist2ToImage, contourDist + blurDist);

            #endregion

            #region Scan for pixels that influence only the diagonal in one of the four quadrants.

            // ScanDiagonal(pattern, QNW, x0, x1, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanDiagonal(pattern, QNE, x1, x0, y0, y1, dist2ToImage, contourDist + blurDist);
            //ScanDiagonal(pattern, QSW, x0, x1, y1, y0, dist2ToImage, contourDist + blurDist);
            //ScanDiagonal(pattern, QSE, x1, x0, y1, y0, dist2ToImage, contourDist + blurDist);

            #endregion

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

        private static void ScanHemisphere(UInt16[,] pattern, UInt16 hMask, UInt16 hPattern, int x0, int x1, int y1, int y0, int[,] dist2ToImage, int dist)
        {
            // Scan direction.
            int dx = (x0 < x1 ? +1 : -1);
            int dy = (y0 < y1 ? +1 : -1);

            // Influence direction.
            int di = 0, dj = 0, iLimit = -1, jLimit = -1;
            if ((hMask & E) == 0) { di = -1; }
            if ((hMask & W) == 0) { di = +1; iLimit = pattern.GetLength(0); }
            if ((hMask & S) == 0) { dj = -1; }
            if ((hMask & N) == 0) { dj = +1; jLimit = pattern.GetLength(1); }

            for (int x = x0; x != x1; x += dx)
            {
                for (int y = y0; y != y1; y += dy)
                {
                    UInt16 p = pattern[x, y];

                    if ((p & hMask) == hPattern)
                    {
                        for (int d = 1, i = x + di, j = y + dj; d < dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                        {
                            int d2i = d * d;
                            if (dist2ToImage[i, j] > d2i)
                            {
                                dist2ToImage[i, j] = d2i;
                            }
                            else
                            {
                                // The remaining points are stronger influenced by other pixels.
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void ScanOctantsAndDiagonal(UInt16[,] pattern, UInt16 qMask, int x0, int x1, int y0, int y1, int[,] dist2ToImage, int dist)
        {
            int dx = (x0 < x1 ? +1 : -1);
            int dy = (y0 < y1 ? +1 : -1);

            UInt16 X = 0, Y = 0;
            switch (qMask)
            {
                case QNW:
                    X = SW; Y = NE;
                    break;
                case QNE:
                    X = SE; Y = NW;
                    break;
                case QSW:
                    X = NW; Y = SE;
                    break;
                case QSE:
                    X = NE; Y = SW;
                    break;
            }

            // Influence direction.
            int di = -dx, dj = -dy, iLimit = -1, jLimit = -1;
            if (di > 0) { iLimit = pattern.GetLength(0); }
            if (dj > 0) { jLimit = pattern.GetLength(1); }

            for (int x = x0; x != x1; x += dx)
            {
                for (int y = y0; y != y1; y += dy)
                {
                    UInt16 p = pattern[x, y];

                    if ((p & qMask) == C)
                    {
                        if ((p & (X + Y)) == (X + Y))
                        {
#if true
                            // The pixel influences the quadrant's diagonal.
                            for (int d = 1, i = x + di, j = y + dj; 2 * d * d < dist * dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                            {
                                bool doBreak = true;

                                int d2i = 2 * d * d;
                                if (dist2ToImage[i, j] > d2i)
                                {
                                    dist2ToImage[i, j] = d2i;
                                    doBreak = false;
                                }

                                d2i = d * d + (d - 1) * (d - 1);
                                if (dist2ToImage[i - di, j] > d2i)
                                {
                                    dist2ToImage[i - di, j] = d2i;
                                    doBreak = false;
                                }
                                if (dist2ToImage[i, j - dj] > d2i)
                                {
                                    dist2ToImage[i, j - dj] = d2i;
                                    doBreak = false;
                                }

                                // The remaining points are stronger influenced by other pixels.
                                if (doBreak) break;
                            }
#endif
                        }
                        else
                        {
                            if ((p & X) == 0)
                            {
#if true
                                // The pixel influences the octant by the X axis.
                                for (int j = y, i0 = x, i1 = x + dist * di; j != jLimit; j += dj)
                                {
                                    for (int i = i0; i != i1 && i != iLimit; i += di)
                                    {
                                        int d2i = (i - x) * (i - x) + (j - y) * (j - y);
                                        if (d2i > dist * dist)
                                        {
                                            i1 -= di;
                                            break;
                                        }
                                        if (dist2ToImage[i, j] > d2i)
                                        {
                                            dist2ToImage[i, j] = d2i;
                                        }
                                        else if (i != x || j != y)
                                        {
                                            // The remaining points are stronger influenced by other pixels.
                                            break;
                                        }
                                    }
                                    if (i0 == i1) break;
                                    i0 += di;
                                    if (i0 == i1) break;
                                }
#endif
                                if ((p & Y) != 0)
                                {
#if true
                                    // The pixel also influences the adjoining diagonal.
                                    for (int d = 1, i = x + di, j = y + dj; 2 * d * d < dist * dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                                    {
                                        int d2i = d * d + (d - 1) * (d - 1);
                                        if (dist2ToImage[i - di, j] > d2i)
                                        {
                                            dist2ToImage[i - di, j] = d2i;
                                        }
                                        else
                                        {
                                            // The remaining points are stronger influenced by other pixels.
                                            break;
                                        }
                                    }
#endif
                                }
                            }
                            if ((p & Y) == 0)
                            {
#if true
                                // The pixel influences the octant by the Y axis.
                                for (int i = x, j0 = y, j1 = y + dist * dj; i != iLimit; i += di)
                                {
                                    for (int j = j0; j != j1 && j != jLimit; j += dj)
                                    {
                                        int d2i = (i - x) * (i - x) + (j - y) * (j - y);
                                        if (d2i > dist * dist)
                                        {
                                            j1 -= dj;
                                            break;
                                        }
                                        if (dist2ToImage[i, j] > d2i)
                                        {
                                            dist2ToImage[i, j] = d2i;
                                        }
                                        else if (i != x || j != y)
                                        {
                                            // The remaining points are stronger influenced by other pixels.
                                            break;
                                        }
                                    }
                                    if (j0 == j1) break;
                                    j0 += dj;
                                    if (j0 == j1) break;
                                }
#endif
                                if ((p & X) != 0)
                                {
#if true
                                    // The pixel also influences the adjoining diagonal.
                                    for (int d = 1, i = x + di, j = y + dj; 2 * d * d < dist * dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                                    {
                                        int d2i = d * d + (d - 1) * (d - 1);
                                        if (dist2ToImage[i, j - dj] > d2i)
                                        {
                                            dist2ToImage[i, j - dj] = d2i;
                                        }
                                        else
                                        {
                                            // The remaining points are stronger influenced by other pixels.
                                            break;
                                        }
                                    }
#endif
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ScanQuadrant(UInt16[,] pattern, UInt16 qMask, int x0, int x1, int y0, int y1, int[,] dist2ToImage, int dist)
        {
            int dx = (x0 < x1 ? +1 : -1);
            int dy = (y0 < y1 ? +1 : -1);

            UInt16 DC = 0;
            switch (qMask)
            {
                case QNE:
                case QSW:
                    DC = NW + SE;
                    break;
                case QNW:
                case QSE:
                    DC = NE + SW;
                    break;
            }

            // Influence direction.
            int di = -dx, dj = -dy, iLimit = -1, jLimit = -1;
            if (di > 0) { iLimit = pattern.GetLength(0); }
            if (dj > 0) { jLimit = pattern.GetLength(1); }

            for (int x = x0; x != x1; x += dx)
            {
                for (int y = y0; y != y1; y += dy)
                {
                    UInt16 p = pattern[x, y];

                    if ((p & qMask) == C)
                    {
                        if ((p & DC) == DC)
                        {
#if false
                            // The pixel influences the quadrant's diagonal.
                            for (int d = 1, i = x + di, j = y + dj; 2 * d * d < dist * dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                            {
                                bool doBreak = true;

                                int d2i = 2 * d * d;
                                if (dist2ToImage[i, j] > d2i)
                                {
                                    dist2ToImage[i, j] = d2i;
                                    doBreak = false;
                                }

                                d2i = d * d + (d - 1) * (d - 1);
                                if (dist2ToImage[i - di, j] > d2i)
                                {
                                    dist2ToImage[i - di, j] = d2i;
                                    doBreak = false;
                                }
                                if (dist2ToImage[i, j - dj] > d2i)
                                {
                                    dist2ToImage[i, j - dj] = d2i;
                                    doBreak = false;
                                }

                                // The remaining points are stronger influenced by other pixels.
                                if (doBreak) break;
                            }
#endif
                        }
                        else
                        {
#if true
                            // The pixel influences the whole quadrant.
                            int i1 = x + dist * di;
                            int j1 = y + dist * dj;
                            for (int i = x; i != i1 && i != iLimit; i += di)
                            {
                                for (int j = y + (i == x ? 1 : 0); j != j1 && j != jLimit; j += dj)
                                {
                                    int d2i = (i - x) * (i - x) + (j - y) * (j - y);
                                    if (d2i > dist * dist)
                                    {
                                        break;
                                    }
                                    if (dist2ToImage[i, j] > d2i)
                                    {
                                        dist2ToImage[i, j] = d2i;
                                    }
                                    else
                                    {
                                        // The remaining points are stronger influenced by other pixels.
                                        // TODO: Improve break condition -- breaks too early
                                        break;
                                    }
                                }
                            }
#endif
                        }
                    }
                }
            }
        }

        private static void ScanDiagonal(UInt16[,] pattern, UInt16 qMask, int x0, int x1, int y0, int y1, int[,] dist2ToImage, int dist)
        {
            int dx = (x0 < x1 ? +1 : -1);
            int dy = (y0 < y1 ? +1 : -1);

            UInt16 DC = 0;
            switch (qMask)
            {
                case QNE:
                case QSW:
                    DC = NW + SE;
                    break;
                case QNW:
                case QSE:
                    DC = NE + SW;
                    break;
            }

            // Influence direction.
            int di = -dx, dj = -dy, iLimit = -1, jLimit = -1;
            if (di > 0) { iLimit = pattern.GetLength(0); }
            if (dj > 0) { jLimit = pattern.GetLength(1); }

            for (int x = x0; x != x1; x += dx)
            {
                for (int y = y0; y != y1; y += dy)
                {
                    UInt16 p = pattern[x, y];

                    if ((p & qMask) == C)
                    {
                        if ((p & DC) == DC)
                        {
#if true
                            // The pixel influences the quadrant's diagonal.
                            for (int d = 1, i = x + di, j = y + dj; 2 * d * d < dist * dist && i != iLimit && j != jLimit; d++, i += di, j += dj)
                            {
                                bool doBreak = true;

                                int d2i = 2 * d * d;
                                if (dist2ToImage[i, j] > d2i)
                                {
                                    dist2ToImage[i, j] = d2i;
                                    doBreak = false;
                                }

                                d2i = d * d + (d - 1) * (d - 1);
                                if (dist2ToImage[i - di, j] > d2i)
                                {
                                    dist2ToImage[i - di, j] = d2i;
                                    doBreak = false;
                                }
                                if (dist2ToImage[i, j - dj] > d2i)
                                {
                                    dist2ToImage[i, j - dj] = d2i;
                                    doBreak = false;
                                }

                                // The remaining points are stronger influenced by other pixels.
                                if (doBreak) break;
                            }
#endif
                        }
                        else
                        {
#if false
                            // The pixel influences the whole quadrant.
                            int i1 = x + dist * di;
                            int j1 = y + dist * dj;
                            for (int i = x; i != i1 && i != iLimit; i += di)
                            {
                                for (int j = y + (i == x ? 1 : 0); j != j1 && j != jLimit; j += dj)
                                {
                                    int d2i = (i - x) * (i - x) + (j - y) * (j - y);
                                    if (d2i > dist * dist)
                                    {
                                        break;
                                    }
                                    if (dist2ToImage[i, j] > d2i)
                                    {
                                        dist2ToImage[i, j] = d2i;
                                    }
                                    else
                                    {
                                        // The remaining points are stronger influenced by other pixels.
                                        break;
                                    }
                                }
                            }
#endif
                        }
                    }
                }
            }
        }

        private static void ExtractPattern(Bitmap image, Color backgroundColor, float fuzziness, UInt16[,] pattern, int[,] dist2ToImage)
        {
            int xMin = 1, xMax = image.Width - 2;
            int yMin = 1, yMax = image.Height - 2;
            int x, y;

            for (x = xMin; x <= xMax; x++)
            {
                for (y = yMin; y <= yMax; y++)
                {
                    if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                    {
                        dist2ToImage[x, y] = 0;
                        pattern[x - 1, y - 1] += SE;
                        pattern[x - 1, y + 0] += E;
                        pattern[x - 1, y + 1] += NE;
                        pattern[x + 0, y - 1] += S;
                        pattern[x + 0, y + 0] += C;
                        pattern[x + 0, y + 1] += N;
                        pattern[x + 1, y - 1] += SW;
                        pattern[x + 1, y + 0] += W;
                        pattern[x + 1, y + 1] += NW;
                    }
                }

                y = 0;
                if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                {
                    dist2ToImage[x, y] = 0;
                    pattern[x - 1, y + 0] += E;
                    pattern[x - 1, y + 1] += NE;
                    pattern[x + 0, y + 0] += C;
                    pattern[x + 0, y + 1] += N;
                    pattern[x + 1, y + 0] += W;
                    pattern[x + 1, y + 1] += NW;
                }

                y = yMax + 1;
                if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                {
                    dist2ToImage[x, y] = 0;
                    pattern[x - 1, y - 1] += SE;
                    pattern[x - 1, y + 0] += E;
                    pattern[x + 0, y - 1] += S;
                    pattern[x + 0, y + 0] += C;
                    pattern[x + 1, y - 1] += SW;
                    pattern[x + 1, y + 0] += W;
                }
            }
            for (y = yMin; y <= yMax; y++)
            {
                x = 0;
                if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                {
                    dist2ToImage[x, y] = 0;
                    pattern[x + 0, y - 1] += S;
                    pattern[x + 0, y + 0] += C;
                    pattern[x + 0, y + 1] += N;
                    pattern[x + 1, y - 1] += SW;
                    pattern[x + 1, y + 0] += W;
                    pattern[x + 1, y + 1] += NW;
                }

                x = xMax + 1;
                if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                {
                    dist2ToImage[x, y] = 0;
                    pattern[x - 1, y - 1] += SE;
                    pattern[x - 1, y + 0] += E;
                    pattern[x - 1, y + 1] += NE;
                    pattern[x + 0, y - 1] += S;
                    pattern[x + 0, y + 0] += C;
                    pattern[x + 0, y + 1] += N;
                }
            }

            x = y = 0;
            if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
            {
                dist2ToImage[x, y] = 0;
                pattern[x + 0, y + 0] += C;
                pattern[x + 0, y + 1] += N;
                pattern[x + 1, y + 0] += W;
                pattern[x + 1, y + 1] += NW;
            }

            x = 0; y = yMax + 1;
            if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
            {
                dist2ToImage[x, y] = 0;
                pattern[x + 0, y - 1] += S;
                pattern[x + 0, y + 0] += C;
                pattern[x + 1, y - 1] += SW;
                pattern[x + 1, y + 0] += W;
            }

            x = xMax + 1; y = 0;
            if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
            {
                dist2ToImage[x, y] = 0;
                pattern[x - 1, y + 0] += E;
                pattern[x - 1, y + 1] += NE;
                pattern[x + 0, y + 0] += C;
                pattern[x + 0, y + 1] += N;
            }

            x = xMax + 1; y = yMax + 1;
            if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
            {
                dist2ToImage[x, y] = 0;
                pattern[x - 1, y - 1] += SE;
                pattern[x - 1, y + 0] += E;
                pattern[x + 0, y - 1] += S;
                pattern[x + 0, y + 0] += C;
            }
        }

        /// <summary>
        /// Modifies the pattern of pixels that are dominated by other pixels.
        /// </summary>
        /// <param name="pattern"></param>
        private static void NormalizePattern(UInt16[,] pattern)
        {
#if false
            int xMin = 1, xMax = pattern.GetLength(0) - 2;
            int yMin = 1, yMax = pattern.GetLength(1) - 2;
            int x, y;

            // TODO ...
#endif
        }
    }
}
