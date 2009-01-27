package swa.ariadne.outlines.grids;

/**
 * Defines a set of rectangular lines boxed into each other.
 */
final
class TightBoxesTile
extends GridTile
{
    //--------------------- Constructors

    /**
     * @param width Width of the tile.
     * @param height Height of the tile.
     * @param invertEveryOtherTile When true, the tile in every second grid cell will be inverted.
     */
    public TightBoxesTile(int width, int height, boolean invertEveryOtherTile)
    {
        super(width, height, invertEveryOtherTile);
    }

    //--------------------- GridTile implementation

    @Override
    protected boolean get(int x, int y)
    {
        // Find the closest distance to any of the tile borders.
        int dl = x - 0;
        int dr = Width() - 1 - x;
        int dt = y - 0;
        int db = Height() - 1 - y;
        int dx = Math.min(dl, dr);
        int dy = Math.min(dt, db);
        int d = Math.min(dx, dy);

        // Return alternatingly true and false.
        // The outermost line should be cleared, the next line is set, etc.
        return ((d / getScale()) % 2 == 1);
    }
}
