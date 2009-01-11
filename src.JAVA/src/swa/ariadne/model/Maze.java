package swa.ariadne.model;

import java.awt.Dimension;
import java.awt.Point;
import java.awt.Rectangle;
import java.lang.Math;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Stack;

import swa.ariadne.outlines.IOutlineShape;
import swa.ariadne.outlines.OutlineShape;
import swa.util.*;

/**
 * A Maze consists of a rectangular grid of {@link MazeSquare MazeSquares}.
 * There is exactly one solution path from the start square to the target square.
 * 
 * @author Stephan.Wacker@web.de
 */
public class Maze
{
    //--------------------- Member variables and Properties 
    
    /** Provides minimum and maximum dimension parameters. */
    private MazeDimensions dimensionsObj;
    
    /** Can encode and decode maze parameters into maze ID strings. */
    private MazeCode codeObj;
    
    /** @return The ID of this maze: 1 is the primary maze */
    public int getMazeId()
    {
        return MazeSquare.PrimaryMazeId;
    }

    /** Width and height of the maze. */
    private Dimension size = new Dimension();
    
    /**
     * @return Size of the maze, as a new Dimension object.
     */
    public Dimension getSize()
    {
        return new Dimension(this.size);
    }
    
    /** @return Width of the maze. */
    public int getXSize()
    {
        return this.size.width;
    }
    
    /** @return Height of the maze. */
    public int getYSize()
    {
        return this.size.height;
    }

    /** Two-dimensional grid of MazeSquares, the dimensions are size.width x size.height. */
    private MazeSquare[][] squares;
    
    /**
     * @param x A valid X coordinate.
     * @param y A valid Y coordinate.
     * @return The MazeSquare at coordinates (x, y).
     */
    public MazeSquare getSquare(int x, int y)
    {
        return this.squares[x][y];
    }
    
    /** Coordinates of the start square. */
    private Point startPoint = new Point();
    
    /** Coordinates of the target square. */
    private Point targetPoint = new Point();
    
    /** @return The start square. */
    public MazeSquare getStartSquare()
    {
        return squares[startPoint.x][startPoint.y];
    }
    
    /** @return The target square. */
    public MazeSquare getTargetSquare()
    {
        return squares[targetPoint.x][targetPoint.y];
    }
    
    /** Travel direction. */
    private WallPosition direction;
    
    /**
     * @return The travel direction.
     * The target square is on this side of the maze and the start square on the opposite side.
     */
    public WallPosition getDirection()
    {
        return this.direction;
    }

    /** The source of random numbers specific to this maze. */
    private Random random;
    /** @return The source of random numbers specific to this maze. */
    public Random getRandom()
    {
        return this.random;
    }

    /** The seed used to initialize the random number generator. */
    private int seed;

    /** @return The seed used to initialize the random generator. */
    public int getSeed()
    {
        return seed;
    }
    
    /**
     * @return A string that encodes the maze parameters.
     * This code can be used to construct an identical maze.
     */
    public String getCode()
    {
        return codeObj.code(this);
    }

