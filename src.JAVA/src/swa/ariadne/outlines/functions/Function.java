package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.Picker;
import swa.util.Point2D;

/**
 * A two-dimensional function.
 * Points evaluating to positive values are considered "inside" the resulting
 * {@link swa.ariadne.outlines.FunctionOutlineShape FunctionOutlineShape}.  
 * 
 * @author Stephan.Wacker@web.de
 */
public abstract
class Function
{
    //--------------------- Member variables and Properties
    
    /**
     * One of four orientations of the coordinate system: 1..4.
     * For value 0, the function is called with parameters function(r, phi) instead of function(x, y).
     */
    private int symmetryRotation = 1;

    /** Factor applied for converting shape coordinates to function coordinates. */
    protected double scale = 1.0;

    /**
     * A distortion method applied to the input coordinates,
     * immediately before the function is called.
     */
    protected Distortion distortion = null;
    
    /**
     * Another function that uses this function as a basis.
     * If this is not null, we will use the client's symmetryRotation, scale and distortion
     * instead of our own.
     */
    private Function clientFunction = null;
    
    /**
     * @return The function object at the top of a function hierarchy.
     * <br> see {@link #clientFunction}
     */
    private Function getMasterFunction()
    {
        if (clientFunction == null)
        {
            return this;
        }
        else
        {
            return clientFunction.getMasterFunction();
        }
    }

    //--------------------- Constructors
    
    /**
     * Constructor.
     * @param clientFunction A function that uses this function as a basis -- or null.
     * @param r A source of random numbers.
     */
    protected Function(Random r, Function clientFunction)
    {
        super();
        
        this.clientFunction = clientFunction;
        
        if (clientFunction != null)
        {
            this.symmetryRotation = clientFunction.symmetryRotation;
            this.scale = clientFunction.scale;
        }
    }
    
    /**
     * Constructor.
     * @param r A source of random numbers.
     * @param clientFunction A function that uses this function as a basis -- or null.
     * @param symmetry Rotational symmetry of the basic function:
     *                 1 = single, 2 = mirror, 4 = four-fold, 0 = rotational
     */
    protected Function(Random r, Function clientFunction, int symmetry)
    {
        this(r, clientFunction);
        
        if (clientFunction != null)
        {
            if (symmetry == 0)
            {
                symmetryRotation = 0;
            }
            else
            {
                symmetryRotation = 1 + r.nextInt(4 / symmetry);
            }
        }
    }
    
    /**
     * Constructor.
     * @param r A source of random numbers.
     * @param clientFunction A function that uses this function as a basis -- or null.
     * @param symmetry Rotational symmetry of the basic function:
     *                 1 = single, 2 = mirror, 4 = four-fold, 0 = rotational
     * @param scaleMin Minimum scaling factor to be applied to the coordinate system.
     * @param scaleMax Maximum scaling factor to be applied to the coordinate system.
     */
    protected Function(Random r, Function clientFunction, int symmetry, double scaleMin, double scaleMax)
    {
        this(r, clientFunction, symmetry);
        
        if (clientFunction != null)
        {
            this.scale = scaleMin + r.nextDouble() * (scaleMax - scaleMin);
        }
    }

    //--------------------- Setup methods
    
    /**
     * Picks one of the applicable distortions.
     * @param r A source of random numbers.
     * @see #initAdditionalParameters(Random)
     */
    public void addDistortion(Random r)
    {
        DistortionSpec[] distortionSpecs = getMasterFunction().getDistortionSpecs();
        
        DistortionSpec spec = new Picker<DistortionSpec>(distortionSpecs).pick(r);
        if (spec != null)
        {
            this.distortion = spec.create(r);
        }
    }
    
    /**
     * If a {@link Function} implementation has more parameters
     * than those provided by the plain {@link Function} base class,
     * they should be configured by overriding this method.
     * <p>
     * <em>Note:</em> This method must be called after {@link #addDistortion(Random)}. 
     * @param r A source of random numbers.
     */
    public void initAdditionalParameters(Random r)
    {
        // do nothing
    }
    //--------------------- Abstract methods and Properties
    
    /**
     * @return All distortion specifications applicable to this function object.
     * <br> This should usually contain an instance of {@link DistortionConstructor#DistortNone()}
     * &ndash; or null, which is equivalent.
     * <br> If the list is empty or null, no distortion will be used.
     */
    protected DistortionSpec[] getDistortionSpecs()
    {
        return null;
    }

    /**
     * Evaluate the basic function for the given point.
     * If applicable, the argument point has already been rotated and distorted.
     * @param p The function argument.
     * @return Positive values for points inside the shape.
     */
    protected abstract double evaluate(Point2D p);
    
    //--------------------- The public function evaluation method
    
    /**
     * Evaluate the function for the given point.
     * @param x Point's X coordinate.
     * @param y Point's Y coordinate.
     * @return Positive values for points inside the shape.
     */
    public double evaluate(double x, double y)
    {
        Point2D p2;
        
        switch (symmetryRotation)
        {
            default:
            case 1: // natural orientation
                p2 = new Point2D(+x, +y);
                break;
            case 2: // 90 degrees rotated
                p2 = new Point2D(-y, +x);
                break;
            case 3: // 180 degrees rotated
                p2 = new Point2D(-x, -y);
                break;
            case 4: // 270 degrees rotated
                p2 = new Point2D(+y, -x);
                break;
        }
        
        p2.x *= +scale;
        p2.y *= -scale;

        if (distortion != null)
        {
            distortion.apply(p2);
        }

        return evaluate(p2);
    }
}
