package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.Picker;

/**
 * Knows all OutlineShapes and can create instances of them.
 * @see #createShape(Random, Dimension, double, double)
 * @see #createShape(Random, OutlineShapeConstructor, Dimension, double, double)
 *
 * @author Stephan.Wacker@web.de
 */
public final
class OutlineShapeFactory
{
    //--------------------- Static variables

    /**
     *  A collection of all available {@linkplain OutlineShapeConstructor OutlineShape constructors}.
     */
    private static OutlineShapeConstructor[] constructors =
    {
        OutlineShapes.Circle(),
        OutlineShapes.Diamond(),
        OutlineShapes.Polygon(),
        OutlineShapes.Function(),
        OutlineShapes.Character(),
        OutlineShapes.Symbol(),
        OutlineShapes.Bitmap(),
        OutlineShapes.Tiles(),
        OutlineShapes.Rectangles(),
        OutlineShapes.Grid(),
        OutlineShapes.GridElement(),
        OutlineShapes.Maze(),
        OutlineShapes.Circles(),
        OutlineShapes.Lines(),
    };

    //--------------------- Static methods for creating an OutlineShape

    /**
     * Creates an OutlineShape object of a randomly chosen Type.
     * If applicable, the shape may be distorted.
     *
     * @param r A source of random numbers.
     * @param mazeSize The size of the maze that the shape should fit in.
     * @param offCenter The (maximum) relative distance from the center.
     * @param shapeSize The relative size; 1.0 will touch the border.
     * @return An OutlineShape object.
     *
     * @see OutlineShapeParameters#OutlineShapeParameters(double, double, double)
     */
    public static OutlineShape createShape(Random r, Dimension mazeSize, double offCenter, double shapeSize)
    {
        OutlineShapeConstructor constructor = new Picker<OutlineShapeConstructor>(constructors).pick(r);
        OutlineShape result = createShape(r, constructor, mazeSize, offCenter, shapeSize);

        // If applicable, replace the shape with a distorted version.
        if (r.nextInt(100) < result.getDistortedPercentage(33))
        {
            result = result.makeDistortedCopy(r);
        }

        return result;
    }

    /**
     * Creates an OutlineShape object of the given Type.
     * This shape will not be distorted.
     *
     * @param r A source of random numbers.
     * @param constructor A specific {@link OutlineShapeConstructor}.
     * @param mazeSize The size of the maze that the shape should fit in.
     * @param offCenter The (maximum) relative distance from the center.
     * @param shapeSize The relative size; 1.0 will touch the border.
     * @return An OutlineShape object.
     *
     * @see OutlineShapeParameters#OutlineShapeParameters(double, double, double)
     */
    public static OutlineShape createShape(Random r, OutlineShapeConstructor constructor, Dimension mazeSize, double offCenter, double shapeSize)
    {
        if (constructor == null) // TODO: remove this code
        {
            constructor = OutlineShapes.Circle();
        }

        double centerX = 0.5, centerY = 0.5;

        double dx = r.nextDouble() - 0.5, dy = r.nextDouble() - 0.5;
        centerX += offCenter * dx;
        centerY += offCenter * dy;

        // Reduce size when we are closer to the center than requested.
        double f = 1.0 - offCenter * 2.0 * (0.5 - Math.max(Math.abs(dx), Math.abs(dy)));

        OutlineShapeParameters params = new OutlineShapeParameters(centerX, centerY, f * shapeSize);
        OutlineShape result = constructor.create(r, mazeSize, params);

        return result;
    }
}
