package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

/**
 * A shape that is formed by covering the plane with a set of overlapping circles.
 * The resulting areas make up the "inside" and "outside" of the shape.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class CirclesOutlineShape
extends GeometricOutlineShape
{
    //--------------------- Member Variables and Properties
    
    /** A set of circles. */
    private CircleOutlineShape[] circles;

    //--------------------- IOutlineShape implementation

    /*
     * Returns true if the given point is covered by an odd number of half planes.
     */
    @Override
    public boolean get(double x, double y)
    {
        int n = 0;
        for (int i = 0; i < circles.length; i++)
        {
            if (circles[i].get(x, y))
            {
                n++;
            }
        }

        return (n % 2 == 1);
    }

    //--------------------- Constructors

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public CirclesOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        super(size, params);

        int n = 3 + r.nextInt(4);
        circles = new CircleOutlineShape[n];

        double xccMin = -0.33 * params.sz, xccMax = XSize() + 0.33 * params.sz;
        double yccMin = -0.33 * params.sz, yccMax = YSize() + 0.33 * params.sz;
        double szcRange = 0.5;
        
        for (int i = 0; i < n; i++)
        {
            // Choose the circle parameters: center and radius.
            double xcc = xccMin + r.nextDouble() * (xccMax - xccMin);
            double ycc = yccMin + r.nextDouble() * (yccMax - yccMin);
            double szc = params.sz * ((1.0 - szcRange) + (2.0 * szcRange));

            // If the center is too far outside of the border, increase the radius.
            double borderDist = Math.min(Math.min(xcc, XSize() - xcc), Math.min(ycc, YSize() - ycc));
            if (borderDist + szc < 0.5 * szc)
            {
                szc = params.sz * (1.0 + 1.0 * szcRange);
            }
            
            circles[i] = new CircleOutlineShape(size, new OutlineShapeParameters(xcc, ycc, szc));
        }
    }
}
