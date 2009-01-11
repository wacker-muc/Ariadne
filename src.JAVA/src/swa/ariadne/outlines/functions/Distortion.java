package swa.ariadne.outlines.functions;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * A continuous 2D mapping.
 * All implementations are created as anonymous types in {@link DistortionConstructor}};
 * 
 * @author Stephan.Wacker@web.de
 */
abstract class Distortion
{
    //--------------------- Member variables
    
    /**
     * Amplitude.
     * A factor by which the basic distortion generating function's value is multiplied.
     * <br>Higher values produce greater distortions.
     */
    protected double a;
    
    /**
     * Frequency.
     * A factor by which the basic distortion generating function's operand is multiplied.
     * <br>E.g. if the function is <em>sin(fx)</em> or <em>cos(fx)</em>,
     * higher values produce shorter waves.
     */
    protected double f;

    //--------------------- Constructors

    /**
     * Constructor.
     * @param a {@linkplain Distortion#a Amplitude}.
     * @param f {@linkplain Distortion#f Frequency}.
     */
    protected Distortion(double a, double f)
    {
        super();
        this.a = a;
        this.f = f;
    }
    
    //--------------------- Abstract methods
    
    /**
     * @param p A point that will be distorted.
     */
    abstract void apply(Point2D p);
    
    /**
     * @param pp A point that will be distorted.
     */
    abstract void apply(Point2DPolar pp);
}