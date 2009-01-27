package swa.util;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.io.InputStream;
import java.util.Random;

import javax.imageio.ImageIO;

/**
 * Provides access to the image resource files in a local directory or JAR file.
 */
public class ImageResourceReader
extends ResourceReader
{
    //--------------------- Constructors

    /**
     * Constructor.
     * @param clientClass A class whose local directory or JAR file contains the resources.
     * @param inventoryResourceName Name of a text file resource that contains
     * a list of names of resources managed by this object.
     */
    public ImageResourceReader(Class<?> clientClass, String inventoryResourceName)
    {
        super(clientClass, inventoryResourceName);
    }

    //--------------------- Resource providing methods

    /**
     * @param r A source of random numbers.
     * @return One of the images available in this resource collection.
     */
    public BufferedImage pickOne(Random r)
    {
        int p = r.nextInt(count());
        String resourceName = resourceNames.get(p);
        System.out.println("BitmapResource.pickOne(): " + resourceName);
        BufferedImage result = null;

        try
        {
            InputStream stream = clientClass.getResourceAsStream(resourceName);
            result = ImageIO.read(stream);
        }
        catch (IOException e)
        {
            // do nothing
        }

        return result;
    }
}
