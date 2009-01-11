package swa.util.tests;

import static org.junit.Assert.*;
import org.junit.Test;

import java.awt.Color;

import swa.util.ColorBuilder;

/**
 * This is a test class for {@link swa.util.ColorBuilder}.
 * 
 * @author Stephan.Wacker@web.de
 */
public class ColorBuilderTest
{
    //--------------------- Unit tests for ColorBuilder.ConvertHSBToColor

    /**
     * Test method for {@link ColorBuilder#convertHSBToColor(float, float, float)}.
     */
    @Test
    public final void CB_testConvertHSBToColor_01()
    {
        // Full saturation, full brightness.
        testConvertHSB(0.0F, 1.0F, 1.0F, 255, 0, 0);
    }

    /**
     * Test method for {@link ColorBuilder#convertHSBToColor(float, float, float)}.
     */
    @Test
    public final void CB_testConvertHSBToColor_02()
    {
        // No saturation, full brightness.
        testConvertHSB(0.0F, 0.0F, 1.0F, 255, 255, 255);
    }

    /**
     * Test method for {@link ColorBuilder#convertHSBToColor(float, float, float)}.
     */
    @Test
    public final void CB_testConvertHSBToColor_03()
    {
        // No saturation, no brightness.
        testConvertHSB(0.0F, 0.0F, 0.0F, 0, 0, 0);
    }

    /**
     * Test method for {@link ColorBuilder#convertHSBToColor(float, float, float)}.
     */
    @Test
    public final void CB_testConvertHSBToColor_04()
    {
        int r = 128, g = 128, b = 0;
        float[] hsb = Color.RGBtoHSB(r, g, b, null);
        testConvertHSB(hsb[0], hsb[1], hsb[2], r, g, b);
    }

    /**
     * Tests whether {@link ColorBuilder#convertHSBToColor(float, float, float)}
     * returns the expected value.
     * @param h The given hue value.
     * @param s The given saturation value.
     * @param b The given brightness value.
     * @param R The expected red value.
     * @param G The expected green value.
     * @param B The expected blue value.
     */
    private static void testConvertHSB(float h, float s, float b, int R, int G, int B)
    {
        Color actual = ColorBuilder.convertHSBToColor(h, s, b);
        Color expected = new Color(R, G, B);
        String message = String.format("Colors converting (%.2f, %.2f, %.2f) don't match", h, s, b);

        assertEquals(message, expected, actual);
    }

    //--------------------- Unit tests for ColorBuilder.HueDifference

    /**
     * Test method for {@link ColorBuilder#suggestColors(Color, Color)}.
     */
    //@Test
    public final void CB_testSuggestColors()
    {
        fail("Not yet implemented"); // TODO
    }

    //--------------------- Unit tests for ColorBuilder.HueDifference

    /**
     * Test method for {@link ColorBuilder#hueDifference(float, float)}.
     */
    @Test
    public final void CB_testHueDifference_01()
    {
        float h1 = 0.2F * ColorBuilder.MaxHue, h2 = 0.3F * ColorBuilder.MaxHue, expected = h2 - h1, actual;
        
        actual = ColorBuilder.hueDifference(h1, h2);
        assertEquals(expected, actual, 1e-4);
        
        actual = ColorBuilder.hueDifference(h2, h1);
        assertEquals(expected, actual, 1e-4);
    }

    /**
     * Test method for {@link ColorBuilder#hueDifference(float, float)}.
     */
    @Test
    public final void CB_testHueDifference_02()
    {
        float h1 = 0.2F * ColorBuilder.MaxHue, h2 = 0.6F * ColorBuilder.MaxHue, expected = h2 - h1, actual;
        
        actual = ColorBuilder.hueDifference(h1, h2);
        assertEquals(expected, actual, 1e-4);
        
        actual = ColorBuilder.hueDifference(h2, h1);
        assertEquals(expected, actual, 1e-4);
    }

    /**
     * Test method for {@link ColorBuilder#hueDifference(float, float)}.
     */
    @Test
    public final void CB_testHueDifference_03()
    {
        float h1 = 0.2F * ColorBuilder.MaxHue, h2 = 0.9F * ColorBuilder.MaxHue, expected = ColorBuilder.MaxHue - (h2 - h1), actual;
        
        actual = ColorBuilder.hueDifference(h1, h2);
        assertEquals(expected, actual, 1e-4);
        
        actual = ColorBuilder.hueDifference(h2, h1);
        assertEquals(expected, actual, 1e-4);
    }
}
