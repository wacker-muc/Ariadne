using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Mazes
{
    public partial class ContourImage
    {
        #region Internal types for the block scan algorithm.

        private struct ScannedPixel
        {
            /// <summary>
            /// Pixel coordinates.
            /// </summary>
            public int x, y;

            /// <summary>
            /// Distance from the currently examined point p.
            /// d2 = (p.x - x)^2 + (p.y - y)^2
            /// </summary>
            public int d2;

            public override string ToString()
            {
                return string.Format("({0}, {1}) @ {2}", x, y, d2);
            }
        }

        private class ScanList
        {
            /// <summary>
            /// Pixels that are part of the image (not of the background).
            /// </summary>
            protected ScannedPixel[] pixels;

            /// <summary>
            /// Returns the ScannedPixel at index p.
            /// </summary>
            /// <param name="p">An index between 0 and Count-1.</param>
            /// <returns></returns>
            public ScannedPixel this[int p]
            {
                get { return pixels[p]; }
            }

            /// <summary>
            /// Number of valid entries in pixels.
            /// </summary>
            public virtual int Count
            {
                get { return count; }
            }
            protected int count;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="capacity"></param>
            protected ScanList(int capacity)
            {
                this.pixels = new ScannedPixel[capacity];
            }

            public void Add(int x, int y)
            {
                int p = count++;
                pixels[p].x = x;
                pixels[p].y = y;
                pixels[p].d2 = -1; // will be set correctly and used by the BlockScanList.GetClosestDist2() method
            }
        }

        // TODO: Replace ordered array with buckets of equal distance.
        private class BlockScanList : ScanList
            , IComparer<ScannedPixel>
        {
            /// <summary>
            /// Squared scan range parameters.
            /// </summary>
            private /* TODO: readonly */ int contourDist2, dist2;

            /// <summary>
            /// Number of entries (at the beginning) that are closer than ContourDist.
            /// Divides the pixels[] list into a "contour region" and a "blur region".
            /// </summary>
            private int contourCount;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="contourDist"></param>
            /// <param name="blurDist"></param>
            public BlockScanList(int contourDist, int blurDist)
                // Note: The optimum size would be 1 less in both directions.
                //       In X direction, the scan window may become 1 pixel too wide.
                : base((2 * (contourDist + blurDist)) * (2 * (contourDist + blurDist)))
            {
                this.contourDist2 = contourDist * contourDist;
                this.dist2 = (contourDist + blurDist) * (contourDist + blurDist);
            }

            /// <summary>
            /// 
            /// </summary>
            /// TODO: Eliminate together with Clone().
            private BlockScanList()
                : base(0)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// TODO: Eliminate this method by changing the scan direction bebetween following lines.
            public BlockScanList Clone()
            {
                BlockScanList result = new BlockScanList();
                result.contourDist2 = this.contourDist2;
                result.dist2 = this.dist2;
                result.pixels = (ScannedPixel[])this.pixels.Clone();
                result.count = this.count;
                result.contourCount = this.contourCount;
                return result;
            }

            /// <summary>
            /// Returns the (squared) distance of the closest pixel from the focus point.
            /// If the Pixels list is empty, returns a value greater than the maximum scan distance.
            /// </summary>
            /// <param name="focusX"></param>
            /// <param name="focusY"></param>
            /// <returns>a value between 0 and dist2+1</returns>
            /// Observation: While contourCount > 0, the array is never updated and sorted completely.
            ///              Therefore, contourCount will always be either 0 or 1.
            public int GetClosestDist2(int focusX, int focusY)
            {
                #region Try to find a pixel that is (still) in the contour region.
                while (contourCount > 0)
                {
                    int dx = pixels[0].x - focusX, dy = pixels[0].y - focusY;
                    int d2 = dx * dx + dy * dy;

                    if (d2 <= contourDist2)
                    {
                        return d2;
                    }
                    else
                    {
                        DegradeContourPixel(0);
                    }
                }
                #endregion

                #region Update the pixels' d2 value to the distance from the given focus point coordinates.
                int updatedDist2 = UpdatePixelDistance(focusX, focusY, false);
                if (updatedDist2 <= contourDist2)
                {
                    return updatedDist2;
                }
                #endregion

                #region Order the pixels[] array by d2 and return the first pixel's d2 value.
                if (count == 0)
                {
                    // The pixels[] list is empty.
                    return dist2 + 1;
                }

                Array.Sort(pixels, 0, count, this);

                return pixels[0].d2;
                #endregion
            }

            /// <summary>
            /// Updates the pixel's d2 value.
            /// If updateAll is false, returns the first d2 value that is .le. contourDist2.
            /// </summary>
            /// <param name="focusX"></param>
            /// <param name="focusY"></param>
            /// <param name="updateAll">when true, all pixels are updated</param>
            /// <returns></returns>
            /// TODO: Expand this mehtod where it is called.
            private int UpdatePixelDistance(int focusX, int focusY, bool updateAll)
            {
                for (int i = count; i-- > 0; )
                {
                    int dx = pixels[i].x - focusX, dy = pixels[i].y - focusY;
                    int d2 = dx * dx + dy * dy;

                    if (d2 <= contourDist2)
                    {
                        // We have found a contour pixel in the blur region.
                        // Note: As we process the pixels[] list from last (added) to first (added),
                        //       this pixel is likely to be on the right edge of the contour range.
                        //       It should be a valid contour pixel for the next few iterations.
                        PromoteBlurPixel(i);
                        if (!updateAll)
                        {
                            return d2;
                        }
                    }
                    else if (d2 >= dist2 && pixels[i].x < focusX)
                    {
                        // This pixel leaves the scan window (which moves from left to right).
                        RemoveAt(i);
                    }
                    else
                    {
                        pixels[i].d2 = d2;
                    }
                }
                return dist2 + 1;
            }

            /// <summary>
            /// Add all pixels of the given ColumnScanList.
            /// </summary>
            /// <param name="columnScanList"></param>
            /// <param name="focusY">current scan line</param>
            public void Add(ColumnScanList columnScanList, int focusY)
            {
                for (int i = 0; i < columnScanList.Count; i++)
                {
                    int ii = i + columnScanList.LowerBound;
                    this.Add(columnScanList[ii].x, columnScanList[ii].y);
                }

                // Now, the scan window width is .le. scanDiameter.
            }

            /// <summary>
            /// Removes outdated pixels if less than the required number of positions in pixels[] is free.
            /// </summary>
            /// <param name="scanWindowLeft">pixels to the left of (and including) this position will be removed</param>
            /// <param name="spaceRequired"></param>
            public void EnsureSpace(int scanWindowLeft, int spaceRequired)
            {
                // Protect against an index overflow.
                if (this.Count + spaceRequired >= pixels.Length)
                {
                    // Eliminate all outdated pixels that have not been removed until now.
                    // The pixels at x0 are at least scanRadius away from the current focusX.
                    for (int i = count; i-- > contourCount; )
                    {
                        if (pixels[i].x <= scanWindowLeft)
                        {
                            RemoveAt(i);
                        }
                    }

                    // Now, the scan window width is .lt. scanDiameter.
                }
            }

            /// <summary>
            /// Removes a pixel from the list.
            /// Call this method when a pixel exceeds the relevant maximum distance.
            /// </summary>
            /// <param name="p">a valid index, contourCount .le. p .lt. count</param>
            private void RemoveAt(int p)
            {
                pixels[p] = pixels[--count];
            }

            /// <summary>
            /// Moves a pixel from the contour region to the blur region.
            /// Call this method when a pixel in the contour region exceeds the contour distance.
            /// </summary>
            /// <param name="p">a valid index, p .lt. contourCount</param>
            private void DegradeContourPixel(int p)
            {
                Swap(p, --contourCount);
            }

            /// <summary>
            /// Moves a pixel from the blur region to the contour region.
            /// Call this method when a contour pixel is found in the blur region.
            /// </summary>
            /// <param name="p">a valid index, p .lt. contourCount</param>
            private void PromoteBlurPixel(int p)
            {
                Swap(p, contourCount++);
            }

            /// <summary>
            /// Exchanges two pixels in the list.
            /// </summary>
            /// <param name="p"></param>
            /// <param name="q"></param>
            private void Swap(int p, int q)
            {
                if (p != q)
                {
                    ScannedPixel tmp = pixels[p];
                    pixels[p] = pixels[q];
                    pixels[q] = tmp;
                }
            }

            #region IComparer<ScannedPixel> Members

            int IComparer<ScannedPixel>.Compare(ScannedPixel a, ScannedPixel b)
            {
                return (a.d2 - b.d2);
            }

            #endregion

            public override string ToString()
            {
                int xMin = int.MaxValue, xMax = 0;
                int yMin = int.MaxValue, yMax = 0;
                int d2Min = int.MaxValue, d2Max = 0;
                for (int i = 0; i < count; i++)
                {
                    int x = pixels[i].x;
                    int y = pixels[i].y;
                    int d2 = pixels[i].d2;
                    xMin = Math.Min(xMin, x);
                    xMax = Math.Max(xMax, x);
                    yMin = Math.Min(yMin, y);
                    yMax = Math.Max(yMax, y);
                    if (d2 >= 0)
                    {
                        d2Min = Math.Min(d2Min, d2);
                        d2Max = Math.Max(d2Max, d2);
                    }
                }

                return string.Format("BlockScanList[{0}/{1}], ({2}x{3}) = [{4}..{5}]x[{6}..{7}], d = [{8:0.00}..{9:0.00}]"
                    , contourCount, count
                    , (xMax - xMin + 1), (yMax - yMin + 1)
                    , xMin, xMax, yMin, yMax
                    , Math.Sqrt(d2Min), Math.Sqrt(d2Max));
            }
        }

        private class ColumnScanList : ScanList
        {
            /// <summary>
            /// First valid index in pixels[].
            /// </summary>
            public int LowerBound
            {
                get { return lowerBound; }
            }
            private int lowerBound;

            public override int Count
            {
                get { return count - lowerBound; }
            }

            public ScannedPixel First
            {
                get { return base[lowerBound]; }
            }

            public int X
            {
                // Note: All pixels in the column have the same X value.
                //       Even if the list is empty, the first item was initialized by the constructor.
                get { return this.pixels[0].x; }
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="imageHeight">total height of the image column</param>
            /// <param name="x"></param>
            public ColumnScanList(int imageHeight, int x)
                : base(imageHeight)
            {
                // Initialize the value used by this.X
                this.pixels[0].x = x;
            }

            public void RemoveFirst()
            {
                this.lowerBound += 1;
            }

            public override string ToString()
            {
                return string.Format("ColumnScanList({0})[{1}]: p = [{2}..{3}]",
                    X, Count,
                    lowerBound, count - 1);
            }
        }

        #endregion

        /// <summary>
        /// Returns a bitmap to be applied to the given image.
        /// Correct but inefficient algorithm.
        /// Precondition: No part of the image is closer than GetBorderWidth() to the Bitmap border.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="boundingBox">resulting area that is not completely masked</param>
        /// <returns></returns>
        /// Too much time is consumed by pruning and sorting large arrays (~3.000 items).
        /// TODO: Set color of larger regions instead of single pixels.
        private static Bitmap GetMask_BlockScan(Bitmap image, Color backgroundColor, float fuzziness, out Rectangle boundingBox)
        {
            int contourDist = ContourDistance;
            int blurDist = GetBlurDistance(backgroundColor);

            // Create a new Bitmap with the same resolution as the original image.
            Bitmap result = new Bitmap(image.Width, image.Height, Graphics.FromImage(image));

            #region Create local variables used in the block scan.

            // The image is assumed to be inside a core region around which there is a border of the background color.
            int borderWidth = contourDist + blurDist;
            int coreLeft = borderWidth, coreRight = image.Width - borderWidth;
            int coreTop = borderWidth, coreBottom = image.Height - borderWidth;

            // The image is scanned with a window centered around the current pixel
            // and comprising all pixels closer than the countourDist plus blurDist.
            int scanRadius = contourDist + blurDist - 1;
            int scanDiameter = 2 * scanRadius; // see also BlockScanList() constructor

            // The image pixels within the scan window are kept in a list ordered by increasing distance from the focus point.
            BlockScanList scanList = new BlockScanList(contourDist, blurDist);

            // Each line scan starts with the scan list of the first pixel in the previous line.
            // Initially, the list is empty.
            BlockScanList prevLineScanList = new BlockScanList(contourDist, blurDist);

            // When the scan window advances by one column, the pixels of that column are added to the scanList.
            // These column lists are remembered for each point on the current scan line
            // and will be re-used on the next scan line.
            // The column lists are ordered from top to bottom, i.e. in direction of the lines scanned.
            ColumnScanList[] scanColumns = new ColumnScanList[image.Width];
            for (int x = 0; x < scanColumns.Length; x++)
            {
                scanColumns[x] = new ColumnScanList(image.Height, x);
            }

            #endregion

            #region Scan the surroundings of every point for image pixels.

            // Coordinates of the bounding box.
            int bbxMin = int.MaxValue, bbxMax = int.MinValue, bbyMin = bbxMin, bbyMax = bbxMax;

            // The color of all mask pixels will be set to black with an appropriate transparency.
            Color black = Color.Black;
            black = Color.Magenta; // TODO: delete this line
            Color transparent = Color.FromArgb(0, black);

            // Sweep the image with a scan widow from left to right.

            int contourDist2 = contourDist * contourDist;
            int blurDist2 = scanRadius * scanRadius;

            int w = result.Width, h = result.Height;

            for (int y = 0, yy = scanRadius; y < h; y++, yy++)
            {
                for (int x = 0, xx = scanRadius; x < w; x++, xx++)
                {
                    #region Determine the mask pixel's transparency.
                    int d2 = scanList.GetClosestDist2(x, y);
                    Color maskColor;

                    if (d2 > blurDist2)
                    {
                        maskColor = black;
                    }
                    else
                    {
                        bbxMin = Math.Min(bbxMin, x);
                        bbxMax = Math.Max(bbxMax, x);
                        bbyMin = Math.Min(bbyMin, y);
                        bbyMax = Math.Max(bbyMax, y);

                        if (d2 <= contourDist2)
                        {
                            maskColor = transparent;
                        }
                        else
                        {
                            int a = (int)(255 * (Math.Sqrt(d2) - contourDist) / blurDist);
                            maskColor = Color.FromArgb(a, black);
                        }
                    }

                    result.SetPixel(x, y, maskColor);
                    #endregion

                    #region Add the pixels of the next column entering the focus window.
                    if (xx < coreRight)
                    {
                        // Remove outdated pixels if free space in the scanList is precarious.
                        scanList.EnsureSpace(xx - scanDiameter, scanDiameter + 1);

                        // Use the previous line's scan column.
                        if (scanColumns[xx].Count > 0)
                        {
                            // Remove the first entry if it is too far above the current line.
                            if (scanColumns[xx].First.y <= yy - scanDiameter)
                            {
                                scanColumns[xx].RemoveFirst();
                            }

                            // Add the pixels from the previous line's scan column.
                            scanList.Add(scanColumns[xx], y + 1);
                        }

                        if (yy < coreBottom)
                        {
                            // Add the pixel at the far corner of the focus window.
                            if (ColorDistance(image.GetPixel(xx, yy), backgroundColor) > fuzziness)
                            {
                                scanColumns[xx].Add(xx, yy);
                                scanList.Add(xx, yy);
                            }
                        }
                    }
                    #endregion
                }

                // Start the next line scan with the current line's initial scan list.
                scanList = prevLineScanList;

                #region Add the pixels of the next row entering the focus window.
                if (yy < coreBottom)
                {
                    for (int i = coreLeft; i < 0 + scanDiameter; i++)
                    {
                        if (ColorDistance(image.GetPixel(i, yy), backgroundColor) > fuzziness)
                        {
                            scanList.Add(i, yy);
                        }
                    }
                }
                #endregion

                // Remember this scan list for the next iteration.
                prevLineScanList = scanList.Clone();
            }

            #endregion

            #region Create the bounding box.

            boundingBox = new Rectangle(bbxMin, bbyMin, bbxMax - bbxMin + 1, bbyMax - bbyMin + 1);

            #endregion

            return result;
        }
    }
}
