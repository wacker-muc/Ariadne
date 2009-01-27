package swa.ariadne.outlines;

import java.awt.Dimension;

import swa.util.Point2D;

/**
 * A simple {@link OutlineShape} in the form of a square standing on one of its corners.
 *
 * @author Stephan.Wacker@web.de
 */
class DiamondOutlineShape
extends GeometricOutlineShape
{
    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2D pc = params.makePoint2D(x, y);

        return (Math.abs(pc.x) + Math.abs(pc.y) <= params.getSize());
    }

    //--------------------- Constructors

    /**
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public DiamondOutlineShape(Dimension size, OutlineShapeParameters params)
    {
        super(size, params);
    }
}
