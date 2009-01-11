package swa.ariadne.outlines;

import java.awt.Dimension;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * Characteristic parameters of a {@link GeometricOutlineShape}: location and size.
 * 
 * @author Stephan.Wacker@web.de
 */
public final class OutlineShapeParameters
{
    //--------------------- Member Variables and Properties
    
    /** Shape center, in shape coordinates. */
    Point2D center;
    /** Shape size, measured from the center, in shape coordinates. */
    double sz;

    /** When true, the coordinates still need to be converted to shape coordinates. */
    private boolean dimensionsAreRelative;
    
    //--------------------- Constructor
    
    /**
     * Constructor, using relative coordinates.
     * @param centerX X coordinate, relative to total nominal width; 0.0 = top, 1.0 = bottom.
     * @param centerY Y coordinate, relative to total nominal height; 0.0 = left, 1.0 = right.
     * @param relativeSize Size, relative to distance of center from the nominal border; 1.0 will touch the border.
     */
    public OutlineShapeParameters(double centerX, double centerY, double relativeSize)
    {
        this.center = new Point2D(centerX, centerY);
        this.sz = relativeSize;
        this.dimensionsAreRelative = true;
    }

    //--------------------- Setup methods
    
    /**
     * Converts the relative coordinates given in the constructor to (absolute) shape coordinates.
     * <p>If any argument is 0, no conversion is done; the parameters given to the constructor are assumed to be in shape coordinates already.
     * <br>The conversion is done only once, i.e. a second call will leave the parameters unchanged.
     * 
     * @param size The nominal size of the shape.
     * @return The (same) converted OutlineShapeParameters object.
     */
    public OutlineShapeParameters convertToShapeCoordinates(Dimension size)
    {
        if (dimensionsAreRelative && size.width > 0 && size.height > 0)
        {
            // Convert the center coordinates to the shape coordinate system.
            center.x *= size.width;
            center.y *= size.height;

            // Determine the radius of a circle that would touch the nearest border.
            // If the center is beyond one of the borders, that border is not considered.
            double r = Double.MAX_VALUE;
            if (center.x > 0) r = Math.min(sz, center.x);
            if (center.y > 0) r = Math.min(sz, center.y);
            if (center.x < size.width) r = Math.min(sz, size.width - center.x);
            if (center.y < size.height) r = Math.min(sz, size.height - center.y);

            // Convert the relative size to an absolute size.
            sz *= r;
        }
        
        // Fixate the converted dimensions. 
        dimensionsAreRelative = false;

        return this;
    }

    //--------------------- Support for geometric operations
    
    /**
     * @param x X coordinate, in shape coordinates.
     * @param y Y coordinate, in shape coordinates.
     * @return A point in geometry coordinates, measured from the center.
     * <br> Note: While the outline shape's Y coordinate points south,
     * the the geometry's Y coordinate points north. 
     */
    public Point2D makePoint2D(double x, double y)
    {
        // TODO: Maybe we should divide by size, as well...?!?
        return new Point2D(x - center.x, -1 * (y - center.y));
    }
    
    /**
     * @param x X coordinate, in shape coordinates.
     * @param y Y coordinate, in shape coordinates.
     * @return A point in geometry coordinates, measured from the center.
     * <br> Note: While the outline shape's Y coordinate points south,
     * the the geometry's Y coordinate points north. 
     */
    public Point2DPolar makePoint2DPolar(double x, double y)
    {
        return makePoint2D(x, y).asPolar();
    }
}