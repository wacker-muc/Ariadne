package swa.ariadne.gui.maze;

import java.awt.Dimension;
import java.awt.Point;
import java.awt.Rectangle;
import java.util.Random;

import swa.ariadne.model.MazeCode;
import swa.ariadne.model.MazeDimensions;
import swa.ariadne.model.MazeSquare;

/**
 * Contains the dimensions of the maze constituents: walls, squares, paths.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class MazeGeometry
{
    //--------------------- Constants

    /** Minimum grid width: {@value}. */
    public static final int MinGridWidth = 2;
    /** Maximum grid width: {@value}. */
    public static final int MaxGridWidth = 40;

    /** Minimum grid width when using automatic settings: {@value}. */
    static final int MinAutoGridWidth = 6;
    /** Maximum grid width when using automatic settings: {@value}. */
    static final int MaxAutoGridWidth = 12;

    /** Minimum grid width when using automatic settings without walls: {@value}. */
    static final int MinAutoGridWidthWithoutWalls = 4;
    /** Maximum grid width when using automatic settings without walls: {@value}. */
    static final int MaxAutoGridWidthWithoutWalls = 9;

    /** Minimum square width: {@value}. */
    public static final int MinSquareWidth = 1;
    /** Maximum square width: {@value}. */
    public static final int MaxSquareWidth = MaxGridWidth - 1;

    /** Minimum path width: {@value}. */
    public static final int MinPathWidth = 1;
    /** Maximum path width: {@value}. */
    public static final int MaxPathWidth = MaxSquareWidth;

    /** Minimum wall width: {@value}. */
    public static final int MinWallWidth = 1;
    /** Maximum wall width: {@value}. */
    public static final int MaxWallWidth = MaxGridWidth / 2;

    //--------------------- Member Variables and Properties
    
    /** Size of a maze square, between two walls. */
    public int squareWidth;
    /** Width of a wall. */
    public int wallWidth = -1;
    /** Size of the maze grid, gridWidth = squareWidth + wallWidth. */
    public int gridWidth;
    /** Width of the painted path. */
    public int pathWidth;
    
    /** Location of the top left square, in graphics coordinates. */
    public /* TODO: private */ Point offset = new Point();
    
    /**
     * @param x A square's X maze coordinate.
     * @return The square's left X coordinate.
     */
    public int getSquareX(int x)
    {
        return offset.x + x * gridWidth;
    }
    
    /**
     * @param y A square's Y maze coordinate.
     * @return The square's top Y coordinate.
     */
    public int getSquareY(int y)
    {
        return offset.y + y * gridWidth;
    }
    
    /**
     * @param sq A square in the maze.
     * @return The square's top left position.
     */
    public Point getSquarePosition(MazeSquare sq)
    {
        return new Point(getSquareX(sq.getXPos()), getSquareY(sq.getYPos()));
    }
    
    /**
     * @param x A square's X maze coordinate.
     * @return The square's left wall's left X coordinate.
     */
    public int getWallX(int x)
    {
        return getSquareX(x) - wallWidth;
    }
    
    /**
     * @param y A square's Y maze coordinate.
     * @return The square's left wall's left Y coordinate.
     */
    public int getWallY(int y)
    {
        return getSquareY(y) - wallWidth;
    }
    
    /**
     * @param sq A square in the maze.
     * @return The square's top left wall's top left position.
     */
    public Point getWallPosition(MazeSquare sq)
    {
        return new Point(getSquareX(sq.getXPos()), getSquareY(sq.getYPos()));
    }
    
    /**
     * @param x A square's X maze coordinate.
     * @return The square's path's left X coordinate.
     */
    public int getPathX(int x)
    {
        return getSquareX(x) + (squareWidth - pathWidth) / 2;
    }
    
    /**
     * @param y A square's Y maze coordinate.
     * @return The square's path's left Y coordinate.
     */
    public int getPathY(int y)
    {
        return getSquareY(y) + (squareWidth - pathWidth) / 2;
    }
    
    /**
     * @param sq A square in the maze.
     * @return The square's path's top left position.
     */
    public Point getPathPosition(MazeSquare sq)
    {
        return new Point(getPathX(sq.getXPos()), getPathY(sq.getYPos()));
    }

    /**
     * Sets our {@link #offset} so that the maze is centered within the given rectangle. 
     * @param targetRect The rectangle into which the maze will be painted.
     * @param mazeSize Size of the painted maze.
     */
    public void setOffset(Rectangle targetRect, Dimension mazeSize)
    {
        offset.x = targetRect.x + (targetRect.width - mazeSize.width * gridWidth + 1 * wallWidth) / 2;
        offset.y = targetRect.y + (targetRect.height - mazeSize.height * gridWidth + 1 * wallWidth) / 2;
        System.out.println("setOffset: " + offset);
    }

    /**
     * Sets our {@link #offset} so that the maze is centered within a rectangle of the given size. 
     * @param targetSize The size of the drawing area at location (0, 0).
     * @param mazeSize Size of the painted maze.
     */
    public void setOffset(Dimension targetSize, Dimension mazeSize)
    {
        offset.x = 0 + (targetSize.width - mazeSize.width * gridWidth + 1 * wallWidth) / 2;
        offset.y = 0 + (targetSize.height - mazeSize.height * gridWidth + 1 * wallWidth) / 2;
        System.out.println("setOffset: " + offset);
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param squareWidth Size of a maze square, between two walls.
     * @param wallWidth Width of a wall.
     * @param pathWidth Width of the painted path.
     */
    public MazeGeometry(int squareWidth, int wallWidth, int pathWidth)
    {
        this.squareWidth = squareWidth;
        this.wallWidth = wallWidth;
        this.gridWidth = squareWidth + wallWidth;
        this.pathWidth = pathWidth;
        adjustPathWidth();
    }

    /**
     * Constructor.
     * Derive reasonable parameter values from the given gridWidth.
     * @param gridWidth The given grid width.
     * @param visibleWalls Whether the maze walls are painted or not (wallWidth = 0). 
     */
    public MazeGeometry(int gridWidth, boolean visibleWalls)
    {
        this.gridWidth = gridWidth;
        
        if (visibleWalls)
        {
            this.wallWidth = Math.max(MinWallWidth, Math.min(MaxWallWidth, (int)(0.3 * gridWidth)));
            this.squareWidth = gridWidth - wallWidth;
            this.pathWidth = (int)(0.7 * squareWidth);
        }
        else
        {
            this.wallWidth = 0;
            this.squareWidth = gridWidth - wallWidth;
            this.pathWidth = (int)(0.75 * squareWidth);
        }

        adjustPathWidth();
    }

    /**
     * Make (squareWidth - pathWidth) an even number.
     * That will make sure that the path is centered nicely between the walls.
     */
    private void adjustPathWidth()
    {
        if (wallWidth > 0 && (squareWidth - pathWidth) % 2 != 0)
        {
            pathWidth -= 1;
        }
        if (pathWidth < 2)
        {
            pathWidth = squareWidth;
        }
    }

    //--------------------- Auxiliary Methods

    @Override
    public String toString()
    {
        return String.format(getClass().getName() + "[gw=%d, ww=%d, sw=%d, pw=%d @ (%d, %d)]",
                gridWidth, wallWidth, squareWidth, pathWidth, offset.x, offset.y);
    }
    
    //--------------------- Static Methods

    /**
     * @param r A source of random numbers.
     * @param visibleWalls Whether the maze walls are painted or not (wallWidth = 0).
     * @param targetRectangle The canvas geometry.
     * @return A random grid width between the constant minimum and maximum values.
     */
    static int suggestGridWidth(Random r, boolean visibleWalls, Rectangle targetRectangle)
    {
        int minWidth = (visibleWalls ? MinAutoGridWidth : MinAutoGridWidthWithoutWalls);
        int maxWidth = (visibleWalls ? MaxAutoGridWidth : MaxAutoGridWidthWithoutWalls);

        /* TODO: Use a larger grid width for the first maze.
        if (this.wallWidth < 0)
        {
            minWidth = (minWidth + maxWidth) / 2;
        }
        */

        int result = minWidth + r.nextInt(maxWidth - minWidth);

        // Make sure we do not exceed the maximally allowed dimensions.
        MazeDimensions dim = MazeDimensions.getInstance(MazeCode.DefaultCodeVersion);
        while (targetRectangle.width / result > dim.getMaxXSize() || targetRectangle.height / result > dim.getMaxYSize())
        {
            ++result;
        }

        return result;
    }

    //--------------------- Coordinate System
    
    /**
     * @param xCanvas 
     * @param leftBiased 
     * @return The maze X coordinate corresponding to the given canvas coordinate.
     */
    public int xCoordinate(int xCanvas, boolean leftBiased)
    {
        int result = xCanvas - offset.x;
        result += (leftBiased ? 0 : +1) * wallWidth;
        result /= gridWidth;

        return result;
    }
    
    /**
     * @param yCanvas 
     * @param topBiased 
     * @return The maze Y coordinate corresponding to the given canvas coordinate.
     */
    public int yCoordinate(int yCanvas, boolean topBiased)
    {
        int result = yCanvas - offset.y;
        result += (topBiased ? 0 : +1) * wallWidth;
        result /= gridWidth;

        return result;
    }
}
