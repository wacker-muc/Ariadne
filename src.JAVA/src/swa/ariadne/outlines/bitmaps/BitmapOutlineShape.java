package swa.ariadne.outlines.bitmaps;

import java.awt.Dimension;
import java.awt.Point;
import java.awt.image.BufferedImage;
import java.util.Random;

import swa.ariadne.outlines.OutlineShape;
import swa.ariadne.outlines.OutlineShapeParameters;
import swa.util.Bitmap;
import swa.util.ImageResourceReader;
import swa.util.ImageUtil;

/**
 * An {@link OutlineShape} defined by the contour of a black and white bitmap image.
 */
public
class BitmapOutlineShape
extends OutlineShape
{
    //--------------------- Member variables and Properties

    /** A {@link Bitmap} with an underlying black-and-white image. */
    Bitmap bitmap;

    /** The location within this shape where the {@link #bitmap} is placed. */
    private final Point offset = new Point();

    /** False for a black-on-white image, true for a white-on-black image. */
    private final boolean hasBlackBackground;

    //--------------------- OutlineShape implementation

    @Override
    public boolean get(int x, int y)
    {
        int xi = x - offset.x, yi = y - offset.x;

        if (xi < 0 || yi < 0 || xi >= bitmap.getWidth() || yi >= bitmap.getHeight())
        {
            return hasBlackBackground;
        }
        else
        {
            return (bitmap.getBrightness(xi, yi) <= 0.5);
        }
    }

    //--------------------- Constructors

    /**
     * Constructor used by the {@link swa.ariadne.outlines.OutlineShapeFactory OutlineShapeFactory}.
     * Chooses one of the {@linkplain #chooseBitmap(Random) available bitmaps}}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public BitmapOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        this(chooseBitmap(r), size, params);
    }

    /**
     * @param img A black and white image.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    private BitmapOutlineShape(BufferedImage img, Dimension size, OutlineShapeParameters params)
    {
        super(size);
        params.convertToShapeCoordinates(size);
        double scale = 2 * params.getSize() / Math.max(img.getWidth(), img.getHeight());

        BufferedImage scaledImg = ImageUtil.resize(ImageUtil.toBufferedImage(img), (int)(img.getWidth() * scale), (int)(img.getHeight() * scale));
        this.bitmap = new Bitmap(scaledImg);
        this.offset.x = (int)(params.getCenter().x - this.bitmap.getWidth() / 2.0);
        this.offset.y = (int)(params.getCenter().y - this.bitmap.getHeight() / 2.0);

        // [start] Determine the background color: black or white.

        int w = this.bitmap.getWidth() - 1, h = this.bitmap.getHeight() - 1;
        float cornerBrightness = 0;
        cornerBrightness += this.bitmap.getBrightness(0, 0);
        cornerBrightness += this.bitmap.getBrightness(w, 0);
        cornerBrightness += this.bitmap.getBrightness(0, h);
        cornerBrightness += this.bitmap.getBrightness(w, h);

        this.hasBlackBackground = (cornerBrightness < 0.5F * 4);

        // [end]
    }

    //--------------------- Static methods for creating a bitmap image

    /** The {@link ImageResourceReader} for this package. */
    private static final ImageResourceReader resourceReader = new ImageResourceReader(BitmapOutlineShape.class, "resources.txt");

    /**
     * @param r A source of random numbers.
     * @return One of the available bitmaps.
     */
    private static BufferedImage chooseBitmap(Random r)
    {
        return resourceReader.pickOne(r);
    }
}
