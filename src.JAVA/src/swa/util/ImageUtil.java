package swa.util;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.GraphicsConfiguration;
import java.awt.GraphicsDevice;
import java.awt.GraphicsEnvironment;
import java.awt.HeadlessException;
import java.awt.Image;
import java.awt.RenderingHints;
import java.awt.Transparency;
import java.awt.image.BufferedImage;
import java.awt.image.ColorModel;
import java.awt.image.PixelGrabber;

import javax.swing.ImageIcon;

/**
 * Provides some Image manipulation methods.
 */
public final class ImageUtil
{
    /**
     * @param image The image that needs to be converted.
     * @return A buffered image with the contents of the given image.
     * @see <a href="http://www.exampledepot.com/egs/java.awt.image/Image2Buf.html">"http://www.exampledepot.com/egs/java.awt.image/Image2Buf.html"</a>
     */
    public static BufferedImage toBufferedImage(Image image)
    {
        if (image instanceof BufferedImage)
        {
            return (BufferedImage)image;
        }

        // This code ensures that all the pixels in the image are loaded
        @SuppressWarnings("unused")
        Image icon = new ImageIcon(image).getImage();

        // Determine if the image has transparent pixels; for this method's
        // implementation, see e661 Determining If an ImageUtil Has Transparent Pixels
        boolean hasAlpha = hasAlpha(image);

        // Create a buffered image with a format that's compatible with the screen
        BufferedImage result = createBufferedImage(image.getWidth(null), image.getHeight(null), (hasAlpha ? Transparency.BITMASK : Transparency.OPAQUE));

        if (result == null)
        {
            // Create a buffered image using the default color model
            int type = BufferedImage.TYPE_INT_RGB;
            if (hasAlpha)
            {
                type = BufferedImage.TYPE_INT_ARGB;
            }
            result = new BufferedImage(image.getWidth(null), image.getHeight(null), type);
        }

        // Copy image to buffered image
        Graphics g = result.createGraphics();

        // Paint the image onto the buffered image
        g.drawImage(image, 0, 0, null);
        g.dispose();

        return result;
    }

    /**
     * @param width Width of the created image.
     * @param height Height of the created image.
     * @param transparency The specified {@link Transparency} mode.
     * @return A {@link BufferedImage} of the given dimensions
     *         that is compatible with the default screen device.
     */
    public static BufferedImage createBufferedImage(int width, int height, int transparency)
    {
        BufferedImage result = null;

        try
        {
            GraphicsEnvironment ge = GraphicsEnvironment.getLocalGraphicsEnvironment();

            // Create the buffered image
            GraphicsDevice gs = ge.getDefaultScreenDevice();
            GraphicsConfiguration gc = gs.getDefaultConfiguration();
            result = gc.createCompatibleImage(width, height, transparency);
        }
        catch (HeadlessException e)
        {
            // The system does not have a screen
        }

        return result;
    }

    /**
     * @param image The image that is evaluated.
     * @return True if the specified image has transparent pixels.
     * @see <a href="http://www.exampledepot.com/egs/java.awt.image/HasAlpha.html">"http://www.exampledepot.com/egs/java.awt.image/HasAlpha.html"</a>
     */
    public static boolean hasAlpha(Image image)
    {
        // If buffered image, the color model is readily available
        if (image instanceof BufferedImage)
        {
            BufferedImage bimage = (BufferedImage)image;
            return bimage.getColorModel().hasAlpha();
        }

        // Use a pixel grabber to retrieve the image's color model;
        // grabbing a single pixel is usually sufficient
        PixelGrabber pg = new PixelGrabber(image, 0, 0, 1, 1, false);
        try
        {
            pg.grabPixels();
        }
        catch (InterruptedException e)
        {
            // do nothing
        }

        // Get the image's color model
        ColorModel cm = pg.getColorModel();

        return cm.hasAlpha();
    }

    /**
     * @param img An image.
     * @param newW Height of the resized image.
     * @param newH Width of the resized image.
     * @return A new image resized to the given width and height.
     * @see <a href="http://www.javalobby.org/articles/ultimate-image/#11">"http://www.javalobby.org/articles/ultimate-image/#11"</a>
     */
    public static BufferedImage resize(BufferedImage img, int newW, int newH)
    {
        int w = img.getWidth();
        int h = img.getHeight();
        BufferedImage result = new BufferedImage(newW, newH, img.getType());
        Graphics2D g = result.createGraphics();

        g.setRenderingHint(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BILINEAR);
        g.drawImage(img, 0, 0, newW, newH, 0, 0, w, h, null);
        g.dispose();

        return result;
    }
}
