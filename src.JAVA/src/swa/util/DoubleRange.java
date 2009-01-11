package swa.util;

import java.util.Random;

/**
 * A range defined by a minimum and maximum value.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class DoubleRange
{
    /** Minimum value. */
    public double min;
    /** Maximum value. */
    public double max;
    
    /** When positive, the {@link #pick(Random)} method will return multiples of this unit. */
    public double unit = 0.0;
    
    /**
     * @param min Minimum value.
     * @param max Maximum value.
     */
    public DoubleRange(double min, double max)
    {
        this.min = min;
        this.max = max;
    }

    /**
     * @param r A source of random numbers.
     * @return <ul><li>
     * If unit <= 0: A value between <em>min</em> (inclusive) and <em>max</em> (exclusive):
     * <em>min <= result < max</em>
     * </li><li>
     * If unit > 0: One of the values <em>min &times; unit</em> to <em>max &times; unit</em> (both inclusive).
     * </li></ul>  
     */
    public double pick(Random r)
    {
        if (unit > 0)
        {
            return unit * ((int)min + r.nextInt((int)max - (int)min + 1));
        }
        else
        {
            return min + r.nextDouble() * (max - min);
        }
    }
}
