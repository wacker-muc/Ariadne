package swa.ariadne.outlines.grids;

import java.util.Arrays;
import java.util.List;

/**
 * Collection of several {@link GridTile}s which are all painted
 * into the same {@link Grid} cell.
 *
 * @author Stephan.Wacker@web.de
 */
final
class CombinedTile
extends GridTile
{
    //--------------------- Member variables and Properties

    /** Collection of basic tiles. */
    private final List<GridTile> tiles;

    //--------------------- Constructors

    /**
     * @param width Width of the tile.
     * @param height Height of the tile.
     * @param invertEveryOtherTile When true, the tile in every second grid cell will be inverted.
     * @param gridTiles A collection of basic {@link GridTile}s, all having the same size.
     */
    public CombinedTile(int width, int height, boolean invertEveryOtherTile, GridTile[] gridTiles)
    {
        super(width, height, invertEveryOtherTile);
        this.tiles = Arrays.asList(gridTiles);
    }

    //--------------------- GridTile implementation

    @Override
    protected boolean get(int x, int y)
    {
        boolean result = false;

        for (GridTile item : tiles)
        {
            result ^= item.get(x, y);
        }

        return result;
    }

    @Override
    public GridTile setScale(int value)
    {
        super.setScale(value);

        for (GridTile item : tiles)
        {
            item.setScale(value);
        }

        return this;
    }
}
