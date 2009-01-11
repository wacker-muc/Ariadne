package swa.ariadne.logic;

import java.util.*;

import swa.ariadne.model.*;

/**
 * A {@link IMazeSolver MazeSolver} with many concurrent paths.
 * Visits all neighbor squares of the current path's end before advancing to the next path.
 */
public
class RoundRobinFlooder extends FlooderBase
{
    //--------------------- Constructor

    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    public RoundRobinFlooder(Maze maze, IMazeDrawer mazeDrawer)
    {
        super(maze, mazeDrawer);
    }
    
    //--------------------- FlooderBase Implementation

    @Override
    protected int selectPathIdx()
    {
        // Always select the first open path.
        return 0;
    }

    @Override
    protected WallPosition selectDirection(MazeSquare sq, List<WallPosition> openWalls)
    {
        return openWalls.get(0);
    }

}
