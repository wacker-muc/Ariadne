package swa.ariadne.gui.images;

import java.util.LinkedList;

/**
 * Collection of {@link RelativePoint RelativePoints} for all pairs of directions.
 * A single entry defines properties of the region around a point on the contour
 * of an image object, depending on its immediate left and right neighbor on that contour.
 *
 * @author Stephan.Wacker@web.de
 */
final class SurroundingPoints
{
    //--------------------- Types.

    /**
     * A collection of {@link RelativePoint RelativePoints}.
     * This is needed only because Java doesn't allow arrays of generic types. :-(
     */
    private class RelativePointCollection
    {
        /** A collection of {@link RelativePoint RelativePoints}. */
        public LinkedList<RelativePoint> collection;

        /**
         * Constructor.
         */
        public RelativePointCollection()
        {
            this.collection = new LinkedList<RelativePoint>();
        }
    }

    //--------------------- Member variables and Properties.

    /** The collections. */
    RelativePointCollection[][] data;

    //--------------------- Constructors.

    /**
     * Constructor.
     */
    SurroundingPoints()
    {
        this.data = new RelativePointCollection[8][8];
    }


    /**
     * Adds the given {@link RelativePoint} to the collection.
     * @param nbL Direction of the left neighbor pixel.
     * @param nbR Direction of the right neighbor pixel.
     * @param rp A {@link RelativePoint}.
     */
    public void add(int nbL, int nbR, RelativePoint rp)
    {
        this.data[nbL][nbR].collection.add(rp);
    }


    /**
     * @param nbL Direction of the left neighbor pixel.
     * @param nbR Direction of the right neighbor pixel.
     * @return A collection of {@link RelativePoint RelativePoints}.
     */
    public Iterable<RelativePoint> get(int nbL, int nbR)
    {
        return this.data[nbL][nbR].collection;
    }
}
