package swa.ariadne.outlines;

import java.awt.Dimension;

/**
 * Comprises the methods that make up the {@link OutlineShape} behavior.
 *
 * @author Stephan.Wacker@web.de
 */
public
interface IOutlineShape
{
    /**
     * Function that discriminates the shape's inside from its outside.
     * <p> Note: The arguments are not restricted to the nominal size of the shape.
     * @param x Point's X coordinate.
     * @param y Point's Y coordinate.
     * @return True if the given point is inside the shape.
     */
    boolean get(int x, int y);

    /**
     * @return The shape's nominal size.
     */
    Dimension getSize();
}