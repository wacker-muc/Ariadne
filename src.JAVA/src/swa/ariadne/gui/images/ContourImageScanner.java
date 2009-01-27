package swa.ariadne.gui.images;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Transparency;
import java.awt.image.BufferedImage;
import java.awt.image.ColorModel;
import java.util.ArrayList;
import java.util.List;

import swa.util.Bitmap;
import swa.util.ImageUtil;

/**
 * Provides a method for scanning the contour of a foreground object within an image
 * in front of a uniform background.
 *
 * @author Stephan.Wacker@web.de
 */
final
class ContourImageScanner
{
    //--------------------- Constants

    /** The maximum value returned by {@link ContourImageScanner#colorDistance(int)}. */
    public static final int MaxColorDistance = 255;

    //--------------------- Types

    /**
     * Properties of a point on the immediate contour of the image's foreground object.
     */
    private final class ScanPoint
    {
        //--------------------- Member variables and Properties

        /** X coordinate. */
        public int x;
        /** Y coordinate. */
        public int y;
        /** Direction to left neighbor pixel. */
        public int nbL;
        /** Direction to right neighbor pixel. */
        public int nbR;

        //--------------------- Constructors

        /**
         * Constructor.
         * @param x X coordinate.
         * @param y Y coordinate.
         */
        public ScanPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.nbL = this.nbR = -1;
        }
        /**
         * Constructor.
         */
        public ScanPoint()
        {
            this(0, 0);
        }
    }

    //--------------------- Constants

    /** The {@linkplain ColorModel#getRGBdefault() default} RBG {@link ColorModel}. */
    private static final ColorModel cm = ColorModel.getRGBdefault();

    //--------------------- Member variables and Properties

    /** The color that dominates the image background. */
    private Color backgroundColor;

    /**
     * @return The color that dominates the image background.
     */
    public Color getBackgroundColor()
    {
        return this.backgroundColor;
    }

    /** Red component of the background color. */
    private int bgR;
    /** Green component of the background color. */
    private int bgG;
    /** Blue component of the background color. */
    private int bgB;

    /** The image that was given in the constructor. */
    private BufferedImage image;

    /** A Bitmap representation of the image. */
    private Bitmap bitmap;

    /** The points on the immediate contour of an image object. */
    private ScanLines inside;

    /** The points on the extended contour (the region with 100% influence). */
    private ScanLines contour;

    /**
     * @return The image's {@link #contour}.
     */
    public ScanLines getContour()
    {
        return this.contour;
    }

    /** The points on the outside border of the image objects' influence regions. */
    private ScanLines border;

    /**
     * @return The image's {@link #border}.
     */
    public ScanLines getBorder()
    {
        return this.border;
    }

    /** Contour pixel influence regions, background color dependent. */
    private ContourSurroundings surroundings;

    /**
     * @return The image's {@link ContourSurroundings} object.
     */
    public ContourSurroundings getSurroundings()
    {
        return surroundings;
    }

    //--------------------- Constructors

    /**
     * @param image The original image.
     */
    public ContourImageScanner(BufferedImage image)
    {
        this.image = image;
        this.bitmap = new Bitmap(this.image);
    }


    /**
     * Initializes this object's {@link ContourSurroundings} that can be retrieved via
     * {@link #getSurroundings()}.
     * <p>
     * This method should be called after a valid
     * {@linkplain #getBackgroundColor() background color}
     * has been determined.
     */
    public void initSurroundings()
    {
        this.surroundings = new ContourSurroundings(backgroundColor);
    }

    /**
     * Extends the {@link #image} with a background color frame.
     * Thus, all image pixels are located at least
     * {@linkplain ContourSurroundings#getFrameWidth() frame width}
     * from the result's border.
     * @return The extended {@link #image}.
     */
    public BufferedImage grow()
    {
        int width = image.getWidth(), height = image.getHeight();

        // Extend the image with a frame of the background color.
        // Thus, all parts of the image are at least that far away from the border.
        int frameWidth = surroundings.getFrameWidth();
        width += 2 * frameWidth;
        height += 2 * frameWidth;

        // Extend the image by another two pixels.
        // Thus, even the frame won't touch the border.
        // Otherwise, the contour scan could fail and interpret disparate outside regions as included inside regions.
        width += 2;
        height += 2;

        // Create a new BufferedImage with the same resolution as the original image.
        BufferedImage extendedImage = ImageUtil.createBufferedImage(width, height, Transparency.OPAQUE);
        Graphics g = extendedImage.getGraphics();
        g.setColor(backgroundColor);
        g.fillRect(0, 0, width, width);
        g.drawImage(image, frameWidth, frameWidth, null);

        this.image = extendedImage;
        this.bitmap = new Bitmap(this.image);

        return this.image;
    }

    /**
     * Initializes the various {@linkplain ScanLine scan line} lists.
     * @param width Maximum pixel position on a scan line +1, width of the image.
     * @param height Number of horizontal scan lines, height of the image.
     */
    private void initializeScanLines(int width, int height)
    {
        this.inside = new ScanLines(height, width, false);
        this.contour = new ScanLines(height, width, true);
        this.border = new ScanLines(height, width, true);
    }

    //--------------------- Methods for analyzing the image background.

    /**
     * Determines the color that is dominant in the {@link #image} border (width: 2 pixels).
     * @param fuzziness Maximum color distance that is considered equal to the dominant color.
     * @return The ratio of background color pixels among all analyzed pixels (0.0 .. 1.0).
     */
    public float guessBackgroundColor(int fuzziness)
    {
        int width = bitmap.getWidth(), height = bitmap.getHeight();
        int borderWidth = 2;
        int xMin = 0, xMax = bitmap.getWidth() - 1;
        int yMin = 0, yMax = bitmap.getHeight() - 1;

        // Some circular shapes touch the four borders in the middle region.
        int xMid0 = width * 9 / 20, xMid1 = width * 11 / 20;
        int yMid0 = height * 9 / 20, yMid1 = height * 11 / 20;

        // [start] Collect a sample of pixels near the image border.

        List<Integer> pixels = new ArrayList<Integer>(borderWidth * (width + height));

        for (int x = 0; x < width; x += 1 + x % 7)
        {
            if (xMid0 <= x && x <= xMid1) { continue; }
            pixels.add(bitmap.getPixel(x, yMin + (x + 0) % borderWidth));
            pixels.add(bitmap.getPixel(x, yMax - (x + 1) % borderWidth));
        }
        for (int y = 0 + borderWidth; y < height - borderWidth; y += 1 + y % 5)
        {
            if (yMid0 <= y && y <= yMid1) { continue; }
            pixels.add(bitmap.getPixel(xMin + (y + 0) % borderWidth, y));
            pixels.add(bitmap.getPixel(xMax - (y + 1) % borderWidth, y));
        }

        // [end]

        // [start] Determine average color of the pixels on the border.

        // Sum of RGB pixel values.
        int rSum = 0, gSum = 0, bSum = 0;

        for (int px : pixels)
        {
            rSum += cm.getRed(px);
            gSum += cm.getGreen(px);
            bSum += cm.getBlue(px);
        }

        int n = pixels.size();
        this.bgR = rSum / n;
        this.bgG = gSum / n;
        this.bgB = bSum / n;
        this.backgroundColor = new Color(bgR, bgG, bgB);

        // [end]

        // [start] Determine number of pixels that are within the fuzziness range.

        int nShare = 0;

        for (int px : pixels)
        {
            if (colorDistance(px) <= fuzziness)
            {
                ++nShare;
            }
        }

        float result = (float)nShare / (float)n;

        // [end]

        return result;
    }

    /**
     * Compares the given color with the {@link #backgroundColor}.
     * <p>
     * <em>Note:</em> This method is called extremely often.  Therefore, it is highly optimized.
     * @param pixel A pixel value in the default RGB ColorSpace, sRGB.
     * @return A value between 0 (identical colors) and 255 (opposite colors).
     */
    private final int colorDistance(int pixel)
    {
        int pxR = cm.getRed(pixel), pxG = cm.getGreen(pixel), pxB = cm.getBlue(pixel);
        int result = 0;

        if (pxR - bgR > result) { result = pxR - bgR; }
        if (bgR - pxR > result) { result = bgR - pxR; }
        if (pxG - bgG > result) { result = pxG - bgG; }
        if (bgG - pxG > result) { result = bgG - pxG; }
        if (pxB - bgB > result) { result = pxB - bgB; }
        if (bgB - pxB > result) { result = bgB - pxB; }

        return result;
    }

    //--------------------- Methods for following an object contour.

    /**
     * @param fuzziness Maximum color distance that is considered equal to the background color.
     * @return A transparency map of the same size as the image.
     */
    public int[][] scan(int fuzziness)
    {
        // Create the scan line lists and add the required terminator entries.
        int width = image.getWidth(), height = image.getHeight();
        initializeScanLines(width, height);

        // For every pixel: The alpha value to be used in the mask.
        // Actually, we store (a - 256); thus, the default initial value 0 is like "extremely high".
        int[][] alpha = new int[width][height];

        // Maximum and minimum distance between scan lines.
        final int dsMax = 16 * 1024;
        final int dsMin = 4;

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
                int x1 = inside.get(y, i);

                for (int x = 1; x < width; x += 1)
                {
                    if (x >= x1)
                    {
                        // We have reached the contour of a known object.
                        // Skip to the next contour point.
                        x = inside.get(y, ++i);       // Point where the scan line leaves the object.
                        x1 = inside.get(y, ++i);      // Point of next object or terminator image.Width.
                        continue;
                    }

                    if (colorDistance(bitmap.getPixel(x, y)) > fuzziness)
                    {
                        if (scanObject(fuzziness, x, y, alpha))
                        {
                            ++nObjects;

                            // Make X advance to the object point where the scan line leaves the object.
                            // The new object's point (x, y) is at the current c position.
                            x = inside.get(y, ++i);       // Point where the scan line leaves the object.
                            x1 = inside.get(y, ++i);      // Point on next object or terminator image.Width.
                        }
                    }
                }
            }
        }

        return alpha;
    }

    /**
     * Scans the contour of an object starting at a given point on the contour.
     * (x0-1,y0) is outside and (x0,y0) is inside the image.
     * The influence regions of all contour pixels are applied to the given alpha map.
     * @param fuzziness Maximum color distance that is considered equal to the background color.
     * @param x0 X coordinate of a point on the contour.
     * @param y0 Y coordinate of a point on the contour.
     * @param alpha A transparency map.
     * @return True if a sufficiently large object was detected.
     */
    private boolean scanObject(int fuzziness, int x0, int y0, int[][] alpha)
    {
        // [start] Choose an initial focus point with a left and right neighbor.

        // Use the start point as the focus point.
        ScanPoint p = new ScanPoint(x0, y0);

        // Determine right neighbor, set p.nbR.
        rightNeighbor(fuzziness, p, RelativePoint.NbW);
        if (p.nbR == RelativePoint.NbW)
        {
            // There is no neighbor pixel.  One-pixel objects are ignored.
            return false;
        }

        // Determine left neighbor, set p.nbL.
        ScanPoint pl = leftNeighbor(fuzziness, p, p.nbR);

        // [end]

        // Set up termination conditions.
        // (p.x, p.y) will be processed first;
        // we are finished when we return to the same pixel in the same direction.
        ScanPoint p1 = p;

        // [start] Scan the outside contour of the object.

        do
        {
            // Number of neighbors of the current nbL that need not be tested
            // when we advance to the following left neighbor (see LeftNeighbor()).
            // This number is positive when some relevant points were already tested
            // when we advanced from nbR to the current point.
            int skipNeighbors = 0;

            // [start] Register the object point in the "inside" scan lines.

            switch (p.nbR * 8 + p.nbL)
            {
                case (RelativePoint.NbW * 8 + RelativePoint.NbE):       //  R o L
                case (RelativePoint.NbE * 8 + RelativePoint.NbW):       //    .
                    // do nothing; we'll wait what happens next
                    skipNeighbors = 1;
                    break;

                case (RelativePoint.NbSW * 8 + RelativePoint.NbE):      //    o L
                case (RelativePoint.NbNE * 8 + RelativePoint.NbW):      //  R .
                    // do nothing; we'll wait what happens next
                    skipNeighbors = 1;
                    break;

                case (RelativePoint.NbW * 8 + RelativePoint.NbSE):      //  R o
                case (RelativePoint.NbE * 8 + RelativePoint.NbNW):      //    . L
                    // do nothing; wait until (xL,yL) is processed in the next iteration
                    skipNeighbors = 1;
                    break;

                case (RelativePoint.NbSW * 8 + RelativePoint.NbSE):     //    o
                case (RelativePoint.NbNE * 8 + RelativePoint.NbNW):     //  R . L
                    //                      //    .
                    // do nothing; wait until (xL,yL) is processed in the next iteration
                    skipNeighbors = 2;
                    break;

                case (RelativePoint.NbW * 8 + RelativePoint.NbNE):      //
                case (RelativePoint.NbW * 8 + RelativePoint.NbN):       //  L L L
                case (RelativePoint.NbW * 8 + RelativePoint.NbNW):      //  R o
                case (RelativePoint.NbW * 8 + RelativePoint.NbW):       //
                case (RelativePoint.NbE * 8 + RelativePoint.NbSW):
                case (RelativePoint.NbE * 8 + RelativePoint.NbS):
                case (RelativePoint.NbE * 8 + RelativePoint.NbSE):
                case (RelativePoint.NbE * 8 + RelativePoint.NbE):
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbS * 8 + RelativePoint.NbNW):      //
                case (RelativePoint.NbS * 8 + RelativePoint.NbW):       //  L
                case (RelativePoint.NbN * 8 + RelativePoint.NbSE):      //  L o
                case (RelativePoint.NbN * 8 + RelativePoint.NbE):       //    R
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbS * 8 + RelativePoint.NbNE):      //
                case (RelativePoint.NbS * 8 + RelativePoint.NbN):       //    L L
                case (RelativePoint.NbN * 8 + RelativePoint.NbSW):      //    o .
                case (RelativePoint.NbN * 8 + RelativePoint.NbS):       //    R
                    inside.InsertObjectPoint(p.y, p.x);
                    skipNeighbors = 1;
                    break;

                case (RelativePoint.NbNW * 8 + RelativePoint.NbSE):     //
                case (RelativePoint.NbNW * 8 + RelativePoint.NbE):      //  R
                case (RelativePoint.NbSE * 8 + RelativePoint.NbNW):     //    o L
                case (RelativePoint.NbSE * 8 + RelativePoint.NbW):      //      L
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbNW * 8 + RelativePoint.NbS):      //  R
                case (RelativePoint.NbSE * 8 + RelativePoint.NbN):      //  . o
                //                          //    L
                    inside.InsertObjectPoint(p.y, p.x);
                    skipNeighbors = 1;
                    break;

                case (RelativePoint.NbNW * 8 + RelativePoint.NbSW):     //    R
                case (RelativePoint.NbSE * 8 + RelativePoint.NbNE):     //  . . o
                //                          //    L
                    inside.InsertObjectPoint(p.y, p.x);
                    skipNeighbors = 2;
                    break;

                case (RelativePoint.NbSW * 8 + RelativePoint.NbNE):     //
                case (RelativePoint.NbSW * 8 + RelativePoint.NbN):      //  L L L
                case (RelativePoint.NbSW * 8 + RelativePoint.NbNW):     //  L o
                case (RelativePoint.NbSW * 8 + RelativePoint.NbW):      //  R
                case (RelativePoint.NbNE * 8 + RelativePoint.NbSW):
                case (RelativePoint.NbNE * 8 + RelativePoint.NbS):
                case (RelativePoint.NbNE * 8 + RelativePoint.NbSE):
                case (RelativePoint.NbNE * 8 + RelativePoint.NbE):
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbNW * 8 + RelativePoint.NbNE):     //
                case (RelativePoint.NbNW * 8 + RelativePoint.NbN):      //  R L L
                case (RelativePoint.NbNW * 8 + RelativePoint.NbNW):     //    o
                case (RelativePoint.NbSE * 8 + RelativePoint.NbSW):     //
                case (RelativePoint.NbSE * 8 + RelativePoint.NbS):
                case (RelativePoint.NbSE * 8 + RelativePoint.NbSE):
                    inside.InsertObjectPoint(p.y, p.x);
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbSW * 8 + RelativePoint.NbSW):     //    o
                case (RelativePoint.NbNE * 8 + RelativePoint.NbNE):     //  R
                    inside.InsertObjectPoint(p.y, p.x);
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbS * 8 + RelativePoint.NbSW):      //
                case (RelativePoint.NbS * 8 + RelativePoint.NbS):       //    o
                case (RelativePoint.NbN * 8 + RelativePoint.NbNE):      //  L R
                case (RelativePoint.NbN * 8 + RelativePoint.NbN):       //
                    inside.InsertObjectPoint(p.y, p.x);
                    inside.InsertObjectPoint(p.y, p.x);
                    break;

                case (RelativePoint.NbW * 8 + RelativePoint.NbSW):      //
                case (RelativePoint.NbW * 8 + RelativePoint.NbS):       //  R o
                case (RelativePoint.NbE * 8 + RelativePoint.NbNE):      //  L L
                case (RelativePoint.NbE * 8 + RelativePoint.NbN):       //
                case (RelativePoint.NbS * 8 + RelativePoint.NbSE):      //
                case (RelativePoint.NbS * 8 + RelativePoint.NbE):       //  o L
                case (RelativePoint.NbN * 8 + RelativePoint.NbNW):      //  R L
                case (RelativePoint.NbN * 8 + RelativePoint.NbW):       //
                    // do nothing; this is an impossible contour sequence
                    break;

                case (RelativePoint.NbNW * 8 + RelativePoint.NbW):      //  R
                case (RelativePoint.NbSE * 8 + RelativePoint.NbE):      //  L o
                                            //
                case (RelativePoint.NbSW * 8 + RelativePoint.NbS):      //    o
                case (RelativePoint.NbNE * 8 + RelativePoint.NbN):      //  R L
                    // do nothing; this is an impossible contour sequence
                    break;
            }

            // [end]

            // Apply the focus point's influence region to the resulting distance map.
            for (RelativePoint rp : surroundings.influenceRegions.get(p.nbL, p.nbR))
            {
                int i = p.x + rp.rx, j = p.y + rp.ry;
                if (rp.a - 256 < alpha[i][j])
                {
                    alpha[i][j] = rp.a - 256;
                }
            }

            // Enter the focus point's border points into the respective border scan lines.
            for (RelativePoint rp : surroundings.borderLimits.get(p.nbL, p.nbR))
            {
                border.InsertPair(p.y + rp.ry, p.x - rp.rx, p.x + rp.rx);
            }

            // Enter the focus point's contour points into the respective contour scan lines.
            for (RelativePoint rp : surroundings.contourLimits.get(p.nbL, p.nbR))
            {
                contour.InsertPair(p.y + rp.ry, p.x - rp.rx, p.x + rp.rx);
            }

            // Advance the focus to the next object pixel, set p.nbL.
            p = pl;
            pl = leftNeighbor(fuzziness, p, p.nbR + skipNeighbors);

        } while (p.x != p1.x || p.y != p1.y || p.nbL != p1.nbL);

        // [end]

        return true;
    }

    //--------------------- Methods for following an object contour.

    /**
     * Locates the left neighbor of a contour pixel that is also on the object contour.
     * @param fuzziness Maximum color distance that is considered equal to the background color.
     * @param current The current object contour {@link ScanPoint}.
     * @param nbR Direction of the current pixel's (closest) right neighbor.
     * @return The left neighbor.
     */
    private ScanPoint leftNeighbor(int fuzziness, ScanPoint current, int nbR)
    {
        ScanPoint result = new ScanPoint();
        for (current.nbL = (nbR + 1) % 8; ; current.nbL = (current.nbL + 1) % 8)
        {
            result.x = current.x + RelativePoint.NbDX[current.nbL]; result.y = current.y + RelativePoint.NbDY[current.nbL];
            if (current.nbL == current.nbR)
            {
                break; // We have completed the circle.
            }

            if (colorDistance(bitmap.getPixel(result.x, result.y)) > fuzziness)
            {
                break;
            }
        }

        result.nbR = (current.nbL + 4) % 8;
        return result;
    }

    /**
     * Locates the right neighbor of a contour pixel that is also on the object contour.
     * @param fuzziness Maximum color distance that is considered equal to the background color.
     * @param current The current object contour {@link ScanPoint}.
     * @param nbL Direction of the current pixel's (closest) left neighbor.
     * @return The right neighbor.
     */
    private ScanPoint rightNeighbor(int fuzziness, ScanPoint current, int nbL)
    {
        ScanPoint result = new ScanPoint();
        for (current.nbR = (nbL - 1 + 8) % 8; ; current.nbR = (current.nbR - 1 + 8) % 8)
        {
            result.x = current.x + RelativePoint.NbDX[current.nbR]; result.y = current.y + RelativePoint.NbDY[current.nbR];
            if (current.nbR == current.nbL)
            {
                break; // We have completed the circle.
            }

            if (colorDistance(bitmap.getPixel(result.x, result.y)) > fuzziness)
            {
                break;
            }
        }

        result.nbL = (current.nbR + 4) % 8;
        return result;
    }


}
