package swa.ariadne.outlines;

import java.awt.Dimension;
import java.awt.image.BufferedImage;
import java.util.Random;

import swa.util.Bitmap;

/**
 * An {@link OutlineShape} that is based on a two dimensional array of boolean values.
 * The values can be set and cleared explicitly.
 *
 * @author Stephan.Wacker@web.de
 */
public
class ExplicitOutlineShape
extends OutlineShape
{
    //--------------------- Member variables and Properties

    /**
     * Explicit contents of this shape.
     * Non-zero values define the inside of the shape.
     */
    private final byte[][] squares;

    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(int x, int y)
    {
        if (0 <= x && x < this.getWidth() && 0 <= y && y < this.getHeight())
        {
            return (squares[x][y] != 0);
        }
        else
        {
            return false;
        }
    }

    /**
     * Sets the value at the given location.
     * @param x Point's X coordinate.
     * @param y Point's Y coordinate.
     * @param value True for the inside, false for the outside.
     */
    public void set(int x, int y, boolean value)
    {
        if (0 <= x && x < this.getWidth() && 0 <= y && y < this.getHeight())
        {
            this.squares[x][y] = (value == true ? (byte)1 : (byte)0);
        }
    }

    /**
     * Inverts the value at the given location.
     * @param x Point's X coordinate.
     * @param y Point's Y coordinate.
     */
    public void invert(int x, int y)
    {
        this.set(x, y, !this.get(x, y));
    }

    //--------------------- Constructors

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     */
    public ExplicitOutlineShape(Dimension size)
    {
        super(size);

        this.squares = new byte[getWidth()][getHeight()];
    }

    /**
     * Constructor.
     * @param template A shape that is copied.
     */
    public ExplicitOutlineShape(IOutlineShape template)
    {
        this(template, null);
    }

    /**
     * Constructor.
     * @param template A shape that is copied.
     * @param reserved Defines the reserved areas which will not become part of the constructed shape.
     */
    public ExplicitOutlineShape(IOutlineShape template, IOutlineShape reserved)
    {
        this(template.getSize());

        for (int x = 0; x < this.getWidth(); x++)
        {
            for (int y = 0; y < this.getHeight(); y++)
            {
                this.set(x, y, (template.get(x, y) && (reserved == null || !reserved.get(x, y))));
            }
        }
    }

    /**
     * @param template A black and white bitmap image that defines the shape.
     */
    public ExplicitOutlineShape(BufferedImage template)
    {
        this(new Dimension(template.getWidth(), template.getHeight()));

        Random random = swa.util.RandomFactory.createRandom();
        Bitmap bitmap = new Bitmap(template);

        for (int x = 0; x < this.getWidth(); x++)
        {
            for (int y = 0; y < this.getHeight(); y++)
            {
                // black -> true, white -> false, gray -> (true | false)
                // 0/3 .. 1/3 .. 2/3 .. 3/3
                switch ((int)Math.floor(bitmap.getBrightness(x, y) * 0.999 * 3))
                {
                    case 0:
                        this.set(x, y, true);
                        break;
                    case 1:
                        this.set(x, y, random.nextInt(2) == 0);
                        break;
                    case 2:
                        this.set(x, y, false);
                        break;
                }
            }
        }
    }

    //--------------------- OutlineShape implementation

    /**
     * @param template A shape that is copied.
     * @param reserved Defines the reserved areas which will not become part of the constructed shape.
     * @return The largest subset of the template shape whose squares are all connected to each other.
     * @see OutlineShape#makeConnectedSubset(IOutlineShape)
     */
    public static OutlineShape makeConnectedSubset(IOutlineShape template, IOutlineShape reserved)
    {
        ExplicitOutlineShape result = new ExplicitOutlineShape(template, reserved);

        //--------------------- Scan the shape for connected areas.

        byte subsetId = 1;
        int largestAreaSize = 0;
        byte largestAreaId = 0;

        for (int x = 0; x < result.getWidth(); x++)
        {
            for (int y = 0; y < result.getHeight(); y++)
            {
                if (result.squares[x][y] == 1 && subsetId < Byte.MAX_VALUE)
                {
                    int areaSize = result.fillSubset(x, y, ++subsetId);
                    if (areaSize > largestAreaSize)
                    {
                        largestAreaSize = areaSize;
                        largestAreaId = subsetId;
                    }
                }
            }
        }

        //--------------------- Leave only the largest subset, eliminate all others.

        for (int x = 0; x < result.getWidth(); x++)
        {
            for (int y = 0; y < result.getHeight(); y++)
            {
                result.set(x, y, (result.squares[x][y] == largestAreaId));
            }
        }

        return result;
    }

    /**
     * @param template A shape that is copied.
     * @param reserved Defines the reserved areas which will not become part of the constructed shape.
     * @return The template shape, augmented by all totally enclosed areas.
     * @see OutlineShape#makeClosure()
     */
    public static OutlineShape makeClosure(OutlineShape template, IOutlineShape reserved)
    {
        ExplicitOutlineShape result = new ExplicitOutlineShape(template.makeInverse());

        //--------------------- Scan and mark the reserved areas.

        if (reserved != null)
        {
            byte reservedId = 3;

            for (int x = 0; x < result.getWidth(); x++)
            {
                for (int y = 0; y < result.getHeight(); y++)
                {
                    if (reserved.get(x, y))
                    {
                        result.squares[x][y] = reservedId;
                    }
                }
            }
        }

        //--------------------- Scan all outside areas.

        byte outsideId = 2;
        int x0 = 0, x1 = result.getWidth() - 1, y0 = 0, y1 = result.getHeight() - 1;

        for (int x = 0; x < result.getWidth(); x++)
        {
            if (result.squares[x][y0] == 1)
            {
                result.fillSubset(x, y0, outsideId);
            }
            if (result.squares[x][y1] == 1)
            {
                result.fillSubset(x, y1, outsideId);
            }
        }
        for (int y = 0; y < result.getHeight(); y++)
        {
            if (result.squares[x0][y] == 1)
            {
                result.fillSubset(x0, y, outsideId);
            }
            if (result.squares[x1][y] == 1)
            {
                result.fillSubset(x1, y, outsideId);
            }
        }

        //--------------------- Add the areas which were not reached.

        for (int x = 0; x < result.getWidth(); x++)
        {
            for (int y = 0; y < result.getHeight(); y++)
            {
                // 0: square is part of the template (not part of its inverse)
                // 1: square is not part of the template, but was not reached
                result.set(x, y, (result.squares[x][y] <= 1));
            }
        }

        return result;
    }

    /**
     * Mark all squares connected to (x, y) with the given ID.
     * @param xStart X coordinate.
     * @param yStart Y coordinate.
     * @param id A unique subset id greater than 1.
     * @return Number of squares in the subset.
     */
    private int fillSubset(int xStart, int yStart, byte id)
    {
        int x = xStart, y = yStart;
        int result = 0;

        // Use an array of (x, y) coordinates large enough to add all squares of the shape.
        // Only squares with value == 1 are added, the value is then changed to the given ID.
        // Thus, no square will be added twice.
        int[] xp = new int[getWidth() * getHeight()], yp = new int[getWidth() * getHeight()];
        int k = 0, n = 0;

        // Add the given square to the list.
        if (squares[x][y] == 1)
        {
            xp[n] = x; yp[n] = y; n++;
            squares[x][y] = id; result++;
        }

        // Scan all squares in the list.
        while (k < n)
        {
            x = xp[k]; y = yp[k]; k++;

            if (x > 0 && squares[x - 1][y] == 1)
            {
                xp[n] = x - 1; yp[n] = y; n++;
                squares[x - 1][y] = id; result++;
            }
            if (x + 1 < getWidth() && squares[x + 1][y] == 1)
            {
                xp[n] = x + 1; yp[n] = y; n++;
                squares[x + 1][y] = id; result++;
            }
            if (y > 0 && squares[x][y - 1] == 1)
            {
                xp[n] = x; yp[n] = y - 1; n++;
                squares[x][y - 1] = id; result++;
            }
            if (y + 1 < getHeight() && squares[x][y + 1] == 1)
            {
                xp[n] = x; yp[n] = y + 1; n++;
                squares[x][y + 1] = id; result++;
            }
        }

        return result;
    }
}
