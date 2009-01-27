package swa.ariadne.outlines.grids;

import java.awt.Rectangle;

/**
 * Defines the area of a circle.
 * The circle may be
 * <ul><li>
 * centered in the tile,
 * </li><li>
 * on one of the edges or
 * </li><li>
 * on the corner.
 * </li></ul>
 */
final
class CircleTile extends GridTile
{
    //--------------------- Member variables and Properties

    /** The circle's diameter, in tile coordinates. */
    private final double diameter;
    /** When true, the circle is centered on the vertical tile edge. */
    private final boolean onVerticalEdge;
    /** When true, the circle is centered on the horizontal tile edge. */
    private final boolean onHorizontalEdge;

    /** Center X coordinate, {@linkplain #getScale() scaled}. */
    private double xc;
    /** Center Y coordinate, {@linkplain #getScale() scaled}. */
    private double yc;
    /** The circle's radius, {@linkplain #getScale() scaled}. */
    private double r;

    //--------------------- Constructors

    /**
     * Constructor.
     * @param width Width of the tile.
     * @param height Height of the tile.
     * @param invertEveryOtherTile When true, the tile in every second grid cell will be inverted.
     * @param diameter The circle's diameter, in tile coordinates.
     */
    public CircleTile(int width, int height, boolean invertEveryOtherTile, double diameter)
    {
        this(width, height, invertEveryOtherTile, diameter, false, false);
    }

    /**
     * Constructor.
     * @param width Width of the tile.
     * @param height Height of the tile.
     * @param invertEveryOtherTile When true, the tile in every second grid cell will be inverted.
     * @param diameter The circle's diameter, in tile coordinates.
     * @param onVerticalEdge When true, the circle is centered on the vertical tile edge.
     * @param onHorizontalEdge When true, the circle is centered on the horizontal tile edge.
     */
    public CircleTile(int width, int height, boolean invertEveryOtherTile, double diameter, boolean onVerticalEdge, boolean onHorizontalEdge)
    {
        super(width, height, invertEveryOtherTile);

        this.diameter = diameter;
        this.onVerticalEdge = onVerticalEdge;
        this.onHorizontalEdge = onHorizontalEdge;

        // Initialize the remaining parameters.
        InitializeMembers();
    }

    /**
     * Determine the {@linkplain #getScale() scaled} circle parameters.
     */
    private void InitializeMembers()
    {
        // The relevant coordinates are 0..w-1 and 0..h-1
        this.xc = (onVerticalEdge ? 0.0 : 0.5 * (Width() - 1));
        this.yc = (onHorizontalEdge ? 0.0 : 0.5 * (Height() - 1));

        this.r = 0.5 * getScale() * diameter;

        double kx = (xc + r) - Math.floor(xc + r);
        double ky = (yc + r) - Math.floor(yc + r);
        if (kx < 0.02 || ky < 0.02) // (xc + r) or (yc + r) is very close to an integral number
        {
            // The radius is decreased a bit to get a flat curve instead of a single point.
            r -= 0.02;
        }
    }

    //--------------------- GridTile implementation

    @Override
    protected boolean get(int x, int y)
    {
        double dx = x - xc;
        double dy = y - yc;

        return (dx * dx + dy * dy <= r * r);
    }

    @Override
    protected Rectangle BoundingBox()
    {
        int x0 = (int)Math.floor(xc - r);
        int x1 = (int)Math.ceil(xc + r);
        int y0 = (int)Math.floor(yc - r);
        int y1 = (int)Math.ceil(yc + r);

        return new Rectangle(x0, y0, x1 - x0, y1 - y0);
    }
}
