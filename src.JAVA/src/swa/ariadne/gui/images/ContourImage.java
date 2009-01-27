package swa.ariadne.gui.images;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.Shape;
import java.awt.Transparency;
import java.awt.geom.Area;
import java.awt.image.BufferedImage;

import swa.ariadne.outlines.OutlineShape;
import swa.util.ImageUtil;

/**
 * Represents an image.
 * <p>
 * If the image shows an object on a uniformly colored background,
 * two different versions of the image are offered:
 * <ul><li>
 * The {@linkplain #getTemplateImage() template image} is the original, complete image.
 * </li><li>
 * The {@linkplain #getProcessedImage() processed image} only shows the
 * non-background image object(s) in front of a black background.
 * </li></ul>
 *
 * @author Stephan.Wacker@web.de
 */
public final
class ContourImage
{
    //--------------------- Member variables and Properties

    /** Fully qualified filename of the image. */
    private final String path;

    /** @return Fully qualified filename of the image. */
    public String getPath()
    {
        return path;
    }

    // [start] Buffered images

    /** The image that was given in the constructor. */
    private final BufferedImage template;

    /** The {@link ContourImageScanner} for analyzing the template. */
    private ContourImageScanner scanner;

    /** Bounding box of the image object(s) found in the {@link #template}. */
    private Rectangle bbox;

    /**
     * The mask that is applied to the {@link #template} image.
     * Black mask pixels represent the template's background.
     * On and immediately around the image object(s), the mask is fully transparent.
     */
    private BufferedImage mask;

    /** A Graphics object associated with the {@link #mask}. */
    private Graphics gMask;

    /** The processed image. */
    private BufferedImage image;

    // [end]

    // [start] A clipping shape.

    /** The clipping shape for drawing the {@linkplain #getProcessedImage() processed image}. */
    private Shape borderShape;

    /** @return The clipping shape for drawing the {@linkplain #getProcessedImage() processed image}. */
    public Shape getBorderShape()
    {
        if (borderShape == null)
        {
            return null; // infinite
        }
        else
        {
            return borderShape;
        }
    }

    // [end]

    //--------------------- Public methods and properties.

    /**
     * @return True if a background color has been identified
     * and the processed image actually has a contour.
     */
    public boolean hasContour()
    {
        return (this.template != this.image);
    }

    /** @return The {@link #template} image. */
    public Image getTemplateImage()
    {
        return template;
    }

    /**
     * @return The processed {@link #template} image.
     * Background areas at a certain distance from the image objects are painted black.
     * If no definitive background color was detected, the template is returned unmodified.
     */
    public Image getProcessedImage()
    {
        if (image == null)
        {
            processImage();
        }

        return image;
    }

    /**
     * @return Either the {@linkplain #getProcessedImage() processed}
     * or the {@linkplain #getTemplateImage() template} image,
     * depending on the setting of the {@link #setDisplayProcessedImage(boolean)} property.
     */
    public Image getDisplayedImage()
    {
        return (displayProcessedImage ? getProcessedImage() : getTemplateImage());
    }

    /**
     * Determines whether the template images are processed at all.
     * If false, the DisplayedImage property always returns the template image.
     */
    private static boolean displayProcessedImage = true;

    /** @return The {@link #displayProcessedImage} property. */
    /*
    public static boolean getDisplayProcessedImage()
    {
        return displayProcessedImage;
    }
    */

    /** Sets the {@link #displayProcessedImage} property.
     * @param value If false, the DisplayedImage property always returns the template image.
     */
    public static void setDisplayProcessedImage(boolean value)
    {
        displayProcessedImage = value;
    }

