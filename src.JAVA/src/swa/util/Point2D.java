package swa.util;

/**
 * A simple data structure for a point in two dimensions.
 * 
 * @see Point2DPolar
 * @see java.awt.geom.Point2D
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class Point2D
implements Cloneable
{
    //--------------------- Member Variables and Properties

    /** X coordinate. */
    public double x;
    /** Y coordinate. */
    public double y;

    //--------------------- Constructors

    /**
     * Constructor.
     * @param x X coordinate.
     * @param y Y coordinate.
     */
    public Point2D(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    
    /**
     * Constructor.
     * @param pp A point in Polar coordinates.
     */
    public Point2D(Point2DPolar pp)
    {
        this.assignFrom(pp);
    }
    
    //--------------------- Cloning and converting methods

    @Override
    public Point2D clone()
    {
        return new Point2D(x, y);
    }
    
    /**
     * @return The point converted to polar coordinates.
     */
    public Point2DPolar asPolar()
    {
        return new Point2DPolar(this);
    }
    
    /**
     * Sets this points Cartesian coordinates to the equivalent of the given Polar coordinates.
     * @param pp A point in Polar coordinates.
     */
    public void assignFrom(Point2DPolar pp)
    {
        this.x = pp.r * java.lang.Math.cos(pp.phi);
        this.y = pp.r * java.lang.Math.sin(pp.phi);
    }
    
    //--------------------- Auxiliary methods

    @Override
    public String toString()
    {
        return String.format("(%.2f, %.2f)", x, y);
    }
}