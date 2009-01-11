package swa.ariadne.logic;


import java.util.ArrayList;
import java.util.List;
import java.util.Random;

import swa.ariadne.model.*;
import swa.util.RandomFactory;

/**
 * Base class of all {@link IMazeSolver MazeSolver} classes.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract class SolverBase
implements IMazeSolver
{
    //--------------------- Member Variables and Properties

    /** The problem to be solved. */
    protected Maze maze;

    /** An object that will draw the path while we are solving it. */
    protected IMazeDrawer mazeDrawer;

    /** A source of random numbers. */
    protected Random random;

    //--------------------- Constructor

    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    protected SolverBase(Maze maze, IMazeDrawer mazeDrawer)
    {
        this.maze = maze;
        this.mazeDrawer = mazeDrawer;
        this.random = RandomFactory.createRandom();
    }

    //--------------------- IMazeSolver methods

    /* (non-Javadoc)
     * @see swa.ariadne.logic.IMazeSolver#Reset()
     */
    @Override
    public void reset()
    {
        // no action
    }

    /* (non-Javadoc)
     * @see swa.ariadne.logic.IMazeSolver#Step(swa.ariadne.logic.IMazeSolver.StepResult)
     */
    @Override
    public void step(StepResult out)
    {
        stepI(out);

        /* TODO
        //--------------------- Apply the dead end checker.
        if (deadEndChecker != null && mazeDrawer != null)
        {
            List<MazeSquare> deadSquares = deadEndChecker.Visit(sq2);
            for (MazeSquare deadSq : deadSquares)
            {
                mazeDrawer.DrawDeadSquare(deadSq);
            }
        }
        //---------------------
         */
    }

    //--------------------- Abstract Methods

    /**
     * Travel from one visited square to a neighbor square (through an open wall).
     * Implementation of Step().
     * @param out The output parameters: sq1, sq2, forward.
     * @see IMazeSolver#step(IMazeSolver.StepResult)
     */
    protected abstract void stepI(StepResult out);
    
    /**
     * Select one of the open walls leading away from the given square.
     * @param sq The square at the end point of the current path.
     * @param openWalls The valid continuation directions.
     * @return One of the given directions, depending on the solver strategy.
     */
    protected abstract WallPosition selectDirection(MazeSquare sq, List<WallPosition> openWalls);

    //--------------------- MazeSolver Implementation

    /**
     * Find a path in the maze from the start to the end point.
     */
    public void solve()
    {
        StepResult step = new StepResult();

        while (!maze.isSolved())
        {
            this.step(step);
            if (mazeDrawer != null)
            {
                mazeDrawer.drawStep(step.sq1, step.sq2, step.forward);
            }
        }
    }

    //--------------------- Auxiliary Methods for Derived Classes

    /**
     * @param sq A square in the maze.
     * @param notVisitedOnly When true: Exclude neighbors that have already been visited.
     * @return A list of directions leading from the given square to neighbors through open walls.
     * Neighbors that have been identified as dead ends are excluded (efficient solvers only).
     */
    protected List<WallPosition> getOpenWalls(MazeSquare sq, boolean notVisitedOnly)
    {
        List<WallPosition> result = new ArrayList<WallPosition>(WallPosition.NUM);

        for (WallPosition wp : WallPosition.values())
        {
            if (sq.getWall(wp) == WallState.WS_OPEN)
            {
                MazeSquare sq2 = sq.getNeighbor(wp);

                if (notVisitedOnly)
                {
                    // Exclude squares that have already been visited.
                    if (sq2.isVisited)
                    {
                        continue;
                    }
                }

                /* TODO
                // Exclude squares that need not be visited because they are dead ends.
                if (this.deadEndChecker != null && deadEndChecker.IsDead(sq2))
                {
                    continue;
                }
                 */

                result.add(wp);
            }
        }

        return result;
    }

  //---------------------
}
