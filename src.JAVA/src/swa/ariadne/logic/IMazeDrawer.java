package swa.ariadne.logic;

import java.util.List;

import swa.ariadne.model.MazeSquare;

/**
 * A {@link IMazeSolver MazeSolver} will use these methods to draw the path
 * while it is working on a solution.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IMazeDrawer
{
    /**
     * Draws a section of the path between the given (adjoining) squares.
     * @param sq1 First square, where the step begins.
     * @param sq2 Second square, where the step ends.
     * @param forward Selects the drawing color.
     */
    void drawStep(MazeSquare sq1, MazeSquare sq2, boolean forward);
    
    /**
     * Draws the path between the given squares.
     * @param path A list of squares, each a neighbor of the previous one.
     * @param forward Selects the drawing color.
     */
    void drawPath(List<MazeSquare> path, boolean forward);
    
    /**
     * Paints a square to mark it as "dead".
     * @param sq A square that will never be visited.
     */
    void drawDeadSquare(MazeSquare sq);
}
