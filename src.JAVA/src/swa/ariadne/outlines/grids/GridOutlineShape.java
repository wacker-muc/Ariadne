package swa.ariadne.outlines.grids;

import java.awt.Dimension;
import java.awt.Point;
import java.util.Random;

import swa.ariadne.outlines.ExplicitOutlineShape;
import swa.ariadne.outlines.OutlineShape;
import swa.ariadne.outlines.OutlineShapeParameters;

/**
 * An {@link OutlineShape} created by repeating a certain geometric element in a grid pattern.
 * Another option is to build a single {@link GridTile} scaled to a desired size.
 *
 * @author Stephan.Wacker@web.de
 */
public final
class GridOutlineShape
extends OutlineShape
{
    //--------------------- Member variables and Properties

    /**
     * An {@link ExplicitOutlineShape}
     * into which the actual {@link GridOutlineShape} is painted.
     */
    private final ExplicitOutlineShape baseShape;

    @Override
    public boolean get(int x, int y)
    {
        return baseShape.get(x, y);
    }

    //--------------------- Constructors

    /**
     * Constructor used by the {@link swa.ariadne.outlines.OutlineShapeFactory OutlineShapeFactory}.
     * Chooses one of the {@linkplain #chooseTile(Random) available tiles}}
     * and applies that tile in a grid pattern.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     */
    public GridOutlineShape(Random r, Dimension size)
    {
        this(size, new Grid(chooseTile(r).setScale(r.nextInt(3) == 0 ? 2 : 1)));
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.OutlineShapeFactory OutlineShapeFactory}.
     * Chooses one of the {@linkplain #chooseTile(Random) available tiles}},
     * scaled and positioned according to the given parameters.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public GridOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        this(chooseTile(r).setScaleAndOffset(params.convertToShapeCoordinates(size)));
    }

    /**
     * Constructor.
     * Creates a {@link GridOutlineShape} of the same size as the given tile.
     * @param gridTile A single {@link GridTile}
     * that completely fills the resulting {@link GridOutlineShape}.
     */
    private GridOutlineShape(GridTile gridTile)
    {
        this(new Dimension(gridTile.Width(), gridTile.Height()), gridTile);
    }

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     * @param gridOrTile Either
     * <ul><li>
     * a {@link Grid} that is applied on the whole shape area or
     * </li><li>
     * a {@link GridTile} that is applied once at the location coded in the tile.
     * </li></ul>
     */
    private GridOutlineShape(Dimension size, IGrid gridOrTile)
    {
        super(size);

        this.baseShape = new ExplicitOutlineShape(size);

        // Paint the given grid or tile at the center of the shape,
        // maybe translated by the tile's offset.
        int xLeft = (getWidth() - gridOrTile.Width()) / 2;
        int yTop = (getHeight() - gridOrTile.Height()) / 2;
        gridOrTile.Apply(baseShape, new Point(xLeft, yTop), false);
    }

    //--------------------- Static methods for creating a GridTile

    /**
     * @param r A source of random numbers.
     * @return One of the (currently 15) defined {@link GridTile} patterns.
     */
    private static GridTile chooseTile(Random r)
    {
        GridTile result;
        int width, height;
        double diameter;

        int choice = r.nextInt(15);
        boolean invertEveryOtherTile = (choice % 2 == 0);

        switch (choice)
        {
            default:
            case 0: // simple checkered squares
                width = 3 + r.nextInt(12 - 3 + 1);

                // A combination of zero basic tiles is like an all-black tile.
                result = new CombinedTile(width, width, true, new GridTile[] {});
                invertEveryOtherTile = true;
                break;

            case 1: // large circles, slightly smaller than the checkered squares
            case 2:
                width = 12 + r.nextInt(24 - 12 + 1);
                diameter = width - 4;

                result = new CircleTile(width, width, invertEveryOtherTile, diameter);
                break;

            case 3: // large and small circles
            case 4:
                width = 12 + r.nextInt(24 - 12 + 1);
                diameter = width - 2;

                result = new CombinedTile(width, width, invertEveryOtherTile, new GridTile[] {
                        new CircleTile(width, width, false, diameter),
                        new CircleTile(width, width, false, 0.5 * diameter)
                });
                break;

            case 4 + 1: // halved or quartered circles
            case 4 + 2:
            case 4 + 3:
                width = height = 12 + r.nextInt(24 - 12 + 1);
                diameter = width - 1;
                boolean onVerticalEdge = ((choice & 1) != 0);
                boolean onHorizontalEdge = ((choice & 2) != 0);
                if (onVerticalEdge && !onHorizontalEdge)
                {
                    width += 2;
                    diameter -= 1;
                }
                else if (onHorizontalEdge && !onVerticalEdge)
                {
                    height += 2;
                    diameter -= 1;
                }

                result = new CircleTile(width, height, true, diameter, onVerticalEdge, onHorizontalEdge);
                break;

            case 8: // large overlapping circles almost touching each other diagonally
                width = 20 + r.nextInt(40 - 20 + 1);
                // The circle should pass exactly between the squares (3,2) and (4,2) in the adjoining tile.
                double x = 0.5 * (width - 1) - 3.0;
                double y = 0.5 * (width - 1) + 1.5;
                diameter = 2.0 * Math.sqrt(x * x + y * y);

                result = new CircleTile(width, width, false, diameter);
                break;

            case 9: // rectangular lines, tightly boxed
            case 10:
                width = 8 + r.nextInt(16 - 8 + 1);
                height = 8 + r.nextInt(16 - 8 + 1);

                result = new TightBoxesTile(width, height, invertEveryOtherTile);
                break;

            case 11: // large circles touching each other
            case 12:
            case 13:
            case 14:
                width = height = 12 + r.nextInt(24 - 12 + 1);
                diameter = width;

                // Make one side approx. 50% longer than the other.
                // The difference should be an even number.
                if ((choice / 2) % 2 == 0)
                {
                    width += 2 * (int)(0.25 * width);
                }
                {
                    height += 2 * (int)(0.25 * height);
                }

                result = new CircleTile(width, height, invertEveryOtherTile, diameter);
                break;
        }

        return result;
    }
}
