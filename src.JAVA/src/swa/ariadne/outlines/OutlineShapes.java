package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.ariadne.outlines.bitmaps.BitmapOutlineShape;
import swa.ariadne.outlines.functions.FunctionOutlineShape;
import swa.ariadne.outlines.grids.GridOutlineShape;
import swa.ariadne.outlines.tiles.TilesOutlineShape;

/**
 * Provides constructors for all kinds of {@link OutlineShape} objects.
 * <p>
 * The {@link OutlineShapeFactory} has a collection of these constructors.
 */
final
class OutlineShapes
{
    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain CircleOutlineShape circle} shapes.
     */
    public static OutlineShapeConstructor Circle()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new CircleOutlineShape(size, params);
            }

            @Override
            protected int variants()
            {
                return 7;
            }

            @Override
            protected int attractivity()
            {
                return 12;
            }

            @Override
            protected int recognition()
            {
                return 3;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain DiamondOutlineShape diamond} shapes.
     */
    public static OutlineShapeConstructor Diamond()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new DiamondOutlineShape(size, params);
            }

            @Override
            protected int variants()
            {
                return 1;
            }

            @Override
            protected int attractivity()
            {
                return 5;
            }

            @Override
            protected int recognition()
            {
                return 4;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain PolygonOutlineShape polygon} shapes.
     */
    public static OutlineShapeConstructor Polygon()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new PolygonOutlineShape(r, size, params);
            }

            @Override
            protected int variants()
            {
                return (2 * (10 + 8 + 6 + 4 + 2));
            }

            @Override
            protected int attractivity()
            {
                return 7;
            }

            @Override
            protected int recognition()
            {
                return 3;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain FunctionOutlineShape geometric function} shapes.
     */
    public static OutlineShapeConstructor Function()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new FunctionOutlineShape(r, size, params);
            }

            @Override
            protected int variants()
            {
                return (3 * 8 + 2);
            }

            @Override
            protected int attractivity()
            {
                return 12;
            }

            @Override
            protected int recognition()
            {
                return 2;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain CharacterOutlineShape regular character} shapes.
     */
    public static OutlineShapeConstructor Character()
    {
        // TODO Auto-generated method stub
        return null;
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain SymbolOutlineShape symbol character} shapes.
     */
    public static OutlineShapeConstructor Symbol()
    {
        // TODO Auto-generated method stub
        return null;
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain BitmapOutlineShape bitmap image} shapes.
     */
    public static OutlineShapeConstructor Bitmap()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new BitmapOutlineShape(r, size, params);
            }

            @Override
            protected int variants()
            {
                return 25;
            }

            @Override
            protected int attractivity()
            {
                return 15;
            }

            @Override
            protected int recognition()
            {
                return 1;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain TilesOutlineShape tiled} shapes.
     */
    public static OutlineShapeConstructor Tiles()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new TilesOutlineShape(r, size);
            }

            @Override
            protected int variants()
            {
                return 8;
            }

            @Override
            protected int attractivity()
            {
                return 12;
            }

            @Override
            protected int recognition()
            {
                return 3;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain RectanglesOutlineShape rectangular} shapes.
     */
    public static OutlineShapeConstructor Rectangles()
    {
        // TODO Auto-generated method stub
        return null;
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain GridOutlineShape grid} shapes.
     */
    public static OutlineShapeConstructor Grid()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new GridOutlineShape(r, size);
            }

            @Override
            protected int variants()
            {
                return 7;
            }

            @Override
            protected int attractivity()
            {
                return 10;
            }

            @Override
            protected int recognition()
            {
                return 2;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain GridOutlineShape single grid tile} shapes.
     */
    public static OutlineShapeConstructor GridElement()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new GridOutlineShape(r, size, params);
            }

            @Override
            protected int variants()
            {
                return 7;
            }

            @Override
            protected int attractivity()
            {
                return 8;
            }

            @Override
            protected int recognition()
            {
                return 2;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain MazeOutlineShape maze} shapes.
     */
    public static OutlineShapeConstructor Maze()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new MazeOutlineShape(r, size);
            }

            @Override
            protected int variants()
            {
                return 2;
            }

            @Override
            protected int attractivity()
            {
                return 20;
            }

            @Override
            protected int recognition()
            {
                return 1;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain CirclesOutlineShape multiple circles} shapes.
     */
    public static OutlineShapeConstructor Circles()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new CirclesOutlineShape(r, size, params);
            }

            @Override
            protected int variants()
            {
                return 1;
            }

            @Override
            protected int attractivity()
            {
                return 12;
            }

            @Override
            protected int recognition()
            {
                return 2;
            }
        };
    }

    /**
     * @return A {@linkplain OutlineShapeConstructor constructor}
     * for {@linkplain LinesOutlineShape multiple lines} shapes.
     */
    public static OutlineShapeConstructor Lines()
    {
        return new OutlineShapeConstructor()
        {
            @Override
            public OutlineShape create(Random r, Dimension size, OutlineShapeParameters params)
            {
                return new LinesOutlineShape(r, size);
            }

            @Override
            protected int variants()
            {
                return 1;
            }

            @Override
            protected int attractivity()
            {
                return 8;
            }

            @Override
            protected int recognition()
            {
                return 2;
            }
        };
    }
}
