package swa.ariadne.model;

/**
 * Positions of the walls around a MazeSquare: East, North, West, South.
 * 
 * @author Stephan.Wacker@web.de
 */
public enum WallPosition
{
    /** East. */
    WP_E,
    /** North. */
    WP_N,
    /** West. */
    WP_W,
    /** South. */
    WP_S;
    
    /** Minimum WallPosition value. */
    //public final static int MIN = WP_E.ordinal(); // TODO: remove
    /** Maximum WallPosition value. */
    //public final static int MAX = WP_S.ordinal(); // TODO: remove
    /** Number of WallPosition values. */
    public final static int NUM = values().length;
    
    /**
     * @param side One of the four directions. 
     * @return The opposite direction. 
     */
    public static WallPosition oppositeWall(WallPosition side)
    {
        switch (side)
        {
            case WP_E: return WP_W;
            case WP_N: return WP_S;
            case WP_W: return WP_E;
            case WP_S: return WP_N;
            default: throw new IllegalArgumentException("invalid argument: " + side);
        }
    }
}