package swa.ariadne.gui.maze;

import swa.ariadne.model.Maze;

/**
 * Comprises the methods that a {@link MazeCanvas} (or a substitute) must implement
 * for providing information about the properties of the currently displayed
 * {@link Maze}.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IMazeCanvasProperties
{
    /** @return The code of the currently displayed Maze. */
    String getCode();

    /** @return The Maze that is displayed in this control. */
    Maze getMaze();

    /** @return The width of the currently displayed Maze, in Maze coordinates. */
    int getXSize();

    /** @return The height of the currently displayed Maze, in Maze coordinates. */
    int getYSize();
}
