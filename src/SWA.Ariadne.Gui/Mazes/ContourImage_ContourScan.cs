using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Mazes
{
    public partial class ContourImage
    {
        #region Internal types for the contour scan algorithm.

        private struct RelativePoint
        {
            /// <summary>
            /// Relative position of the point.
            /// </summary>
            public readonly int rx, ry;

            /// <summary>
            /// Distance from the contour pixel, squared.
            /// </summary>
            public readonly int d2;

            public RelativePoint(int rx, int ry, int d2)
            {
                this.rx = rx; this.ry = ry; this.d2 = d2;
            }

            public override string ToString()
            {
                return string.Format("d2({0},{1}) = {2}", rx, ry, d2);
            }
        }

        /// <summary>
        /// Neighbor directions.
        /// For an eighth left/right turn, add +1/-1.
        /// </summary>
        private const int NbE = 0, NbNE = 1, NbN = 2, NbNW = 3, NbW = 4, NbSW = 5, NbS = 6, NbSE = 7;

        /// <summary>
        /// For each neighbor direction the relative X distance, [-1 .. +1].
        /// </summary>
        private static readonly int[] NbDX = { +1, +1, 0, -1, -1, -1, 0, +1, };

        /// <summary>
        /// For each neighbor direction the relative Y distance, [-1 .. +1].
        /// </summary>
        private static readonly int[] NbDY = { 0, -1, -1, -1, 0, +1, +1, +1, };

        /// <summary>
        /// For each pair of (dx+1, dy+1), the corresponding neighbor direction.
        /// </summary>
        private static readonly int[,] Nb = {
            { NbNW, NbW, NbSW }, // dx = -1
            { NbN,   -1, NbS  }, // dx =  0
            { NbNE, NbE, NbSE }, // dx = +1
        };

        /// <summary>
        /// For each combination of a next left and next right neighbor of a pixel,
        /// the influenceRegion is the set of points that are dominated by that pixel
        /// and not by one of the neighbors.
        /// Pixels closer than ContourDistance-sqrt(2) are not recorded.
        /// </summary>
        private static List<RelativePoint>[,] influenceRegions = new List<RelativePoint>[8, 8];

        /// <summary>
        /// Returns the squared distance a contour pixel has
        /// on a point located at (x, y) relatively to that pixel
        /// when the pixel lies between two other pixelsin the given neighbor locations.
        /// </summary>
        /// <param name="nbL">left neighbor</param>
        /// <param name="nbR">right neighbor</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int InfluenceD2(int nbL, int nbR, int x, int y)
        {
            foreach (RelativePoint rp in influenceRegions[nbL, nbR])
            {
                if ((rp.rx == x) && (rp.ry == y))
                {
                    return rp.d2;
                }
            }
            return int.MaxValue;
        }

        private static void PrepareInfluenceRegions(int influenceRange)
        {
            int range2Max = influenceRange * influenceRange;
            int range2Min = ContourDistance * ContourDistance - 2;

            /* Consider four points:
             * - the pixel for which an influence region is calculated, at (0, 0)
             * - a left neighbor at nbL
             * - a right neighbor at nbR
             * - for contour angles .ge. 180: a pixel inside the object on the middle angle between left and right neighbor: nbI
             */
            for (int nbR = 0; nbR < 8; nbR++)
            {
                int dxR = NbDX[nbR], dyR = NbDY[nbR];

                for (int nbL = 0; nbL < 8; nbL++)
                {
                    int dxL = NbDX[nbL], dyL = NbDY[nbL];

#if false
                    // Beginning at nbR, turn by the half angle between left and right.
                    int nbI = (nbR + ((nbL - nbR + 8) % 8) / 2) % 8;
#else
                    // Beginning at nbR, turn by the half angle between left and right.
                    int nbI = (nbL + ((nbR - nbL + 8) % 8) / 2) % 8;
#endif
                    //if (nbR == nbL) { nbI = (nbI + 4) % 8; }
                    int dxI = NbDX[nbI], dyI = NbDY[nbI];

                    // Avoid a situation where nbR and nbI lie exactly on opposite sides of the contour pixel.
                    if (Math.Abs(nbR - nbI) == 4)
                    {
                        // As the original pixel was opposite nbR, we subtract the nbL vector,
                        // thus placing it on the diagonal.
                        dxI -= dxL;
                        dyI -= dyL;
                    }

                    influenceRegions[nbL, nbR] = new List<RelativePoint>();

                    for (int dx = -(influenceRange - 1); dx <= +(influenceRange - 1); dx++)
                    {
                        for (int dy = -(influenceRange - 1); dy <= +(influenceRange - 1); dy++)
                        {
                            // Get the distance to the three points.
                            int d2 = dx * dx + dy * dy;
                            int d2L = (dx - dxL) * (dx - dxL) + (dy - dyL) * (dy - dyL);
                            int d2R = (dx - dxR) * (dx - dxR) + (dy - dyR) * (dy - dyR);
                            int d2I = (dx - dxI) * (dx - dxI) + (dy - dyI) * (dy - dyI);

                            // Check if the center pixel is closest to the point.
                            // For equal distance, the left neighbor should dominate.
                            // Only distances in the relevant range are recorded.
                            if (d2 < d2L && d2 <= d2R && d2 < d2I && range2Min <= d2 && d2 <= range2Max)
                            {
                                influenceRegions[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private static Bitmap GetMask_ContourScan(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = GetBlurDistance(backgroundColor);

            #region Create the result bitmap.

            Color black = Color.Black;
            black = Color.Magenta; // TODO: delete this line
            Color transparent = Color.FromArgb(0, black);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));
            Graphics gMask = Graphics.FromImage(result);
            gMask.FillRectangle(new SolidBrush(transparent), 0, 0, result.Width, result.Height);

            #endregion

            #region Create local variables used in the contour scan.

            PrepareInfluenceRegions(contourDist + blurDist);
            int range2Max = (contourDist + blurDist) * (contourDist + blurDist);

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

            // All points on the contour of an object are collected in scan switch lines.
            List<int>[] contourXs = new List<int>[image.Height];
            for (int i = 0; i < image.Height; i++)
            {
                // Create the list.
                contourXs[i] = new List<int>(5);
                // Add a termination point that is outside of the scanned image.
                contourXs[i].Add(image.Width);
            }

            #endregion

            #region Scan the image for non background objects.

            // Maximum and minimum distance between scan lines.
            const int dsMax = 16 * 1024;
            const int dsMin = 2;

            int nObjects = 0;
            int nMaxObjects = 20;

            // Apply horizontal scan lines in decreasing distance.
            // The generated y values are:
            //   ds =  2: 0, 2, 4, 6, 8, 10, ...   -- all even numbers (is not used)
            //   ds =  4: 1, 5, 9, 13, ...
            //   ds =  8: 3, 11, 19, ...
            //   ds = 16: 7, 23, ...
            for (int ds = dsMax; ds >= 2 * dsMin; ds /= 2)
            {
                if (nObjects > nMaxObjects)
                {
                    break;
                }

                for (int y = ds / 2 - 1; y < image.Height; y += ds)
                {
                    if (nObjects > nMaxObjects)
                    {
                        break;
                    }
                    
                    // Current index in contourXs[y].
                    int c = 0;
                    // X coordinate of the first known object on the Y scan line.
                    int x1 = contourXs[y][c];

                    for (int x = 1; x < image.Width; x += 1)
                    {
                        if (nObjects > nMaxObjects)
                        {
                            break;
                        }

#if false
                        if (dist2ToImage[x, y] <= range2Max)
                        {
                            // There is an object that was already processed.
                            // TODO: Test must be refined for objects smaller than blurDist.
                            // TODO: Use the information in contourXs.
                            break;
                        }
#else
                        if (x == x1)
                        {
                            // We have reached the contour of a known object.
                            // Skip to the next contour point.
                            x = contourXs[y][++c];      // Contour point where the scan line leaves the object.
                            x1 = contourXs[y][c + 1];   // Contour point of next object or terminator image.Width.
                            continue;
                        }
#endif
                        if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                        {
                            if (ScanObject(image, x, y, backgroundColor, fuzziness, dist2ToImage, contourXs))
                            {
                                ++nObjects;

                                // Make X advance to the contour point where the scan line leaves the object.
                                // The new object's contour point (x, y) is at the current c position.
                                x = contourXs[y][++c];      // Contour point where the scan line leaves the object.
                                x1 = contourXs[y][c + 1];   // Contour point of next object or terminator image.Width.
                            }
                        }
                    }
                }
            }

            #endregion

            // TODO: fill inside of objects, using the contourXs.

            #region Set the color of all mask pixels to black with an appropriate transparency; determine the bounding box.

            // Coordinates of the bounding box.
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    double dist = Math.Sqrt(dist2ToImage[x, y]);
                    Color maskColor;

                    if (dist > contourDist + blurDist)
                    {
                        maskColor = black;
#if true
                        continue; // TODO: remove this line
#endif
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
        /// Scans the contour of an object starting at a given point on the contour.
        /// [x0-1,y0] is outside and [x0,y0] is inside the image.
        /// The influence regions of all contour pixels are applied to the dist2Image map.
        /// Returns true if a sufficiently large object was detected.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="fuzziness"></param>
        /// <param name="dist2ToImage"></param>
        /// <param name="controurXs"></param>
        private static bool ScanObject(Bitmap image, int x0, int y0, Color backgroundColor, float fuzziness, int[,] dist2ToImage, List<int>[] contourXs)
        {
            #region Choose an initial focus point with a left and right neighbor.

#if false
            // Use the start point as the focus point's right neighbor.
            int xR = x0, yR = y0;

            // The (first) left neighbor is the initial focus point.
            int nb0, x, y;
            LeftNeighbor(image, backgroundColor, fuzziness, xR, yR, NbW, out nb0, out x, out y);
            if (nb0 == NbW)
            {
                // There is no neighbor pixel.  One-pixel objects are ignored.
                return false;
            }
            int nbR = (nb0 + 4) % 8;

            // The second left neighbor is the focus point's left neighbor.
            int nbL, xL, yL;
            LeftNeighbor(image, backgroundColor, fuzziness, x, y, nbR, out nbL, out xL, out yL);
#else
            // Use the start point as the focus point.
            int x = x0, y = y0;

            // Determine right neighbor.
            int nbR, xR, yR;
            RightNeighbor(image, backgroundColor, fuzziness, x, y, NbW, out nbR, out xR, out yR);
            if (nbR == NbW)
            {
                // There is no neighbor pixel.  One-pixel objects are ignored.
                return false;
            }

            // Determine left neighbor.
            int nbL, xL, yL;
            LeftNeighbor(image, backgroundColor, fuzziness, x, y, nbR, out nbL, out xL, out yL);
#endif

#if false
            // Number of recently passed points on the same scan line.
            int nHorizintalStretch = 0;
#endif

            #endregion

            // Set up termination conditions.
            // (x,y) will be processed first; we are finished when we return to the same pixel in the same direction.
            int x1 = x, y1 = y, nb1 = nbL;

            do
            {
#if false // TODO: false
                string msg = string.Format("[{0},{1}] -{2}-> [{3},{4}]", x, y, nbL, xL, yL);
                System.Console.WriteLine(msg);
#endif

                #region Register the contour point in the ordered list of X coordinates.

#if false
                if (y != yR)        // On a different scan line: insert a new contour point.
                {
                    contourXs[y].Insert(p, x);
                    if (yL == yR)
                    {
                        // We're just touching the scan line; add a second (exit) point.
                        contourXs[y].Insert(p, x);
                    }
                    nHorizintalStretch = 1;
                }
                else if (nHorizintalStretch < 2)
                {
                    // Add a second point on this scan line.
                    contourXs[y].Insert(p, x);
                    ++nHorizintalStretch;
                }
                else if (nHorizintalStretch > 2 && x > xR)
                {
                    // Special case of a very fine needle at the beginning of the contour.
                    // Wait until y changes or we move backwards to the left.
                    if (yL < y || xL <= x)
                    {
                        contourXs[y].Insert(p, x);
                        // The second point on this contour line is the initial (xR, yR) which will be processed last.
                        nHorizintalStretch = 1;
                    }
                }
                else if (x < xR)    // On the same scan line, moving left: update the left contour point.
                {
                    contourXs[y][p] = x;
                }
                else                // On the same scan line, moving right: update the right contour point.
                {
                    contourXs[y][p - 1] = x;
                }
#else
                switch (nbR * 8 + nbL)
                {
                    case (NbW * 8 + NbE):       //  R o L
                    case (NbE * 8 + NbW):       //
                        // do nothing; we'll wait what happens next
                        break;

                    case (NbSW * 8 + NbE):      //    o L
                    case (NbNE * 8 + NbW):      //  R
                        // do nothing; we'll wait what happens next
                        break;

                    case (NbW * 8 + NbSE):      //  R o
                    case (NbE * 8 + NbNW):      //      L
                    //
                    case (NbSW * 8 + NbSE):     //    o
                    case (NbNE * 8 + NbNW):     //  R   L
                        // do nothing; wait until (xL,yL) is processed in the next iteration
                        break;

                    case (NbW * 8 + NbNE):      //
                    case (NbW * 8 + NbN):       //  L L L
                    case (NbW * 8 + NbNW):      //  R o 
                    case (NbW * 8 + NbW):       //
                    case (NbE * 8 + NbSW):
                    case (NbE * 8 + NbS):
                    case (NbE * 8 + NbSE):
                    case (NbE * 8 + NbE):
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbS * 8 + NbNE):      //
                    case (NbS * 8 + NbN):       //  L L L
                    case (NbS * 8 + NbNW):      //  L o
                    case (NbS * 8 + NbW):       //    R
                    case (NbN * 8 + NbSW):
                    case (NbN * 8 + NbS):
                    case (NbN * 8 + NbSE):
                    case (NbN * 8 + NbE):
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbNW * 8 + NbSW):     //
                    case (NbNW * 8 + NbS):      //  R
                    case (NbNW * 8 + NbSE):     //    o L
                    case (NbNW * 8 + NbE):      //  L L L
                    case (NbSE * 8 + NbNE):
                    case (NbSE * 8 + NbN):
                    case (NbSE * 8 + NbNW):
                    case (NbSE * 8 + NbW):
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbSW * 8 + NbNE):     //
                    case (NbSW * 8 + NbN):      //  L L L
                    case (NbSW * 8 + NbNW):     //  L o
                    case (NbSW * 8 + NbW):      //  R
                    case (NbNE * 8 + NbSW):
                    case (NbNE * 8 + NbS):
                    case (NbNE * 8 + NbSE):
                    case (NbNE * 8 + NbE):
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbNW * 8 + NbNE):     //
                    case (NbNW * 8 + NbN):      //  R L L
                    case (NbNW * 8 + NbNW):     //    o
                    case (NbSE * 8 + NbSW):     //
                    case (NbSE * 8 + NbS):
                    case (NbSE * 8 + NbSE):
                        InsertContourPoint(contourXs[y], x);
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbSW * 8 + NbSW):     //    o
                    case (NbNE * 8 + NbNE):     //  R
                        InsertContourPoint(contourXs[y], x);
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbS * 8 + NbSW):      //
                    case (NbS * 8 + NbS):       //    o
                    case (NbN * 8 + NbNE):      //  L R
                    case (NbN * 8 + NbN):       //
                        InsertContourPoint(contourXs[y], x);
                        InsertContourPoint(contourXs[y], x);
                        break;

                    case (NbW * 8 + NbSW):      //
                    case (NbW * 8 + NbS):       //  R o
                    case (NbE * 8 + NbNE):      //  L L
                    case (NbE * 8 + NbN):       //
                    case (NbS * 8 + NbSE):      //
                    case (NbS * 8 + NbE):       //  o L
                    case (NbN * 8 + NbNW):      //  R L
                    case (NbN * 8 + NbW):       //
                        // do nothing; this is an impossible contour sequence
                        break;

                    case (NbNW * 8 + NbW):      //  R
                    case (NbSE * 8 + NbE):      //  L o
                                                //
                    case (NbSW * 8 + NbS):      //    o
                    case (NbNE * 8 + NbN):      //  R L
                        // do nothing; this is an impossible contour sequence
                        break;
                }
#endif

                #endregion

                // Apply the focus point's influence region to the resulting distance map.
                foreach (RelativePoint rp in influenceRegions[nbL, nbR])
                {
                    int i = x + rp.rx, j = y + rp.ry;
                    if (rp.d2 < dist2ToImage[i, j])
                    {
                        dist2ToImage[i, j] = rp.d2;
                    }
                }

                // Advance the focus to the next contour pixel.
                xR = x; yR = y; nbR = (nbL + 4) % 8;
                x = xL; y = yL;
                LeftNeighbor(image, backgroundColor, fuzziness, x, y, nbR, out nbL, out xL, out yL);

            } while (x != x1 || y != y1 || nb1 != nbL);

            return true;
        }

        private static void InsertContourPoint(List<int> contourX, int x)
        {
            // Position where the contour point will be entered into contourXs[y].
            int p;

            // Find the position p with x < contourX[p].
            // Note: As there is a terminator entry image.Width, we will not leave the valid index range.
            for (p = 0; ; p++)
            {
                if (x < contourX[p])
                {
                    contourX.Insert(p, x);
                    break;
                }
            }
        }

        /// <summary>
        /// Locates the left neighbor of a contour pixel that is also on the object contour.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="fuzziness"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nbR">direction of the current pixel's right neighbor</param>
        /// <param name="nbL"></param>
        /// <param name="xL"></param>
        /// <param name="yL"></param>
        private static void LeftNeighbor(Bitmap image, Color backgroundColor, float fuzziness, int x, int y, int nbR, out int nbL, out int xL, out int yL)
        {
            xL = yL = 0; // make compiler happy
            for (nbL = (nbR + 1) % 8; ; nbL = (nbL + 1) % 8)
            {
                xL = x + NbDX[nbL]; yL = y + NbDY[nbL];
                if (nbL == nbR)
                {
                    break; // We have completed the circle.
                }
                
                float cd = ColorDistance(image.GetPixel(xL, yL), backgroundColor);
                if (cd > fuzziness)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Locates the right neighbor of a contour pixel that is also on the object contour.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="fuzziness"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nbL">direction of the current pixel's left neighbor</param>
        /// <param name="nbR"></param>
        /// <param name="xR"></param>
        /// <param name="yR"></param>
        private static void RightNeighbor(Bitmap image, Color backgroundColor, float fuzziness, int x, int y, int nbL, out int nbR, out int xR, out int yR)
        {
            xR = yR = 0; // make compiler happy
            for (nbR = (nbL - 1 + 8) % 8; ; nbR = (nbR - 1 + 8) % 8)
            {
                xR = x + NbDX[nbR]; yR = y + NbDY[nbR];
                if (nbR == nbL)
                {
                    break; // We have completed the circle.
                }

                float cd = ColorDistance(image.GetPixel(xR, yR), backgroundColor);
                if (cd > fuzziness)
                {
                    break;
                }
            }
        }
    }
}
