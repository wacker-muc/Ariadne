package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.IPickable;

/**
 * Implements the equivalent of a constructor for {@link Function} objects.
 * <p>
 * Implements the {@link IPickable} interface.
 *
 * @author Stephan.Wacker@web.de
 */
public abstract
class FunctionConstructor
implements IPickable
{
    //--------------------- Constructor-like functions

    /**
     * @param r A source of random numbers.
     * @param client A {@link Function} object that will use our result {@link Function} object as a basis.
     * @return A {@link Function} object that will use the client's {@link DistortionSpec}.
     */
    protected abstract Function create(Random r, Function client);

    /**
     * @param r A source of random numbers.
     * @return A specific {@link Function} object.
     */
    public Function create(Random r)
    {
        return this.create(r, null);
    }

    //--------------------- IPickable implementation

    /**
     * Subclasses may override this method if they want to be picked more/less often than others.
     * @return A default value of 10.
     */
    @Override
    public int getRatio()
    {
        return 10;
    }
}
