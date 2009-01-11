package swa.util;

/**
 * Lists of objects that implement this interface can be processed by a {@link Picker}.
 * <p>
 * The higher the value of an object's {@link #getRatio()} result,
 * the more often it will be picked.
 */
public
interface IPickable
{
    /**
     * @return The relative frequency with which this item should be picked
     * from a list of similar items.
     * <br> The result must be a positive integer.
     */
    int getRatio();
}
