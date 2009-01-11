package swa.ariadne.logic;

import java.util.List;

import sun.reflect.generics.reflectiveObjects.NotImplementedException;
import swa.ariadne.logic.factory.SolverFactory;
import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;
import swa.ariadne.model.WallPosition;

/**
 * A {@link IMazeSolver MazeSolver} that knows the solution path and follows it without making any error.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class MasterSolver
extends SolverBase
{
    //--------------------- Member Variables and Properties

    /** The path leading from start to end. */
    private List<MazeSquare> path;

    /**
     *  @return The path leading from start to end.
     *  @see SolverFactory#getSolutionPath(Maze)
     */
    public List<MazeSquare> getPath()
    {
        return path;
    }

    /** The current position in the path, while solving. */
    private int pathPos;

    //--------------------- Constructor

    /**
     * Constructor.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     */
    public MasterSolver(Maze maze, IMazeDrawer mazeDrawer)
    {
        super(maze, mazeDrawer);

        // Instead of operating on the original maze, we need to make a copy first.
        Maze helperMaze = maze.clone();
        helperMaze.reset();

        // Use another MazeSolver to find the path from start to end.
        FlooderBase helper = new RoundRobinFlooder(helperMaze, null);
        helper.reset();
        helper.solve();

        this.path = helper.getPathFromStartSquare(maze.getTargetSquare());
    }

    //--------------------- Setup Methods

    @Override
    public void reset()
    {
        super.reset();

        // Move to the start square.
        this.pathPos = 0;

        // Mark the start square as visited.
        this.path.get(pathPos).isVisited = true;
    }

    //--------------------- SolverBase Implementation

    @Override
    protected void stepI(StepResult out)
    {
        if (maze.isSolved())
        {
            throw new Error("Maze is already solved.");
        }

        out.sq1 = this.path.get(pathPos);
        out.sq2 = this.path.get(++pathPos);
        out.forward = true;

        // Mark the next square as visited.
        out.sq2.isVisited = true;
    }

    @Override
    protected WallPosition selectDirection(MazeSquare sq, List<WallPosition> openWalls)
    {
        // As we have our own StepI() implementation, this method will never be called.
        throw new NotImplementedException();
    }
}
