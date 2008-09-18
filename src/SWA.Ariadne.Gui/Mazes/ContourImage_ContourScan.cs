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
        /// the borderLimit is the set of points on the left and right (outside) edge of the influenceRegion.
        /// </summary>
        /// TODO: Combine into one set of borderLimits.
        private static List<RelativePoint>[,]
            leftBorderLimits = new List<RelativePoint>[8, 8],
            rightBorderLimits = new List<RelativePoint>[8, 8];

        /// <summary>
        /// For each combination of a next left and next right neighbor of a pixel,
        /// the contourLimit is the set of points on the left and right (inside) edge of the influenceRegion.
        /// </summary>
        /// TODO: Combine into one set of borderLimits.
        private static List<RelativePoint>[,]
            leftContourLimits = new List<RelativePoint>[8, 8],
            rightContourLimits = new List<RelativePoint>[8, 8];

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
            int result = 0;

            foreach (RelativePoint rp in leftBorderLimits[nbL, nbR])
            {
                if ((rp.rx == x) && (rp.ry == y))
                {
                    result--;
                }
            }
            foreach (RelativePoint rp in rightBorderLimits[nbL, nbR])
            {
                if ((rp.rx == x) && (rp.ry == y))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Set up the influenceRegions and associated data for a given influence range.
        /// </summary>
        /// <param name="influenceRange"></param>
        private static void PrepareInfluenceRegions(int influenceRange)
        {
            // One full pixel inside the fully covered contour range.
            int range2Min = (ContourDistance - 1) * (ContourDistance - 1);
            // Slightly inside the given influence range.
            int range2Max = influenceRange * influenceRange - 1;

            /* Even when influenceRange = ContourDistance
             * we still have at least one border pixel in the given range
             * on every scan line.
             */

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
                    leftContourLimits[nbL, nbR] = new List<RelativePoint>();
                    rightContourLimits[nbL, nbR] = new List<RelativePoint>();

                    // For each scan line: Leftmost and rightmost point in the influence region.
                    // Use index [dy + influenceRange].
                    // Enter dx + influenceRange which is > 0.
                    int[] leftLimitsB = new int[2 * influenceRange], rightLimitsB = new int[2 * influenceRange];
                    int[] leftLimitsC = new int[2 * influenceRange], rightLimitsC = new int[2 * influenceRange];

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

                            if (d2 <= range2Max)
                            {
                                // This value will be entered into the left/right limits if the current point is in the influence region.
                                int borderLimitsEntry = dx + influenceRange;
                                int contourLimitsEntry = -1;

                                // Check if the center pixel is closest to the point.
                                // For equal distance, the left neighbor should dominate.
                                // Only distances in the relevant range are recorded.
                                if (d2 < d2L && d2 <= d2R && d2 < d2I)
                                {
                                    if (range2Min <= d2)
                                    {
                                        // The point is partially influenced.
                                        influenceRegions[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                                    }
                                    else
                                    {
                                        // The point is fully influenced.
                                        contourLimitsEntry = dx + influenceRange;
                                    }
                                }
                                else
                                {
                                    // Overwrite and remove a previous entry in the left/right border limits.
                                    borderLimitsEntry = -1;
                                }

                                if (dx < 0)
                                {
                                    leftLimitsB[dy + influenceRange] = borderLimitsEntry;
                                    if (d2 < range2Min)
                                    {
                                        leftLimitsC[dy + influenceRange] = contourLimitsEntry;
                                    }
                                }
                                if (dx > 0)
                                {
                                    rightLimitsB[dy + influenceRange] = borderLimitsEntry;
                                    if (d2 < range2Min)
                                    {
                                        rightLimitsC[dy + influenceRange] = contourLimitsEntry;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Transfer information from the locally collected left and right limits to the static variables.

                    for (int i = 0; i < 2 * influenceRange; i++)
                    {
                        int dy = i - influenceRange;

                        if (leftLimitsB[i] > 0)
                        {
                            int dx = leftLimitsB[i] - influenceRange;
                            int d2 = dx * dx + dy * dy;
                            leftBorderLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }

                        if (rightLimitsB[i] > 0)
                        {
                            int dx = rightLimitsB[i] - influenceRange;
                            int d2 = dx * dx + dy * dy;
                            rightBorderLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }

                        if (leftLimitsC[i] > 0)
                        {
                            int dx = leftLimitsC[i] - influenceRange;
                            int d2 = dx * dx + dy * dy;
                            leftContourLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }

                        if (rightLimitsC[i] > 0)
                        {
                            int dx = rightLimitsC[i] - influenceRange;
                            int d2 = dx * dx + dy * dy;
                            rightContourLimits[nbL, nbR].Add(new RelativePoint(dx, dy, d2));
                        }
                    }

                    #endregion
                }
            }

            #region Special handling of horizonal stretches.

            /* On a wide horizontal object border (flat top or bottom),
             * the border points registered above are not sufficient.
             * There might be a gap between the region influenced by the pixels
             * on the left and right ends of the stretch.
             */

            // Add a single border pixel right above or below the horizontal directions.
            int dyS = influenceRange - 1, dyN = -dyS;
            leftBorderLimits[NbW, NbE].Add(new RelativePoint(0, dyN, dyN * dyN));
            leftBorderLimits[NbE, NbW].Add(new RelativePoint(0, dyS, dyS * dyS));
            rightBorderLimits[NbW, NbE].Add(new RelativePoint(0, dyN, dyN * dyN));
            rightBorderLimits[NbE, NbW].Add(new RelativePoint(0, dyS, dyS * dyS));

            dyS = ContourDistance - 1; dyN = -dyS;
            leftContourLimits[NbW, NbE].Add(new RelativePoint(0, dyN, dyN * dyN));
            leftContourLimits[NbE, NbW].Add(new RelativePoint(0, dyS, dyS * dyS));
            rightContourLimits[NbW, NbE].Add(new RelativePoint(0, dyN, dyN * dyN));
            rightContourLimits[NbE, NbW].Add(new RelativePoint(0, dyS, dyS * dyS));

            #endregion
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

            // All points on the immediate contour of an object are collected in scan switch lines.
            List<int>[] objectXs;

            // The points on the outside border of the objects' influence regions are collected in scan switch lines.
            // On the scan line, the points are sorted by (unique) increasing X values.
            // The scan line starts with one Right point and ends with one Left point outside of the image width range.
            // Between these, the points mark pairs of Left and Right borders.
            List<int>[] borderXs;

            // The points on the extended contour (the region with 100% influence) are also collected in scan switch lines.
            List<int>[] contourXs;

            // Create and initialize the dist2Image array.
            InitializeDist2ToImage(image.Width, image.Height, out dist2ToImage);

            // Create the scan line lists and add the required terminator entries.
            InitializeScanLines(image.Width, image.Height, out objectXs, out contourXs, out borderXs);

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
                    // Current index in objectXs[y].
                    int c = 0;
                    // X coordinate of the first known object on the Y scan line.
                    int x1 = objectXs[y][c];

                    for (int x = 1; x < image.Width; x += 1)
                    {
                        if (x == x1)
                        {
                            // We have reached the contour of a known object.
                            // Skip to the next contour point.
                            x = objectXs[y][++c];       // Point where the scan line leaves the object.
                            x1 = objectXs[y][++c];      // Point of next object or terminator image.Width.
                            continue;
                        }

                        if (ColorDistance(image.GetPixel(x, y), backgroundColor) > fuzziness)
                        {
                            if (ScanObject(image, x, y, backgroundColor, fuzziness, dist2ToImage, objectXs, contourXs, borderXs))
                            {
                                ++nObjects;

                                // Make X advance to the object point where the scan line leaves the object.
                                // The new object's point (x, y) is at the current c position.
                                x = objectXs[y][++c];       // Point where the scan line leaves the object.
                                x1 = objectXs[y][++c];      // Point on next object or terminator image.Width.
                            }
                        }
                    }
                }
            }

            #endregion

            // Eliminate enclosed regions from border and contour scan lines.
            EliminateInsideRegions(borderXs, 0, 1);
            EliminateInsideRegions(contourXs, 0, 1);

            // Fill outside of objects, using the borderXs.
            FillOutside(gMask, black, borderXs);

            #region Set the color of all mask pixels to black with an appropriate transparency; determine the bounding box.

            // Coordinates of the bounding box.
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            for (int y = 0; y < image.Height; y++)
            {
                if (borderXs[y].Count < 4) // not inside the border of any object
                {
                    continue;
                }

                // Only process regions on the scan line that are
                //  * inside the borderXs and
                //  * outside the contourXs.
                int b = 2, xb = borderXs[y][b]; // right end of the first border region
                int c = 1, xc = contourXs[y][c]; // left end of the first contour region

                for (int x = borderXs[y][1]; x < image.Width; x++)
                {
                    if (x >= xc) // At the left end of a contour region.
                    {
                        // Skip to the right end of the contour region.
                        // Note: This is still inside the current border region.
                        x = contourXs[y][++c];
                        xc = contourXs[y][++c];
                        continue;
                    }

                    if (x > xb) // Beyond the right end of a border region.
                    {
                        if (b + 2 >= borderXs[y].Count) // This was the last border region.
                        {
                            break;
                        }

                        // Skip before the left end of the following inside border region.
                        // Note: This is still outside of the following contour region.
                        x = borderXs[y][++b] - 1;
                        xb = borderXs[y][++b];
                        continue;
                    }

                    double dist = Math.Sqrt(dist2ToImage[x, y]);
                    Color maskColor;

                    if (dist > contourDist + blurDist)
                    {
#if false
                        maskColor = black;
#else
                        // This pixel will also be filled with black as part of the area outside the influence border.
                        continue;
#endif
                    }
                    else
                    {
                        // TODO: Calculate bbox directly ffom borderXs.
                        bbxMin = Math.Min(bbxMin, x);
                        bbxMax = Math.Max(bbxMax, x);
                        bbyMin = Math.Min(bbyMin, y);
                        bbyMax = Math.Max(bbyMax, y);

                        if (dist <= contourDist)
                        {
#if false
                            maskColor = transparent;
#else
                            // The mask's default color is already transparent.
                            continue;
#endif
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
        /// <param name="objectXs"></param>
        /// <param name="contourXs"></param>
        /// <param name="borderXs"></param>
        private static bool ScanObject(Bitmap image, int x0, int y0, Color backgroundColor, float fuzziness, int[,] dist2ToImage, List<int>[] objectXs, List<int>[] contourXs, List<int>[] borderXs)
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
                #region Register the object point in the ordered list of X coordinates.

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
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbS * 8 + NbNE):      //
                    case (NbS * 8 + NbN):       //  L L L
                    case (NbS * 8 + NbNW):      //  L o
                    case (NbS * 8 + NbW):       //    R
                    case (NbN * 8 + NbSW):
                    case (NbN * 8 + NbS):
                    case (NbN * 8 + NbSE):
                    case (NbN * 8 + NbE):
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbNW * 8 + NbSW):     //
                    case (NbNW * 8 + NbS):      //  R
                    case (NbNW * 8 + NbSE):     //    o L
                    case (NbNW * 8 + NbE):      //  L L L
                    case (NbSE * 8 + NbNE):
                    case (NbSE * 8 + NbN):
                    case (NbSE * 8 + NbNW):
                    case (NbSE * 8 + NbW):
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbSW * 8 + NbNE):     //
                    case (NbSW * 8 + NbN):      //  L L L
                    case (NbSW * 8 + NbNW):     //  L o
                    case (NbSW * 8 + NbW):      //  R
                    case (NbNE * 8 + NbSW):
                    case (NbNE * 8 + NbS):
                    case (NbNE * 8 + NbSE):
                    case (NbNE * 8 + NbE):
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbNW * 8 + NbNE):     //
                    case (NbNW * 8 + NbN):      //  R L L
                    case (NbNW * 8 + NbNW):     //    o
                    case (NbSE * 8 + NbSW):     //
                    case (NbSE * 8 + NbS):
                    case (NbSE * 8 + NbSE):
                        InsertObjectPoint(objectXs[y], x);
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbSW * 8 + NbSW):     //    o
                    case (NbNE * 8 + NbNE):     //  R
                        InsertObjectPoint(objectXs[y], x);
                        InsertObjectPoint(objectXs[y], x);
                        break;

                    case (NbS * 8 + NbSW):      //
                    case (NbS * 8 + NbS):       //    o
                    case (NbN * 8 + NbNE):      //  L R
                    case (NbN * 8 + NbN):       //
                        InsertObjectPoint(objectXs[y], x);
                        InsertObjectPoint(objectXs[y], x);
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
                    InsertBorderPoints(borderXs[y + rp.ry], x + rp.rx, x - rp.rx);
                }
                foreach (RelativePoint rp in rightBorderLimits[nbL, nbR])
                {
                    InsertBorderPoints(borderXs[y + rp.ry], x - rp.rx, x + rp.rx);
                }

                // Enter the focus point's contour points into the respective contour scan lines.
                foreach (RelativePoint rp in leftContourLimits[nbL, nbR])
                {
                    InsertBorderPoints(contourXs[y + rp.ry], x + rp.rx, x - rp.rx);
                }
                foreach (RelativePoint rp in rightContourLimits[nbL, nbR])
                {
                    InsertBorderPoints(contourXs[y + rp.ry], x - rp.rx, x + rp.rx);
                }

                // Advance the focus to the next object pixel.
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

        private static void InitializeScanLines(int width, int height, out List<int>[] objectXs, out List<int>[] contourXs, out List<int>[] borderXs)
        {
            objectXs = new List<int>[height];
            contourXs = new List<int>[height];
            borderXs = new List<int>[height];

            for (int i = 0; i < objectXs.Length; i++)
            {
                // Create the lists.
                objectXs[i] = new List<int>(16);
                contourXs[i] = new List<int>(16);
                borderXs[i] = new List<int>(16);

                // Add termination points that are well beyond the regular scan line.

                // One terminator for the object.
                objectXs[i].Add(width + 1);

                // Two terminators for the contour.
                contourXs[i].Add(-2);
                contourXs[i].Add(width + 1);

                // Two terminators for the border.
                borderXs[i].Add(-2);
                borderXs[i].Add(width + 1);
            }
        }

        #endregion


        #region Methods for managing the object points.

        private static void InsertObjectPoint(List<int> objectX, int x)
        {
            // Position where the contour point will be entered into objectXs[y].
            int p;

            // Find the position p with x < objectX[p].
            // Note: As there is a terminator entry image.Width, we will not leave the valid index range.
            for (p = 0; ; p++)
            {
                if (x < objectX[p])
                {
                    objectX.Insert(p, x);
                    break;
                }
            }
        }

        #endregion

        #region Methods for managing the border and contour points.

        /// <summary>
        /// Inserts the given points into the given border scan line.
        /// </summary>
        /// <param name="borderX">scan line of alternating R-LR-LR-...-L border points</param>
        /// <param name="xL">left border point</param>
        /// <param name="xR">right border point</param>
        private static void InsertBorderPoints(List<int> borderX, int xL, int xR)
        {
            // Position where the border points fit into borderX.
            int q = 2;      // A right border point position.

            // Find the position (p,q) where (xL .. xR) overlaps or touches (borderX[p] .. borderX[q]).
            while (q < borderX.Count && borderX[q] < xL - 1)
            {
                q += 2;
            }
            int p = q - 1;  // A (valid) left border point position.

            if (xR + 1 < borderX[p])    // no overlap; insert new LR pair
            {
                borderX.Insert(p, xR);
                borderX.Insert(p, xL);
            }
            else                        // overlap
            {
                // Find the following scan line LR pair that is still within the reach of the new LR pair.
                int p1 = p + 2;
                while (p1 < borderX.Count - 2 && borderX[p1] <= xR + 1)
                {
                    p1 += 2;
                }
                p1 -= 2;

                // Eliminate RL pairs that are overlapped or touched by the new LR pair.
                if (p1 > p)
                {
                    borderX.RemoveRange(q, p1 - p);
                }

                // Extend the current LR pair up to the the extent of the given LR pair.
                borderX[p] = Math.Min(borderX[p], xL);
                borderX[q] = Math.Max(borderX[q], xR);
            }
        }

        /// <summary>
        /// Identifies and eliminates areas in or between the objects defined by the scan lines
        /// that are completely enclosed by the objects.
        /// Note: The shape described in borderXs must not touch any border;
        ///       otherwise cut off areas will be considered "inside" and eliminated.
        /// </summary>
        /// <param name="borderXs">list of normalized scan lines</param>
        /// <param name="y0">index of first valid scan line</param>
        /// <param name="sy">step between consecutive valid scan lines</param>
        /// <returns>number of inside regions found</returns>
        private static int EliminateInsideRegions(List<int>[] borderXs, int y0, int sy)
        {
            SWA.Utilities.Log.WriteLine(">>> EliminateInsideRegions {");

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

                    // Advance the previous line's scan area until it lies at or ahead of this line's area.
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

                        SWA.Utilities.Log.WriteLine(string.Format("creating new reference group: {0}", groupId));
                    }

                    // Add the current area to the selected group.
                    scanLineReferenceGroups[groupId].Add(new Point(p, y));
                    scanAreas[saCurr].Add(new Point(p, groupId));

                    SWA.Utilities.Log.WriteLine(string.Format("adding region ({2},{3}) on line {1} to group {0}", groupId, y, xp, xq));

                    // See if other areas on the previous line also overlap with the current area.
                    while (a + 1 < scanAreas[saPrev].Count)
                    {
                        int a1 = a + 1;
                        Point pa1 = scanAreas[saPrev][a1];
                        int i1 = pa1.X, j1 = i1 + 1;
                        int xi1 = borderXs[y - sy][i1] + 1, xj1 = borderXs[y - sy][j1] - 1;

                        if (xi1 <= xq)
                        {
                            // Get the other area's group ID.
                            int id1 = pa1.Y;

                            // Use the effective group IDs.
                            while (unitedGroupIds[id1] < id1) { id1 = unitedGroupIds[id1]; }
                            while (unitedGroupIds[groupId] < groupId) { groupId = unitedGroupIds[groupId]; }

                            // Unite the two areas' groups.
                            if (groupId < id1)
                            {
                                SWA.Utilities.Log.WriteLine(string.Format("uniting group {0} with {1} on line {2}", id1, groupId, y));
                                unitedGroupIds[id1] = groupId;
                            }
                            else if (id1 < groupId)
                            {
                                SWA.Utilities.Log.WriteLine(string.Format("uniting group {1} with {0} on line {2}", id1, groupId, y));
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

                SWA.Utilities.Log.WriteLine(string.Format("marking inside group {0}", g));

                foreach (Point slr in scanLineReferenceGroups[g])
                {
                    int y = slr.Y, p = slr.X, q = p + 1;

#if true // TODO: false
                    if (p % 2 != 0)
                    {
                        throw new Exception(string.Format("invalid scan line index: p = {0} in line {1} must not be odd", p, y));
                    }
                    if (q >= borderXs[y].Count - 1)
                    {
                        throw new Exception(string.Format("invalid scan line index: q = {0} in line {1} is at/beyond the right end {2}", q, y, borderXs[y].Count));
                    }
#endif

                    // Mark the two RL points for deletion.
                    borderXs[y][p] = -1;
                    borderXs[y][q] = -1;
                }
            }

            /* As all reference groups other than the outside group have been marked for elimination,
             * only the outside group will be left over.
             * The resulting borderXs define a single connected object shape without inclusions.
             */

            #endregion

            if (result > 0)
            {
                #region Remove all inner RL pairs that have been marked for deletion.
                for (int j = 0; j < borderXs.Length; j++)
                {
                    // R-L|RLR...LRL|R-L
                    for (int p = borderXs[j].Count - 4; p >= 2; p -= 2)
                    {
                        if (borderXs[j][p] < 0)
                        {
                            borderXs[j].RemoveRange(p, 2);
                        }
                    }
                }
                #endregion
            }

            SWA.Utilities.Log.WriteLine("} EliminateInsideRegions <<<");

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
            Pen pen = new Pen(color);
            pen.StartCap = pen.EndCap = LineCap.Square;
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