    /**
     * @param gridWidth The maze's grid width.
     * @param wallWidth The maze's wall width.
     * @param offset The location of the image in the maze's Graphics coordinate system.
     * @return An {@link OutlineShape} that defines the area occupied by the image objects' border.
     */
    public OutlineShape getCoveredShape(final int gridWidth, final int wallWidth, final Point offset)
    {
        if (!this.hasContour())
        {
            return null;
        }

        // [start] Define an InsideShapeDelegate.

        int shapeWidth = image.getWidth() / gridWidth + 2;
        int shapeHeight = image.getHeight() / gridWidth + 2;
        final ScanLines border = scanner.getBorder();

        OutlineShape shape = new OutlineShape(new Dimension(shapeWidth, shapeHeight))
        {
            @Override
            public boolean get(int x, int y)
            {
                // Image coordinates of the evaluated maze square.
                // Translations to be considered:
                // 1) given offset of the image within the grid
                // 2) bounding box that was used to crop the template image
                // 3) half the wall width on every side
                int x0 = x * gridWidth - offset.x + bbox.x - wallWidth / 2, x1 = x0 + gridWidth - 1 + wallWidth;
                int y0 = y * gridWidth - offset.y + bbox.y - wallWidth / 2, y1 = y0 + gridWidth - 1 + wallWidth;

                int coveredPixelsCount = 0;
                int coveredPixelsLimit = 3;

                for (int j = y0; j <= y1; j++)
                {
                    if (j < 0 || j >= border.size())
                    {
                        continue;
                    }

                    for (int p = 1, q = p + 1; q < border.size(j); p += 2, q += 2)
                    {
                        if (border.get(j, p) > x1)
                        {
                            // We have passed the evaluated square.
                            // Continue on the next scan line.
                            break;
                        }
                        if (x0 <= border.get(j, q))
                        {
                            // The current scan line area intersects with the evaluated square.
                            int n = 1 + Math.min(border.get(j, q), x1) - Math.max(border.get(j, p), x0);
                            if ((coveredPixelsCount += n) > coveredPixelsLimit)
                            {
                                return true;
                            }
                        }
                    }
                }

                // None (or very few) of the border scan lines intersected with the evaluated square.
                return false;
            }
        };

        // [end]

        // [start] Build a closed OutlineShape.

        OutlineShape result = shape.makeClosure();

        // [end]

        return result;
    }

    //--------------------- Constructors

    /**
     * @param template The original image.
     * @param path The fully qualified filename path of this image.
     */
    public ContourImage(Image template, String path)
    {
        int fuzziness = (int)(0.05 * ContourImageScanner.MaxColorDistance);

        this.path = path;
        this.template = ImageUtil.toBufferedImage(template);
        this.scanner = new ContourImageScanner(this.template);

        float share = (displayProcessedImage ? scanner.guessBackgroundColor(fuzziness) : 0.0F);

        if (share < 0.8F)
        {
            this.mask = null;
            this.scanner = null;
            this.image = this.template;
        }
        else
        {
            scanner.initSurroundings();
        }
    }

    /**
     * Calculates the {@link #getProcessedImage() ProcessedImage} property.
     * Background pixels at a certain distance from the image object are rendered black.
     * There will be a graduated transition from the object contour to the full background black.
     */
    public void processImage()
    {
        if (this.image != null)
        {
            return;
        }

        //Log.WriteLine("{ ProcessImage()");
        this.image = scanner.grow();

        int fuzziness = (int)(0.03 * ContourImageScanner.MaxColorDistance);
        createMask(fuzziness);
        applyMask();

        ScanLines border = scanner.getBorder();
        this.bbox = border.BoundingBox();
        this.image = crop(image, bbox);
        this.mask = crop(mask, bbox);

        // [start] Discard processed image if it fills the enclosing rectangle (almost) completely.

        int areaBbox = bbox.width * bbox.height;
        int areaBorder = border.ScanLinesArea(1);

        if (areaBorder > 0.95 * areaBbox)
        {
            this.image = this.template;
            this.mask = null;
            this.scanner = null;
        }

        // [end]

        if (hasContour())
        {
            this.borderShape = makeBorderShape(border);
        }

        //Log.WriteLine("} ProcessImage()");
    }

