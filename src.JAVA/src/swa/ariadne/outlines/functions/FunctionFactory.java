package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.Picker;

/**
 * Creates {@link Function} objects.
 *
 * @author Stephan.Wacker@web.de
 */
public
class FunctionFactory
{
    //--------------------- Static variables

    /**
     *  A collection of all available {@linkplain FunctionConstructor Function constructors}.
     */
    private static FunctionConstructor[] constructors =
    {
        GeometricFunctions.Ellipsis(),
        GeometricFunctions.Hyperbola(),
        GeometricFunctions.ConcentricCircles(),
        GeometricFunctions.Spiral(),
        GridFunctions.Stripes(),
        GridFunctions.Squares(),
        GridFunctions.SmallCircles(),
        GridFunctions.RoundedSquares(),
        GridFunctions.SmallCirclesSparse(),
        GridFunctions.RoundedSquaresSparse(),
        FractalFunctions.Mandelbrot(),
        FractalFunctions.Julia(),
    };

    //--------------------- Static methods

    /**
     * @param r A source of random numbers.
     * @return Some fully configured {@link Function} object.
     */
    public static Function createInstance(Random r)
    {
        Function result = new Picker<FunctionConstructor>(constructors).pick(r).create(r);

        result.addDistortion(r);
        result.initAdditionalParameters(r);

        return result;
    }
}
