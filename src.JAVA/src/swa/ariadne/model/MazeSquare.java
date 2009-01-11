package swa.ariadne.model;

/**
 * MazeSquares are the building blocks of a {@link Maze}.
 * They have open or closed walls on all four sides.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class MazeSquare
{
    //--------------------- Member variables and Properties 
    
    /**
     * X coordinate. 
     */
    private int _xPos;

    /**
     * @return the xPos
     */
    public int getXPos() {
        return _xPos;
    }
    
    /**
     * Y coordinate. 
     */
    private int _yPos;

    /**
     * @return the yPos
     */
    public int getYPos() {
        return _yPos;
    }

    /**
     *  The four walls around a square.
     */
    private WallState[] _walls = new WallState[WallPosition.NUM];

    /**
     * @param side One of the four directions.
     * @param value The new WallState.
     */
    public void setWall(WallPosition side, WallState value)
    {
        this._walls[side.ordinal()] = value;
    }

    /**
     * @param side One of the four directions.
     * @return The WallState at the given position.
     */
    public WallState getWall(WallPosition side)
    {
        return _walls[side.ordinal()];
    }

    /**
     * Identifies the maze this square belongs to.
     */
    private int _mazeId;
    
    /**
     * Number of valid maze IDs, including the reserved ID.
     */
    public static final int MazeIdRange = 8;

    /**
     * Maximum maze ID (primary and embedded).
     */
    public static final int MaxMazeId = MazeIdRange - 1;

    /**
     * Maze ID assigned to reserved squares.
     */
    public static final int ReservedMazeId = 0;

    /**
     * Maze ID of the primary maze.
     */
    public static final int PrimaryMazeId = 1;

    /**
     * @param value The mazeId to set.
     */
    public void setMazeId(int value)
    {
        if (value >= PrimaryMazeId && value <= MaxMazeId)
        {
            this._mazeId = value;
        }
        else
        {
            throw new Error("invalid MazeId value: " + value);
        }
    }
    
    /**
     * Used to distinguish a primary (host) maze and several embedded mazes.
     * 0: This square is reserved.
     * 1: Belongs to the primary maze.
     * >1: Belongs to an embedded maze.
     * @return The mazeId.
     */
    public int getMazeId()
    {
        return this._mazeId;
    }

    /**
     * Makes the square reserved so that it will not be used by the maze.
     */
    public void setIsReserved()
    {
        this._mazeId = ReservedMazeId;
    }
    
    /**
     * @return true if the square is reserved and should not be used by the maze.
     */
    public boolean getIsReserved()
    {
        return (this._mazeId == ReservedMazeId);
    }
    
    /**
     * Used while building: Square is connected to the maze.
     */
    boolean isConnected = false;
    
    /**
     * Used while solving: Square has been visited.
     */
    public boolean isVisited = false;

    /**
     * Adjoining squares in the four directions.
     */
    private MazeSquare[] _neighbors = new MazeSquare[WallPosition.NUM];

    /**
     * @param side One of the four directions. 
     * @param neighbor The neighbor on the given side.
     */
    public void setNeighbor(WallPosition side, MazeSquare neighbor)
    {
        this._neighbors[side.ordinal()] = neighbor;
    }

    /**
     * @param side One of the four directions. 
     * @return The neighbor square on the given side. 
     */
    public MazeSquare getNeighbor(WallPosition side)
    {
        return this._neighbors[side.ordinal()];
    }

    /**
     * @return Number of closed walls.
     */
    int countClosedWalls()
    {
        int result = 0;

        for (WallPosition wp : WallPosition.values())
        {
            if (getWall(wp) == WallState.WS_CLOSED)
            {
                ++result;
            }
        }

        return result;
    }
    
    //--------------------- Constructors 
    
    /**
     * Constructor.
     * @param xPos X coordinate
     * @param yPos Y coordinate
     */
    public MazeSquare(int xPos, int yPos)
    {
        this._xPos = xPos;
        this._yPos = yPos;
        this._mazeId = PrimaryMazeId;
        
        for (WallPosition wp : WallPosition.values())
        {
            this.setWall(wp, WallState.WS_MAYBE);
        }
    }

    //--------------------- Auxiliary methods 
    
    /**
     * @return A string representation.
     */
    public String toString()
    {
        return _xPos + "/" + _yPos + ": " + _mazeId;
    }
}
