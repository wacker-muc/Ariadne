package swa.ariadne.gui.images;

import java.util.Random;

/**
 * Comprises the public methods of an {@link ImageLoader}.
 *
 * @author Stephan.Wacker@web.de
 */
public interface IImageLoader
{
    /**
     * @param r A source of random numbers.
     * @return The next image generated by the {@link ImageLoader}.
     */
    ContourImage getNext(Random r);

    /**
     * Stops the image loading process.
     */
    void shutdown();

    /**
     * Configures how often the ImageLoader should return null instead of a valid image.
     * @param value A percentage between 0 and 100.
     */
    void setYieldNullPercentage(int value);
}
