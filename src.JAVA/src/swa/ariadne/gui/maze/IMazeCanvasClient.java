package swa.ariadne.gui.maze;

import swa.ariadne.model.Maze;

/**
 * Comprises the callback methods that a client needs to offer
 * to a {@link MazeCanvas}.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IMazeCanvasClient
{
    /**
     * Place reserved areas into the given maze.
     * This method is called before actually building the maze.
     * @param maze
     */
    void makeReservedAreas(Maze maze);

    /**
     * Will be called once after the maze has been painted.
     */
    void afterMazePainted();

    /**
     * Displays information about the running MazeSolver, e.g. in the status line:
     * The elapsed seconds and number of steps.
     */
    void updateStatusLine();
    
    /**
     * Displays Maze and Solver characteristics, e.g. in the window's title:
     * The maze ID, step rate and solver strategy name.
     */
    void updateCaption();
    
    /**
     * @return The name of the current solver strategy.
     */
    String getStrategyName();
}
