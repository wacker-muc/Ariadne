package swa.ariadne.outlines.factory;

import java.awt.Dimension;
import java.lang.reflect.Type;
import java.util.Random;

import swa.ariadne.outlines.CircleOutlineShape;
import swa.ariadne.outlines.CirclesOutlineShape;
import swa.ariadne.outlines.OutlineShape;
import swa.ariadne.outlines.OutlineShapeParameters;

/**
 * Knows all OutlineShapes and can create instances of them.
 * @see #createShape(Random, Dimension, double, double)
 * @see #createShape(Random, Type, Dimension, double, double)
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class OutlineShapeFactory
{
    //--------------------- Types
    
    /**
     * Meta-data for the selection and creation of OutlineShape objects.
     */
    private static final class ShapeInfo
    {
        /** An OutlineShape subclass. */
        Type type;
        
        /** Relative frequency for choosing this shape type. */
        int ratio = 1;
        
        /**
         * Constructor.
         * @param type An OutlineShape subclass.
         */
        public ShapeInfo(Type type)
        {
            this.type = type;
            // TODO: Verify that type can be instantiated.
            // TODO: Get ratio from an annotation.
        }
    }
    
    //--------------------- Static Member variables and Properties

    /** Meta-data for all known OutlineShapes. */
    private static ShapeInfo[] shapeInfos =
    {
        new ShapeInfo(CirclesOutlineShape.class),
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
        int p = r.nextInt(shapeInfos.length);
        ShapeInfo info = shapeInfos[p];
        
        OutlineShape result = createShape(r, info.type, mazeSize, offCenter, shapeSize);

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
     * @param type A specific OutlineShape subclass. 
     * @param mazeSize The size of the maze that the shape should fit in.
     * @param offCenter The (maximum) relative distance from the center.
     * @param shapeSize The relative size; 1.0 will touch the border.
     * @return An OutlineShape object.
     * 
     * @see OutlineShapeParameters#OutlineShapeParameters(double, double, double)
     */
    public static OutlineShape createShape(Random r, Type type, Dimension mazeSize, double offCenter, double shapeSize)
    {
        OutlineShape result;

        double centerX = 0.5, centerY = 0.5;

        double dx = r.nextDouble() - 0.5, dy = r.nextDouble() - 0.5;
        centerX += offCenter * dx;
        centerY += offCenter * dy;

        // Reduce size when we are closer to the center than requested.
        double f = 1.0 - offCenter * 2.0 * (0.5 - Math.max(Math.abs(dx), Math.abs(dy)));
        
        OutlineShapeParameters params = new OutlineShapeParameters(centerX, centerY, f * shapeSize);

        try
        {
            result = (OutlineShape) type.getClass().getConstructor(Random.class, Dimension.class, OutlineShapeParameters.class).newInstance(r, mazeSize, params);
        }
        catch (Exception e)
        {
            result = new CircleOutlineShape(mazeSize, params);
        }

        return result;
    }
}
