package swa.ariadne.outlines;

import java.awt.Dimension;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * Applies a continuous transformation to the coordinate system of an underlying shape.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class DistortedOutlineShape
extends ContinuousOutlineShape
{
    //--------------------- Types

    /**
     * Delegate class.
     * The transform() method should realize a continuous 2D function.
     * <p> Note: As the mapping is from distorted to original coordinates,
     * the function's intended operation needs to be "inverted".
     * E.g., if the distorted shape should be like the original shape,
     * turned <em>counterclockwise</em> by a certain angle, the mapping must turn the
     * given point <em>clockwise</em> by that angle. 
     */
    public static abstract class Distortion
    {
        /**
         * @param p A point in the distorted shape's coordinate system.
         * @return A point in the underlying shape's coordinate system.
         */
        public abstract Point2D transform(Point2D p);
    }

    //--------------------- Member Variables and Properties

    /** The underlying shape. */
    private ContinuousOutlineShape baseShape;

    /** The function implementing the desired mapping. */
    private Distortion distortion;
    
    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2D p = this.distortion.transform(new Point2D(x, y));
        return baseShape.get(p.x, p.y);
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     * @param baseShape The original shape.
     * @param distortion The distortion to be applied.
     */
    protected DistortedOutlineShape(Dimension size, ContinuousOutlineShape baseShape, Distortion distortion)
    {
        super(size);

        this.baseShape = baseShape;
        this.distortion = distortion;
    }

    //--------------------- Static methods returning Distortion delegates

    /**
     * Points are rotated in a counterclockwise direction around the given center.
     * The resulting DistortedOutlineShape will be rotated in a clockwise direction.
     * @param center Center of the distortion; this point is mapped to itself.
     * @param size Radius of the reference circle around the center.
     * @param winding Points on the reference circle are wound by winding*2*PI radians (counterclockwise).
     * @return A distortion that turns a straight line through the given center into a spiral. 
     */
    public static Distortion SpiralDistortion(final Point2D center, final double size, final double winding)
    {
        return new Distortion()
        {
            @Override
            public Point2D transform(Point2D p)
            {
                Point2D p0 = p.clone();
                p0.x -= center.x;
                p0.y -= center.y;
                Point2DPolar pp = p0.asPolar();
                
                pp.phi += pp.r * winding / size * 2.0 * Math.PI;

                Point2D result = pp.asCartesian();
                result.x += center.x;
                result.y += center.y;
                return result;
            }
            
            @Override
            public String toString()
            {
                return String.format("%s(%s, %.2f, %.2f)", "SpiralDistortion", center.toString(), size, winding);
            }
        };
    }

    /**
     * Points are moved radially away from the given center.
     * The resulting DistortedOutlineShape will be indented towards the center.
     * @param center Center of the distortion; this point is mapped to itself.
     * @param waveCount There will be waveCount "corners" that are not distorted.
     * @param waveShift When 0, the points at (2*PI)/(n*waveCount) radians will not be distorted.
     * @param minRatio The distance from the center will be multiplied by a value between minRatio and 1.
     * @return A distortion that indents a circle around the given center.
     */
    public static Distortion RadialWaveDistortion(final Point2D center, final int waveCount, final double waveShift, final double minRatio)
    {
        return new Distortion()
        {
            @Override
            public Point2D transform(Point2D p)
            {
                Point2D p0 = p.clone();
                p0.x -= center.x;
                p0.y -= center.y;
                Point2DPolar pp = p0.asPolar();

                double phi = pp.phi;
                phi -= waveShift * 2.0 * Math.PI;
                phi *= waveCount;
                double k = Math.max(-0.5, Math.min(0.999, (1.0 - minRatio)));
                double f = Math.cos(phi);       //  -1 .. +1, f(0) = +1
                double g = 0.5 * k * (1 - f);   //   0 .. k,  g(0) = 0
                double h = 1 - g;               // 1-k .. 1,  h(0) = 1
                pp.r /= h;

                Point2D result = pp.asCartesian();
                result.x += center.x;
                result.y += center.y;
                return result;
            }
            
            @Override
            public String toString()
            {
                return String.format("%s(%s, %d, %.2f, %.2f)", "RadialWaveDistortion", center.toString(), waveCount, waveShift, minRatio);
            }
        };
    }
}
