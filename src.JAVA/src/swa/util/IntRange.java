package swa.util;

import java.util.Random;

/**
 * A range defined by a minimum and maximum value.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class IntRange
{
    /** Minimum value. */
    public int min;
    /** Maximum value. */
    public int max;
    
    /**
     * @param min Minimum value.
     * @param max Maximum value.
     */
    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    /**
     * @param r A source of random numbers.
     * @return A value between <em>min</em> (inclusive) and <em>max</em> (exclusive).
     * <br> min <= result < max
     */
    public int pick(Random r)
    {
        return min + r.nextInt(max - min);
    }
}
