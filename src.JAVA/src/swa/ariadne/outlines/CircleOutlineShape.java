package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.Point2DPolar;

/**
 * A simple {@link OutlineShape} in the form of a circle.
 * 
 * @author Stephan.Wacker@web.de
 */
public class CircleOutlineShape
extends GeometricOutlineShape
{
    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2DPolar pp = params.makePoint2DPolar(x, y);

        return (pp.r <= params.sz);
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public CircleOutlineShape(Dimension size, OutlineShapeParameters params)
    {
        super(size, params);
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public CircleOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        this(size, params);
    }
    
    //---------------------  OutlineShape implementation

    /** 
     * @return The percentage of instances that should be distorted.
     * <br> The simple CircleOutlineShape should be distorted rather often.
     * @see OutlineShape#getDistortedPercentage(int)
     */
    @Override
    public int getDistortedPercentage(int defaultValue)
    {
        return Math.max(defaultValue, 80);
    }

    @Override
    public OutlineShape makeDistortedCopy(Random r)
    {
        return this.makeRadialWaveDistortedCopy(r);
    }

    /**
     * @param r A source of random numbers.
     * @return A {@link DistortedOutlineShape} based on the current shape
     * and a {@link DistortedOutlineShape#RadialWaveDistortion RadialWaveDistortion}.
     */
    private ContinuousOutlineShape makeRadialWaveDistortedCopy(Random r)
    {
        int n = 3 + r.nextInt(6);                   // number of corners
        double a = r.nextDouble();                  // rotation, 0..1
        double bMin = 0.2 + (n - 2) * 0.03;         // strongly indented sides
        double bMax = 0.85 + (n - 2) * 0.0166;      // almost flat sides
        double b = bMin + r.nextDouble() * (bMax - bMin);

        DistortedOutlineShape.Distortion distortion = DistortedOutlineShape.RadialWaveDistortion(params.center, n, a, b);
        return this.makeDistortedCopy(distortion);
    }
}
