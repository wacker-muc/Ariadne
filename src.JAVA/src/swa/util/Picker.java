package swa.util;

import java.util.Arrays;
import java.util.Random;

/**
 * Offers a {@link #pick(Random)} method that chooses one item from a set.
 * The items may be chosen with different probabilities, as configured by their
 * individual {@linkplain IPickable#getRatio() ratio}.
 * 
 * @param <T> An item type that implements the {@link IPickable} interface.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class Picker<T extends IPickable>
{
    /** An object, like a List, that implements the {@link Iterable} interface. */ 
    Iterable<T> list;
    
    /**
     * Constructor.
     * @param itemList An object, like a List, that implements the {@link Iterable} interface. 
     */
    public Picker(Iterable<T> itemList)
    {
        this.list = itemList;
    }
    
    /**
     * Constructor.
     * @param itemArray An array of items.
     */
    public Picker(T[] itemArray)
    {
        this(Arrays.asList(itemArray));
    }

    /**
     * @param r A source of random numbers.
     * @return Some item from the given list.
     * <br> Items with a higher {@link IPickable#getRatio()} value are chosen more often
     * than those with a lower value.
     */
    public T pick(Random r)
    {
        if (list != null)
        {
            int sumRatios = 0;
            for (T item : list)
            {
                sumRatios += item.getRatio();
            }
            
            int p = r.nextInt(sumRatios); // 0 .. sumRatios-1
            for (T item : list)
            {
                p -= item.getRatio();
                if (p < 0)
                {
                    return item;
                }
            }
        }
        
        return null;
    }
}
