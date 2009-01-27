package swa.ariadne.outlines.tests;

import static org.junit.Assert.assertNotNull;

import java.awt.image.BufferedImage;
import java.util.Random;

import org.junit.Test;

import swa.ariadne.outlines.bitmaps.BitmapOutlineShape;
import swa.util.ImageResourceReader;


/**
 * Test class for swa.ariadne.outlines.DistortedOutlineShape.
 *
 * @author Stephan.Wacker@web.de
 */
public class BitmapResourcesTest
{
    //--------------------- Unit tests for DistortedOutlineShape.SpiralDistortion

    /**
     * Test method for {@link ImageResourceReader#pickOne(java.util.Random)}.
     */
    @Test
    public final void BR_testPickOne_01()
    {
        ImageResourceReader target = new ImageResourceReader(BitmapOutlineShape.class, "resources.txt");
        Random r = new Random();

        for(int i = 0; i < 20; i++)
        {
            BufferedImage img = target.pickOne(r);
            assertNotNull(target.toString() + ".pickOne()", img);
        }
    }
}
