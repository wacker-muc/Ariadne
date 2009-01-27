package swa.ariadne.outlines;

import java.awt.Dimension;
import java.awt.Point;
import java.util.Random;

import swa.ariadne.gui.maze.MazeGeometry;
import swa.ariadne.model.Maze;
import swa.ariadne.model.WallPosition;
import swa.ariadne.model.WallState;

/**
 * A shape that is based upon another {@link Maze}.
 * The shape's inside/outside are derived from the maze's squares/walls.
 * There are two variants:
 * <ul><li>
 * Inside and outside have the same width.
 * </li><li>
 * The outside is one square wide.
 * </li></ul>
 *
 * @author Stephan.Wacker@web.de
 */
class MazeOutlineShape
extends OutlineShape
{
    //--------------------- Member Variables and Properties

    /** An {@link ExplicitOutlineShape} into which the maze is "painted". */
    private final ExplicitOutlineShape baseShape;

    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(int x, int y)
    {
        return this.baseShape.get(x, y);
    }

    //--------------------- Constructors

    /**
     * Constructor.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     */
    public MazeOutlineShape (Random r, Dimension size)
    {
        this(size,
                (r.nextInt(2) == 0),        // selects a thin or thick wall variant
                (1 + r.nextInt(3 + 1)),     // a wall width, for thick walls
                (5 + r.nextInt(4 + 1))      // a grid width, for thin walls
                );
    }

    /**
     * Auxiliary Constructor.
     * @param size Nominal size of the shape.
     * @param thinWalls Chooses between two variants: thin or thick walls.
     * @param thickWallWidth Maze wall width, used only when thinWalls = false.
     * @param thinGridWidth Maze grid width, used only when thinWalls = true.
     */
    private MazeOutlineShape(Dimension size, boolean thinWalls, int thickWallWidth, int thinGridWidth)
    {
        this(size, (thinWalls ? 1 : thickWallWidth), (thinWalls ? thinGridWidth : 2 * thickWallWidth));
    }

    /**
     * Constructor.
     * @param size Nominal size of the shape.
     * @param wallWidth Width of the maze walls, in OutlineShape coordinates.
     * @param gridWidth Width of the maze grid, in OutlineShape coordinates.
     */
    private MazeOutlineShape(Dimension size, int wallWidth, int gridWidth)
    {
        super(size);

        // Determine dimensions of a maze shape that fits tightly around the real maze.
        int mazeWidth = (getWidth() - wallWidth + gridWidth - 1) / gridWidth;
        int mazeHeight = (getHeight() - wallWidth + gridWidth - 1) / gridWidth;
        int mazeAreaX = mazeWidth * gridWidth + wallWidth;
        int mazeAreaY = mazeHeight * gridWidth + wallWidth;

        MazeGeometry geometry = new MazeGeometry(gridWidth - wallWidth, wallWidth, 0);
        geometry.offset = new Point(-(mazeAreaX - getWidth()) / 2, -(mazeAreaY - getHeight()) / 2);

        // Adjust width if the left (and maybe also the right) border would lie on the real maze border.
        if (wallWidth + geometry.offset.x > 0)
        {
            mazeWidth += 1;
            mazeAreaX += gridWidth;
            geometry.offset.x = -(mazeAreaX - getWidth()) / 2;
        }
        if (wallWidth + geometry.offset.y > 0)
        {
            mazeHeight += 1;
            mazeAreaY += gridWidth;
            geometry.offset.y = -(mazeAreaY - getHeight()) / 2;
        }

        this.baseShape = new ExplicitOutlineShape(size);

        Maze maze = buildMaze(mazeWidth, mazeHeight);
        paintBorder(maze, geometry);
        paintWalls(maze, geometry);
    }

    /**
     * @param mazeWidth Width of the maze, number of squares.
     * @param mazeHeight Height of the maze, number of squares.
     * @return A simple {@link Maze} without any special features.
     */
    private Maze buildMaze(int mazeWidth, int mazeHeight)
    {
        Maze result = new Maze(mazeWidth, mazeHeight);

        result.createMaze();

        return result;
    }

    //--------------------- "Painting" methods that implement the walls of the maze

    //--------------------- Methods for "painting" the maze into the outline

    /**
     * Implements the east and south border of the maze shape.
     * @param maze The underlying maze.
     * @param geometry The {@link MazeGeometry} used for painting the maze.
     */
    private void paintBorder(Maze maze, MazeGeometry geometry)
    {
        int width = maze.getXSize() * geometry.gridWidth;
        int height = maze.getYSize() * geometry.gridWidth;

        for (int w = 0; w < geometry.wallWidth; w++)
        {
            drawWall(geometry.offset.x + width + w, geometry.offset.y, 0, 1, height + geometry.wallWidth);
            drawWall(geometry.offset.x, geometry.offset.y + height + w, 1, 0, width + geometry.wallWidth);
        }
    }

    /**
     * Implements the north and west walls of all squares in the maze shape.
     * @param maze The underlying maze.
     * @param geometry The {@link MazeGeometry} used for painting the maze.
     */
    private void paintWalls(Maze maze, MazeGeometry geometry)
    {
        // Draw the west and north walls of every square.
        for (int x = 0; x < maze.getXSize(); x++)
        {
            int cx = geometry.offset.x + x * geometry.gridWidth;

            for (int y = 0; y < maze.getYSize(); y++)
            {
                int cy = geometry.offset.y + y * geometry.gridWidth;

                // Draw the west wall.
                if (maze.getSquare(x, y).getWall(WallPosition.WP_W) == WallState.WS_CLOSED)
                {
                    drawWall(cx, cy, 0, 1, geometry);
                }

                // Draw the north wall.
                if (maze.getSquare(x, y).getWall(WallPosition.WP_N) == WallState.WS_CLOSED)
                {
                    drawWall(cx, cy, 1, 0, geometry);
                }
            }
        }
    }

    /**
     * Implements a single square wall starting at the given coordinates.
     * @param x0 X position of the wall, in outline coordinates.
     * @param y0 Y position of the wall, in outline coordinates.
     * @param dx 0 for a vertical wall, 1 for a horizontal wall.
     * @param dy 1 for a vertical wall, 0 for a horizontal wall.
     * @param geometry The {@link MazeGeometry} used for painting the maze.
     */
    private void drawWall(int x0, int y0, int dx, int dy, MazeGeometry geometry)
    {
        for (int w = 0; w < geometry.wallWidth; w++)
        {
            drawWall(x0 + dy * w, y0 + dx * w, dx, dy, geometry.gridWidth + geometry.wallWidth);
        }
    }

    /**
     * Implements a wall with the given length and width 1 starting at the given coordinates.
     * @param x0 X position of the wall, in outline coordinates.
     * @param y0 Y position of the wall, in outline coordinates.
     * @param dx 0 for a vertical wall, 1 for a horizontal wall.
     * @param dy 1 for a vertical wall, 0 for a horizontal wall.
     * @param len
     */
    private void drawWall(int x0, int y0, int dx, int dy, int len)
    {
        for (int x = x0, y = y0, i = 0; i < len; x += dx, y += dy, i++)
        {
            if (0 <= x && x < baseShape.getWidth() && 0 <= y && y < baseShape.getHeight())
            {
                baseShape.set(x, y, true);
            }
        }
    }
}
