package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.IPickable;
import swa.util.Point2D;

/**
 * Provides constructors for some {@link Function} objects
 * that generate a grid-like arrangement of shapes.
 * <p>
 * The {@link FunctionFactory} has a collection of these constructors.
 * <p>
 * TODO: Assign individual {@link IPickable#getRatio()} values to some instances.
 * 
 * @author Stephan.Wacker@web.de
 */
final
class GridFunctions
{
    /**
     * @return A function that generates parallel stripes. 
     */
    public static FunctionConstructor Stripes()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 2, 0.5, 1.0)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(30, DistortionConstructor.DistortNone()), // TODO: null
                                new DistortionSpec(10, DistortionConstructor.DistortX_CosY(), 0.2, 0.5, 0.5, 2.0),
                                new DistortionSpec(10, DistortionConstructor.DistortX_CosY_Alternating(), 0.15, 0.3, 0.5, 2.0),
                                new DistortionSpec(20, DistortionConstructor.DistortXY_StretchY_Rotate(), 1.0, 1.0, 1, 5, Math.PI / 12),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), 0.25, 0.5),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), -0.15, -0.3),
                        };
                    }

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        return Math.cos(0.5 * Math.PI * p.x);
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates quadratic squares. 
     */
    public static FunctionConstructor Squares()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 0.5, 1.0)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(20, null),
                                new DistortionSpec( 7, DistortionConstructor.DistortX_CosY(), 0.2, 0.5, 0.5, 2.0),
                                new DistortionSpec( 7, DistortionConstructor.DistortY_CosX(), 0.2, 0.5, 0.5, 2.0),
                                new DistortionSpec(15, DistortionConstructor.DistortXY_CosY_CosX(), 0.2, 0.5, 1, 4, 0.5),
                                new DistortionSpec(11, DistortionConstructor.DistortX_CosY_Alternating(), 0.15, 0.3, 1, 4, 0.5),
                                new DistortionSpec(10, DistortionConstructor.DistortXY_StretchY_Rotate(), 1.0, 1.0, 1, 3, Math.PI / 8),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), 0.25, 0.5),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), -0.15, -0.3),
                        };
                    }
                    
                    private Function stripes = Stripes().create(r, this);

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        Point2D q = new Point2D(p.y, p.x);
                        return stripes.evaluate(p) * stripes.evaluate(q);
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a sparse array of circles. 
     */
    public static FunctionConstructor SmallCirclesSparse()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4)
                {
                    // This function will not be distorted.
                    
                    private Function squares = Squares().create(r, this);

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        // The intersection of the Squares function with a plane at this z coordinate
                        // gives a good approximation of circles.
                        return squares.evaluate(p) - 0.6204;
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates an array of circles. 
     */
    public static FunctionConstructor SmallCircles()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 0.8, 1.2)
                {
                    // This function will not be distorted.
                    
                    private Function sparse = SmallCirclesSparse().create(r, this);

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        // Puts a small circle into every grid square, instead of every other square.
                        return Math.abs(sparse.evaluate(p));
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a sparse array of rounded squares;
     * the other squares are extended a bit and connected via their diagonals. 
     */
    public static FunctionConstructor RoundedSquaresSparse()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 0.8, 1.2)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(50, null),
                                new DistortionSpec(20, DistortionConstructor.DistortXY_CosY_CosX(), 0.1, 0.3),
                                new DistortionSpec(30, DistortionConstructor.DistortX_CosY_Alternating(), 0.1, 0.3),
                        };
                    }
                    
                    private Function squares = Squares().create(r, this);

                    @Override
                    protected double evaluate(Point2D p)
                    {
                        // The intersection of the Squares function with a plane at this z coordinate
                        // takes away a small part from the border of the squares.
                        return squares.evaluate(p) - 0.05;
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a closely packed grid of (rounded) tiles. 
     */
    public static FunctionConstructor RoundedSquares()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 0.8, 1.2)
                {
                    @Override
                    protected DistortionSpec[] getDistortionSpecs()
                    {
                        return new DistortionSpec[] {
                                new DistortionSpec(30, null),
                                new DistortionSpec(15, DistortionConstructor.DistortXY_CosY_CosX(), 0.2, 0.3),
                                new DistortionSpec(25, DistortionConstructor.DistortXY_CosY_CosX_Alternating(), 0.15, 0.25),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), 0.25, 0.5),
                                new DistortionSpec(15, DistortionConstructor.DistortR_Exp(), -0.15, -0.3),
                        };
                    }

                    private Function sparse = RoundedSquaresSparse().create(r, this);
                    
                    @Override
                    protected double evaluate(Point2D p)
                    {
                        // Puts a rounded square into every grid square, instead of every other square.
                        return Math.abs(sparse.evaluate(p) - t);
                    }
                    
                    /** An additional tuning parameter. */
                    private double t = 0;

                    /**
                     * Configures the {@link #t} parameter.
                     * <p>
                     * There should be a small gap (approx. one maze square wide) between the tiles.
                     * Some tiles may even touch.
                     * @param r1 A source of random numbers (not used).
                     */
                    @Override
                    public void initAdditionalParameters(Random r1)
                    {
                        // Find the minimum function value at integer coordinates, when t is 0.
                        t = 0;
                        Point2D p = new Point2D(0, 0);
                        
                        Point2D pMin = p.clone();
                        double zMin = this.evaluate(pMin);
                        
                        for (int i = 1; i <= 6; i++)
                        {
                            p.x = Math.round(i / this.scale) * this.scale;
                            double z = this.evaluate(p);
                            if (z < zMin)
                            {
                                zMin = z;
                                pMin.x = p.x;
                            }
                        }

                        // Get the smaller of the two function values at half a square width distance.
                        double delta = 0.5 * this.scale;
                        zMin = Math.min(this.evaluate(new Point2D(pMin.x - delta, 0)), this.evaluate(new Point2D(pMin.x + delta, 0)));

                        // t can be determined directly from zMin:
                        // If t > zMin, the function value will become negative.
                        t = zMin * 1.001;

                        // For a distorted coordinate system, we need a larger value to get connected gap lines.
                        if (this.distortion != null)
                        {
                            t *= 1.5;
                        }
                    }
                };
            }
        };
    }
}
