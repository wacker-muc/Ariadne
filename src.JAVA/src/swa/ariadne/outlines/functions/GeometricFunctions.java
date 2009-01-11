package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.IPickable;
import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * Provides constructors for some {@link Function} objects
 * that generate typical geometric shapes like circles, ellipses, hyperbolas, etc.
 * <p>
 * The {@link FunctionFactory} has a collection of these constructors.
 * <p>
 * TODO: Assign individual {@link IPickable#getRatio()} values to some instances.
 * 
 * @author Stephan.Wacker@web.de
 */
final
class GeometricFunctions
{
    /**
     * @return A function that generates a circle or ellipsis. 
     */
    public static FunctionConstructor Ellipsis()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 1.5, 2.0)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(100, DistortionConstructor.DistortXY_StretchY_Rotate(), 1.6, 3.0, 0.0, Math.PI),
                        };
                    }

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        return (1.0 - (p.x * p.x + p.y * p.y));
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a hyperbola. 
     */
    public static FunctionConstructor Hyperbola()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 1.5, 2.0)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(40, null),
                                new DistortionSpec(40, DistortionConstructor.DistortY_Stretch(), 0.5, 1.5),
                                new DistortionSpec(20, DistortionConstructor.DistortXY_StretchY_Rotate(), 0.8, 1.2, Math.PI * 1 / 12, Math.PI * 5 / 12),
                        };
                    }

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        return (1.0 - (p.x * p.x + p.y * p.y));
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates concentric circles. 
     */
    public static FunctionConstructor ConcentricCircles()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 0, 0.5, 2.0)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(40, null),
                                new DistortionSpec(30, DistortionConstructor.DistortR_Exp(), -0.25, -0.15),
                                new DistortionSpec(30, DistortionConstructor.DistortR_Exp(), +0.2, +0.5),
                        };
                    }

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        Point2DPolar pp = p.asPolar();
                        return Math.cos(0.5 * Math.PI * pp.r);
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a spiral. 
     */
    public static FunctionConstructor Spiral()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 0, 0.4, 1.2)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(40, null),
                                new DistortionSpec(30, DistortionConstructor.DistortR_Exp(), -0.4, -0.2),
                                new DistortionSpec(30, DistortionConstructor.DistortR_Exp(), +0.2, +0.5),
                        };
                    }

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        Point2DPolar pp = p.asPolar();
                        return Math.cos(0.5 * Math.PI * pp.r + pp.phi);
                    }
                };
            }
        };
    }
}
