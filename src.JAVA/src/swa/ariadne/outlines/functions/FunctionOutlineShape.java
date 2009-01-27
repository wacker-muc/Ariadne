package swa.ariadne.outlines.functions;

import java.awt.Dimension;
import java.util.Random;

import swa.ariadne.outlines.GeometricOutlineShape;
import swa.ariadne.outlines.OutlineShape;
import swa.ariadne.outlines.OutlineShapeParameters;
import swa.util.Point2D;

/**
 * An {@link OutlineShape} defined by a geometric function in two dimensions: <em>f(x, y)</em>.
 * <br>The dividing line between "inside" and "outside" is defined by the function's roots,
 * i.e. the contour is where the function <em>f(x, y)</em> becomes zero.
 *
 * @author Stephan.Wacker@web.de
 */
public
class FunctionOutlineShape
extends GeometricOutlineShape
{
    //--------------------- Member variables and Properties

    /** The function defining the outline shape. */
    private final Function function;

    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2D pc = params.makePoint2D(x, y);
        pc.x *= params.getSize();
        pc.y *= params.getSize();

        double z = function.evaluate(pc.x, pc.y);
        return (z > 0);
    }

    //--------------------- Constructor

    /**
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     * @param function A method with signature <em>double f(double x, double y)</em>.
     * @param symmetryRotation 0 if the function should be called with polar coordinates;
     * <br> 1..4 for a rotation by (r - 1) * 90 degrees
     */
    public FunctionOutlineShape(Dimension size, OutlineShapeParameters params, Function function, int symmetryRotation)
    {
        super(size, params);

        double n = 5; // number of units (in function coordinates) that span the shape's size

        // Adjust the shape parameters.
        // The coordinate system's center is moved to an integer value (in shape coordinates).
        // The size parameter will be used for scaling the function arguments.
        this.params = new OutlineShapeParameters(
                Math.round(this.params.getCenter().x),
                Math.round(this.params.getCenter().y),
                n / this.params.getSize()).convertToShapeCoordinates(null);

        this.function = function;
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.OutlineShapeFactory OutlineShapeFactory}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public FunctionOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        super(size, params);

        this.function = FunctionFactory.createInstance(r);
    }
}
