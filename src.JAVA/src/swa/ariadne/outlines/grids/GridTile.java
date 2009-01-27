package swa.ariadne.outlines.grids;

import java.awt.Point;
import java.awt.Rectangle;

import swa.ariadne.outlines.ExplicitOutlineShape;
import swa.ariadne.outlines.OutlineShapeParameters;

/**
 * A pattern of set and cleared squares that can be applied to an {@link ExplicitOutlineShape}.
 * A repeating pattern may be formed by applying the same {@link GridTile} in many cells of a {@link Grid}.
 * <p>
 * Note: The BoundingBox of a tile pattern may extend over the tile boundary.
 *
 * @author Stephan.Wacker@web.de
 */
abstract
class GridTile
implements IGrid
{
    //--------------------- Member variables and Properties

    /** Width of the tile. */
    private final int width;
    /** Height of the tile. */
    private final int height;

    /**
     * When false, all tiles are painted the same way.
     * <br> When true, the tile in every second grid cell is inverted,
     * i.e. the meaning of "inside" and "outside" is swapped.
     */
    private final boolean invertEveryOtherTile;

    /** Scaling factor by which the basic pattern is enlarged. */
    private int scale = 1;

    /**
     *  @return The {@link #scale} factor.
     */
    public int getScale()
    {
        return this.scale;
    }

    /** A translation used when this tile
     * is {@linkplain #Apply(ExplicitOutlineShape, Point, boolean) applied}. */
    private final Point offset = new Point(0, 0);

    /**
     * @return The area where the pattern is actually applied.
     * <br> This area is not necessarily confined to the tile area.
     * <p>
     * <emNote:</em> The {@link #offset} is not taken into account.
     */
    /*default*/ Rectangle BoundingBox()
    {
        return new Rectangle(0, 0, Width(), Height());
    }

    //--------------------- Abstract methods

    /**
     * @param x X coordinate, relative to tile's top left corner.
     * @param y Y coordinate, relative to tile's top left corner.
     * @return True when the given square is set.
     */
    protected abstract boolean get(int x, int y);

    //--------------------- Constructors

    /**
     * Constructor.
     * @param width Width of the tile.
     * @param height Height of the tile.
     * @param invertEveryOtherTile When true, the tile in every second grid cell will be inverted.
     */
    public GridTile(int width, int height, boolean invertEveryOtherTile)
    {
        this.width = width;
        this.height = height;
        this.invertEveryOtherTile = invertEveryOtherTile;
    }

    /** Sets the {@link #scale} factor.
     * @param value A positive integer.
     * @return This object.
     */
    public GridTile setScale(int value)
    {
        this.scale = Math.max(1, value);
        return this;
    }

    /**
     * Calculates a {@link #scale} factor and {@link #offset} from the given parameters
     * The resulting tile will have the desired shape size and location.
     * @param params Characteristic parameters of the generated {@link GridOutlineShape}:
     * location and size.
     * @return This object.
     */
    public GridTile setScaleAndOffset(OutlineShapeParameters params)
    {
        double sz = params.getSize();
        this.setScale((int)Math.min(2 * sz / this.Width(), 2 * sz / this.Height()));

        // Adjust the location where the result's baseShape should be applied.
        this.offset.x = (int)(params.getCenter().x - 0.5 * this.Width());
        this.offset.y = (int)(params.getCenter().y - 0.5 * this.Height());

        return this;
    }

    //--------------------- IGrid implementation

    @Override
    public int Width()
    {
        return this.scale * this.width;
    }

    @Override
    public int Height()
    {
        return this.scale * this.height;
    }

    @Override
    public final void Apply(ExplicitOutlineShape target, Point at, boolean inInvertedCell)
    {
        boolean b = (invertEveryOtherTile ? !inInvertedCell : true);

        Rectangle bbox = this.BoundingBox();
        for (int x = bbox.x; x < bbox.x + bbox.width; x++)
        {
            for (int y = 0; y < bbox.y + bbox.height; y++)
            {
                if (this.get(x, y) == b)
                {
                    target.invert(x + at.x + offset.x, y + at.y + offset.y);
                }
            }
        }
    }
}
