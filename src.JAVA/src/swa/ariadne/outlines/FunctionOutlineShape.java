package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.ariadne.outlines.functions.Function;
import swa.ariadne.outlines.functions.FunctionFactory;
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
    private Function function;

    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2D pc = params.makePoint2D(x, y);
        pc.x *= params.sz;
        pc.y *= params.sz;

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

        // The coordinate system's center is adjusted to an integer value (in shape coordinates).
        this.params.center.x = Math.round(this.params.center.x);
        this.params.center.y = Math.round(this.params.center.y);

        // The size parameter will be used for scaling the function arguments.
        double n = 5; // number of units (in function coordinates) that span the shape's size
        this.params.sz = n / this.params.sz;
        
        this.function = function;
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
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
