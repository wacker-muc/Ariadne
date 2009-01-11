package swa.ariadne.logic;

import java.util.List;

import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;
import swa.ariadne.model.WallPosition;

/**
 * A {@link IMazeSolver MazeSolver} with one current path and backtracking.
 * At a crossing in forward direction: Chooses a random open wall.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class RandomBacktracker
extends BacktrackerBase
{
    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    public RandomBacktracker(Maze maze, IMazeDrawer mazeDrawer)
    {
        super(maze, mazeDrawer);
    }

    @Override
    protected WallPosition selectDirection(MazeSquare sq, List<WallPosition> openWalls) 
    {
        return openWalls.get(random.nextInt(openWalls.size()));
    }

}
