package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.Point2D;

/**
 * A simple {@link OutlineShape} in the form of a square standing on one of its corners.
 * 
 * @author Stephan.Wacker@web.de
 */
public class DiamondOutlineShape
extends GeometricOutlineShape
{
    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2D pc = params.makePoint2D(x, y);

        return (Math.abs(pc.x) + Math.abs(pc.y) <= params.sz);
    }

    //--------------------- Constructor

    /**
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public DiamondOutlineShape(Dimension size, OutlineShapeParameters params)
    {
        super(size, params);
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public DiamondOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        this(size, params);
    }

    //--------------------- Static methods for creating OutlineShapes

    /**
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     * @return A DiamondOutlineShape.
     */
    public static OutlineShape randomInstance(Random r, Dimension size, OutlineShapeParameters params)
    {
        return new DiamondOutlineShape(size, params);
    }
}