    //--------------------- Mask related methods.

    /**
     * Builds the bitmap mask and bounding box to be applied to the template image.
     * Areas dominated by the background color will be black.
     * Other areas will be transparent.
     * @param fuzziness Maximum color distance that is considered equal to the dominant color.
     */
    private void createMask(int fuzziness)
    {
        int contourDist = ContourSurroundings.ContourDistance;
        int blurDist = scanner.getSurroundings().getBlurDistance();

        // [start] Create the resulting mask.

        Color black = Color.BLACK;
        //black = Color.MAGENTA; // TODO: delete this line
        Color transparent = new Color(0, 0, 0, 0);

        int width = image.getWidth(), height = image.getHeight();
        this.mask = ImageUtil.createBufferedImage(width, height, Transparency.TRANSLUCENT);
        this.gMask = mask.getGraphics();
        gMask.setColor(transparent);
        gMask.fillRect(0, 0, width, height);

        // [end]

        // [start] Scan the image for non background objects.

        int[][] alpha = scanner.scan(fuzziness);

        ScanLines border = scanner.getBorder();
        ScanLines contour = scanner.getContour();

        // [end]

        // Eliminate enclosed regions from border and contour scan lines.
        border.EliminateInsideRegions(0, 1);
        contour.EliminateInsideRegions(0, 1);

        // Derive another set of scan lines from the border, closer to the object.
        ScanLines insetBorder = border.ShrinkRegion(blurDist + contourDist / 2);

        // The contour keeps the area around the object untouched.
        // The insetBorder avoids entering into areas enclosed by the border (and a blurred contour) only.
        contour.UniteScanLines(insetBorder);

        // Fill outside of objects, using the border.
        fillOutside(black, border);

        // Set the color of the mask pixels between border and contour to black with an appropriate transparency.
        paintGradient(black, border, contour, alpha);

        // Visualize the scan line contours.
        if (false) // used for debugging only
        {
            drawContour(contour, Color.RED);
            drawContour(insetBorder, Color.BLUE);
            drawContour(border, Color.CYAN);
        }
    }

    //--------------------- Image related methods.

    /**
     * Draws the {@link #mask} over the {@link #image}.
     */
    private void applyMask()
    {
        Graphics g = image.getGraphics();
        g.drawImage(mask, 0, 0, null);
    }

    /**
     * @param source A {@link BufferedImage}.
     * @param srcRect A {@link Rectangle} that defines the area which will be extracted.
     * @return The part of the source image defined by the given rectangle.
     */
    private static BufferedImage crop(BufferedImage source, Rectangle srcRect)
    {
        if (srcRect.width < source.getWidth() - 2 || srcRect.height < source.getHeight() - 2)
        {
            // Create a new Bitmap with the same resolution as the original image.
            BufferedImage result = ImageUtil.createBufferedImage(srcRect.width, srcRect.height,
                    (ImageUtil.hasAlpha(source) ? Transparency.TRANSLUCENT : Transparency.OPAQUE));
            Graphics g = result.getGraphics();

            g.drawImage(source, 0, 0, srcRect.width, srcRect.height, srcRect.x, srcRect.y, srcRect.x + srcRect.width, srcRect.y + srcRect.height, null);

            return result;
        }
        else
        {
            return source;
        }
    }

    /**
     * Calculates the {@link #borderShape} property.
     * @param border The outside contour of the image object(s) influence region.
     * @return A {@link Shape} that can be used as a clipping region
     * when drawing the {@linkplain #getProcessedImage() processed image}.
     */
    private Shape makeBorderShape(ScanLines border)
    {
        Area result = new Area(); // TODO: Polygon
        result.reset();

        int dx = bbox.x, dy = bbox.y;

        for (int y = 0; y < border.size(); y++)
        {
            for (int p = 1, q = p + 1; q < border.size(y); p += 2, q += 2)
            {
                int xp = border.get(y, p), xq = border.get(y, q);
                result.add(new Area(new Rectangle(xp - dx, y - dy, 1 + xq - xp, 1)));
            }
        }

        return result;
    }

