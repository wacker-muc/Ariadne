package swa.util;

/**
 * Supplies mathematical functions missing in {@link java.lang.Math}.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class Math
{
    /**
     * Greatest common divisor (GCD).
     * @param a
     * @param b
     * @return The greatest common divisor of the given two integer numbers.
     */
    public static int gcd(int a, int b)
    {
        int aa = a, bb = b, t;
        while (bb != 0)
        {
            t = aa % bb;
            aa = bb;
            bb = t;
        }

        return a;
    }
}
