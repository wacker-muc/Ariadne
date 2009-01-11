package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.IPickable;
import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * Provides constructors for some {@link Function} objects
 * that generate typical fractal shapes like the Mandelbrot set.
 * <p>
 * The {@link FunctionFactory} has a collection of these constructors.
 * <p>
 * TODO: Assign individual {@link IPickable#getRatio()} values to some instances.
 * 
 * @author Stephan.Wacker@web.de
 */
final
class FractalFunctions
{
    /**
     * @return A function that generates the Mandelbrot set. 
     */
    public static FunctionConstructor Mandelbrot()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 5.0, 7.0)
                {
                    /**
                     * @return
                     * <ul><li> If the given point is inside the Mandelbrot set: +1.0.
                     * </li><li> Otherwise: The (negative) number of iterations
                     *           until the point was confirmed as not belonging to the Mandelbrot set.
                     *           The higher the (absolute) value, the closer the point is to the border.
                     * </li></ul>
                     */
                    @Override
                    protected double evaluate(Point2D c)
                    {
                        double zr = 0.0, zi = 0.0;

                        int n = 200;
                        for (int k = 1; k <= n; k++)
                        {
                            // In complex coordinates: x is mapped to x^2 - c.

                            double xr2 = zr * zr - zi * zi;
                            double xi2 = 2 * zr * zi;

                            // Apply the given point's parameters.  c.x is shifted to bring the resulting figure into the shape center.
                            zr = xr2 + (c.x - 0.5);
                            zi = xi2 + c.y;

                            if (zr * zr + zi * zi > 2.0)
                            {
                                return -k;
                            }
                        }

                        return +1;
                    }
                };
            }
        };
    }

    /**
     * @return A function that generates a Julia set. 
     */
    public static FunctionConstructor Julia()
    {
        return new FunctionConstructor()
        {
            @Override
            protected Function create(final Random r, Function clientFunction)
            {
                return new Function(r, clientFunction, 4, 4.0, 5.0)
                {
                    /**
                     * @return
                     * <ul><li> If the given point is inside the Julia set: +1.0.
                     * </li><li> Otherwise: The (negative) number of iterations
                     *           until the point was confirmed as not belonging to the Julia set.
                     *           The higher the (absolute) value, the closer the point is to the border.
                     * </li></ul>
                     */
                    @Override
                    protected double evaluate(Point2D p)
                    {
                        double zr = p.x, zi = p.y;
                        
                        // The escape limit is rather low to get a thicker shape.
                        // Thus, we avoid that the shape breaks up into several not connected parts.
                        int n = 20;
                        
                        for (int k = 1; k <= n; k++)
                        {
                            // In complex coordinates: x is mapped to x^2 - c.

                            double zr2 = zr * zr - zi * zi;
                            double zi2 = 2 * zr * zi;

                            // Apply the Mandelbrot coordinate parameters.
                            zr = zr2 + c.x;
                            zi = zi2 + c.y;

                            if (zr * zr + zi * zi > 2.0)
                            {
                                return -k;
                            }
                        }

                        return +1;
                    }
                    
                    /**
                     * The Julia set parameter.
                     */
                    private Point2D c = new Point2D(0, 0);

                    /**
                     * Configures the {@link #c} parameter
                     * which will be a point close to the border of the Mandelbrot set.
                     * @param r1 A source of random numbers (not really used).
                     */
                    @Override
                    public void initAdditionalParameters(Random r1)
                    {
                        Function mandelbrot = Mandelbrot().create(r1, this);
                        
                        // [start] Find a point close to the Mandelbrot border.

                        // Start with two random points, one inside and one outside of the Mandelbrot set.
                        Point2D c0 = new Point2DPolar(0.099, 2.0 * Math.PI * r1.nextDouble()).asCartesian(); // inside
                        Point2D c1 = new Point2DPolar(2.001, 2.0 * Math.PI * r1.nextDouble()).asCartesian(); // outside
                        
                        // see the coordinate translation in Mandelbrot()
                        c0.x += 0.5;
                        c1.x += 0.5;

                        // Bisect the interval [c0..c1] until the distance is less than 10^-6.
                        for (int k = 0; k < 20; k++)
                        {
                            // Determine whether the interval's midpoint is inside or outside.
                            Point2D c2 = new Point2D(0.5 * (c0.x + c1.x), 0.5 * (c0.y + c1.y));
                            double m0 = mandelbrot.evaluate(c0); // TODO: remove
                            double m1 = mandelbrot.evaluate(c1); // TODO: remove
                            double m2 = mandelbrot.evaluate(c2.x, c2.y);

                            if (m2 > 0) // inside
                            {
                                c0 = c2;
                            }
                            else
                            {
                                // Stop when we are close enough to the border.
                                if (m2 < -20 && k > 6)
                                {
                                    break;
                                }

                                c1 = c2;
                            }
                        }

                        // [end]

                        // Set the Julia set parameter to the selected (inside) Mandelbrot point coordinates.
                        c.x = c0.x - 0.5; // see the coordinate translation in Mandelbrot()
                        c.y = c0.y;
                    }
                };
            }
        };
    }
}
