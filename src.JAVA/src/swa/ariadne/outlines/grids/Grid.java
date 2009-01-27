package swa.ariadne.outlines.grids;

import java.awt.Point;
import java.awt.Rectangle;

import swa.ariadne.outlines.ExplicitOutlineShape;

/**
 * A grid is made of a {@link GridTile} that is applied repeatedly in a rectangular grid
 * over the whole target shape.
 */
class Grid
implements IGrid
{
    //--------------------- Member variables and Properties

    /**
     *  The basic {@link GridTile} that is applied in every grid cell.
     */
    private final GridTile baseTile;

    //--------------------- Constructors

    /**
     * @param baseTile A {@link GridTile} that is applied in every grid cell.
     */
    public Grid(GridTile baseTile)
    {
        this.baseTile = baseTile;
    }

    //--------------------- IGrid implementation

    @Override
    public int Width()
    {
        return baseTile.Width();
    }

    @Override
    public int Height()
    {
        return baseTile.Height();
    }

    @Override
    public void Apply(ExplicitOutlineShape target, Point at, boolean inInvertedCell)
    {
        // Determine location of the center tile.
        int x = at.x;
        int y = at.y;

        // Determine bounding box of the center tile.
        Rectangle bbox = baseTile.BoundingBox();
        bbox.x += x;
        bbox.y += y;

        // Move tile location to the left/top so that the bounding box is just inside of the shape.
        int i = (bbox.x + bbox.width) / baseTile.Width();
        int j = (bbox.y + bbox.height) / baseTile.Height();
        int k = (i + j) % 2;
        x -= i * baseTile.Width();
        y -= j * baseTile.Height();
        bbox.x -= i * baseTile.Width();
        bbox.y -= j * baseTile.Height();

        // Apply the tile in all locations where the bounding box is still inside of the shape.
        for (i = 0; bbox.x + i * baseTile.Width() < target.getWidth(); i++)
        {
            for (j = 0; bbox.y + j * baseTile.Height() < target.getHeight(); j++)
            {
                boolean inverted = (i + j + k) % 2 == 0;
                baseTile.Apply(target, new Point(x + i * baseTile.Width(), y + j * baseTile.Height()), inverted);
            }
        }
    }
}
