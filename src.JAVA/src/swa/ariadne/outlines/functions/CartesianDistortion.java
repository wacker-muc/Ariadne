package swa.ariadne.outlines.functions;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * A {@link Distortion} that operates on Cartesian coordinates.
 * <br> Subclasses need to implement the {@link Distortion#apply(Point2D)} method. 
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class CartesianDistortion extends Distortion
{
    //--------------------- Constructors

    /**
     * Constructor.
     * @param a {@linkplain Distortion#a Amplitude}.
     * @param f {@linkplain Distortion#f Frequency}.
     */
    protected CartesianDistortion(double a, double f)
    {
        super(a, f);
    }

    //--------------------- Distortion implementation

    /**
     * Do the distortion using {@link Distortion#apply(Point2D)}.
     * @param pp A point that will be distorted.
     */
    @Override
    final void apply(Point2DPolar pp)
    {
        Point2D pc = pp.asCartesian();
        this.apply(pc);
        pp.assignFrom(pc);
    }
}