    //--------------------- Painting methods.

    /**
     * Paints the area outside of the given contour in the given color.
     * @param backgroundColor The background color, usually black.
     * @param border The outside contour of the image object(s) influence region.
     */
    private void fillOutside(Color backgroundColor, ScanLines border)
    {
        gMask.setColor(backgroundColor);

        for (int y = 0; y < border.size(); y++)
        {
            for (int p = 0, q = p + 1; q < border.size(y); p += 2, q += 2)
            {
                int x0 = border.get(y, p) + 1, x1 = border.get(y, q) - 1;
                if (x0 == x1)
                {
                    // Single dots are not drawn properly. :-(
                    x1 += 1;
                }
                gMask.drawRect(x0, y, x1 - x0 + 1, 1);
            }
        }
    }

    /**
     * Paints the area in {@link #mask} between the image's given border and contour
     * with a transparent variant of the given background color.
     * @param backgroundColor The background color, usually black.
     * @param border The area around the image objects where the mask is not fully transparent.
     * @param contour The area around the image objects where the mask is fully opaque.
     * @param alpha The alpha map that defines the amount of transparency.
     */
    private void paintGradient(Color backgroundColor, ScanLines border, ScanLines contour, int[][] alpha)
    {
        // Prepare a translucent color for every alpha value.
        int R = backgroundColor.getRed(), G = backgroundColor.getGreen(), B = backgroundColor.getBlue();
        int[] aRGB = new int[256];
        for (int a = 0; a < 256; a++)
        {
            aRGB[a] = new Color(R, G, B, a).getRGB();
        }

        int width = mask.getWidth(), height = mask.getHeight();
        for (int y = 0; y < height; y++)
        {
            if (border.size(y) < 4) // not inside the border of any object
            {
                continue;
            }

            // Only process regions on the scan line that are
            //  * inside the border and
            //  * outside the contour.
            int b = 2, xb = border.get(y, b); // right end of the first border region
            int c = 1, xc = contour.get(y, c); // left end of the first contour region

            for (int x = border.get(y, 1); x < width; x++)
            {
                if (x >= xc) // At the left end of a contour region.
                {
                    // Skip to the right end of the contour region.
                    // Note: This is still inside the current border region.
                    x = contour.get(y, ++c);
                    xc = contour.get(y, ++c);
                    continue;
                }

                if (x > xb) // Beyond the right end of a border region.
                {
                    if (b + 2 >= border.size(y)) // This was the last border region.
                    {
                        break;
                    }

                    // Skip before the left end of the following inside border region.
                    // Note: This is still outside of the following contour region.
                    x = border.get(y, ++b) - 1;
                    xb = border.get(y, ++b);
                    continue;
                }

                // Paint the mask pixel with its given alpha value.
                int a = alpha[x][y] + 256;
                if (0 <= a && a < 255)
                {
                    int maskRGB = aRGB[a];
                    mask.setRGB(x, y, maskRGB);
                }
            }
        }
    }

    /**
     * Paints into the {@link #mask} the pixels marked by points on the given contour
     * in the given color.
     * @param contour The ScanLines defining a contour.
     * @param color The color used for painting the contour.
     */
    private void drawContour(ScanLines contour, Color color)
    {
        int rgb = color.getRGB();
        for (int y = 0; y < contour.size(); y++)
        {
            for (int p = 1; p < contour.size(y) - 1; p++)
            {
                int x = contour.get(y, p);
                mask.setRGB(x, y, rgb);
            }
        }
    }

    //--------------------- Auxiliary methods.

    @Override
    public String toString()
    {
        if (path != null)
        {
            return path;
        }
        else
        {
            return super.toString();
        }
    }
}
