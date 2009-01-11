package swa.ariadne.outlines.functions;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * A {@link Distortion} that operates on Polar coordinates.
 * <br> Subclasses need to implement the {@link Distortion#apply(Point2DPolar)} method. 
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class PolarDistortion extends Distortion
{
    //--------------------- Constructors

    /**
     * Constructor.
     * @param a {@linkplain Distortion#a Amplitude}.
     * @param f {@linkplain Distortion#f Frequency}.
     */
    protected PolarDistortion(double a, double f)
    {
        super(a, f);
    }

    //--------------------- Distortion implementation

    /**
     * Do the distortion using {@link Distortion#apply(Point2DPolar)}.
     * @param p A point that will be distorted.
     */
    @Override
    final void apply(Point2D p)
    {
        Point2DPolar pp = p.asPolar();
        this.apply(pp);
        p.assignFrom(pp);
    }
}