    /**
     * Usually, the bounding box covers the whole maze area.
     * The bounding box may be smaller for an embedded maze or when there are reserved areas on the border.
     * <p>Note: This code is not very efficient, but it is not executed very often, either.
     * @see OutlineShape#getBoundingBox()
     * @return A rectangle that tightly includes the squares of this maze.
     */
    private Rectangle getBoundingBox()
    {
        int xMin = size.width, xMax = 0, yMin = size.height, yMax = 0;

        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                if (squares[x][y].getMazeId() == this.getMazeId())
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
     * @return True if the end point has been visited.
     */
    public boolean isSolved()
    {
        return this.getTargetSquare().isVisited;
    }

    /**
     * @return True if this maze and all embedded mazes have been solved.
     */
    public boolean isFinished()
    {
        boolean result = this.isSolved();

        /* TODO
        for (Maze item : embeddedMazes)
        {
            result &= item.IsSolved();
        }
        */

        return result;
    }

    /** Position and dimensions of some reserved areas. */
    private List<Rectangle> reservedAreas = new ArrayList<Rectangle>();
    
    /** For every reservedArea: An OutlineShape that defines the area actually covered by the contour image. */
    private List<IOutlineShape> reservedAreaShapes = new ArrayList<IOutlineShape>();

    //--------------------- Constructor.
    
    /**
     * Constructor.
     * Create a maze with the given dimensions.
     * @param width Width of the maze, number of squares.
     * @param height Height of the maze, number of squares.
     */
    public Maze(int width, int height)
    {
        this(width, height, MazeCode.DefaultCodeVersion);
    }
    
    /**
     * Constructor.
     * Create a maze with the given dimensions.
     * @param width Width of the maze, number of squares.
     * @param height Height of the maze, number of squares.
     * @param codeVersion 0 or 1.
     */
    public Maze(int width, int height, int codeVersion)
    {
        this(width, height, codeVersion, -1);
    }
    
    /**
     * Constructor.
     * Create a maze with the given dimensions.
     * @param width Width of the maze, number of squares.
     * @param height Height of the maze, number of squares.
     * @param codeVersion 0 or 1.
     * @param seed Initialization of the random number generator.
     */
    public Maze(int width, int height, int codeVersion, int seed)
    {
        this.dimensionsObj = MazeDimensions.getInstance(codeVersion);
        this.codeObj = MazeCode.getInstance(codeVersion);

        this.size.width = Math.max(dimensionsObj.getMinSize(), Math.min(dimensionsObj.getMaxXSize(), width));
        this.size.height = Math.max(dimensionsObj.getMinSize(), Math.min(dimensionsObj.getMaxYSize(), height));

        // Get an initial random seed and use that to create the Random.
        if (seed < 0)
        {
            this.seed = RandomFactory.nextInt(codeObj.getSeedLimit());
        }
        else
        {
            this.seed = seed;
        }
        //this.seed = 123; // TODO: remove
        this.random = RandomFactory.createRandom(this.seed);
    }
    
    /** @return A copy of this maze. */
    public Maze clone()
    {
        Maze clone = new Maze(size.width, size.height);

        clone.startPoint = (Point)this.startPoint.clone();
        clone.targetPoint = (Point)this.targetPoint.clone();
        clone.direction = this.direction;
        clone.seed = this.seed;
        /* TODO:
        clone.reservedAreas = this.reservedAreas;
        clone.reservedAreaShapes = this.reservedAreaShapes;
        clone.reservedShape = this.reservedShape;
        clone.embeddedMazeShapes = this.embeddedMazeShapes;
        clone.embeddedMazes = new List<EmbeddedMaze>(embeddedMazes.Count);
        for (EmbeddedMaze em : embeddedMazes)
        {
            ** TODO:
            em.Clone(clone)
            clone.embeddedMazes.Add((EmbeddedMaze)em.Clone());
            **
        }
         */

        clone.createSquares();

        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                for (WallPosition wp : WallPosition.values())
                {
                    clone.squares[x][y].setWall(wp, this.squares[x][y].getWall(wp));
                }
            }
        }

