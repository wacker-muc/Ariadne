package swa.ariadne.gui.maze;

import java.awt.Graphics;
import java.awt.Image;
import java.awt.Rectangle;
import java.awt.image.ImageObserver;

/**
 * Comprises the callback methods that a client needs to offer
 * to a {@link MazePainter}.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IMazePainterClient
{
    /**
     * TODO: Maybe remove this method if it is not required.
     * @return True if the client is still intact and accepts painting commands.
     */
    boolean isAlive();
    
    /** @return The graphics object where the maze is painted. */
    Graphics getDisplayGraphics();
    
    /** @return The position and size of the drawing area. */
    Rectangle getDisplayRectangle();
    
    /**
     * Will be called before the maze is drawn for the first time.
     * The client should fill the reserved areas.
     * @param g The Graphics to be used for painting.
     */
    void paintReservedAreas(Graphics g);

    /**
     * Will be called once after the maze has been painted.
     */
    void afterMazePainted();
    
    /** Updates the display. */
    void Update();

    /**
     * @param width The requested image width.
     * @param height The requested image height.
     * @return A new Image object.
     */
    Image createImage(int width, int height);
    
    /**
     * @return The ImageObserver corresponding to the result of createImage().
     * @see IMazePainterClient#createImage(int, int) 
     */
    ImageObserver getImageObserver();
}
