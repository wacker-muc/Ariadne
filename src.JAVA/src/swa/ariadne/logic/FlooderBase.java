package swa.ariadne.logic;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;
import swa.ariadne.model.WallPosition;


/**
 * Base class for all MazeSolvers using a flooding strategy.
 */
abstract
class FlooderBase
extends SolverBase
{
    //--------------------- Member Variables

    /**
     * Additional data for every MazeSquare required by the Flooder algorithm.
     */
    private class MazeSquareExtension
    {
        /** Number of the open paths that lead away from this square. */
        public int openPathCount;

        /** Predecessor of this square on its path. */
        public MazeSquare previousSquare;
    }

    /**
     * For every MazeSquare: a counter of the open paths that lead away from it.
     * A path is considered "open" if its end point is in the list of active squares.
     */
    private MazeSquareExtension[][] mazeExtension;

    /**
     *  All squares passed in forward direction are collected in a list.
     */
    protected List<MazeSquare> list = new ArrayList<MazeSquare>();

    //--------------------- Constructor

    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    public FlooderBase(Maze maze, IMazeDrawer mazeDrawer)
    {
        super(maze, mazeDrawer);

        this.mazeExtension = new MazeSquareExtension[maze.getXSize()][maze.getYSize()];
    }

    //--------------------- Setup Methods

    /* (non-Javadoc)
     * @see SolverBase#Reset()
     */
    @Override
    public void reset()
    {
        super.reset();

        list.clear();

        // Move to the start square.
        MazeSquare sq = maze.getStartSquare();

        // Add the start square to the list.
        list.add(sq);
        sq.isVisited = true;

        // As we may not retract beyond the start square, it needs to have a positive count.
        mazeExtension[sq.getXPos()][sq.getYPos()].openPathCount = 1;
    }

    //--------------------- SolverBase Implementation

    /* (non-Javadoc)
     * @see SolverBase#Step(IMazeSolver.StepResult)
     */
    @Override
    public void step(StepResult out)
    {
        super.step(out);

        if (out.forward) // Always true; Flooders only travel forwards.
        {
            mazeExtension[out.sq1.getXPos()][out.sq1.getYPos()].openPathCount += 1;
            mazeExtension[out.sq2.getXPos()][out.sq2.getYPos()].previousSquare = out.sq1;

            // This might also be done in the Reset() method.  But it is not too late here.
            mazeExtension[out.sq2.getXPos()][out.sq2.getYPos()].openPathCount = 0;
        }
    }

    /* (non-Javadoc)
     * @see SolverBase#StepI(IMazeSolver.StepResult)
     */
    @Override
    protected void stepI(StepResult out)
    {
        if (maze.isSolved())
        {
            throw new Error("Maze is already solved.");
        }

        List<WallPosition> openWalls;

        while (true)
        {
            // Get a current square but leave it in the queue.
            int p = selectPathIdx();
            out.sq1 = list.get(p);

            // Possible choices of open walls (not visited).
            openWalls = getOpenWalls(out.sq1, true);

            if (openWalls.size() == 0)
            {
                list.remove(p);
                // TODO: MarkDeadBranch(sq1);
            }
            else
            {
                // If this was the last open wall of sq1, it can be removed from the list.
                if (openWalls.size() == 1)
                {
                    list.remove(p);
                }

                // sq1 is the square from which we want to continue.
                break;
            }
        }

        // Select one (any) of the neighbor squares.
        WallPosition wp = selectDirection(out.sq1, openWalls);

        out.sq2 = out.sq1.getNeighbor(wp);
        out.forward = true;

        // Add the next square to the list.
        list.add(out.sq2);
        out.sq2.isVisited = true;
    }

    //--------------------- Abstract Methods
    
    /**
     * @return An index within the flooder's list of open paths.
     * @see SolverBase#selectDirection(MazeSquare, List)
     */
    protected abstract int selectPathIdx();
    
    //--------------------- Support Methods for the MasterSolver
    
    /**
     * @param target A visited square, usually, the target square.
     * @return The path leading from the start square to the given square.
     */
    List<MazeSquare> getPathFromStartSquare(MazeSquare target)
    {
        List<MazeSquare> result = new LinkedList<MazeSquare>();

        MazeSquare sq = target;
        result.add(sq);

        while (sq != this.maze.getStartSquare())
        {
            sq = this.mazeExtension[sq.getXPos()][sq.getYPos()].previousSquare;
            result.add(0, sq);
        }

        return result;
    }
}
