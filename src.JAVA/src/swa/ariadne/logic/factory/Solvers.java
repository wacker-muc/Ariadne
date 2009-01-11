package swa.ariadne.logic.factory;

import swa.ariadne.logic.IMazeDrawer;
import swa.ariadne.logic.IMazeSolver;
import swa.ariadne.logic.MasterSolver;
import swa.ariadne.logic.RandomBacktracker;
import swa.ariadne.logic.RoundRobinFlooder;
import swa.ariadne.model.Maze;

/**
 * Provides constructor-like methods for the {@link IMazeSolver} objects.
 * <p>
 * The {@link SolverFactory} has a collection of these constructors.
 * 
 * @author Stephan.Wacker@web.de
 */
final
class Solvers
{
    /**
     * @return A constructor for a RandomBacktracker. 
     */
    public static SolverConstructor RandomBacktracker()
    {
        return new SolverConstructor(swa.util.Stack.getCallingMethod())
        {
            @Override
            public IMazeSolver createI(Maze maze, IMazeDrawer mazeDrawer)
            {
                return new RandomBacktracker(maze, mazeDrawer);
            }
        };
    }

    /**
     * @return A constructor for a MasterSolver. 
     */
    public static SolverConstructor MasterSolver()
    {
        return new SolverConstructor(swa.util.Stack.getCallingMethod())
        {
            @Override
            public IMazeSolver createI(Maze maze, IMazeDrawer mazeDrawer)
            {
                return new MasterSolver(maze, mazeDrawer);
            }
        };
    }

    /**
     * @return A constructor for a RoundRobinFlooder. 
     */
    public static SolverConstructor RoundRobinFlooder()
    {
        return new SolverConstructor(swa.util.Stack.getCallingMethod())
        {
            @Override
            public IMazeSolver createI(Maze maze, IMazeDrawer mazeDrawer)
            {
                return new RoundRobinFlooder(maze, mazeDrawer);
            }
        };
    }
}
