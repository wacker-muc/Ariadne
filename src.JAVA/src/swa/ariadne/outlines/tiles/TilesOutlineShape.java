package swa.ariadne.outlines.tiles;

import java.awt.Dimension;
import java.awt.image.BufferedImage;
import java.util.Random;

import swa.ariadne.outlines.ExplicitOutlineShape;
import swa.ariadne.outlines.OutlineShape;
import swa.util.ImageResourceReader;

/**
 * Short introduction sentence. More description afterwards.
 */
public final class TilesOutlineShape extends OutlineShape
{
    //--------------------- Member variables and Properties

    /** The pattern is stored in an ExplicitOutlineShape. */
    private final ExplicitOutlineShape tile;

    /**
     * Each column in the tile can be repeated more than once.
     * length: {@link #tile}<code>.getWidth()</code).
     */
    private final int[] xRepetitions;

    /**
     * Each line in the tile can be repeated more than once.
     * length: {@link #tile}<code>.getHeight()</code).
     */
    private final int[] yRepetitions;

    /** The effective tile size, resulting from the repetitions. */
    private final Dimension tileSize = new Dimension();

    /**
     * Mapping of shape coordinates to tile coordinates.
     * length: {@link #tileSize}<code>.width</code>.
     */
    private int[] xMap;

    /**
     * Mapping of shape coordinates to tile coordinates.
     * length: {@link #tileSize}<code>.height</code>.
     */
    private int[] yMap;

    /**
     * Sets the value at the given location.
     * @param x Point's X coordinate.
     * @param y Point's Y coordinate.
     * @param value True for the inside, false for the outside.
     */
    public void set(int x, int y, boolean value)
    {
        tile.set(x, y, value);
    }

    //--------------------- OutlineShape implementation

    @Override
    public boolean get(int x, int y)
    {
        return tile.get(xMap[x % tileSize.width], yMap[y % tileSize.height]);
    }

    //--------------------- Constructors

    /**
     * Constructor used by the {@link swa.ariadne.outlines.OutlineShapeFactory OutlineShapeFactory}.
     * Chooses one of the {@linkplain #chooseBitmap(Random) available bitmaps}}.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     */
    public TilesOutlineShape(Random r, Dimension size)
    {
        // TODO: add other variants of tiles, using a TilePainter
        this(chooseBitmap(r), size);
    }

    /**
     * Constructor.
     * @param bitmap A black and white bitmap image that defines the tile.
     * @param size Nominal size of the shape.
     */
    private TilesOutlineShape(BufferedImage bitmap, Dimension size)
    {
        // TODO: rotate or flip the shape
        this(size, new ExplicitOutlineShape(bitmap));
    }

    /**
     * @param size
     * @param tile
     */
    public TilesOutlineShape(Dimension size, ExplicitOutlineShape tile)
    {
        super(size);

        this.tile = tile;
        this.xRepetitions = new int[tile.getWidth()];
        this.yRepetitions = new int[tile.getHeight()];

        for (int i = 0; i < xRepetitions.length; i++)
        {
            xRepetitions[i] = 1;
        }
        for (int i = 0; i < yRepetitions.length; i++)
        {
            yRepetitions[i] = 1;
        }

        UpdateTileSize();
    }

    //--------------------- Auxiliary methods

    /**
     *
     */
    private void UpdateTileSize()
    {
        tileSize.width = tileSize.height = 0;

        for (int i = 0; i < xRepetitions.length; i++)
        {
            tileSize.width += xRepetitions[i];
        }
        for (int i = 0; i < yRepetitions.length; i++)
        {
            tileSize.height += yRepetitions[i];
        }

        UpdateCoordinateMapping();
    }

    /**
     * Updates the {@link #xMap} and {@link #yMap} coordinate mappings.
     * Is called whenever the repetition numbers are changed.
     */
    private void UpdateCoordinateMapping()
    {
        this.xMap = new int[tileSize.width];
        this.yMap = new int[tileSize.height];

        // Determine the origin of a tile centered in the drawing area.
        int x0 = (this.getWidth() - tileSize.width) / 2, y0 = (this.getHeight() - tileSize.height) / 2;

        // Make sure x0 and y0 are positive.
        if (x0 < 0)
        {
            x0 += tileSize.width;
        }
        if (y0 < 0)
        {
            y0 += tileSize.height;
        }

        // [start] Derive the xMap and yMap of a (wrapped) tile in the top left corner of the shape.

        for (int xt = 0, xm = 0; xt < tile.getWidth(); xt++)
        {
            for (int i = 0; i < xRepetitions[xt]; i++, xm++)
            {
                xMap[(xm + x0) % tileSize.width] = xt;
            }
        }
        for (int yt = 0, ym = 0; yt < tile.getHeight(); yt++)
        {
            for (int i = 0; i < yRepetitions[yt]; i++, ym++)
            {
                yMap[(ym + y0) % tileSize.height] = yt;
            }
        }

        // [end]
    }

    //--------------------- Static methods for creating a bitmap image

    /** The {@link ImageResourceReader} for this package. */
    static final ImageResourceReader resourceReader = new ImageResourceReader(TilesOutlineShape.class, "resources.txt");

    /**
     * @param r A source of random numbers.
     * @return One of the available bitmaps.
     */
    private static BufferedImage chooseBitmap(Random r)
    {
        return resourceReader.pickOne(r);
    }
}
