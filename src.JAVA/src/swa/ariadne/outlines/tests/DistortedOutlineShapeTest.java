package swa.ariadne.outlines.tests;

import static org.junit.Assert.*;
import org.junit.Test;

import swa.ariadne.outlines.DistortedOutlineShape;
import swa.ariadne.outlines.DistortedOutlineShape.Distortion;
import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * This is a test class for swa.ariadne.outlines.DistortedOutlineShape.
 * 
 * @author Stephan.Wacker@web.de
 */
public class DistortedOutlineShapeTest
{
    //--------------------- Constants

    /** sqrt(2) */
    private static final double s2 = Math.sqrt(2.0);
    
    //--------------------- Unit tests for DistortedOutlineShape.SpiralDistortion

    /**
     * Test method for {@link DistortedOutlineShape#SpiralDistortion(Point2D, double, double)}.
     */
    @Test
    public final void DOS_testSpiralDistortion_01()
    {
        Point2D center = new Point2D(0, 0);
        double size = 1.0;
        double winding = 0.25;
        Distortion target = DistortedOutlineShape.SpiralDistortion(center, size, winding);

        assertDistortionFixpoint(target, center);
        assertDistortion(target, new Point2D(1.0, 0.0), new Point2D(0.0, 1.0));
        assertDistortion(target, new Point2D(s2/2, s2/2), new Point2D(-s2/2, s2/2));
        assertDistortion(target, new Point2D(2.0, 0.0), new Point2D(-2.0, 0.0));
    }

    /**
     * Test method for {@link DistortedOutlineShape#SpiralDistortion(Point2D, double, double)}.
     */
    @Test
    public final void DOS_testSpiralDistortion_02()
    {
        Point2D center = new Point2D(0, 0);
        double size = 1.0;
        double winding = 0.125;
        Distortion target = DistortedOutlineShape.SpiralDistortion(center, size, winding);

        assertDistortionFixpoint(target, center);
        assertDistortion(target, new Point2D(1.0, 0.0), new Point2D(s2/2, s2/2));
        assertDistortion(target, new Point2D(s2/2, s2/2), new Point2D(0.0, 1.0));
        assertDistortion(target, new Point2D(2.0, 0.0), new Point2D(0.0, 2.0));
    }

    /**
     * Test method for {@link DistortedOutlineShape#SpiralDistortion(Point2D, double, double)}.
     */
    @Test
    public final void DOS_testSpiralDistortion_03()
    {
        Point2D center = new Point2D(0, 0);
        double size = 2.0;
        double winding = 0.25;
        Distortion target = DistortedOutlineShape.SpiralDistortion(center, size, winding);

        assertDistortionFixpoint(target, center);
        assertDistortion(target, new Point2D(2.0, 0.0), new Point2D(0.0, 2.0));
        assertDistortion(target, new Point2D(s2, s2), new Point2D(-s2, s2));
        assertDistortion(target, new Point2D(1.0, 0.0), new Point2D(s2/2, s2/2));
    }

    /**
     * Test method for {@link DistortedOutlineShape#SpiralDistortion(Point2D, double, double)}.
     */
    @Test
    public final void DOS_testSpiralDistortion_04()
    {
        Point2D center = new Point2D(1, 1);
        double size = 1.0;
        double winding = 0.25;
        Distortion target = DistortedOutlineShape.SpiralDistortion(center, size, winding);

        assertDistortionFixpoint(target, center);
        assertDistortion(target, new Point2D(2.0, 1.0), new Point2D(1.0, 2.0));
        assertDistortion(target, new Point2D(3.0, 1.0), new Point2D(-1.0, 1.0));
    }

    //--------------------- Unit tests for DistortedOutlineShape.RadialWaveDistortion

    /**
     * Test method for {@link DistortedOutlineShape#RadialWaveDistortion(Point2D, int, double, double)}.
     */
    @Test
    public void DOS_testRadialWaveDistortion_01()
    {
        int n = 4;
        double s = 0;
        double m = 0.5;
        testRadialWaveDistortion(n, s, m);
    }

    /**
     * Test method for {@link DistortedOutlineShape#RadialWaveDistortion(Point2D, int, double, double)}.
     */
    @Test
    public void DOS_testRadialWaveDistortion_02()
    {
        int n = 4;
        double s = 0.25;
        double m = 0.5;
        testRadialWaveDistortion(n, s, m);
    }

