package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.IPickable;

/**
 * Implements the equivalent of a constructor for {@link OutlineShape} objects.
 * <p>
 * Implements the {@link IPickable} interface.
 *
 * @author Stephan.Wacker@web.de
 */
public abstract
class OutlineShapeConstructor
implements IPickable
{
    //--------------------- Constructor-like functions

    /**
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params
     * @return A specific {@link OutlineShape} object.
     */
    abstract OutlineShape create(Random r, Dimension size, OutlineShapeParameters params);

    //--------------------- IPickable implementation

    @Override
    public final int getRatio()
    {
        return 100 * variants() * attractivity() / recognition();
    }

    //--------------------- Abstract Properties that determine the IPickable ratio

    /**
     * @return How many different shape variants does this {@link OutlineShapeConstructor} produce?
     */
    protected abstract int variants();

    /**
     * @return How attractive are these {@link OutlineShape}s?
     * <br> 5 = low, 10 = regular, 15 = high, 20 = very high.
     */
    protected abstract int attractivity();

    /**
     * @return How easy will these {@link OutlineShape}s be recognized?
     * <br> 1 = hard, 2 = regular, 3 = easy, 4 = very easy,
     */
    protected abstract int recognition();

    //--------------------- Auxiliary methods

    @Override
    public String toString()
    {
        return String.format("%s {v=%d, a=%d, r=%d}", super.toString(),
                variants(), attractivity(), recognition());
    }
}
