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
    
    /** When true, {@link #pick(Random)} will return a maximum of {@link #max}, otherwise {@link #max max}-1 */
    public boolean includingMax;
    
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
     * @return A value between <em>min</em> (inclusive)
     *         and <em>max</em> (exclusive or inclusive, depending on {@link #includingMax}).
     * <ul><li>
     * If {@link #includingMax} is false: min <= result < max
     * </li><li>
     * If {@link #includingMax} is true: min <= result <= max
     * </li></ul>
     */
    public int pick(Random r)
    {
        return min + r.nextInt(max - min + (includingMax ? 1 : 0));
    }
}
