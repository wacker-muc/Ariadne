package swa.ariadne.gui.images;

/**
 * A RelativePoint is located at a certain position relative to a point on the object contour.
 * It also has an Alpha value that only depends on its distance.
 *
 * @author Stephan.Wacker@web.de
 */
final
class RelativePoint
{
    //--------------------- Constants.

    // For an eighth left/right turn, add +1/-1.
    /** Neighbor direction: east */
    public final static int NbE = 0;
    /** Neighbor direction: north east */
    public final static int NbNE = 1;
    /** Neighbor direction: east */
    public final static int NbN = 2;
    /** Neighbor direction: north west */
    public final static int NbNW = 3;
    /** Neighbor direction: west */
    public final static int NbW = 4;
    /** Neighbor direction: south west */
    public final static int NbSW = 5;
    /** Neighbor direction: south */
    public final static int NbS = 6;
    /** Neighbor direction: south east */
    public final static int NbSE = 7;

    /** For each neighbor direction the relative X distance, [-1 .. +1]. */
    public final static int[] NbDX = { +1, +1, 0, -1, -1, -1, 0, +1, };

    /** For each neighbor direction the relative Y distance, [-1 .. +1]. */
    public final static int[] NbDY = { 0, -1, -1, -1, 0, +1, +1, +1, };

    /** For each pair of (dx+1, dy+1), the corresponding neighbor direction. */
    public final static int[][] Nb = {
        { NbNW, NbW, NbSW }, // dx = -1
        { NbN,   -1, NbS  }, // dx =  0
        { NbNE, NbE, NbSE }, // dx = +1
    };

    //--------------------- Member variables and Properties.

    /** Relative X position of the point. */
    public int rx;
    /** Relative Y position of the point. */
    public int ry;

    /** Alpha value to be applied to the pixel at (rx, ry). */
    public int a;

    //--------------------- Constructors.

    /**
     * Constructor.
     * @param rx Relative X position of the point.
     * @param ry Relative Y position of the point.
     * @param a Alpha value to be applied to the pixel at (rx, ry).
     */
    public RelativePoint(int rx, int ry, int a)
    {
        this.rx = rx; this.ry = ry; this.a = a;
    }

    /**
     * Constructor.
     * @param rx Relative X position of the point.
     * @param ry Relative Y position of the point.
     */
    public RelativePoint(int rx, int ry)
    {
        this.rx = rx; this.ry = ry; this.a = 0;
    }

    //--------------------- Auxiliary methods.

    @Override
    public String toString()
    {
        return String.format("a(%d,%d) = %d", rx, ry, a);
    }
}
