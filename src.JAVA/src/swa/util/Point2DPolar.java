package swa.util;

/**
 * A simple data structure for a point in two dimensions.
 * 
 * @see Point2D
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class Point2DPolar
implements Cloneable
{
    //--------------------- Member Variables and Properties

    /** Distance from the origin. */
    public double r;
    /** Angle measured from the X axis. */
    public double phi;

    //--------------------- Constructor

    /**
     * Constructor.
     * @param r Distance from the origin.
     * @param phi Angle measured from the X axis.
     */
    public Point2DPolar(double r, double phi)
    {
        this.r = r;
        this.phi = phi;
    }
    
    /**
     * Constructor.
     * @param pc A point in Cartesian coordinates.
     */
    public Point2DPolar(Point2D pc)
    {
        this.assignFrom(pc);
    }

    //--------------------- Cloning and converting methods

    @Override
    public Point2DPolar clone()
    {
        return new Point2DPolar(r, phi);
    }
    
    /**
     * @return The point converted to Cartesian coordinates.
     */
    public Point2D asCartesian()
    {
        return new Point2D(this);
    }
    
    /**
     * Sets this points Polar coordinates to the equivalent of the given Cartesian coordinates.
     * @param pc A point in Cartesian coordinates.
     */
    public void assignFrom(Point2D pc)
    {
        this.r = java.lang.Math.sqrt(pc.x * pc.x + pc.y * pc.y);
        this.phi = java.lang.Math.atan2(pc.y, pc.x);
    }
    
    //--------------------- Auxiliary methods

    @Override
    public String toString()
    {
        return String.format("(%.2f @ %.2f)", r, phi);
    }
}