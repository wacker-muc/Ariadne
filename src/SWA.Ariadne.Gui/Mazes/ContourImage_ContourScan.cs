using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

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
        /// from a point located at (x, y) relatively to that pixel.
        /// Returns int.MaxValue if the given point is not influenced by the pixel
        /// but by one of the given neighbor pixels.
        /// </summary>
        /// <param name="nbL">left neighbor</param>
        /// <param name="nbR">right neighbor</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// Note: This method is only used by Unit Tests.
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

        /// <summary>
        /// For each combination of a next left and next right neighbor of a pixel,
        /// the borderLimit is the set of points on the left and right edge of the influenceRegion.
        /// </summary>
        private static List<RelativePoint>[,]
            leftBorderLimits = new List<RelativePoint>[8, 8],
            rightBorderLimits = new List<RelativePoint>[8, 8];

        /// <summary>
        /// Returns -1/+1 if the point at x, y is on the left/right border of the influence region
        /// defined by the given two neighbor directions.
        /// Returns 0 otherwise.
        /// </summary>
        /// <param name="nbL"></param>
        /// <param name="nbR"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// Note: This method is only used by Unit Tests.
        private static int BorderLimitType(int nbL, int nbR, int x, int y)
        {
            foreach (RelativePoint rp in leftBorderLimits[nbL, nbR])
            {
                if ((rp.rx == x) && (rp.ry == y))
                {
                    return -1;
                }
            }
            foreach (RelativePoint rp in rightBorderLimits[nbL, nbR])
            {
                if ((rp.rx == x) && (rp.ry == y))
                {
                    return +1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Set up the influenceRegions and associated data for a given influence range.
        /// </summary>
        /// <param name="influenceRange"></param>
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

                    // Beginning at nbR, turn by the half angle between left and right.
                    int nbI = (nbL + ((nbR - nbL + 8) % 8) / 2) % 8;
                    int dxI = NbDX[nbI], dyI = NbDY[nbI];

                    // Avoid a situation where nbR and nbI lie exactly on opposite sides of the contour pixel.
                    if (Math.Abs(nbR - nbI) == 4)
                    {
                        // As the original pixel was opposite nbR, we subtract the nbL vector,
                        // thus placing it on the diagonal.
                        // Note: This point is still sufficiently close to the focus pixel.
                        dxI -= dxL;
                        dyI -= dyL;
                    }

                    #region Calculate the influence region for the current left and right neighbor.

                    influenceRegions[nbL, nbR] = new List<RelativePoint>();
                    leftBorderLimits[nbL, nbR] = new List<RelativePoint>();
                    rightBorderLimits[nbL, nbR] = new List<RelativePoint>();

                    // For each scan line: Leftmost and rightmost point in the influence region.
                    // Use index [dy + influenceRange].
                    // Enter dx + influenceRange which is > 0.
                    int[] leftLimits = new int[2 * influenceRange], rightLimits = new int[2 * influenceRange];

#if false
                    for (int dx = -(influenceRange - 1); dx <= +(influenceRange - 1); dx++)
                    {
#else
                    // We want to traverse dx from the center outwards,
                    // facilitating the registry of left and right limits.
                    for (int i = 1; i < 2 * influenceRange; i++)
                    {
                        // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                        int dx = (i / 2) * (i % 2 == 0 ? +1 : -1);
#endif

#if false
                        for (int dy = -(influenceRange - 1); dy <= +(influenceRange - 1); dy++)
                        {
#else
                        for (int j = 1; j < 2 * influenceRange; j++)
                        {
                            // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                            int dy = (j / 2) * (j % 2 == 0 ? +1 : -1);
#endif
                            // Get the distance to the three points.
                            int d2 = dx * dx + dy * dy;
                            int d2L = (dx - dxL) * (dx - dxL) + (dy - dyL) * (dy - dyL);
                            int d2R = (dx - dxR) * (dx - dxR) + (dy - dyR) * (dy - dyR);
                            int d2I = (dx - dxI) * (dx - dxI) + (dy - dyI) * (dy - dyI);

                            if (range2Min <= d2 && d2 <= range2Max)
                            {
                                // This value will be entered into the left/right limits if the current point is in the influence region.
                                int limitsEntry = dx + influenceRange;

                                // Check if the center pixel is closest to the point.
                                // For equal distance, the left neighbor should dominate.
                                // Only distances in the relevant range are recorded.
                                if (d2 < d2L && d2 <= d2R && d2 < d2I)
                                {
                                    influenceRegions[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                                }
                                else
                                {
                                    // Overwrite and remove a previous entry in the left/right limits.
                                    limitsEntry = -1;
                                }

                                if (dx < 0)
                                {
                                    leftLimits[dy + influenceRange] = limitsEntry;
                                }
                                if (dx > 0)
                                {
                                    rightLimits[dy + influenceRange] = limitsEntry;
                                }
                            }
                        }
                    }

                    #endregion

                    #region Transfer information from the locally collected left and right limits to the static variables.

                    for (int i = 0; i < 2 * influenceRange; i++)
                    {
                        int dx = leftLimits[i] - influenceRange;
                        int dy = i - influenceRange;
                        int d2 = dx * dx + dy * dy;

                        if (leftLimits[i] > 0)
                        {
                            leftBorderLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }

                        dx = rightLimits[i] - influenceRange;
                        d2 = dx * dx + dy * dy;

                        if (rightLimits[i] > 0)
                        {
                            rightBorderLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }
                    }

                    #endregion
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
            int[,] dist2ToImage;

            // All points on the contour of an object are collected in scan switch lines.
            List<int>[] contourXs;

            // The points on the outside border of the objects' influence regions are collected in scan switch lines.
            List<int>[] borderXs;
            // We need to distinguish between left and right points; true is left and false is right.
            List<bool>[] borderXsLR;

            // Create and initialize the dist2Image array.
            InitializeDist2ToImage(image.Width, image.Height, out dist2ToImage);

            // Create the scan line lists and add the required terminator entries.
            InitializeScanLines(image.Width, image.Height, out contourXs, out borderXs, out borderXsLR);

            #endregion

            #region Scan the image for non background objects.

            // Maximum and minimum distance between scan lines.
            const int dsMax = 16 * 1024;
            const int dsMin = 2;

            int nObjects = 0;

            // Apply horizontal scan lines in decreasing distance.
            // The generated y values are:
            //   ds =  2: 0, 2, 4, 6, 8, 10, ...   -- all even numbers (is not used)
            //   ds =  4: 1, 5, 9, 13, ...
            //   ds =  8: 3, 11, 19, ...
            //   ds = 16: 7, 23, ...
            for (int ds = dsMax; ds >= 2 * dsMin; ds /= 2)
            {
                for (int y = ds / 2 - 1; y < image.Height; y += ds)
                {
                    // Current index in contourXs[y].
                    int c = 0;
                    // X coordinate of the first known object on the Y scan line.
                    int x1 = contourXs[y][c];

                    for (int x = 1; x < image.Width; x += 1)
                    {
                        if (x == x1)
                        {
                            // We have reached the contour of a known object.
                            // Skip to the next contour point.
                            x = contourXs[y][++c];      // Contour point where the scan line leaves the object.
                            x1 = contourXs[y][++c];     // Contour point of next object or terminator image.Width.
                            continue;
                        }

                        if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                        {
                            if (ScanObject(image, x, y, backgroundColor, fuzziness, dist2ToImage, contourXs, borderXs, borderXsLR))
                            {
                                ++nObjects;

                                // Make X advance to the contour point where the scan line leaves the object.
                                // The new object's contour point (x, y) is at the current c position.
                                x = contourXs[y][++c];      // Contour point where the scan line leaves the object.
                                x1 = contourXs[y][++c];     // Contour point of next object or terminator image.Width.
                            }
                        }
                    }
                }
            }

            #endregion

            // Normalize the collected border scan lines.
            EliminateOverlaps(borderXs, borderXsLR);

            // Eliminate enclosed regions from border scan lines.
#if false
            EliminateInsideRegions(borderXs, borderXsLR, dsMin - 1, dsMin);
#else
            EliminateInsideRegions(borderXs, borderXsLR, 0, 1);
#endif

#if false
            // Fill inside of objects, using the contourXs.
            Brush insideBrush = new SolidBrush(transparent);
            GraphicsPath contourPath = GetPath(contourXs);
            gMask.FillPath(insideBrush, contourPath);
#endif

            // Fill outside of objects, using the borderXs.
            Brush outsideBrush = new SolidBrush(black);
#if false
            GraphicsPath borderPath = GetPath(borderXs);
            gMask.FillPath(outsideBrush, outsideBrush);
#else
#if false
            Point[] borderPoints = GetBorderPoints(borderXs);
            gMask.FillPolygon(outsideBrush, porderPoints);
#else
            FillOutside(gMask, black, borderXs);
#endif
#endif

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
        /// <param name="borderXs"></param>
        /// <param name="borderXsLR"></param>
        private static bool ScanObject(Bitmap image, int x0, int y0, Color backgroundColor, float fuzziness, int[,] dist2ToImage, List<int>[] contourXs, List<int>[] borderXs, List<bool>[] borderXsLR)
        {
            #region Choose an initial focus point with a left and right neighbor.

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

            #endregion

            // Set up termination conditions.
            // (x,y) will be processed first; we are finished when we return to the same pixel in the same direction.
            int x1 = x, y1 = y, nb1 = nbL;

            #region Scan the outside contour of the object.

            do
            {
                #region Register the contour point in the ordered list of X coordinates.

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

                // Enter the focus point's border points into the respective border scan lines.
                foreach (RelativePoint rp in leftBorderLimits[nbL, nbR])
                {
                    int i = x + rp.rx, j = y + rp.ry;
                    InsertBorderPoint(borderXs[j], borderXsLR[j], i, true);
#if true
                    // Add another symmetrical border point to keep the LR balance.
                    // Note: If this border point is not relevant, it will be safely eliminated later.
                    // TODO: Sometimes, this helps but sometimes it creates completeley useless results!?!
                    i -= 2 * rp.rx;
                    InsertBorderPoint(borderXs[j], borderXsLR[j], i, false);
#endif
                }
                foreach (RelativePoint rp in rightBorderLimits[nbL, nbR])
                {
                    int i = x + rp.rx, j = y + rp.ry;
                    InsertBorderPoint(borderXs[j], borderXsLR[j], i, false);
#if true
                    i -= 2 * rp.rx;
                    InsertBorderPoint(borderXs[j], borderXsLR[j], i, true);
#endif
                }

                // Advance the focus to the next contour pixel.
                xR = x; yR = y; nbR = (nbL + 4) % 8;
                x = xL; y = yL;
                LeftNeighbor(image, backgroundColor, fuzziness, x, y, nbR, out nbL, out xL, out yL);

            } while (x != x1 || y != y1 || nb1 != nbL);

            #endregion

            return true;
        }

        #region Methods for initializing the data structures.

        private static void InitializeDist2ToImage(int width, int height, out int[,] dist2ToImage)
        {
            dist2ToImage = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    dist2ToImage[x, y] = int.MaxValue;
                }
            }
        }

        private static void InitializeScanLines(int width, int height, out List<int>[] contourXs, out List<int>[] borderXs, out List<bool>[] borderXsLR)
        {
            contourXs = new List<int>[height];
            borderXs = new List<int>[height];
            borderXsLR = new List<bool>[height];

            for (int i = 0; i < contourXs.Length; i++)
            {
                // Create the lists.
                contourXs[i] = new List<int>(16);
                borderXs[i] = new List<int>(16);
                borderXsLR[i] = new List<bool>(16);

                // Add termination points that are well beyond the regular scan line.

                // One terminator for the contour.
                contourXs[i].Add(width + 1);

                // Two terminators for the border.
                borderXs[i].Add(-2);
                borderXsLR[i].Add(false);
                borderXs[i].Add(width + 1);
                borderXsLR[i].Add(true);
            }
        }

        #endregion


        #region Methods for managing the contour points.

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

        #endregion

        #region Methods for managing the border points.

        /// <summary>
        /// Inserts the given point into the given border scan line.
        /// </summary>
        /// <param name="borderX"></param>
        /// <param name="borderLR"></param>
        /// <param name="x"></param>
        /// <param name="leftOrRight"></param>
        private static void InsertBorderPoint(List<int> borderX, List<bool> borderLR, int x, bool leftOrRight)
        {
            // Position where the border point will be entered into borderXs[y].
            int p;

            // Find the position p with x < borderX[p].
            // Note: As there is a terminator entry image.Width, we will not leave the valid index range.
            for (p = 0; ; p++)
            {
                // On the same X position, left borders should be inserted first so that they can be successfully eliminated.
                if (x < borderX[p] || x == borderX[p] && leftOrRight == true)
                {
                    borderX.Insert(p, x);
                    borderLR.Insert(p, leftOrRight);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all but the first level of left-right border pairs.
        /// </summary>
        /// <param name="borderXs">
        /// Pairs of borders that are enclosed within other, outer borders will be removed.
        /// </param>
        /// <param name="borderXsLR">
        /// Will also be shortened and converted to an alternating sequence of left and right borders.
        /// </param>
        private static void EliminateOverlaps(List<int>[] borderXs, List<bool>[] borderXsLR)
        {
            for (int j = 0; j < borderXs.Length; j++)
            {
                for (int n = 0, p = borderXs[j].Count - 2, q = -1; p >= 1; p--)
                {
                    if (borderXsLR[j][p] == false /*right*/)
                    {
                        if (++n == 2)
                        {
                            q = p;
                        }
                    }
                    else
                    {
                        if (--n == 1)
                        {
                            // Eliminate the points in the overlapped region p .. q.
                            borderXs[j].RemoveRange(p, q - p + 1);
                            borderXsLR[j].RemoveRange(p, q - p + 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies and eliminates areas in or between the objects defined by the scan lines
        /// that are completely enclosed by the objects.
        /// Note: The shape described in borderXs must not touch any border;
        ///       otherwise cut off areas will be considered "inside" and eliminated.
        /// </summary>
        /// <param name="borderXs">list of normalized scan lines</param>
        /// <param name="borderXsLR">alternating left and right</param>
        /// <param name="y0">index of first valid scan line</param>
        /// <param name="sy">step between consecutive valid scan lines</param>
        /// <returns>number of inside regions found</returns>
        private static int EliminateInsideRegions(List<int>[] borderXs, List<bool>[] borderXsLR, int y0, int sy)
        {
            #region Create local data structures.

            /* Unresolved enclosed areas up to the current scan line.
             * 
             * The Points in this data structure are interpreted like this:
             * y = index in a list or array of scan lines.
             * x = index within the scan line; references the first of a pair of start .. end positions on the scan line.
             * 
             * scanLineReferenceGroups[i] is a list of scan line references that belong to the same (potentially) enclosed area.
             */
            List<List<Point>> scanLineReferenceGroups = new List<List<Point>>();

            /* Unresolved enclosed areas on current and previous scan line.
             * The two lists are used alternatingly; saPrev and saCurr are the indices.
             * 
             * The Points in this data structure are interpreted like this:
             * y = index in scanLineReferenceGroups, ID of that group.
             * x = index within the scan line; references the first of a pair of start .. end positions on the scan line.
             */
            List<Point>[] scanAreas = new List<Point>[2];
            int saPrev = 0, saCurr = 1;
            scanAreas[0] = new List<Point>();
            scanAreas[1] = new List<Point>();

            /* Effective ID of a reference group.
             * When two areas are found to be connected, the one with the greater ID will be united with the other group.
             * For independent groups, unitedGroupIds[i] == i.
             */
            List<int> unitedGroupIds = new List<int>();

            #endregion

            #region Set up a group referencing the outside region.

            // Add all outside areas on the first scan line to a first reference group.
            // As the scan line is normalized, these areas are between even and odd positions.
            int outsideGroupId = scanLineReferenceGroups.Count; // == 0
            unitedGroupIds.Add(outsideGroupId);
            scanLineReferenceGroups.Add(new List<Point>((borderXs[y0].Count - 1) / 2));
            for (int p = 0, q = 1; q < borderXs[y0].Count; p += 2, q += 2)
            {
                int xp = borderXs[y0][p], xq = borderXs[y0][q];
                scanLineReferenceGroups[outsideGroupId].Add(new Point(p, y0));
                scanAreas[saPrev].Add(new Point(p, outsideGroupId));
            }

            #endregion

            #region Collect the inside areas of all following scan lines in reference groups.

            for (int y = y0 + sy; y < borderXs.Length; y += sy)
            {
                int a = 0;                  // index into scanAreas[saPrev]
                Point pa = scanAreas[saPrev][a];
                int i = pa.X, j = i + 1;    // indices into borderXs
                // Note that the x coordinates are those of the object; the spaces between are two pixels smaller.
                int xi = borderXs[y - sy][i] + 1, xj = borderXs[y - sy][j] - 1;

                // Process the areas between objects on the current scan line.
                // As the scan line is normalized, these areas are between even and odd positions.
                for (int p = 0, q = 1; q < borderXs[y].Count; p += 2, q += 2)
                {
                    int xp = borderXs[y][p] + 1, xq = borderXs[y][q] - 1;

                    // Advance the previous line's scan area until it lies at or before this line's area.
                    // Note: The last (outside) area on the previous line extends to the right image border.
                    while (xp > xj)
                    {
                        pa = scanAreas[saPrev][++a];
                        i = pa.X; j = i + 1;
                        xi = borderXs[y - sy][i] + 1; xj = borderXs[y - sy][j] - 1;
                    }

                    // Determine which group the current area should belong to.
                    int groupId;
                    if (xi <= xq) // The two areas overlap: xp <= xi <= xq <= xj.
                    {
                        // Add this area to the previous area's group.
                        groupId = pa.Y;
                    }
                    else
                    {
                        // Open a new group.
                        groupId = scanLineReferenceGroups.Count;
                        unitedGroupIds.Add(groupId);
                        scanLineReferenceGroups.Add(new List<Point>((borderXs[y].Count - 1) / 2));
                    }

                    // Add the current area to the selected group.
                    scanLineReferenceGroups[groupId].Add(new Point(p, y));
                    scanAreas[saCurr].Add(new Point(p, groupId));

                    // See if other areas on the previous line also overlap with the current area.
                    while (a + 1 < scanAreas[saPrev].Count)
                    {
                        int a1 = a + 1;
                        Point pa1 = scanAreas[saPrev][a1];
                        int i1 = pa1.X, j1 = i1 + 1;
                        int xi1 = borderXs[y - sy][i1] + 1, xj1 = borderXs[y - sy][j1] - 1;

                        if (xi1 <= xq)
                        {
                            // Unite the two areas' groups.
                            int id1 = pa1.Y;
                            if (groupId < id1)
                            {
                                unitedGroupIds[id1] = groupId;
                            }
                            else if (id1 < groupId)
                            {
                                unitedGroupIds[groupId] = groupId = id1;
                            }
                            else
                            {
                                // Do nothing; these separate areas already belong to the same group.
                            }

                            // Advance on the previous scan line.
                            a = a1; pa = pa1; i = i1; j = j1; xi = xi1; xj = xj1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Swap the two scan line indices.
                int saTmp = saPrev;
                saPrev = saCurr;
                saCurr = saTmp;
                scanAreas[saCurr].Clear();
            }

            #endregion

            #region Mark the areas of all inside groups for elimination.

            int result = 0;

            for (int g = 0; g < scanLineReferenceGroups.Count; g++)
            {
                // Determine effective group id.
                int id = g; while (unitedGroupIds[id] < id) { id = unitedGroupIds[id]; }
                if (id == outsideGroupId)
                {
                    continue;
                }

                if (unitedGroupIds[g] == g)
                {
                    ++result;
                }

                foreach (Point slr in scanLineReferenceGroups[g])
                {
                    int y = slr.Y, p = slr.X, q = p + 1;

                    // Reverse the scan line point's orientation: R..L -> L..R.
                    borderXsLR[y][p] = !borderXsLR[y][p]; // true
                    borderXsLR[y][q] = !borderXsLR[y][q]; // false
                }
            }

            /* As all reference groups other than the outside group have been marked for elimination,
             * only the outside group will be left over.
             * The resulting borderXs define a single connected object shape without inclusions.
             */

            #endregion

            if (result > 0)
            {
                // The LR sense of all identified inside regions have been swapped.
                // Now they look like overlapped object regions and may be eliminated using the overlap method.
                EliminateOverlaps(borderXs, borderXsLR);
            }

            return result;
        }

        #endregion

        #region Methods for following an object contour.

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

        #endregion

        #region Fill operations.

        private static void FillOutside(Graphics g, Color color, List<int>[] borderXs)
        {
            FillOutside_Simple(g, color, borderXs);
        }

        private static void FillOutside_Simple(Graphics g, Color color, List<int>[] borderXs)
        {
            Pen pen = new Pen(new SolidBrush(color));
            pen.StartCap = pen.EndCap = LineCap.Flat;
            pen.Width = 1;

            for (int y = 0; y < borderXs.Length; y++)
            {
                for (int p = 0, q = p + 1; q < borderXs[y].Count; p += 2, q += 2)
                {
                    int x0 = borderXs[y][p] + 1, x1 = borderXs[y][q] - 1;
                    g.DrawLine(pen, x0, y, x1, y);
                }
            }
        }

        #endregion
    }
}
