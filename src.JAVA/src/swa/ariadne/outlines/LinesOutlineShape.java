package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.Point2DPolar;

/**
 * A shape that is formed by dividing the plane with a set of straight lines.
 * The resulting polygonal areas make up the "inside" and "outside" of the shape.
 * 
 * @author Stephan.Wacker@web.de
 */
class LinesOutlineShape
extends GeometricOutlineShape
{
    //--------------------- Types
    
    /**
     * A shape that divides the plane along a single straight line.
     * One half of the plan is "inside", the other "outside" of the shape.
     */
    private final class LineOutlineShape extends GeometricOutlineShape
    {
        //--------------------- Member Variables and Properties
        
        /** Angle of the normal vector. */
        private double normalPhi;

        @Override
        public boolean get(double x, double y)
        {
            Point2DPolar pp = params.makePoint2DPolar(x, y);

            double dPhi = pp.phi - normalPhi + 3.0 * Math.PI;
            dPhi -= Math.floor(dPhi / (2.0 * Math.PI)) * 2.0 * Math.PI;

            return (dPhi <= Math.PI);
        }

        //--------------------- Constructor

        /**
         * Constructor.
         * @param size Nominal size of the shape.
         * @param params Characteristic parameters of the shape: location and size.
         * @param normalPhi Angle of the normal vector.
         */
        public LineOutlineShape(Dimension size, OutlineShapeParameters params, double normalPhi)
        {
            super(size, params);
            
            // Normalize the given angle.
            Point2DPolar pp = new Point2DPolar(1.0, normalPhi).asCartesian().asPolar();
            
            this.normalPhi = pp.phi;
        }

    }

    //--------------------- Member Variables and Properties
    
    /** A set of half planes. */
    private LineOutlineShape[] lines;

    //--------------------- IOutlineShape implementation

    /*
     * Returns true if the given point is covered by an odd number of half planes.
     */
    @Override
    public boolean get(double x, double y)
    {
        int n = 0;
        for (int i = 0; i < lines.length; i++)
        {
            if (lines[i].get(x, y))
            {
                n++;
            }
        }

        return (n % 2 == 1);
    }

    //--------------------- Constructor

    /**
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     */
    public LinesOutlineShape(Random r, Dimension size)
    {
        super(size, new OutlineShapeParameters(0, 0, 1));

        int n = 4 + r.nextInt(3);
        lines = new LineOutlineShape[n];

        double xccMin = 0.15 * params.sz, xccMax = XSize() - 0.15 * params.sz;
        double yccMin = 0.15 * params.sz, yccMax = YSize() - 0.15 * params.sz;
        
        for (int i = 0; i < n; i++)
        {
            // Choose the line parameters: center and slant.
            double xcc = xccMin + r.nextDouble() * (xccMax - xccMin);
            double ycc = yccMin + r.nextDouble() * (yccMax - yccMin);
            double slant = r.nextDouble() * Math.PI;

            lines[i] = new LineOutlineShape(size, new OutlineShapeParameters(xcc, ycc, 1.0), slant);
        }
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public LinesOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        this(r, size);
    }

    //--------------------- Static methods for creating OutlineShapes

    /**
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @return A LinesOutlineShape.
     */
    public static OutlineShape randomInstance(Random r, Dimension size)
    {
        return new LinesOutlineShape(r, size);
    }
}
