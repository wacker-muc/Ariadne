package swa.util;

import java.util.Random;

/**
 * Creates Random number generators with distinct seeds.
 * Use this class if your application may use several {@link Random} objects at the same time.
 * The simple Random() constructor would seed them all with the (identical) current time.
 * 
 * @author Stephan.Wacker@web.de
 */
public class RandomFactory
{
    /** Source of new seed values. */
    private static Random r = new Random();

    /**
     * @return A new Random object with a random initial seed.
     */
    public static Random createRandom()
    {
        return new Random(r.nextInt());
    }

    /**
     * @param seed Initialization of the random number generator.
     * @return A new Random object with the specific given initial seed.
     */
    public static Random createRandom(int seed)
    {
        return new Random(seed);
    }
    
    /**
     * If only a small number of values is needed, this method may be used
     * instead of {@link #createRandom()}.
     * @param limit The (exclusive) upper limit for the range of numbers.
     * @return A random number between 0 and range-1.
     */
    public static int nextInt(int limit)
    {
        return r.nextInt(limit);
    }
}
