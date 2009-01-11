package swa.ariadne.logic;

import java.util.List;
import java.util.Stack;

import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;
import swa.ariadne.model.WallPosition;

/**
 * A MazeSolver with one current path and backtracking.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class BacktrackerBase
extends SolverBase
{
    //--------------------- Member variables

    /**
     * All squares passed in forward direction are collected on a stack.
     */
    private Stack<MazeSquare> stack = new Stack<MazeSquare>();

    //---------------------

    //--------------------- Constructor

    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    public BacktrackerBase(Maze maze, IMazeDrawer mazeDrawer)
    {
        super(maze, mazeDrawer);
    }

    //---------------------

    //--------------------- Setup methods

    /* (non-Javadoc)
     * @see swa.ariadne.logic.IMazeSolver#Reset()
     */
    @Override
    public void reset()
    {
        super.reset();

        stack.clear();

        // Move to the start square.
        MazeSquare sq = maze.getStartSquare();

        // Push the start square onto the stack.
        stack.push(sq);
        sq.isVisited = true;
    }

    //---------------------

    //--------------------- Runtime methods

    /* (non-Javadoc)
     * @see swa.ariadne.logic.SolverBase#StepI()
     */
    @Override
    protected void stepI(StepResult out)
    {
        if (maze.isSolved())
        {
            throw new Error("Maze is already solved.");
        }

        // Get the current square.
        out.sq1 = stack.peek();

        // Possible choices of open walls (not visited).
        List<WallPosition> openWalls = getOpenWalls(out.sq1, true);

        if (openWalls.size() > 0)
        {
            // Select one of the neighbor squares.
            WallPosition wp = selectDirection(out.sq1, openWalls);

            out.sq2 = out.sq1.getNeighbor(wp);
            out.forward = true;

            // Push the next square onto the stack.
            stack.push(out.sq2);
            out.sq2.isVisited = true;
        }
        else
        {
            // Pop the current square from the stack.
            stack.pop();

            out.sq2 = stack.peek();
            out.forward = false;
        }
    }

    //---------------------
}
