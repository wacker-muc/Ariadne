package swa.ariadne.gui.maze;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Point;
import java.awt.Rectangle;
import java.util.List;

import swa.ariadne.logic.*;
import swa.ariadne.model.*;
import swa.ariadne.settings.AriadneSettingsData;
import swa.util.ColorBuilder;

/**
 * The MazePainter is responsible for the painting operations in a {@link MazeCanvas}.
 * It only needs the client's Graphics object but none of its Component abilities.
 * 
 * @author Stephan.Wacker@web.de
 */
final public
class MazePainter
implements IMazeDrawer
{
    //--------------------- Constants

    /** Reference Color for deriving forward and backward path colors. */
    private static final Color MinColor = Color.getColor("DarkSlateBlue", Color.decode("#483d8b"));
    /** Reference Color for deriving forward and backward path colors. */
    private static final Color MaxColor = Color.getColor("Gold", Color.decode("#ffd700"));

    //--------------------- Member Variables

    // TODO: remove this variable
    /** The owner of the Graphics we are painting in. */
    private IMazePainterClient client;
    
    /** The maze we are painting. */
    private Maze maze;
    
    /** @return The maze we are painting. */
    public Maze getMaze()
    {
        return maze;
    }

    /** The parameters of the maze to be constructed. */
    private AriadneSettingsData settingsData;

    /** Defines which walls are visible. */
    private AriadneSettingsData.WallVisibilityEnum wallVisibility = AriadneSettingsData.WallVisibilityEnum.Always;

    /** @return True if the walls don't have zero width. */
    public boolean hasVisibleWalls()
    {
        return (getWallVisibility() != AriadneSettingsData.WallVisibilityEnum.Never);
    }
    /** @return The visibility of walls. */
    private AriadneSettingsData.WallVisibilityEnum getWallVisibility()
    {
            return (this.settingsData == null ? this.wallVisibility : this.settingsData.wallVisibility);
    }
    
    /**
     * When true:
     * Don't use a double buffering method.
     * Reduce grid size.
     */
    public boolean screenSaverPreviewMode = false;
    
    /** The canvas we are finally painting to. */
    private Graphics targetGraphics;
    /** The canvas geometry. */
    private Rectangle targetRectangle;

    /** The canvas we are painting to, may be a buffered image. */
    private Graphics myGraphics;
    /** An off-screen graphics buffer. */
    private Image myBuffer;
    /** True if the maze has never been displayed. */
    private boolean notYetDisplayed = true;

    /** The dimensions of the maze constituents. */
    /* TODO: private */ MazeGeometry geometry;
    
    /**
     * @param geometry A new set of dimensions.
     */
    void setGeometry(MazeGeometry geometry)
    {
        this.geometry = geometry;
    }
    
    /** The color for painting walls. */
    private Color wallColor = Color.decode("#808080");
    /** The color for painting forward steps. */
    private Color forwardColor = Color.getColor("GreenYellow", Color.decode("#adff2f"));
    /** The color for painting backward steps and completely examined paths. */
    private Color backwardColor = Color.getColor("Brown", Color.decode("#a52a2a"));
    /** The color for painting dead-end squares. */
    private Color deadEndColor = Color.decode("#404040");
    
    /**
     * A counter that switches the target square between two states:
     * When it is 0 or another even number, it is painted normally (red).
     * When it is an odd number, it is painted invisible (black).
     */
    private int blinkingCounter = 0;
    
    /**
     * Activates the painting of end points.
     */
    public void resetBlinkingCounter()
    {
        // TODO: get initial value from master painter
        blinkingCounter = 0;
    }
    
    /**
     * Switches the end point between painted and invisible.
     */
    public void advanceBlinkingCounter()
    {
        // Ignore this message if we have not been properly started.
        if (blinkingCounter < 0)
        {
            return;
        }
        
        /* TODO
        // Forward this message to the shared painters.
        for (MazePainter item : sharedPainters)
        {
            if (item.blinkingCounter >= 0)
            {
                item.advanceBlinkingCounter();
            }
        }
        */

        ++ blinkingCounter;

        paintEndpoints(myGraphics);
        if (client == null || client.isAlive())
        {
            // TODO: renderBufferedGraphics();
        }
        if (client != null)
        {
            client.Update();
        }
    }
    
    /**
     * @return Red.
     */
    private Color getStartSquareColor()
    {
        return Color.RED;
    }
    
    /**
     * @return Red or black depending on the blinkingCounter.
     */
    private Color getTargetSquareColor()
    {
        if (maze.isSolved())               return getStartSquareColor();
        if (this.blinkingCounter % 2 == 0) return getStartSquareColor();
        return Color.BLACK;
    }
    
    //--------------------- Constructor and Initialization
    
    /**
     * Create a MazePainter that paints into the given Graphics.
     * @param client The owner of the Graphics object we will paint in.
     * @param geometry The maze constituent dimensions.
     * @param screenSaverPreviewMode When true, we paint directly into a small frame.
     */
    public MazePainter(IMazePainterClient client, MazeGeometry geometry, boolean screenSaverPreviewMode)
    {
        this.client = client;
        this.geometry = geometry;
        this.screenSaverPreviewMode = screenSaverPreviewMode;

        this.chooseColors();
        
        // TODO: Use double buffering.
        this.myBuffer = null;
        this.myGraphics = null;
    }

    /**
     * Chooses a new set of forward and backward colors.
     */
    public void chooseColors()
    {
        Color[] colors = ColorBuilder.suggestColors(MinColor, MaxColor);
        this.forwardColor = colors[0];
        this.backwardColor = colors[1];
    }

    //--------------------- IMazeDrawer Implementation
    
    /**
     * Construct a maze that fits into the drawing area.
     * @param canvasClient The MazeCanvas's client; it may reserve some areas in the maze before it is built.
     */
    public void createMaze(IMazeCanvasClient canvasClient)
    {
        // Update the client dimensions, if it was resized.
        this.targetRectangle = client.getDisplayRectangle();

        // Determine dimensions of a maze that fits into the drawing area.
        Dimension dim = getMazeDimensions();

        // Complete the configuration of the MazeGeometry.
        // TODO: This assumes we are painting to an off-screen buffer.
        geometry.setOffset(targetRectangle.getSize(), dim);

        // Create a maze object.
        this.maze = new Maze(dim.width, dim.height);

        // Configure the maze layout.
        if (canvasClient != null)
        {
            canvasClient.makeReservedAreas(maze);
        }

        // Build the maze.
        maze.createMaze();
    }
    
    //--------------------- Runtime Methods.

    /**
     * Reset to the initial state (before the maze is solved).
     */
    public void reset()
    {
        if (maze != null)
        {
            maze.reset();
        }

        // Destroy the current buffer; it will be re-created in the OnPaint() method.
        if (myGraphics != null)
        {
            myGraphics = null;
            myBuffer = null;
            notYetDisplayed = true;
        }

        /* TODO
        // Forward this message to the shared painters.
        for (MazePainter item : sharedPainters)
        {
            item.gBuffer = null;
            item.Reset();
        }
        */
    }
    
    //--------------------- IMazeDrawer Implementation
    
    /**
     * @return The dimensions of a maze that fits into the targetRectangle.
     */
    private Dimension getMazeDimensions()
    {
        int usableWidth = targetRectangle.width;
        int usableHeight = targetRectangle.height;
        
        if (geometry.wallWidth > 0)
        {
            usableWidth -= geometry.wallWidth;
            usableHeight -= geometry.wallWidth;
        }
        else
        {
            // Add the unused space between the painted path and the not painted wall.
            // Note: The start or target square may be slightly cut off if it is directly on the border.
            usableWidth += (geometry.squareWidth - geometry.pathWidth);
            usableHeight += (geometry.squareWidth - geometry.pathWidth);
        }

        int xSize = usableWidth / geometry.gridWidth;
        int ySize = usableHeight / geometry.gridWidth;
        
        return new Dimension(xSize, ySize);
    }
    
    @Override
    public void drawStep(MazeSquare sq1, MazeSquare sq2, boolean forward)
    {
        Point p1 = geometry.getPathPosition(sq1);
        Point p2 = geometry.getPathPosition(sq2);
        
        if (p2.x < p1.x) { int tmp = p1.x; p1.x = p2.x; p2.x = tmp; }
        if (p2.y < p1.y) { int tmp = p1.y; p1.y = p2.y; p2.y = tmp; }

        Graphics g = this.myGraphics;

        /* TODO
        // Draw the background image in newly visited squares.
        if (backgroundImage != null)
        {
            // Maybe draw walls around the visited square.
            if (forward && this.WallVisibility == AriadneSettingsData.WallVisibilityEnum.WhenVisited)
            {
                Graphics bg = Graphics.FromImage(backgroundImage);
                this.PaintWalls(bg, sq2);
            }

            PaintBackgroundSquare(g, sq1);
            PaintBackgroundSquare(g, sq2);
        }
        */

        // Draw a line from sq1 to sq2.
        g.setColor(forward ? this.forwardColor : this.backwardColor);
        g.fillRect(p1.x, p1.y, (p2.x - p1.x + geometry.pathWidth), (p2.y - p1.y + geometry.pathWidth));

        // Maybe redraw the end point.
        if (sq1 == maze.getStartSquare() || sq2 == maze.getStartSquare() || sq1 == maze.getTargetSquare() || sq2 == maze.getTargetSquare())
        {
            this.paintEndpoints(g);
        }

        /* TODO
        // Maybe draw walls around the visited square.
        if (forward && this.WallVisibility == AriadneSettingsData.WallVisibilityEnum.WhenVisited)
        {
            this.PaintWalls(g, sq2);
        }
        */
    }

    @Override
    public void drawPath(List<MazeSquare> path, boolean forward)
    {
        Graphics g = myGraphics;

        for (int i = 1; i < path.size(); i++)
        {
            this.drawStep(path.get(i - 1), path.get(i), forward);
        }
        
        // Redraw the square where the branching occurred.
        MazeSquare sq = path.get(path.size() - 1);
        if (sq == maze.getStartSquare())
        {
            PaintSquare(g, this.getStartSquareColor(), sq);
        }
        else
        {
            this.paintPathDot(sq, forwardColor);
        }
    }

    @Override
    public void drawDeadSquare(MazeSquare sq)
    {
        paintPathDot(sq, deadEndColor);
    }

    //--------------------- Painting Methods
    
    /**
     * Renders the buffered image to the output device.
     * On first time, the buffer is created and the maze (without any path) is painted. 
     * @param g 
     */
    public void paint(Graphics g)
    {
        System.out.println("MazePainter.paint, geometry=" + geometry);
        
        // Make g the current targetGraphics.
        if (targetGraphics != null && targetGraphics != g)
        {
            if (myGraphics == targetGraphics)
            {
                myGraphics = g;
            }

            targetGraphics = g;
        }
        
        if (geometry == null)
        {
            return;
        }
        
        // On first time, create a graphics buffer and draw the static maze.
        //
        if (notYetDisplayed)
        {
            // Use the previously prepared buffer, if possible.
            if (myGraphics != null)
            {
                // For a brief moment, display a black screen.
                targetGraphics.setColor(Color.BLACK);
                targetGraphics.fillRect(targetRectangle.x, targetRectangle.y, targetRectangle.width, targetRectangle.height);
                // TODO: targetGraphics.flush();
                // TODO: Thread.sleep(120); // milliseconds
            }
            else
            {
                paintMaze();
            }
            notYetDisplayed = false;
        }

        if (client == null || client.isAlive())
        {
            renderBufferedGraphics();
        }
    }

    //--------------------- Private Painting Methods
    
    /**
     * Creates the GraphicsBuffer and draws the static maze.
     */
    public void paintMaze()
    {
        //Log.WriteLine("{ PaintMaze()");
        createCurrentGraphics();
        
        paintMaze(myGraphics);

        /* TODO:
        // Let all shared painters use the new graphics object.
        for (MazePainter item : sharedPainters)
        {
            item.gBuffer = this.gBuffer;
            item.gBufferAlternate = null;
        }
        */
        //Log.WriteLine("} PaintMaze()");
    }

    /**
     * Draws the static maze.
     * @param g
     */
    private void paintMaze(Graphics g)
    {
        // If there is a background image, we need to paint the maze into it, as well.
        Graphics bg = null;
        
        g.setColor(Color.BLACK);
        if (myGraphics == targetGraphics)
        {
            g.fillRect(targetRectangle.x, targetRectangle.y, targetRectangle.width, targetRectangle.height);
        }
        else
        {
            g.fillRect(0, 0, targetRectangle.width, targetRectangle.height);
        }

        /* TODO
        if (backgroundImage != null)
        {
            bg = Graphics.FromImage(backgroundImage);
            this.backgroundImageSetupCompletion();
        }
        */

        // Call the painterDelegate first
        // as it may also paint into areas that are later covered by the maze.
        if (client != null)
        {
            client.paintReservedAreas(g);
            if (bg != null)
            {
                client.paintReservedAreas(bg);
            }
        }

        /* TODO
        if (settingsData != null && settingsData.VisibleOutlines)
        {
            // Note: These shapes will not be drawn into the background image.
            PaintOutlineShape(g);
            PaintEmbeddedMazes(g);
        }
        */

        // Paint the maze into the given Graphics and into the background image.
        switch (this.getWallVisibility())
        {
            default:
            case Always:

                paintBorder(g);
                paintWalls(g);

                if (bg != null)
                {
                    paintBorder(bg);
                    paintWalls(bg);
                }

                break;
            
            case Never:

                break;
            
            case WhenVisited:

                paintWalls(g, maze.getStartSquare());

                if (bg != null)
                {
                    paintWalls(bg, maze.getStartSquare());
                }

                break;
        }

        paintEndpoints(g);
        
        if (false)
        {
            // Show offset location.
            int d = 2;
            g.setColor(Color.RED);
            g.drawLine(geometry.offset.x - d, geometry.offset.y, geometry.offset.x + d, geometry.offset.y);
            g.drawLine(geometry.offset.x, geometry.offset.y - d, geometry.offset.x, geometry.offset.y + d);
        }
        
        if (client != null)
        {
            client.afterMazePainted();
        }
    }

    /**
     * Paints a border around the maze.
     * @param g
     */
    private void paintBorder(Graphics g)
    {
        /* Actually, we only paint the east and south wall of every
         * square on the respective border.
         * We don't optimize by drawing long lines or a rectangle
         * because there may be reserved areas on the border
         * and the walls may be open instead of closed.
         */

        int gw = geometry.gridWidth, ww = geometry.wallWidth, wl = gw + ww;

        g.setColor(wallColor);
        
        // Draw the south walls of every square on the southern border.
        for (int x = 0, y = maze.getYSize(); x < maze.getXSize(); x++)
        {
            int cx = geometry.getWallX(x);
            int cy = geometry.getWallY(y);
            MazeSquare sq = maze.getSquare(x, maze.getYSize()-1);

            if (sq.getWall(WallPosition.WP_S) == WallState.WS_CLOSED)
            {
                g.fillRect(cx, cy, wl, ww);
            }
        }

        // Draw the east walls of every square on the eastern border.
        for (int y = 0, x = maze.getXSize(); y < maze.getYSize(); y++)
        {
            int cx = geometry.getWallX(x);
            int cy = geometry.getWallY(y);
            MazeSquare sq = maze.getSquare(maze.getXSize()-1, y);

            if (sq.getWall(WallPosition.WP_E) == WallState.WS_CLOSED)
            {
                g.fillRect(cx, cy, ww, wl);
            }
        }
    }

    /**
     * Paints the closed inner walls.
     * @param g
     */
    private void paintWalls(Graphics g)
    {
        int gw = geometry.gridWidth, ww = geometry.wallWidth, wl = gw + ww;

        g.setColor(wallColor);

        // We'll only draw the west and north walls of every square.
        for (int x = 0; x < maze.getXSize(); x++)
        {
            int cx = geometry.getWallX(x);
            for (int y = 0; y < maze.getYSize(); y++)
            {
                int cy = geometry.getWallY(y);
                MazeSquare sq = maze.getSquare(x, y);

                // Draw the west wall.
                if (sq.getWall(WallPosition.WP_W) == WallState.WS_CLOSED)
                {
                    g.fillRect(cx, cy, ww, wl);
                }

                // Draw the north wall.
                if (sq.getWall(WallPosition.WP_N) == WallState.WS_CLOSED)
                {
                    g.fillRect(cx, cy, wl, ww);
                }
            }
        }
    }

    /**
     * Paints the closed walls around a given square.
     * This method is called for the visited squares when the walls are initially invisible.
     * @param g
     * @param sq
     */
    private void paintWalls(Graphics g, MazeSquare sq)
    {
        Point p = geometry.getWallPosition(sq);
        int gw = geometry.gridWidth, ww = geometry.wallWidth, wl = gw + ww;

        g.setColor(wallColor);

        // Draw the west wall.
        if (sq.getWall(WallPosition.WP_W) == WallState.WS_CLOSED)
        {
            g.fillRect(p.x, p.y, ww, wl);
        }

        // Draw the north wall.
        if (sq.getWall(WallPosition.WP_N) == WallState.WS_CLOSED)
        {
            g.fillRect(p.x, p.y, wl, ww);
        }

        // Draw the east wall.
        if (sq.getWall(WallPosition.WP_E) == WallState.WS_CLOSED)
        {
            g.fillRect(p.x + ww, p.y, ww, wl);
        }

        // Draw the south wall.
        if (sq.getWall(WallPosition.WP_S) == WallState.WS_CLOSED)
        {
            g.fillRect(p.x, p.y + ww, wl, ww);
        }
    }

    /**
     * Paints the start and target point.
     * @param g The canvas.
     */
    private void paintEndpoints(Graphics g)
    {
        // Activate the blinkingCounter, if necessary.
        if (blinkingCounter < 0)
        {
            resetBlinkingCounter();
        }

        PaintSquare(g, this.getStartSquareColor(), maze.getStartSquare());
        PaintSquare(g, this.getTargetSquareColor(), maze.getTargetSquare());
    }

    /**
     * Paints a maze square in the given color.
     * @param g The canvas.
     * @param color The color.
     * @param sq The square.
     */
    private void PaintSquare(Graphics g, Color color, MazeSquare sq)
    {
        Point p = geometry.getSquarePosition(sq);
        
        g.setColor(color);
        g.fillRect(p.x, p.y, geometry.squareWidth, geometry.squareWidth);
    }
    

    /**
     * Paints a dot with the given color in the given square.
     * @param sq The square where a dot is painted.
     * @param color The color of the dot.
     */
    private void paintPathDot(MazeSquare sq, Color color) // TODO: rearrange to (g, color, sq)
    {
        Graphics g = this.myGraphics;

        /* TODO:
        if (PaintBackgroundSquare(g, sq))
        {
            // Don't paint a dot if the square is part of the background image.
            return;
        }
        */

        Point p = geometry.getPathPosition(sq);

        g.setColor(color);
        g.fillRect(p.x, p.y, geometry.pathWidth, geometry.pathWidth);
    }

    //--------------------- Methods for Buffered Painting
    
    /**
     * Initializes targetGraphics, targetRectangle and myGraphics.
     */
    private void createCurrentGraphics()
    {
        if (myGraphics != null)
        {
            // do nothing
            return;
        }

        this.targetGraphics = client.getDisplayGraphics();
        this.targetRectangle = client.getDisplayRectangle();

        if (false)
        {
            // direct painting
            myGraphics = targetGraphics;
            geometry.setOffset(targetRectangle, maze.getSize());
        }
        else
        {
            // buffered painting
            myBuffer = client.createImage(targetRectangle.width, targetRectangle.height);
            myGraphics = myBuffer.getGraphics();
            geometry.setOffset(targetRectangle.getSize(), maze.getSize());
        }
    }

    /**
     * Copies the buffered image to the actual targetGraphics.
     */
    public void renderBufferedGraphics()
    {
        if (targetGraphics == null || targetRectangle == null)
        {
            return;
        }
        
        if (myGraphics != targetGraphics)
        {
            if (true)
            {
                targetGraphics.drawImage(myBuffer, targetRectangle.x, targetRectangle.y, client.getImageObserver());
            }
            else
            {
                targetGraphics.drawImage(myBuffer, 0, 0, client.getImageObserver());
            }
        }
    }
    
    /**
     * Copies the buffered image to the given Graphics.
     * @param g A new targetGraphics object.
     */
    public void renderBufferedGraphics(Graphics g)
    {
        targetGraphics = g;
        renderBufferedGraphics();
    }
}
