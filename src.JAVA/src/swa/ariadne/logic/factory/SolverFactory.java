package swa.ariadne.logic.factory;

import java.lang.reflect.Type;
import java.util.List;

import swa.ariadne.logic.IMazeDrawer;
import swa.ariadne.logic.IMazeSolver;
import swa.ariadne.logic.MasterSolver;
import swa.ariadne.logic.RandomBacktracker;
import swa.ariadne.logic.RoundRobinFlooder;
import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;
import swa.util.RandomFactory;

/**
 * Knows the various MazeSolver strategies and can create instances of them.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class SolverFactory
{
    //--------------------- Static Member variables and Properties

    /** The Types of all implemented MazeSolver strategies. */
    private static Type[] solverTypes = {
        // TODO:
        // ProximityBacktracker.class,
        // OpposedBacktracker.class,
        RandomBacktracker.class,
        MasterSolver.class,
        // RightHandWalker.class,
        // LeftHandWalker.class,
        // RandomWalker.class,
        RoundRobinFlooder.class,
        // CloseFlooder.class,
        // FarFlooder.class,
        // OpposedFlooder.class,
        // ProximityFlooder.class,
        // HesitatingFlooder.class,
        // CenterFlooder.class,
        // CornerFlooder.class,
        // ForwardFlooder.class,
        // BackwardFlooder.class,
        // RandomForwardFlooder.class,
        // RandomBackwardFlooder.class,
        // ThickestBranchFlooder.class,
        // ThinnestBranchFlooder.class,
        // SpreadingFlooder.class,
        // RandomFlooder.class,
    };
    
    /**
     *  A collection of all available IMazeSolver constructors.
     *  TODO: use solvers instead of solverTypes
     */
    private static SolverConstructor[] solvers = {
        Solvers.RandomBacktracker(),
        Solvers.MasterSolver(),
        Solvers.RoundRobinFlooder(),
    };

    /**
     * @return An array with all implemented MazeSolver strategies.
     */
    public static Type[] getSolverTypes()
    {
        return solverTypes;
    }

    /**
     * The default MazeSolver strategy.
     */
    public static final String DefaultStrategy = "RandomBacktracker";

    /**
     *  A prefix that may be added to a strategy name.
     *  If the strategyName passed to CreateSolver() begins with this prefix,
     *  the returned MazeSolver will employ a dead-end detection algorithm.
     *  @see #createSolver(String, Maze, IMazeDrawer)
     */
    public static final String EfficientPrefix = "Efficient";

    //--------------------- Static methods for creating a MazeSolver

    /**
     * @param solverType A specific MazeSolver type. 
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A new MazeSolver of the given Type.
     */
    private static IMazeSolver createSolver(Type solverType, Maze maze, IMazeDrawer mazeDrawer)
    {
        IMazeSolver result;
        
        try
        {
            result = (IMazeSolver) solverType.getClass().getConstructor(Maze.class, IMazeDrawer.class).newInstance(maze, mazeDrawer);
            result.reset();
        }
        catch (Exception e)
        {
            result = createDefaultSolver(maze, mazeDrawer);
        }

        return result;
    }

    /**
     * If the given strategyName is valid, that type is created; otherwise a random type is returned.
     * @param strategyName Name of the Solver Type.
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A new MazeSolver of the given type name.
     */
    public static IMazeSolver createSolver(String strategyName, Maze maze, IMazeDrawer mazeDrawer)
    {
        IMazeSolver result;
        boolean isEfficient = false;

        /* TODO
        if (strategyName != null)
        {
            if (strategyName.StartsWith(EfficientPrefix))
            {
                strategyName = strategyName.Substring(EfficientPrefix.Length);
                isEfficient = true;
            }
        }
        */

        Type strategy = getSolverType(strategyName);
        if (strategy != null)
        {
            // If strategyName is a valid solver type name:
            result = createSolver(strategy, maze, mazeDrawer);
            if (isEfficient)
            {
                // TODO result.MakeEfficient();
            }
        }
        else
        {
            // Otherwise (strategy name is "any"):
            result = createSolver(maze, mazeDrawer);
        }

        return result;
    }

    /**
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A new MazeSolver of a random type.
     */
    private static IMazeSolver createSolver(Maze maze, IMazeDrawer mazeDrawer)
    {
        while (true)
        {
            Type t = solverTypes[RandomFactory.nextInt(solverTypes.length)];
            boolean shouldBeEfficient = (RandomFactory.nextInt(2) == 0);
            // TODO: shouldBeEfficient &= RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS);

            /* TODO
            if (t == RandomWalker.class)
            {
                // too dumb
                continue;
            }
            */

            if (t == MasterSolver.class)
            {
                // too smart
                continue;
            }

            IMazeSolver result = createSolver(t, maze, mazeDrawer);

            /* TODO:
            if (shouldBeEfficient && HasEfficientVariant(t))
            {
                result.MakeEfficient();
            }
            */

            return result;
        }
    }
    
    /**
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A MazeSolver object implementing the default strategy.
     */
    public static IMazeSolver createDefaultSolver(Maze maze, IMazeDrawer mazeDrawer)
    {
        IMazeSolver result = new RandomBacktracker(maze, mazeDrawer);
        result.reset();
        return result;
    }

    //--------------------- Static methods related to the solution

    /**
     * @param maze The problem to be solved.
     * @return The list of squares on the solution path from start to target square.
     */
    public static List<MazeSquare> getSolutionPath(Maze maze)
    {
        // Note: This code must not modify the given maze.
        MasterSolver m = new MasterSolver(maze, null);
        return m.getPath();
    }

    //--------------------- Auxiliary methods

    /**
     * @param name The name of one of the solver types. 
     * @return A valid MazeSolver Type or null if the name is not declared.
     */
    private static Type getSolverType(String name)
    {
        for (Type t : solverTypes)
        {
            if (t.toString().equals(name))
            {
                return t;
            }
        }
        return null;
    }
}
