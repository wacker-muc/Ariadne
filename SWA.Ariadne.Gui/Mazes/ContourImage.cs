using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SWA.Ariadne.Gui.Mazes
{
    public class ContourImage
    {
        #region Constants

        private const int ContourDistance = 16;
        private const int BlurDistanceMax = 12;
        private const int MaxColorDistance = 255;

        #endregion

        #region Internal types.

        private struct RelativePoint
        {
            /// <summary>
            /// Relative position of the point.
            /// </summary>
            public readonly int rx, ry;

            /// <summary>
            /// Alpha value to be applied to the pixel at (rx, ry).
            /// </summary>
            public readonly int a;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="rx"></param>
            /// <param name="ry"></param>
            /// <param name="d2"></param>
            public RelativePoint(int rx, int ry, int a)
            {
                this.rx = rx; this.ry = ry; this.a = a;
            }

            /// <summary>
            /// Constructor.
            /// Creates a RelativePoint whose d2 value is irrelevant.
            /// </summary>
            /// <param name="rx"></param>
            /// <param name="ry"></param>
            public RelativePoint(int rx, int ry)
            {
                this.rx = rx; this.ry = ry; this.a = 0;
            }

            public override string ToString()
            {
                return string.Format("a({0},{1}) = {2}", rx, ry, a);
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
        /// For each combination of a next left and next right neighbor of a pixel,
        /// the borderLimit is the set of points on the left and/or right (outside) edge of the influenceRegion.
        /// </summary>
        /// Note: As these points are applied symmetrically on the left and right, only non-negative values are stored.
        private static List<RelativePoint>[,] borderLimits = new List<RelativePoint>[8, 8];

        /// <summary>
        /// For each combination of a next left and next right neighbor of a pixel,
        /// the contourLimit is the set of points on the left and/or right (inside) edge of the influenceRegion.
        /// </summary>
        /// Note: As these points are applied symmetrically on the left and right, only non-negative values are stored.
        private static List<RelativePoint>[,] contourLimits = new List<RelativePoint>[8, 8];

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

            /* Note: Even when influenceRange = ContourDistance
             * we still have at least one border pixel in the given range
             * on every scan line.
             */

            // Prepare a mapping of d2 (distance-squared) values to alpha values.
            int[] alphaMap = new int[range2Max + 1];
            for (int d2 = range2Min; d2 <= range2Max; d2++)
            {
                double d = Math.Sqrt(d2);
                int a = (int)(255 * (d - ContourDistance) / (influenceRange - ContourDistance));
                alphaMap[d2] = a;
            }

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

                    #region Calculate the influence region, contour and border limits for the current left and right neighbor.

                    influenceRegions[nbL, nbR] = new List<RelativePoint>();
                    borderLimits[nbL, nbR] = new List<RelativePoint>();
                    contourLimits[nbL, nbR] = new List<RelativePoint>();

                    // For each scan line: Leftmost and rightmost point in the influence region.
                    // Use index [dy + influenceRange].
                    // Enter dx + influenceRange which is > 0.
                    // Note: We need to collect left and right limits separately.
                    //       Otherwise, a non-influence on one side would overwrite an influence on the other side.
                    int[] leftLimitsB = new int[2 * influenceRange], rightLimitsB = new int[2 * influenceRange];
                    int[] leftLimitsC = new int[2 * influenceRange], rightLimitsC = new int[2 * influenceRange];

                    // We want to traverse dx from the center outwards,
                    // facilitating the registry of left and right limits.
                    for (int i = 1; i < 2 * influenceRange; i++)
                    {
                        // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                        int dx = (i / 2) * (i % 2 == 0 ? +1 : -1);
                        int dxAbs = Math.Abs(dx) + influenceRange; // TODO: rename

                        for (int j = 1; j < 2 * influenceRange; j++)
                        {
                            // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                            int dy = (j / 2) * (j % 2 == 0 ? +1 : -1);
                            int dyAbs = dy + influenceRange; // TODO: rename

                            // Get the distance to the three points.
                            int d2 = dx * dx + dy * dy;
                            int d2L = (dx - dxL) * (dx - dxL) + (dy - dyL) * (dy - dyL);
                            int d2R = (dx - dxR) * (dx - dxR) + (dy - dyR) * (dy - dyR);
                            int d2I = (dx - dxI) * (dx - dxI) + (dy - dyI) * (dy - dyI);

                            if (d2 <= range2Max)
                            {
                                // This value will be entered into the left/right limits if the current point is in the influence region.
                                int borderLimitsEntry = dxAbs;
                                int contourLimitsEntry = -1;

                                // Check if the center pixel is closest to the point.
                                // For equal distance, the left neighbor should dominate.
                                // Only distances in the relevant range are recorded.
                                if (d2 < d2L && d2 <= d2R && d2 < d2I)
                                {
                                    if (range2Min <= d2)
                                    {
                                        // The point is partially influenced.
                                        influenceRegions[nbL, nbR].Add(new RelativePoint(dx, dy, alphaMap[d2]));
                                    }
                                    else
                                    {
                                        // The point is fully influenced.
                                        contourLimitsEntry = dxAbs;
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

                        if (leftLimitsB[i] > 0 || rightLimitsB[i] > 0)
                        {
                            int dx = Math.Max(leftLimitsB[i], rightLimitsB[i]) - influenceRange;
                            borderLimits[nbL, nbR].Add(new RelativePoint(dx, dy));
                        }

                        if (leftLimitsC[i] > 0 || rightLimitsC[i] > 0)
                        {
                            int dx = Math.Max(leftLimitsC[i], rightLimitsC[i]) - influenceRange;
                            contourLimits[nbL, nbR].Add(new RelativePoint(dx, dy));
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
            borderLimits[NbW, NbE].Add(new RelativePoint(0, dyN));
            borderLimits[NbE, NbW].Add(new RelativePoint(0, dyS));

            dyS = ContourDistance - 1; dyN = -dyS;
            contourLimits[NbW, NbE].Add(new RelativePoint(0, dyN));
            contourLimits[NbE, NbW].Add(new RelativePoint(0, dyS));

            #endregion
        }

        #endregion

        #region Member variables and Properties.

        private Color backgroundColor;
        int bgR, bgG, bgB;

        /// <summary>
        /// Gets the width of a region around the image that should be blurred gradually
        /// from the background color to complete black.
        /// </summary>
        /// <returns></returns>
        private int BlurDistance
        {
            get
            {
                float b = backgroundColor.GetBrightness();
                return (int)(Math.Sqrt(b) * BlurDistanceMax);
            }
        }

        /// <summary>
        /// Gets the width of a region around the image that should not be completely black.
        /// That is the ContourDistance plus the BlurDistance for the given background color.
        /// </summary>
        /// <returns></returns>
        private int FrameWidth
        {
            get
            {
                return ContourDistance + BlurDistance;
            }
        }

        /// <summary>
        /// The image that was given in the constructor.
        /// </summary>
        private Bitmap template;

        /// <summary>
        /// The mask that is applied to the template image.
        /// Black mask pixels represent the template's background.
        /// </summary>
        private Bitmap mask;

        /// <summary>
        /// A Graphics object associated with the mask.
        /// </summary>
        private Graphics gMask;

        /// <summary>
        /// Gets the processed template image.
        /// Background areas at a certain distance from the image objects are painted black.
        /// </summary>
        public Image ProcessedImage
        {
            get
            {
                if (image == null)
                {
                    CreateImage();

                    int fuzziness = (int)(0.03 * MaxColorDistance);
                    Rectangle bbox = CreateMask(fuzziness);
                    ApplyMask();

                    image = Crop(image, bbox);
                    mask = Crop(mask, bbox);
                }

                return image; 
            }
        }
        private Bitmap image;

        #endregion

        #region Constructor.

        /// <summary>
        /// Returns an image with the background color set to black.
        /// The background is guessed from the pixels on the image border.
        /// The contour of the image (without its background) is extended by ContourDistance and BlurDistance.
        /// </summary>
        /// <param name="template">the original image</param>
        /// <param name="mask">will be set to the mask that was applied to the template</param>
        /// <returns></returns>
        public ContourImage(Image template)
        {
            int fuzziness = (int)(0.05 * MaxColorDistance);

            this.template = template as Bitmap;
            if (this.template == null)
            {
                this.template = new Bitmap(template);
            }

            float share = GuessBackgroundColor(fuzziness);

            if (share < 0.8F)
            {
                this.mask = null;
                this.image = this.template;
                return;
            }
        }

        #endregion

        #region Methods for identifying non-background objects in an image.

        /// <summary>
        /// Builds the bitmap mask to be applied to the template image.
        /// Areas dominated by the background color will be black.
        /// Other areas will be transparent.
        /// Returns the resulting area that is not completely masked.
        /// </summary>
        /// <param name="fuzziness"></param>
        /// 
        private Rectangle CreateMask(int fuzziness)
        {
            int contourDist = ContourDistance;
            int blurDist = BlurDistance;

            #region Create the result bitmap.

            Color black = Color.Black;
            black = Color.Magenta; // TODO: delete this line
            Color transparent = Color.FromArgb(0, black);

            // Create a new Bitmap with the same resolution as the original image.
            int width = image.Width, height = image.Height;
            this.mask = new Bitmap(width, height, Graphics.FromImage(image));
            this.gMask = Graphics.FromImage(mask);
            gMask.FillRectangle(new SolidBrush(transparent), 0, 0, width, height);

            #endregion

            #region Create local variables used in the contour scan.

            PrepareInfluenceRegions(contourDist + blurDist);
            int range2Max = (contourDist + blurDist) * (contourDist + blurDist);

            // For every pixel: The alpha value to be used in the mask.
            // Actually, we store (a - 256); thus, the default initial value 0 is like "extremely high".
            int[,] alpha = new int[width, height];

            // All points on the immediate contour of an object are collected in scan switch lines.
            List<int>[] inside;

            // The points on the outside border of the objects' influence regions are collected in scan switch lines.
            // On the scan line, the points are sorted by (unique) increasing X values.
            // The scan line starts with one Right point and ends with one Left point outside of the image width range.
            // Between these, the points mark pairs of Left and Right borders.
            List<int>[] border;

            // The points on the extended contour (the region with 100% influence) are also collected in scan switch lines.
            List<int>[] contour;

            // Create the scan line lists and add the required terminator entries.
            InitializeScanLines(width, height, out inside, out contour, out border);

            #endregion

            #region Scan the image for non background objects.

            // Maximum and minimum distance between scan lines.
            const int dsMax = 16 * 1024;
            const int dsMin = 4;

            int nObjects = 0;

            // Apply horizontal scan lines in decreasing distance.
            // The generated y values are:
            //   ds =  2: 0, 2, 4, 6, 8, 10, ...   -- all even numbers (is not used)
            //   ds =  4: 1, 5, 9, 13, ...
            //   ds =  8: 3, 11, 19, ...
            //   ds = 16: 7, 23, ...
            for (int ds = dsMax; ds >= 2 * dsMin; ds /= 2)
            {
                for (int y = ds / 2 - 1; y < height; y += ds)
                {
                    // Current index in inside[y].
                    int i = 0;
                    // X coordinate of the first known object on the Y scan line.
                    int x1 = inside[y][i];

                    for (int x = 1; x < width; x += 1)
                    {
                        if (x >= x1)
                        {
                            // We have reached the contour of a known object.
                            // Skip to the next contour point.
                            x = inside[y][++i];       // Point where the scan line leaves the object.
                            x1 = inside[y][++i];      // Point of next object or terminator image.Width.
                            continue;
                        }

                        if (ColorDistance(image.GetPixel(x, y)) > fuzziness)
                        {
                            if (ScanObject(x, y, fuzziness, alpha, inside, contour, border))
                            {
                                ++nObjects;

                                // Make X advance to the object point where the scan line leaves the object.
                                // The new object's point (x, y) is at the current c position.
                                x = inside[y][++i];       // Point where the scan line leaves the object.
                                x1 = inside[y][++i];      // Point on next object or terminator image.Width.
                            }
                        }
                    }
                }
            }

            #endregion

            // Eliminate enclosed regions from border and contour scan lines.
            EliminateInsideRegions(border, 0, 1);
            EliminateInsideRegions(contour, 0, 1);

            // Contour derived from object.
            DrawContour(contour, Color.Red);

            // Derive another set of scan lines from the border, closer to the object.
            List<int>[] insetBorder = ShrinkRegion(border, blurDist + contourDist / 2);

            // Contour derived from border.
            DrawContour(insetBorder, Color.Blue);

            // The contour keeps the area around the object untouched.
            // The insetBorder avoids entering into areas enclosed by the border (and a blurred contour) only.
            UniteScanLines(insetBorder, contour);

            // Fill outside of objects, using the border.
            FillOutside(black, border);

            // Set the color of the mask pixels between border and contour to black with an appropriate transparency.
            PaintGradient(alpha, border, contour, contourDist, blurDist, black);

            // Border.
            DrawContour(border, Color.Cyan);

            return BoundingBox(border);
        }

        /// <summary>
        /// Scans the contour of an object starting at a given point on the contour.
        /// (x0-1,y0) is outside and (x0,y0) is inside the image.
        /// The influence regions of all contour pixels are applied to the alpha map.
        /// Returns true if a sufficiently large object was detected.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="fuzziness"></param>
        /// <param name="alpha"></param>
        /// <param name="inside"></param>
        /// <param name="contour"></param>
        /// <param name="border"></param>
        private bool ScanObject(int x0, int y0, int fuzziness, int[,] alpha, List<int>[] inside, List<int>[] contour, List<int>[] border)
        {
            #region Choose an initial focus point with a left and right neighbor.

            // Use the start point as the focus point.
            int x = x0, y = y0;

            // Determine right neighbor.
            int nbR, xR, yR;
            RightNeighbor(fuzziness, x, y, NbW, out nbR, out xR, out yR);
            if (nbR == NbW)
            {
                // There is no neighbor pixel.  One-pixel objects are ignored.
                return false;
            }

            // Determine left neighbor.
            int nbL, xL, yL;
            LeftNeighbor(fuzziness, x, y, nbR, out nbL, out xL, out yL);

            #endregion

            // Set up termination conditions.
            // (x,y) will be processed first; we are finished when we return to the same pixel in the same direction.
            int x1 = x, y1 = y, nb1 = nbL;

            #region Scan the outside contour of the object.

            do
            {
                // Number of neighbors of the current nbL that need not be tested
                // when we advance to the following left neighbor (see LeftNeighbor()).
                // This number is positive when some relevant points were already tested
                // when we advanced from nbR to the current point.
                int skipNeighbors = 0;

                #region Register the object point in the "inside" scan lines.

                switch (nbR * 8 + nbL)
                {
                    case (NbW * 8 + NbE):       //  R o L
                    case (NbE * 8 + NbW):       //    .
                        // do nothing; we'll wait what happens next
                        skipNeighbors = 1;
                        break;

                    case (NbSW * 8 + NbE):      //    o L
                    case (NbNE * 8 + NbW):      //  R .
                        // do nothing; we'll wait what happens next
                        skipNeighbors = 1;
                        break;

                    case (NbW * 8 + NbSE):      //  R o
                    case (NbE * 8 + NbNW):      //    . L
                        // do nothing; wait until (xL,yL) is processed in the next iteration
                        skipNeighbors = 1;
                        break;

                    case (NbSW * 8 + NbSE):     //    o
                    case (NbNE * 8 + NbNW):     //  R . L
                        //                      //    .
                        // do nothing; wait until (xL,yL) is processed in the next iteration
                        skipNeighbors = 2;
                        break;

                    case (NbW * 8 + NbNE):      //
                    case (NbW * 8 + NbN):       //  L L L
                    case (NbW * 8 + NbNW):      //  R o 
                    case (NbW * 8 + NbW):       //
                    case (NbE * 8 + NbSW):
                    case (NbE * 8 + NbS):
                    case (NbE * 8 + NbSE):
                    case (NbE * 8 + NbE):
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbS * 8 + NbNW):      //
                    case (NbS * 8 + NbW):       //  L
                    case (NbN * 8 + NbSE):      //  L o
                    case (NbN * 8 + NbE):       //    R
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbS * 8 + NbNE):      //
                    case (NbS * 8 + NbN):       //    L L
                    case (NbN * 8 + NbSW):      //    o .
                    case (NbN * 8 + NbS):       //    R
                        InsertObjectPoint(inside[y], x);
                        skipNeighbors = 1;
                        break;

                    case (NbNW * 8 + NbSE):     //
                    case (NbNW * 8 + NbE):      //  R
                    case (NbSE * 8 + NbNW):     //    o L
                    case (NbSE * 8 + NbW):      //      L
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbNW * 8 + NbS):      //  R
                    case (NbSE * 8 + NbN):      //  . o
                    //                          //    L
                        InsertObjectPoint(inside[y], x);
                        skipNeighbors = 1;
                        break;

                    case (NbNW * 8 + NbSW):     //    R
                    case (NbSE * 8 + NbNE):     //  . . o
                    //                          //    L
                        InsertObjectPoint(inside[y], x);
                        skipNeighbors = 2;
                        break;

                    case (NbSW * 8 + NbNE):     //
                    case (NbSW * 8 + NbN):      //  L L L
                    case (NbSW * 8 + NbNW):     //  L o
                    case (NbSW * 8 + NbW):      //  R
                    case (NbNE * 8 + NbSW):
                    case (NbNE * 8 + NbS):
                    case (NbNE * 8 + NbSE):
                    case (NbNE * 8 + NbE):
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbNW * 8 + NbNE):     //
                    case (NbNW * 8 + NbN):      //  R L L
                    case (NbNW * 8 + NbNW):     //    o
                    case (NbSE * 8 + NbSW):     //
                    case (NbSE * 8 + NbS):
                    case (NbSE * 8 + NbSE):
                        InsertObjectPoint(inside[y], x);
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbSW * 8 + NbSW):     //    o
                    case (NbNE * 8 + NbNE):     //  R
                        InsertObjectPoint(inside[y], x);
                        InsertObjectPoint(inside[y], x);
                        break;

                    case (NbS * 8 + NbSW):      //
                    case (NbS * 8 + NbS):       //    o
                    case (NbN * 8 + NbNE):      //  L R
                    case (NbN * 8 + NbN):       //
                        InsertObjectPoint(inside[y], x);
                        InsertObjectPoint(inside[y], x);
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
                    if (rp.a - 256 < alpha[i, j])
                    {
                        alpha[i, j] = rp.a - 256;
                    }
                }

                // Enter the focus point's border points into the respective border scan lines.
                foreach (RelativePoint rp in borderLimits[nbL, nbR])
                {
                    InsertPair(border[y + rp.ry], x - rp.rx, x + rp.rx);
                }

                // Enter the focus point's contour points into the respective contour scan lines.
                foreach (RelativePoint rp in contourLimits[nbL, nbR])
                {
                    InsertPair(contour[y + rp.ry], x - rp.rx, x + rp.rx);
                }

                // Advance the focus to the next object pixel.
                xR = x; yR = y; nbR = (nbL + 4) % 8;
                x = xL; y = yL;
                LeftNeighbor(fuzziness, x, y, (nbR + skipNeighbors) % 8, out nbL, out xL, out yL);

            } while (x != x1 || y != y1 || nb1 != nbL);

            #endregion

            return true;
        }

        #endregion

        #region Methods for following an object contour.

        /// <summary>
        /// Locates the left neighbor of a contour pixel that is also on the object contour.
        /// </summary>
        /// <param name="fuzziness"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nbR">direction of the current pixel's right neighbor</param>
        /// <param name="nbL"></param>
        /// <param name="xL"></param>
        /// <param name="yL"></param>
        private void LeftNeighbor(int fuzziness, int x, int y, int nbR, out int nbL, out int xL, out int yL)
        {
            xL = yL = 0; // make compiler happy
            for (nbL = (nbR + 1) % 8; ; nbL = (nbL + 1) % 8)
            {
                xL = x + NbDX[nbL]; yL = y + NbDY[nbL];
                if (nbL == nbR)
                {
                    break; // We have completed the circle.
                }

                if (ColorDistance(image.GetPixel(xL, yL)) > fuzziness)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Locates the right neighbor of a contour pixel that is also on the object contour.
        /// </summary>
        /// <param name="fuzziness"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nbL">direction of the current pixel's left neighbor</param>
        /// <param name="nbR"></param>
        /// <param name="xR"></param>
        /// <param name="yR"></param>
        private void RightNeighbor(int fuzziness, int x, int y, int nbL, out int nbR, out int xR, out int yR)
        {
            xR = yR = 0; // make compiler happy
            for (nbR = (nbL - 1 + 8) % 8; ; nbR = (nbR - 1 + 8) % 8)
            {
                xR = x + NbDX[nbR]; yR = y + NbDY[nbR];
                if (nbR == nbL)
                {
                    break; // We have completed the circle.
                }

                if (ColorDistance(image.GetPixel(xR, yR)) > fuzziness)
                {
                    break;
                }
            }
        }

        #endregion

        #region Methods for initializing the data structures.

        private static void InitializeScanLines(int width, int height, out List<int>[] inside, out List<int>[] contour, out List<int>[] border)
        {
            inside = new List<int>[height];
            contour = new List<int>[height];
            border = new List<int>[height];

            for (int i = 0; i < inside.Length; i++)
            {
                // Create the lists.
                inside[i] = new List<int>(16);
                contour[i] = new List<int>(16);
                border[i] = new List<int>(16);

                // Add termination points that are well beyond the regular scan line.

                // One terminator for the object.
                inside[i].Add(width + 1);

                // Two terminators for the contour.
                contour[i].Add(-2);
                contour[i].Add(width + 1);

                // Two terminators for the border.
                border[i].Add(-2);
                border[i].Add(width + 1);
            }
        }

        #endregion

        #region Methods for managing the object points.

        private static void InsertObjectPoint(List<int> scanLine, int x)
        {
            // Find the position p with x < objectX[p].
            // Note: As there is a terminator entry image.Width, we will not leave the valid index range.
            for (int p = 0; ; p++)
            {
                if (x < scanLine[p])
                {
                    scanLine.Insert(p, x);
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
        private static void InsertPair(List<int> scanLine, int xL, int xR)
        {
            // Position where the border points fit into the scanLine.
            int q = 2;      // A right border point position.

            // Find the position (p,q) where (xL .. xR) overlaps or touches (scanLine[p] .. scanLine[q]).
            while (q < scanLine.Count && scanLine[q] < xL - 1)
            {
                q += 2;
            }
            int p = q - 1;  // A (valid) left border point position.

            if (xR + 1 < scanLine[p])    // no overlap; insert new LR pair
            {
                scanLine.Insert(p, xR);
                scanLine.Insert(p, xL);
            }
            else                        // overlap
            {
                // Find the following scan line LR pair that is still within the reach of the new LR pair.
                int p1 = p + 2;
                while (p1 < scanLine.Count - 2 && scanLine[p1] <= xR + 1)
                {
                    p1 += 2;
                }
                p1 -= 2;

                // Eliminate RL pairs that are overlapped or touched by the new LR pair.
                if (p1 > p)
                {
                    scanLine.RemoveRange(q, p1 - p);
                }

                // Extend the current LR pair up to the the extent of the given LR pair.
                scanLine[p] = Math.Min(scanLine[p], xL);
                scanLine[q] = Math.Max(scanLine[q], xR);
            }
        }

        /// <summary>
        /// Identifies and eliminates areas in or between the objects defined by the scan lines
        /// that are completely enclosed by the objects.
        /// Note: The shape described in scanLines must not touch any image border;
        ///       otherwise cut off areas will be considered "inside" and eliminated.
        /// </summary>
        /// <param name="scanLines">list of normalized scan lines</param>
        /// <param name="y0">index of first valid scan line</param>
        /// <param name="sy">step between consecutive valid scan lines</param>
        /// <returns>number of inside regions found</returns>
        private static int EliminateInsideRegions(List<int>[] scanLines, int y0, int sy)
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
            scanLineReferenceGroups.Add(new List<Point>((scanLines[y0].Count - 1) / 2));
            for (int p = 0, q = 1; q < scanLines[y0].Count; p += 2, q += 2)
            {
                int xp = scanLines[y0][p], xq = scanLines[y0][q];
                scanLineReferenceGroups[outsideGroupId].Add(new Point(p, y0));
                scanAreas[saPrev].Add(new Point(p, outsideGroupId));
            }

            #endregion

            #region Collect the inside areas of all following scan lines in reference groups.

            for (int y = y0 + sy; y < scanLines.Length; y += sy)
            {
                int a = 0;                  // index into scanAreas[saPrev]
                Point pa = scanAreas[saPrev][a];
                int i = pa.X, j = i + 1;    // indices into scanLines
                // Note that the x coordinates are those of the object; the spaces between are two pixels smaller.
                int xi = scanLines[y - sy][i] + 1, xj = scanLines[y - sy][j] - 1;

                // Process the areas between objects on the current scan line.
                // As the scan line is normalized, these areas are between even and odd positions.
                for (int p = 0, q = 1; q < scanLines[y].Count; p += 2, q += 2)
                {
                    int xp = scanLines[y][p] + 1, xq = scanLines[y][q] - 1;

                    // Advance the previous line's scan area until it lies at or ahead of this line's area.
                    // Note: The last (outside) area on the previous line extends to the right image border.
                    while (xp > xj)
                    {
                        pa = scanAreas[saPrev][++a];
                        i = pa.X; j = i + 1;
                        xi = scanLines[y - sy][i] + 1; xj = scanLines[y - sy][j] - 1;
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
                        scanLineReferenceGroups.Add(new List<Point>((scanLines[y].Count - 1) / 2));

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
                        int xi1 = scanLines[y - sy][i1] + 1, xj1 = scanLines[y - sy][j1] - 1;

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

                    // Mark the two RL points for deletion.
                    scanLines[y][p] = -1;
                    scanLines[y][q] = -1;
                }
            }

            /* As all reference groups other than the outside group have been marked for elimination,
             * only the outside group will be left over.
             * The resulting scanLines define a single connected object shape without inclusions.
             */

            #endregion

            if (result > 0)
            {
                #region Remove all inner RL pairs that have been marked for deletion.
                for (int j = 0; j < scanLines.Length; j++)
                {
                    // R-L|RLR...LRL|R-L
                    for (int p = scanLines[j].Count - 4; p >= 2; p -= 2)
                    {
                        if (scanLines[j][p] < 0)
                        {
                            scanLines[j].RemoveRange(p, 2);
                        }
                    }
                }
                #endregion
            }

            SWA.Utilities.Log.WriteLine("} EliminateInsideRegions <<<");

            return result;
        }

        private static List<int>[] ShrinkRegion(List<int>[] scanLines, int margin)
        {
            List<int>[] result = new List<int>[scanLines.Length];
            int xMin = scanLines[0][0], xMax = scanLines[0][scanLines[0].Count - 1];

            // Start with the border scan lines, indented left and right by the margin distance.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new List<int>(scanLines[i].Count);
                result[i].Add(xMin);
                result[i].Add(xMax);

                for (int p = 1, q = p + 1; q < scanLines[i].Count; p += 2, q += 2)
                {
                    int xp = scanLines[i][p] + margin, xq = scanLines[i][q] - margin;
                    if (xp <= xq)
                    {
                        InsertPair(result[i], scanLines[i][p] + margin, scanLines[i][q] - margin);
                    }
                }
            }

            // Overlay the border scan lines, translated by all points on a circle of radius margin.
            int d2Max = (margin + 1) * (margin + 1) - 1;
            for (int dx = 0, dy = margin; dx <= dy; dx++)
            {
                // Four octants, close to Y axis.
                IntersectScanLines(scanLines, result, +dx, +dy);
                IntersectScanLines(scanLines, result, +dx, -dy);
                IntersectScanLines(scanLines, result, -dx, +dy);
                IntersectScanLines(scanLines, result, -dx, -dy);

                // Four octants, close to X axis.
                IntersectScanLines(scanLines, result, +dy, +dx);
                IntersectScanLines(scanLines, result, +dy, -dx);
                IntersectScanLines(scanLines, result, -dy, +dx);
                IntersectScanLines(scanLines, result, -dy, -dx);

                // Advance south if we would leave the circle.
                if ((dx + 1) * (dx + 1) + dy * dy > d2Max)
                {
                    dy -= 1;
                }
            }

            return result;
        }

        private static void IntersectScanLines(List<int>[] source, List<int>[] target, int dx, int dy)
        {
            IntersectScanLines(source, target, dx, dy, 1);
        }

        private static void UniteScanLines(List<int>[] source, List<int>[] target)
        {
            // Uniting the regions on two scan lines is like intersecting the gaps inbetween them.  :-)
            IntersectScanLines(source, target, 0, 0, 0);
        }

        private static void IntersectScanLines(List<int>[] source, List<int>[] target, int dx, int dy, int p0)
        {
            for (int yt = 0, ys = yt - dy; yt < target.Length; yt++, ys++)
            {
                int tc = target[yt].Count;

                // Skip empty target scan lines.
                if (tc <= 2)
                {
                    continue;
                }

                if (ys < 0 || ys >= source.Length || source[ys].Count <= 2)
                {
                    if (p0 % 2 == 1)
                    {
                        // Clear the target scan line.
                        target[yt].RemoveRange(1, tc - 2);
                    }
                    continue;
                }

                int sc = source[ys].Count;

                int ps = p0, pt = p0, qs = ps + 1, qt = pt + 1; // left and right margin of first region
                int sl = source[ys][ps] + dx, sr = source[ys][qs] + dx;
                int tl = target[yt][pt], tr = target[yt][qt];

                while (true)
                {
                    if (tr < sl) // c left of b
                    {
                        // Remove the current contour region.
                        target[yt].RemoveRange(pt, 2); tc -= 2;
                        if (qt >= tc) // behind last target region
                        {
                            break;
                        }
                        tl = target[yt][pt]; tr = target[yt][qt];

                        continue;
                    }
                    else if (sr < tl) // b left of c
                    {
                        // Advance to the next border region.
                        ps += 2; qs += 2;
                        if (qs >= sc) // behind last source region
                        {
                            // Remove the current and all other remaining target regions.
                            target[yt].RemoveRange(pt, tc - qt);
                            break;
                        }
                        sl = source[ys][ps] + dx; sr = source[ys][qs] + dx;

                        continue;
                    }

                    if (tl < sl)
                    {
                        // Adjust the left margin of this target region.
                        target[yt][pt] = tl = sl;
                    }

                    if (sr < tr) // c extends to the right of b
                    {
                        // Split this contour region.
                        target[yt].InsertRange(pt + 2, new int[] {
                            sr, // split location, will be corrected in the following iterations
                            tr, // current right margin
                        }); tc += 2;
                        target[yt][qt] = tr = sr; // split location

                        // Advance to the next source region.
                        ps += 2; qs += 2;
                        if (qs >= sc) // behind last source region
                        {
                            // Remove the remaining target regions.
                            target[yt].RemoveRange(pt + 2, tc - qt - 2);
                            break;
                        }
                        sl = source[ys][ps] + dx; sr = source[ys][qs] + dx;
                    }

                    // Advance to the next target region.
                    pt += 2; qt += 2;
                    if (qt >= tc) // behind last target region
                    {
                        break;
                    }
                    tl = target[yt][pt]; tr = target[yt][qt];
                }
            }
        }

        #endregion

        #region Image related methods.

        /// <summary>
        /// Determines the color that is dominant in the template image border (width: 2 pixels).
        /// Returns the ratio of background color pixels among all pixels (0.0 .. 1.0)
        /// </summary>
        /// <param name="fuzziness">maximum color distance that is considered equal to the dominant color</param>
        /// <returns></returns>
        private float GuessBackgroundColor(int fuzziness)
        {
            int borderWidth = 2;
            int xMin = 0, xMax = template.Width - 1;
            int yMin = 0, yMax = template.Height - 1;

            #region Collect a sample of pixels near the image border.

            List<Color> pixels = new List<Color>(borderWidth * (template.Width + template.Height));

            for (int x = 0; x < template.Width; x += 1 + x % 7)
            {
                pixels.Add(template.GetPixel(x, yMin + (x + 0) % borderWidth));
                pixels.Add(template.GetPixel(x, yMax - (x + 1) % borderWidth));
            }
            for (int y = 0 + borderWidth; y < template.Height - borderWidth; y += 1 + y % 5)
            {
                pixels.Add(template.GetPixel(xMin + (y + 0) % borderWidth, y));
                pixels.Add(template.GetPixel(xMax - (y + 1) % borderWidth, y));
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
            this.bgR = rSum / n;
            this.bgG = gSum / n;
            this.bgB = bSum / n;
            this.backgroundColor = Color.FromArgb(bgR, bgG, bgB);

            #endregion

            #region Determine number of pixels that are within the fuzziness range.

            int nShare = 0;

            foreach (Color px in pixels)
            {
                if (ColorDistance(px) <= fuzziness)
                {
                    ++nShare;
                }
            }

            float result = (float)nShare / (float)pixels.Count;

            #endregion

            return result;
        }

        /// <summary>
        /// Compares the given color with the backgroundColor.
        /// Returns a value between 0 (identical colors) and 255 (opposite colors).
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        /// Note: This method is called extremely often.  Therefore, it is highly optimized.
        private int ColorDistance(Color color)
        {
            int colR = color.R, colG = color.G, colB = color.B;
            int result = 0;

            if (colR - bgR > result) { result = colR - bgR; }
            if (bgR - colR > result) { result = bgR - colR; }
            if (colG - bgG > result) { result = colG - bgG; }
            if (bgG - colG > result) { result = bgG - colG; }
            if (colB - bgB > result) { result = colB - bgB; }
            if (bgB - colB > result) { result = bgB - colB; }
            
            return result;
        }

        /// <summary>
        /// Copies the template image into a Bitmap image that is augmented with a background color frame.
        /// Thus, all image pixels are located at least GetFrameWidth() from the result's border.
        /// </summary>
        /// <param name="image"></param>
        private void CreateImage()
        {
            int width = template.Width, height = template.Height;

            // Extend the image with a frame of the background color.
            // Thus, all parts of the image are at least that far away from the border.
            int frameWidth = FrameWidth;
            width += 2 * frameWidth;
            height += 2 * frameWidth;

            // Extend the image by another two pixels.
            // Thus, even the frame won't touch the border.
            // Otherwise, the contour scan could fail and interpret disparate outside regions as included inside regions.
            width += 2;
            height += 2;

            // Create a new Bitmap with the same resolution as the original image.
            this.image = new Bitmap(width, height, Graphics.FromImage(template));
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), new Rectangle(0, 0, image.Width, image.Height));
            g.DrawImageUnscaled(template, frameWidth, frameWidth);
        }

        /// <summary>
        /// Draws the mask over the image.
        /// </summary>
        private void ApplyMask()
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

        #endregion

        #region Mask related methods.

        private void FillOutside(Color color, List<int>[] scanLines)
        {
            Pen pen = new Pen(color);
            pen.StartCap = pen.EndCap = LineCap.Square;
            pen.Width = 1;

            for (int y = 0; y < scanLines.Length; y++)
            {
                for (int p = 0, q = p + 1; q < scanLines[y].Count; p += 2, q += 2)
                {
                    int x0 = scanLines[y][p] + 1, x1 = scanLines[y][q] - 1;
                    if (x0 == x1)
                    {
                        // Single dots are not drawn properly. :-(
                        x1 += 1;
                    }
                    gMask.DrawLine(pen, x0, y, x1, y);
                }
            }
        }

        private static Rectangle BoundingBox(List<int>[] scanLines)
        {
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            for (int y = 0; y < scanLines.Length; y++)
            {
                if (scanLines[y].Count > 2)
                {
                    if (bbyMin == int.MaxValue) { bbyMin = y; }
                    bbyMax = y;
                    bbxMin = Math.Min(bbxMin, scanLines[y][1]);
                    bbxMax = Math.Max(bbxMax, scanLines[y][scanLines[y].Count - 2]);
                }
            }

            return new Rectangle(bbxMin, bbyMin, bbxMax - bbxMin + 1, bbyMax - bbyMin + 1);
        }

        private void PaintGradient(int[,] alpha, List<int>[] border, List<int>[] contour, int contourDist, int blurDist, Color black)
        {
            Color[] aColor = new Color[256];
            for (int a = 0; a < 256; a++)
            {
                aColor[a] = Color.FromArgb(a, black);
            }

            int width = mask.Width, height = mask.Height;
            for (int y = 0; y < height; y++)
            {
                if (border[y].Count < 4) // not inside the border of any object
                {
                    continue;
                }

                // Only process regions on the scan line that are
                //  * inside the border and
                //  * outside the contour.
                int b = 2, xb = border[y][b]; // right end of the first border region
                int c = 1, xc = contour[y][c]; // left end of the first contour region

                for (int x = border[y][1]; x < width; x++)
                {
                    if (x >= xc) // At the left end of a contour region.
                    {
                        // Skip to the right end of the contour region.
                        // Note: This is still inside the current border region.
                        x = contour[y][++c];
                        xc = contour[y][++c];
                        continue;
                    }

                    if (x > xb) // Beyond the right end of a border region.
                    {
                        if (b + 2 >= border[y].Count) // This was the last border region.
                        {
                            break;
                        }

                        // Skip before the left end of the following inside border region.
                        // Note: This is still outside of the following contour region.
                        x = border[y][++b] - 1;
                        xb = border[y][++b];
                        continue;
                    }

                    // Paint the mask pixel with its given alpha value.
                    int a = alpha[x, y] + 256;
                    if (0 <= a && a < 255)
                    {
                        Color maskColor = aColor[a];
                        mask.SetPixel(x, y, maskColor);
                    }
                }
            }
        }

        /// <summary>
        /// Paints into the mask the pixels marked by points on the given scanLines in the given color.
        /// </summary>
        /// <param name="scanLines"></param>
        /// <param name="color"></param>
        private void DrawContour(List<int>[] scanLines, Color color)
        {
#if false // TODO: false; used for debugging only
            for (int y = 0; y < scanLines.Length; y++)
            {
                for (int p = 1; p < scanLines[y].Count - 1; p++)
                {
                    int x = scanLines[y][p];
                    mask.SetPixel(x, y, color);
                }
            }
#endif
        }

        #endregion
    }
}
