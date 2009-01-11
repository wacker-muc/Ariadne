package swa.ariadne.outlines.functions;

import java.util.Random;

import swa.util.IPickable;

/**
 * This class implements the equivalent of a constructor for {@link Function} objects.
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
     * @param client A function object that will use our result Function object as a basis.
     * @return A Function object that will use the client's DistortionSpecs.
     */
    protected abstract Function create(Random r, Function client);
    
    /**
     * @param r A source of random numbers.
     * @return A specific Function object.
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
