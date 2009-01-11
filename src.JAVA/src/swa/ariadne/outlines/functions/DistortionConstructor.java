package swa.ariadne.outlines.functions;

import swa.util.Point2D;
import swa.util.Point2DPolar;

/**
 * This class implements the equivalent of a constructor for Distortion objects.
 * The static methods of this class return the constructors of various Distortions.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract class DistortionConstructor
{
    //--------------------- Abstract methods that are implemented in anonymous types
    
    /**
     * @param a Amplitude.
     * @param f Frequency.
     * @return A specific Distortion object configured with the given parameters.
     */
    abstract Distortion create(double a, double f);
    
    //--------------------- Constructors
    
    /**
     * Private constructor.
     * All instances need to be created by the static methods.
     */
    private DistortionConstructor()
    {
        super();
    }
    
    //--------------------- Static methods
    
    /**
     * @return A distortion that leaves both coordinates unmodified. 
     */
    public static DistortionConstructor DistortNone()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new Distortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        // do nothing
                    }

                    @Override
                    void apply(Point2DPolar pp)
                    {
                        // do nothing
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that adds a cosine wave to the X parameter.
     * <br> (x , y) -> (x - cos(y) , y)
     */
    public static DistortionConstructor DistortX_CosY()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        p.x -= this.a * Math.cos(0.5 * Math.PI * this.f * p.y);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that adds a cosine wave to the Y parameter.
     * <br> (x , y) -> (x , (y - cos(x))
     */
    public static DistortionConstructor DistortY_CosX()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        p.y -= this.a * Math.cos(0.5 * Math.PI * this.f * p.x);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that adds a cosine wave to both the X and Y parameters.
     * <br> (x , y) -> ((x - cos(y) , (y - cos(x))
     */
    public static DistortionConstructor DistortXY_CosY_CosX()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        double x0 = p.x;
                        p.x -= this.a * Math.cos(0.5 * Math.PI * this.f * p.y);
                        p.y -= this.a * Math.cos(0.5 * Math.PI * this.f * x0);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that adds a cosine wave to the X parameter.
     * Every second wave is inverted.
     * <br> (x , y) -> (x +/- cos(y) , y)
     */
    public static DistortionConstructor DistortX_CosY_Alternating()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        // The wave lines are located where x is a whole number.
                        double sgnX = ((int)Math.round(0.5 * (p.x - 1.0)) % 2 == 0 ? +1.0 : -1.0);
                        
                        p.x -= sgnX * this.a * Math.cos(0.5 * Math.PI * this.f * p.y);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that adds a cosine wave to both the X and Y parameters.
     * Every second wave is inverted.
     * <br> (x , y) -> (x +/- cos(y) , (y +/- cos(x))
     */
    public static DistortionConstructor DistortXY_CosY_CosX_Alternating()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        // The wave lines are located where x is a whole number.
                        double sgnX = ((int)Math.round(0.5 * (p.x - 1.0)) % 2 == 0 ? +1.0 : -1.0);
                        double sgnY = ((int)Math.round(0.5 * (p.y - 1.0)) % 2 == 0 ? +1.0 : -1.0);
                        
                        double x0 = p.x;
                        p.x -= sgnX * this.a * Math.cos(0.5 * Math.PI * this.f * p.y);
                        p.y -= sgnY * this.a * Math.cos(0.5 * Math.PI * this.f * x0);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that stretches the shape by {@link Distortion#a a} in Y direction.
     * <br> (x , y) -> (x , y / a)
     */
    public static DistortionConstructor DistortY_Stretch()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        p.x -= this.a * Math.cos(0.5 * Math.PI * this.f * p.y);
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that stretches the shape by {@link Distortion#a a} in Y direction
     * and rotates it by {@link Distortion#f f}.
     */
    public static DistortionConstructor DistortXY_StretchY_Rotate()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new CartesianDistortion(a, f)
                {
                    @Override
                    void apply(Point2D p)
                    {
                        // The distortions need to be applied in inverse order: first rotate, then stretch.
                        Point2DPolar pp = p.asPolar();
                        pp.phi -= this.f;
                        p.assignFrom(pp);
                        p.y /= this.a;
                    }
                };
            }
        };
    }

    /**
     * @return A distortion that scales the radius exponentially, creating a concave bending.
     */
    public static DistortionConstructor DistortR_Exp()
    {
        return new DistortionConstructor()
        {
            @Override
            Distortion create(double a, double f)
            {
                return new PolarDistortion(a, f)
                {
                    @Override
                    void apply(Point2DPolar pp)
                    {
                        pp.r = Math.pow(pp.r, 1 + this.a);

                        // Compensate for the overall change of scale.
                        // This gives a reasonably balanced picture.
                        pp.r /= (1 + this.a);
                    }
                };
            }
        };
    }
}