package swa.ariadne.outlines;

import java.awt.Dimension;
import java.awt.Rectangle;
import java.util.Random;

/**
 * An OutlineShape defines a contour line dividing the shape's inside and outside.
 * When used by the MazeBuilder, the walls on the contour line should all be closed
 * (with the exception of a single entry).
 *
 * @author Stephan.Wacker@web.de
 */
public abstract
class OutlineShape
implements IOutlineShape
{
    //--------------------- Member Variables and Properties

    /** Nominal size of the shape. */
    private final Dimension size;

    /** @return Nominal width of the shape. */
    public final int getWidth()
    {
        return this.size.width;
    }

    /** @return Nominal height of the shape. */
    public final int getHeight()
    {
        return this.size.height;
    }

    /** @return Nominal size of the shape. */
    public Dimension getSize()
    {
        return (Dimension) this.size.clone();
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     */
    protected OutlineShape(Dimension size)
    {
        this.size = (Dimension) size.clone();
    }

    //--------------------- OutlineShape implementation

    /**
     * @return A rectangle that tightly includes the actual shape, in shape coordinates.
     * <br>Note: The result is limited to the nominal width and height.
     */
    public Rectangle getBoundingBox()
    {
        int xMin = size.width, xMax = 0, yMin = size.height, yMax = 0;

        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                if (this.get(x, y) == true)
                {
                    xMin = Math.min(xMin, x);
                    xMax = Math.max(xMax, x);
                    yMin = Math.min(yMin, y);
                    yMax = Math.max(yMax, y);
                }
            }
        }

        return new Rectangle(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
    }

    /**
     * @return The number of points that are inside the shape.
     * <br>Note: The result is limited to the nominal width and height.
     */
    public int getArea()
    {
        int result = 0;

        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                if (this.get(x, y) == true)
                {
                    ++result;
                }
            }
        }

        return result;
    }

    //--------------------- Logical operations on an OutlineShape

    /**
     * @param reserved Defines the reserved areas which will not become part of the constructed shape.
     * @return The largest subset of this shape whose squares are all connected to each other.
     */
    public OutlineShape makeConnectedSubset(IOutlineShape reserved)
    {
        return ExplicitOutlineShape.makeConnectedSubset(this, reserved);
    }

    /**
     * @return This shape, augmented by all totally enclosed areas.
     */
    public OutlineShape makeClosure()
    {
        return ExplicitOutlineShape.makeClosure(this, null);
    }

    /**
     * @param reserved Defines the reserved areas which will not become part of the constructed shape.
     * @return This shape, augmented by all totally enclosed areas.
     * Areas enclosed between this shape and the reserved area are added to the closure.
     */
    public OutlineShape makeClosure(IOutlineShape reserved)
    {
        return ExplicitOutlineShape.makeClosure(this, reserved);
    }

    /**
     * @return A shape whose inside and outside are swapped.
     */
    public IOutlineShape makeInverse()
    {
        return new IOutlineShape()
        {
            @Override
            public boolean get(int x, int y)
            {
                return !this.get(x, y);
            }

            @Override
            public Dimension getSize()
            {
                return this.getSize();
            }
        };
    }

    //--------------------- Methods for applying a distortion to the original shape

    /**
     * Subclasses that want to provide distorted versions should override this method.
     * @param r A source of random numbers.
     * @return A {@link DistortedOutlineShape} based on the current shape.
     * If no distortion is applicable, the default implementation returns the current
     * object unmodified.
     */
    public OutlineShape makeDistortedCopy(Random r)
    {
        return this;
    }

    /**
     * @param defaultValue Default value; the result should be at least this much.
     * @return The percentage of instances that should be distorted.
     * <br> The default implementation returns the defaultValue unmodified;
     * other subclasses may choose a different value.
     */
    public int getDistortedPercentage(int defaultValue)
    {
        return defaultValue;
    }

    //--------------------- Auxiliary methods

    @Override
    public String toString()
    {
        return String.format("%s: [%dx%d], bbox = %s, area = %d",
            this.getClass().toString(),
            this.getWidth(), this.getHeight(),
            this.getBoundingBox().toString(),
            this.getArea());
    }
}
