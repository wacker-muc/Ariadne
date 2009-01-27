package swa.ariadne.gui.images;

import java.util.ArrayList;

/**
 * Alternating sequence of <em>left border</em> / <em>right border</em>
 * positions on a horizontal line.
 *
 * @author Stephan.Wacker@web.de
 */
final
class ScanLine
{
    //--------------------- Member variables and Properties.

    /** Positions on the scan line. */
    /*default*/ ArrayList<Integer> data;

   /**
    * @return The number of points on this scan line.
    */
    public int size()
    {
        return data.size();
    }

    //--------------------- Constructors.

    /**
     * Constructor.
     * @param width Maximum pixel position on a scan line +1, width of the image.
     * @param bothTerminators If true, the scan line will be terminated on both ends,
     * otherwise only on the right end.
     */
    ScanLine(int width, boolean bothTerminators)
    {
        this.data = new ArrayList<Integer>(16);

        // Add termination points that are well beyond the regular scan line.
        if (bothTerminators)
        {
            data.add(-2);
        }
        data.add(width + 1);
    }

    /**
     * Constructor.
     * <br> Only used by the {@link #emptyClone()} method.
     * @param initialCapacity Initial capacity of the internal list.
     */
    private ScanLine(int initialCapacity)
    {
        this.data = new ArrayList<Integer>(initialCapacity);
    }

    /**
     * @return A {@link ScanLine} object of the same size as its source.
     * The terminator entries are copied, but otherwise the result will be empty.
     */
    /*default*/ ScanLine emptyClone()
    {
        int size = this.size();
        ScanLine result = new ScanLine(size);

        // Add termination points.
        if (size % 2 == 0)
        {
            result.data.add(this.data.get(0));      // left terminator
        }
        result.data.add(this.data.get(size-1));     // right terminator

        return result;
    }

    //--------------------- Methods for managing the object points.

    /**
     * Insert a point into this scan line.
     * @param x The X coordinate of a point on the image object contour.
     */
    public void InsertObjectPoint(int x)
    {
        // Find the position p with x < data[p].
        // Note: As there is a terminator entry image.Width, we will not leave the valid index range.
        for (int p = 0; ; p++)
        {
            if (x < data.get(p))
            {
                data.add(p, x);
                break;
            }
        }
    }

    //--------------------- Methods for managing the border and contour points.

    /**
     * Insert two points into the indicated scan line.
     * @param xL The X coordinate of the left border point.
     * @param xR The X coordinate of the right border point.
     */
    public void InsertPair(int xL, int xR)
    {
        // Position where the border points fit into the scanLine.
        int q = 2;      // A right border point position.
        int xq = 0;

        int count = data.size();

        // Find the position (p,q) where (xL .. xR) overlaps or touches (data[p] .. data[q]).
        while (q < count && (xq = data.get(q)) < xL - 1)
        {
            q += 2;
        }
        int p = q - 1;  // A (valid) left border point position.
        int xp = data.get(p);

        if (xR + 1 < xp)            // no overlap; insert new LR pair
        {
            data.add(p, xR);
            data.add(p, xL);
        }
        else                        // overlap
        {
            // Find the following scan line LR pair that is still within the reach of the new LR pair.
            int p1 = p + 2;
            while (p1 < count - 2 && data.get(p1) <= xR + 1)
            {
                p1 += 2;
            }
            p1 -= 2;

            // Eliminate RL pairs that are overlapped or touched by the new LR pair.
            if (p1 > p)
            {
                for (int i = 0; i < p1 - p; i++)
                {
                    data.remove(q);
                }
                xq = data.get(q);
            }

            // Extend the current LR pair up to the the extent of the given LR pair.
            if (xL < xp) { data.set(p, xL); }
            if (xR > xq) { data.set(q, xR); }
        }
    }

    /**
     * Removes the given number of items, starting at the given position.
     * @param p Index position within {@link #data}.
     * @param count Number of items that shall be removed.
     */
    public void RemoveRange(int p, int count)
    {
        for(int i = 0; i < count; i++)
        {
            this.data.remove(p);
        }
    }
}
