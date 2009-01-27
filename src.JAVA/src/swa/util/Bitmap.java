package swa.util;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.awt.image.ColorModel;

/**
 * An object that extracts the pixel data from a {@link BufferedImage}
 * and provides efficient access to single pixels.
 *
 * @author Stephan.Wacker@web.de
 */
public final
class Bitmap
{
    //--------------------- Member variables and Properties

    /** The width of the image from which the pixel data was extracted. */
    private final int width;

    /**
     * @return The width of the image from which the pixel data was extracted.
     */
    public int getWidth()
    {
        return this.width;
    }

    /** The height of the image from which the pixel data was extracted. */
    private final int height;

    /**
     * @return The height of the image from which the pixel data was extracted.
     */
    public int getHeight()
    {
        return this.height;
    }

    /** The pixel data that was extracted from the image given in the constructor. */
    private final int[] pixels;

    /** Data used in the {@link #getBrightness} method. */
    private final float tmpHsbValues[] = new float[3];

    //--------------------- Static class variables

    /** The {@linkplain ColorModel#getRGBdefault() default RGB color model}. */
    static ColorModel cm = ColorModel.getRGBdefault();

    //--------------------- Public image accessing methods

    /**
     * @param x The X coordinate.
     * @param y The Y coordinate.
     * @return The pixel at the given location in the default RGB color model.
     * @throws ArrayIndexOutOfBoundsException If the given coordinates are invalid.
     */
    public int getPixel(int x, int y)
    {
        return pixels[y * width + x];
    }

    /**
     * @param x The X coordinate.
     * @param y The Y coordinate.
     * @return The brightness of the pixel at the given coordinates, 0.0 = black, 1.0 = white.
     */
    public float getBrightness(int x, int y)
    {
        int px = getPixel(x, y);
        Color.RGBtoHSB(cm.getRed(px), cm.getGreen(px), cm.getBlue(px), tmpHsbValues);

        return tmpHsbValues[2];
    }

    //--------------------- Constructors

    /**
     * Constructor.
     * @param image A {@link BufferedImage}.
     * <br> After the pixel data has been extracted, we keep no reference to the image.
     */
    public Bitmap(BufferedImage image)
    {
        this.width = image.getWidth();
        this.height = image.getHeight();
        this.pixels = new int[width * height];
        image.getRGB(0, 0, width, height, pixels, 0, width);
    }
}
