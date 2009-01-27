package swa.ariadne.outlines.grids;

import java.awt.Point;

import swa.ariadne.outlines.ExplicitOutlineShape;

/**
 * A pattern of set and cleared squares that can be applied to an {@link ExplicitOutlineShape}.
 * There are {@linkplain GridTile simple} and {@linkplain Grid repeated} implementations.
 *
 * @author Stephan.Wacker@web.de
 */
interface IGrid
{
    //--------------------- Abstract methods

    /** @return Width of the grid cell. */
    public int Width();

    /** @return Height of the grid cell. */
    public int Height();

    /**
     * Apply the current {@link IGrid} to the given target shape.
     * Every set square will be inverted.
     * @param target An {@link ExplicitOutlineShape} into which the grid is "painted".
     * @param at Location of the top left corner of a {@link GridTile}.
     * <br> Note: The tile is not restricted to a single grid cell
     * and may extend into neighbor cells.
     * @param inInvertedCell Indicates whether the we are painting into an inverted cell.
     */
    public abstract void Apply(ExplicitOutlineShape target, Point at, boolean inInvertedCell);
}
