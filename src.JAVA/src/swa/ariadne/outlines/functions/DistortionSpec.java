package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.DoubleRange;
import swa.util.IPickable;

/**
 * Specification for creating Distortions with a certain parameter range.
 * <p>
 * Implements the {@link IPickable} interface.
 * 
 * @author Stephan.Wacker@web.de
 */
class DistortionSpec
implements IPickable
{
    //--------------------- Member variables

    /** Relative ratio with which this distortion should be applied. */
    public int ratio = 100;

    /** Distortion amplitude factor range. */
    public DoubleRange aRange = new DoubleRange(0.0, 1.0);

    /** Frequency range. */
    public DoubleRange fRange = new DoubleRange(0.5, 2.0);

    /** A constructor for Distortion objects. */
    private DistortionConstructor constructor;

    //--------------------- IPickable implementation
    
    @Override
    public int getRatio()
    {
        return this.ratio;
    }

    //--------------------- Constructors
    
    /**
     * @param ratio Relative ratio with which this distortion should be applied.
     * @param constructor A constructor for Distortion objects.
     */
    public DistortionSpec(int ratio, DistortionConstructor constructor)
    {
        this(ratio, constructor, 1.0, 1.0, 1.0, 1.0);
    }
    
    /**
     * @param ratio Relative ratio with which this distortion should be applied.
     * @param constructor A constructor for Distortion objects.
     * @param aMin Minimum distortion amplitude factor.
     * @param aMax Maximum distortion amplitude factor.
     */
    public DistortionSpec(int ratio, DistortionConstructor constructor, double aMin, double aMax)
    {
        this(ratio, constructor, aMin, aMax, 1.0, 1.0);
    }

    /**
     * @param ratio Relative ratio with which this distortion should be applied.
     * @param constructor A constructor for Distortion objects.
     * @param aMin Minimum distortion amplitude factor.
     * @param aMax Maximum distortion amplitude factor.
     * @param fMin Minimum distortion frequency, approx. 1.0
     * @param fMax Maximum distortion frequency, approx. 1.0
     */
    public DistortionSpec(int ratio, DistortionConstructor constructor, double aMin, double aMax, double fMin, double fMax)
    {
        this.ratio = ratio;
        this.constructor = constructor;
        this.aRange.min = aMin;
        this.aRange.max = aMax;
        this.fRange.min = fMin;
        this.fRange.max = fMax;
    }

    /**
     * @param ratio Relative ratio with which this distortion should be applied.
     * @param constructor A constructor for Distortion objects.
     * @param aMin Minimum distortion amplitude factor.
     * @param aMax Maximum distortion amplitude factor.
     * @param fMin Minimum distortion frequency, approx. 1.0
     * @param fMax Maximum distortion frequency, approx. 1.0
     * @param fUnit Distortion frequency unit.
     */
    public DistortionSpec(int ratio, DistortionConstructor constructor, double aMin, double aMax, int fMin, int fMax, double fUnit)
    {
        this(ratio, constructor, aMin, aMax, fMin, fMax);
        this.fRange.unit = fUnit;
    }
    
    //--------------------- Creating a Distortion
    
    /**
     * @param r A source of random numbers.
     * @return A fully configured Distortion object.
     */
    public Distortion create(Random r)
    {
        Distortion result = constructor.create(aRange.pick(r), fRange.pick(r));
        
        return result;
    }
}