        return clone;
    }

    //--------------------- Building a Maze.
    
    /**
     * Create the actual maze data structures.
     * Call this after all parameters have been defined.
     */
    public void createMaze()
    {
        // Create all MazeSquare objects.
        createSquares();

        // Fix reserved areas.
        fixReservedAreas();
        // TODO: CloseWallsAroundReservedAreas(); // TODO: This might be discarded.
        
        // Divide the area into a main maze and several embedded mazes.
        // TODO: FixEmbeddedMazes();
        
        // Put walls around the outline shape and the whole maze.
        // TODO: FixOutlineShape();
        fixBorderWalls();

        // Construct the inner walls of the main maze and choose a start and target square.
        buildMaze();
        placeEndpoints();

        // Do the same for all embedded mazes.
        /* TODO:         foreach (EmbeddedMaze m in this.embeddedMazes)
        {
            m.BuildMaze();
            m.PlaceEndpoints();
        }
        */
    }

    /**
     * Create the grid of MazeSquares.
     * All squares are connected to their four neighbor squares in the grid.
     */
    private void createSquares()
    {
        //--------------------- Create the squares.
        
        this.squares = new MazeSquare[size.width][size.height];
        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                squares[x][y] = new MazeSquare(x, y);
            }
        }

        //--------------------- Connect the squares with their neighbors.

        for (int x0 = 0; x0 < size.width; x0++)
        {
            for (int y0 = 0, y1 = 1; y1 < size.height; y0++, y1++)
            {
                squares[x0][y0].setNeighbor(WallPosition.WP_S, squares[x0][y1]);
                squares[x0][y1].setNeighbor(WallPosition.WP_N, squares[x0][y0]);
            }
        }
        for (int y0 = 0; y0 < size.height; y0++)
        {
            for (int x0 = 0, x1 = 1; x1 < size.width; x0++, x1++)
            {
                squares[x0][y0].setNeighbor(WallPosition.WP_E, squares[x1][y0]);
                squares[x1][y0].setNeighbor(WallPosition.WP_W, squares[x0][y0]);
            }
        }

        //---------------------
    }

    /**
     * Convert all undecided walls to either closed or open.
     * In the resulting maze, there must be a path from every square to every other square.
     * There must not be any circles, i.e. the maze must have a tree-like structure.
     */
    private void buildMaze()
    {
        // We hold a number of active squares in a stack.
        // Make the initial capacity sufficient to hold all squares.
        //
        Stack<MazeSquare> stack = new Stack<MazeSquare>(); //new Stack<MazeSquare>(size.width * size.height);

        List<MazeSquare> outlineSquares = new ArrayList<MazeSquare>();
        List<WallPosition> outlineWalls = new ArrayList<WallPosition>();
        
        //--------------------- Start with a single random cell in the stack.
        
        while (true)
        {
            int x = random.nextInt(size.width);
            int y = random.nextInt(size.height);
            MazeSquare sq = this.squares[x][y];
            if (sq.getMazeId() == this.getMazeId())
            {
                sq.isConnected = true;
                stack.push(sq);
                break;
            }
        }

        //--------------------- Extend the maze by visiting the cells next to those in the stack.
        
        while (stack.size() > 0)
        {
            List<WallPosition> unresolvedWalls = new ArrayList<WallPosition>(WallPosition.NUM);
            MazeSquare sq0 = stack.pop();

            // Collect the unfixed walls of sq0.
            //
            for (WallPosition wp : WallPosition.values())
            {
                switch (sq0.getWall(wp))
                {
                    case WS_MAYBE:
                        MazeSquare sq = sq0.getNeighbor(wp);

                        if (sq.isConnected || sq.getMazeId() != sq0.getMazeId())
                        {
                            sq0.setWall(wp, WallState.WS_CLOSED);
                            sq.setWall(WallPosition.oppositeWall(wp), WallState.WS_CLOSED);
                        }
                        else
                        {
                            unresolvedWalls.add(wp);
                        }
                        break;

                    case WS_OUTLINE:
                        outlineSquares.add(sq0);
                        outlineWalls.add(wp);
                        break;
                }
            }

            // Discard this square if it has no unresolved walls.
            if (unresolvedWalls.size() == 0)
            {
                // Note: This is the only place that may end the loop.
                // If the stack is empty: Open one outline wall.
                if (stack.size() == 0)
                {
                    while (outlineSquares.size() > 0)
                    {
                        // Select a random square with an outline wall.
                        int p = random.nextInt(outlineSquares.size());
                        MazeSquare sq = outlineSquares.remove(p);
                        WallPosition wp = outlineWalls.remove(p);

                        if (sq.getWall(wp) == WallState.WS_OUTLINE)
                        {
                            sq.setWall(wp, WallState.WS_MAYBE);
                            stack.push(sq);
                            // This square will be used in the next iteration.
                            break; // from while(outlineSquares)
                        }
                    }
                }

                continue; // no walls to choose from
            }

            // Add the current cell to the stack.
            // Note: Do this before replacing unresolvedWalls with preferredWalls.
            if (unresolvedWalls.size() > 1)
            {
                stack.push(sq0);
            }

            /* TODO
            // Use only preferred wall positions.
            if (unresolvedWalls.size() > 1 && irregularMazeShape != null
                && (random.nextInt(100) < irregularMazeShape.ApplicationPercentage(this.irregularity)))
            {
                boolean[] preferredPositions = irregularMazeShape.PreferredDirections(sq0);
                List<WallPosition> preferredWalls = new ArrayList<WallPosition>(unresolvedWalls.size());
                for (WallPosition p : unresolvedWalls)
                {
                    if (preferredPositions[p.ordinal()])
                    {
                        preferredWalls.add(p);
                    }
                }
                if (preferredWalls.size() > 0)
                {
                    unresolvedWalls = preferredWalls;
                }
            }
            */

            // Choose one wall.
            WallPosition wp0 = unresolvedWalls.get(random.nextInt(unresolvedWalls.size()));
            MazeSquare sq1 = sq0.getNeighbor(wp0);

            // Open the wall.
            sq0.setWall(wp0, WallState.WS_OPEN);
            sq1.setWall(WallPosition.oppositeWall(wp0), WallState.WS_OPEN); 

            // Add the new cell to the stack.
            sq1.isConnected = true;
            stack.push(sq1);

        } // while stack is not empty
    }

    /**
     * Put closed walls around the maze.
     * Next to reserved squares, the walls will be open instead of closed.
     */
    private void fixBorderWalls()
    {
        int x1 = 0, x2 = getXSize() - 1, y1 = 0, y2 = getYSize() - 1;
        WallState open = WallState.WS_OPEN, closed = WallState.WS_CLOSED;

        for (int x = 0; x < getXSize(); x++)
        {
            this.squares[x][y1].setWall(WallPosition.WP_N, (this.squares[x][y1].getIsReserved() ? open : closed));
            this.squares[x][y2].setWall(WallPosition.WP_S, (this.squares[x][y2].getIsReserved() ? open : closed));
        }
        for (int y = 0; y < getYSize(); y++)
        {
            this.squares[x1][y].setWall(WallPosition.WP_W, (this.squares[x1][y].getIsReserved() ? open : closed));
            this.squares[x2][y].setWall(WallPosition.WP_E, (this.squares[x2][y].getIsReserved() ? open : closed));
        }
    }

    /**
     * Mark the squares inside the reserved areas.
     */
    private void fixReservedAreas()
    {
        for (int i = 0; i < this.reservedAreas.size(); i++)
        {
            Rectangle rect = this.reservedAreas.get(i);
            IOutlineShape shape = this.reservedAreaShapes.get(i);

            for (int x = 0; x < rect.width; x++)
            {
                for (int y = 0; y < rect.height; y++)
                {
                    if (shape == null || shape.get(x, y) == true)
                    {
                        this.squares[x + rect.x][y + rect.y].setIsReserved();
                    }
                }
            }
        }
    }

    /**
     * Choose a start and target point near opposite borders.
     * The target point should be in a dead end (a square with three closed walls).
     */
    private void placeEndpoints()
    {
        // The end points should be placed close to the border of the maze's bounding box.
        Rectangle bbox = this.getBoundingBox();

        boolean reject = true;
        while (reject)
        {
            // Choose a travel direction (one of four)
            this.direction = WallPosition.values()[this.random.nextInt(4)];

            // a small portion of the maze size (in travel direction)
            int edgeWidth = 0;
            switch (direction)
            {
                case WP_N:
                case WP_S:
                    // vertical
                    edgeWidth = 2 + bbox.height * 2 / 100;
                    break;
                case WP_E:
                case WP_W:
                    // horizontal
                    edgeWidth = 2 + bbox.width * 2 / 100;
                    break;
            }

            // distance of start and end point from the maze border
            int edgeDistStart = 0
                + random.nextInt(edgeWidth)
                + random.nextInt(edgeWidth)
                + random.nextInt(edgeWidth)
                ;
            int edgeDistEnd = 0
                + random.nextInt(edgeWidth)
                + random.nextInt(edgeWidth)
                + random.nextInt(edgeWidth)
                ;

            edgeDistStart = Math.min(edgeDistStart, MazeDimensions.MaxBorderDistance);
            edgeDistEnd = Math.min(edgeDistEnd, MazeDimensions.MaxBorderDistance);

            // The two rows (or columns) that make up the (positive) travel direction.
            int lesserRow = 0, greaterRow = 0;

            switch (direction)
            {
                case WP_N:
                    // start at bottom, end at top
                    startPoint.x = bbox.x + random.nextInt(bbox.width);
                    startPoint.y = greaterRow = bbox.y + bbox.height - 1 - edgeDistStart;
                    targetPoint.x = bbox.x + random.nextInt(bbox.width);
                    targetPoint.y = lesserRow = bbox.y + edgeDistEnd;
                    break;
                case WP_E:
                    // start at left, end at right
                    startPoint.x = lesserRow = bbox.x + edgeDistEnd;
                    startPoint.y = bbox.y + random.nextInt(bbox.height);
                    targetPoint.x = greaterRow = bbox.x + bbox.width - 1 - edgeDistStart;
                    targetPoint.y = bbox.y + random.nextInt(bbox.height);
                    break;
                case WP_S:
                    // start at top, end at bottom
                    startPoint.x = bbox.x + random.nextInt(bbox.width);
                    startPoint.y = lesserRow = bbox.y + edgeDistEnd;
                    targetPoint.x = bbox.x + random.nextInt(bbox.width);
                    targetPoint.y = greaterRow = bbox.y + bbox.height - 1 - edgeDistStart;
                    break;
                case WP_W:
                    // start at right, end at left
                    startPoint.x = greaterRow = bbox.x + bbox.width - 1 - edgeDistStart;
                    startPoint.y = bbox.y + random.nextInt(bbox.height);
                    targetPoint.x = lesserRow = bbox.x + edgeDistEnd;
                    targetPoint.y = bbox.y + random.nextInt(bbox.height);
                    break;
            }

            //--------------------- Reject unusable squares.

            // Verify that the end points are actually part of this maze.
            //
            reject = (this.getSquare(startPoint.x, startPoint.y).getMazeId() != this.getMazeId()
                   || this.getSquare(targetPoint.x, targetPoint.y).getMazeId() != this.getMazeId());

            // Verify that the squares are not aligned against the intended travel direction.
            // This also eliminates two other cases: same square and squares outside the maze.
            //
            if (lesserRow >= greaterRow)
            {
                reject = true;
            }

            // Prefer real dead ends.
            // Reject an end point with less than three walls (with ratio 90%).
            //
            if ((this.getSquare(targetPoint.x, targetPoint.y).countClosedWalls() < WallPosition.NUM - 1) && (random.nextInt(100) < 90))
            {
                reject = true;
            }
        }
    }

    //--------------------- Reserved Area Management.

    /**
     * Reserves a rectangular region of the given dimensions at a random location.
     * The area will not touch any other reserved area.
     * <p> TODO: Get width and height from the shape.
     * @param width Width of the area to be reserved, in Maze coordinates.
     * @param height Height of the area to be reserved, in Maze coordinates.
     * @param borderDistance Minimum number of squares between the reserved area and the maze border.
     * @param shape Actual shape of the area to be reserved.
     * @return The location of the reserved area, in Maze coordinates
     * -- or null if no valid location was found.
     */
    public Rectangle reserveRectangle(int width, int height, int borderDistance, IOutlineShape shape)
    {
        // Reject very large areas.
        if (width < 2 || height < 2 || width > getXSize() - Math.max(4, 2 * borderDistance) || height > getYSize() - Math.max(4, 2 * borderDistance))
        {
            return null;
        }

        for (int nTries = 0; nTries < 100; nTries++)
        {
            // Choose a random location.
            // The resulting rectangle may touch the borders.
            int x = borderDistance + random.nextInt(getXSize() - width - 2 * borderDistance);
            int y = borderDistance + random.nextInt(getYSize() - height - 2 * borderDistance);

            if (reserveRectangle(x, y, width, height, shape))
            {
                return new Rectangle(x, y, width, height);
            }
        }

        return null;
    }
    
    /**
     * @param x X location of the area to be reserved, in Maze coordinates.
     * @param y Y location of the area to be reserved, in Maze coordinates.
     * @param width Width of the area to be reserved, in Maze coordinates.
     * @param height Height of the area to be reserved, in Maze coordinates.
     * @param shape Actual shape of the area to be reserved.
     * @return True if the reservation was successful.
     */
    public boolean reserveRectangle(int x, int y, int width, int height, IOutlineShape shape)
    {
        int x0 = x, y0 = y, w0 = width, h0 = height;
        
        // Restrict to the actual maze area.
        if (x0 < 0)
        {
            w0 += x0;
            x0 = 0;
        }
        if (y0 < 0)
        {
            h0 += y0;
            y0 = 0;
        }
        w0 = Math.min(getXSize() - x0, w0);
        h0 = Math.min(getYSize() - y0, h0);

        // Reject very large areas.
        if (w0 < 1 || h0 < 1 || w0 > getXSize() - 4 || h0 > getYSize() - 4)
        {
            return false;
        }

        // The candidate rectangle.
        Rectangle candidate = new Rectangle(x0, y0, w0, h0);

        // The candidate, extended with two squares around all four edges.
        Rectangle extendedCandidate = new Rectangle(x0 - 2, y0 - 2, w0 + 4, h0 + 4);

        boolean reject = false;
        for (Rectangle rect : this.reservedAreas)
        {
            // Reject the candidate if its extension would intersect with another reserved area.
            if (extendedCandidate.intersects(rect))
            {
                reject = true;
            }
        }

        if (!reject)
        {
            reservedAreas.add(candidate);
            reservedAreaShapes.add(shape);
            return true;
        }

        return false;
    }
    
    //--------------------- Runtime Methods.
    
    /**
     * Reset to the initial state (before the maze is solved).
     */
    public void reset()
    {
        // clear the visited region
        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                squares[x][y].isVisited = false;
            }
        }
    }

    //--------------------- Auxiliary Methods.
    
    /** @return A string representation. */
    public String toString()
    {
        return getCode() + ": [" + getXSize() + "," + getYSize() + "], (" + startPoint.x + "," + startPoint.y + ") -> (" + targetPoint.x + "," + targetPoint.y + ")";  
    }
}
