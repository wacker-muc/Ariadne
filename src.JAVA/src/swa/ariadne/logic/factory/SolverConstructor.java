package swa.ariadne.logic.factory;

import swa.ariadne.logic.IMazeDrawer;
import swa.ariadne.logic.IMazeSolver;
import swa.ariadne.model.Maze;
import swa.util.IPickable;

/**
 * This class implements the equivalent of a constructor for {@link IMazeSolver MazeSolver} objects.
 * <p>
 * Implements the {@link IPickable} interface.
 * 
 * @author Stephan.Wacker@web.de
 */
abstract
class SolverConstructor
implements IPickable
{
    //--------------------- Member variables and Properties
    
    /** The name of this constuctor's solver. */
    private String name;
    
    /**
     * @return The name of this constuctor's solver.
     */
    public String getName()
    {
        return name;
    }
    
    //--------------------- Constructors
    
    /**
     * Constructor
     * @param name
     */
    protected SolverConstructor(String name)
    {
        super();
        this.name = name;
    }
    
    //--------------------- Constructor-like functions
    
    /**
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A new, fully configured IMazeSolver object.
     */
    public IMazeSolver create(Maze maze, IMazeDrawer mazeDrawer)
    {
        IMazeSolver result = this.createI(maze, mazeDrawer);
        result.reset();
        return result;
    }
    
    /**
     * @param maze The problem to be solved.
     * @param mazeDrawer An object that can draw the maze. 
     * @return A new MazeSolver IMazeSolver object, not yet configured.
     */
    protected abstract IMazeSolver createI(Maze maze, IMazeDrawer mazeDrawer);
    
    //--------------------- IPickable implementation
    
    /**
     * Subclasses may override this method if they want to be picked more/less often than others. 
     * @return A default value of 10.
     */
    @Override
    public int getRatio()
    {
        return 10;
    }
}