    /**
     * Test method for {@link DistortedOutlineShape#RadialWaveDistortion(Point2D, int, double, double)}.
     */
    @Test
    public void DOS_testRadialWaveDistortion_03()
    {
        int n = 3;
        double s = 0;
        double m = 0.5;
        testRadialWaveDistortion(n, s, m);
    }

    /**
     * Test method for {@link DistortedOutlineShape#RadialWaveDistortion(Point2D, int, double, double)}.
     */
    @Test
    public void DOS_testRadialWaveDistortion_04()
    {
        int n = 3;
        double s = 0.25;
        double m = 0.5;
        testRadialWaveDistortion(n, s, m);
    }

    /**
     * Test method for {@link DistortedOutlineShape#RadialWaveDistortion(Point2D, int, double, double)}.
     */
    @Test
    public void DOS_testRadialWaveDistortion_05()
    {
        Point2D center = new Point2D(0, 0);
        double m = 0.5;
        
        for (int n = 3; n <= 12; n++)
        {
            double s = (n % 2 == 1 ? 0.25 : (n % 4 == 2 ? 0.00 : 0.5 / n));

            Distortion target = DistortedOutlineShape.RadialWaveDistortion(center, n, s, m);

            assertDistortion(target, new Point2D(0.0, -1.0), new Point2D(0.0, -2.0));
            assertDistortionFixpoint(target, new Point2DPolar(1.0, 2.0 * Math.PI * (0.75 + 0.5 / n)));
        }
    }

    /**
     * Tests the characteristic properties of a Distortion with the given parameters.
     * @param n Number of waves.
     * @param s Shift of the waves.
     * @param m Minimum shift ratio.
     */
    private static void testRadialWaveDistortion(int n, double s, double m)
    {
        Point2D center = new Point2D(0, 0);
        Distortion target = DistortedOutlineShape.RadialWaveDistortion(center, n, s, m);

        Point2DPolar pp0, pp1;

        assertDistortionFixpoint(target, center);
        assertDistortionFixpoint(target, new Point2DPolar(1.0, 2.0 * Math.PI * (s + 0.0 / n)));
        assertDistortionFixpoint(target, new Point2DPolar(1.0, 2.0 * Math.PI * (s + 1.0 / n)));

        pp0 = new Point2DPolar(1.0, 2.0 * Math.PI * (s + 0.5 / n));
        pp1 = pp0.clone();
        pp1.r /= m;
        assertDistortion(target, pp0, pp1);

        pp0 = new Point2DPolar(1.0, 2.0 * Math.PI * (s - 0.5 / n));
        pp1 = pp0.clone();
        pp1.r /= m;
        assertDistortion(target, pp0, pp1);
    }
    
    //--------------------- Auxiliary methods

    /**
     * Assert that the given operand is not modified by the distortion.
     * @param target The analyzed distortion.
     * @param operand The point that will be transformed.
     */
    private static void assertDistortionFixpoint(Distortion target, Point2D operand)
    {
        assertDistortion(target, operand, operand);
    }

    /**
     * Assert that the given operand is mapped to the expected result.
     * @param target The analyzed distortion.
     * @param operand The point that will be transformed.
     * @param expected The expected transformed point.
     */
    private static void assertDistortion(Distortion target, Point2D operand, Point2D expected)
    {
        double delta = 1.0e-6;
        Point2D actual = target.transform(operand);
        assertEquals(target + "(" + operand + ")" + ": X coordinate mismatch", expected.x, actual.x, delta);
        assertEquals(target + "(" + operand + ")" + ": Y coordinate mismatch", expected.y, actual.y, delta);
    }

    /**
     * Assert that the given operand is not modified by the distortion.
     * @param target The analyzed distortion.
     * @param operand The point that will be transformed.
     */
    private static void assertDistortionFixpoint(Distortion target, Point2DPolar operand)
    {
        assertDistortionFixpoint(target, new Point2D(operand));
    }

    /**
     * Assert that the given operand is mapped to the expected result.
     * @param target The analyzed distortion.
     * @param operand The point that will be transformed.
     * @param expected The expected transformed point.
     */
    private static void assertDistortion(Distortion target, Point2DPolar operand, Point2DPolar expected)
    {
        assertDistortion(target, new Point2D(operand), new Point2D(expected));
    }
}
