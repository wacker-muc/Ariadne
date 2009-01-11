package swa.ariadne.outlines;

import java.awt.Dimension;

/**
 * Base class for {@link OutlineShape OutlineShapes} that are formed of a single geometric shape.
 * E.g. circles, algebraic curves, polygons.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class GeometricOutlineShape
extends ContinuousOutlineShape
{
    //--------------------- Member Variables and Properties

    /** Characteristic parameters of the GeometricOutlineShape: location and size. */
    protected OutlineShapeParameters params;

    //--------------------- Constructor

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    protected GeometricOutlineShape(Dimension size, OutlineShapeParameters params)
    {
        super(size);

        this.params = params.convertToShapeCoordinates(size);
    }
}
