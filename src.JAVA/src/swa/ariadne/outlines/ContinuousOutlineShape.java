package swa.ariadne.outlines;

import java.awt.Dimension;

/**
 * A ContinuousOutlineShape can evaluate double precision coordinates.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class ContinuousOutlineShape
extends OutlineShape
{
    //--------------------- IOutlineShape implementation

    /**
     * Function that discriminates the shape's inside from its outside.
     * <p> Note: The arguments are not restricted to the nominal width and height of the shape.
     * @param x Point's X coordinate. 
     * @param y Point's Y coordinate.
     * @return True if the given point is inside the shape.
     * @see ContinuousOutlineShape#get(int, int)
     */
    public abstract boolean get(double x, double y);

    @Override
    public boolean get(int x, int y)
    {
        return this.get((double)x, (double)y);
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     */
    protected ContinuousOutlineShape(Dimension size)
    {
        super(size);
    }

    //--------------------- Methods for applying a Distortion to the original shape

    /**
     * @param distortion A Distortion that is applied to the current shape. 
     * @return An OutlineShape based on the current shape and the given distortion.
     */
    ContinuousOutlineShape makeDistortedCopy(DistortedOutlineShape.Distortion distortion)
    {
        return new DistortedOutlineShape(new Dimension(XSize(), YSize()), this, distortion);
    }
}
