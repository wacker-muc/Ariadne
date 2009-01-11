package swa.ariadne.gui.maze;

import java.awt.Canvas;
import java.awt.Color;
import java.awt.Component;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.image.ImageObserver;
import java.util.List;
import java.util.ArrayList;
import java.util.Random;

import swa.ariadne.model.Maze;
import swa.util.RandomFactory;

/**
 * A Canvas that displays a {@link Maze}.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class MazeCanvas
extends Canvas
implements IMazeCanvasProperties, IMazePainterClient
{
    //--------------------- Constants
    
    /** Required attribute of a serializable class. */
    private static final long serialVersionUID = -8444713145263134354L;

    //--------------------- Member Variables and Properties
    
    /** The object that contains or controls this MazeCanvas. */
    private IMazeCanvasClient client;
    
    /** The MazePainter responsible for all painting activities. */
    private MazePainter painter;
    
    /** This Graphics object is set in the paint() method. */ 
    private Graphics myGraphics;

    /** A list of locations (in graphics coordinates) where other components will be painted. */
    private List<Rectangle> componentLocations = new ArrayList<Rectangle>();

    /** @return The MazePainter responsible for all painting activities. */
    public MazePainter getMazePainter()
    {
        return this.painter;
    }

    //--------------------- Constructors and Initialization
    
    /**
     * Constructor.
     * This only creates the Component; the maze is created with the Setup() method. 
     * @param client The creator of this MazeCanvas.
     */
    public MazeCanvas(IMazeCanvasClient client)
    {
        this.client = client;
        initGUI();
        this.painter = new MazePainter(this, null, false);
    }

    /**
     * Constructor.
     * <br> This default constructor is required for the GUI designer only.
     * @deprecated
     */
    public MazeCanvas()
    {
        this(null);
    }
    
    /**
     * Sets the client of this MazeCanvas after it has been constructed.
     * <br> This method is required for the GUI designer only.
     * @param client The creator of this MazeCanvas.
     * @deprecated
     */
    public void setClient(IMazeCanvasClient client)
    {
        this.client = client;
    }

    /**
     * Initialize the attributes of this component.
     */
    private void initGUI()
    {
        this.setBackground(Color.BLACK);
    }

    /**
     * Create a new maze that fills the canvas.
     */
    public void setup()
    {
        /* TODO
        if (painter.HasBufferAlternate)
        {
            // the Setup() method was already executed for creating the alternate buffer
            this.Invalidate();
            return;
        }
        */

        /* TODO
        if (settingsData != null)
        {
            this.TakeParametersFrom(settingsData);
            return;
        }
        */

        //--------------------- Set up the painter.

        Random r = RandomFactory.createRandom();
        boolean visibleWalls = true;
        int gridWidth = MazeGeometry.suggestGridWidth(r, visibleWalls, this.getDisplayRectangle());
        MazeGeometry geometry = new MazeGeometry(gridWidth, visibleWalls);

        this.setup(geometry);
    }

    /**
     * Create a new maze based on the given geometry.
     * @param geometry The maze grid sizes. 
     */
    public void setup(MazeGeometry geometry)
    {
        System.out.println("MazeCanvas.Setup, geometry=" + geometry);
        
        painter.setGeometry(geometry);
        painter.chooseColors();
        createMaze();
        Reset();
    }

    /**
     * Construct a maze that fits into the drawing area.
     */
    private void createMaze()
    {
        System.out.println("MazeCanvas.CreateMaze");
        
        /* TODO
        this.imageLocations.clear();
        */
        this.componentLocations.clear();

        painter.createMaze(this.client);

        if (this.client != null)
        {
            // TODO if (allowUpdates)
            {
                this.client.updateStatusLine();
                this.client.updateCaption();
            }
        }
    }
    
    //--------------------- Runtime Methods.

    /**
     * Reset to the initial state (before the maze is solved).
     */
    public void Reset()
    {
        painter.reset();

        painter.resetBlinkingCounter();

        // TODO: if (allowUpdates)
        {
            this.invalidate();
            this.repaint();
        }

        /* TODO
        // If the window is minimized, there will be no OnPaint() event.
        // Therefore we Paint the maze directly.
        // if (this.ParentForm.WindowState == FormWindowState.Minimized)
        {
            // TODO: Reset() is called twice but should be called only once.
            painter.PaintMaze(this.PaintImages);
        }
        */
    }

    //--------------------- IMazeCanvasProperties Implementation

    @Override
    public String getCode()
    {
        return (getMaze() == null ? "---" :
            //painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeCode :
            getMaze().getCode());
    }

    @Override
    public Maze getMaze()
    {
        return painter.getMaze();
    }
    
    @Override
    public int getXSize()
    {
        return (getMaze() == null ? -1 :
            //painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeDimensions.Width :
            getMaze().getXSize());
    }

    @Override
    public int getYSize()
    {
        return (getMaze() == null ? -1 :
            //painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeDimensions.Height :
            getMaze().getYSize());
    }

    //--------------------- IMazePainterClient Implementation

    @Override
    public boolean isAlive()
    {
        return true;
    }
    
    @Override
    public Graphics getDisplayGraphics()
    {
        // return this.getGraphics();
        return this.myGraphics;
    }

    /**
     * @return A Rectangle within the bounds of the Graphics that we are painting.
     * @see #getDisplayGraphics()
     */
    @Override
    public Rectangle getDisplayRectangle()
    {
        // This is for a Graphics painting to an off-screen image.
        Rectangle result = new Rectangle(0, 0, this.getWidth(), this.getHeight());
        result.grow(-2, -2); // TODO: 0 in screen saver mode
        return result;
    }
    
    @Override
    public void paintReservedAreas(Graphics g)
    {
        // TODO Auto-generated method stub
    }


    /**
     * Forward this method to our client.
     */
    @Override
    public void afterMazePainted()
    {
        if (client != null)
        {
            client.afterMazePainted();
        }
    }
    @Override
    public void Update()
    {
        // TODO Auto-generated method stub
    }
    
    @Override
    public Image createImage(int width, int height)
    {
        return super.createImage(width, height);
    }
    
    @Override
    public ImageObserver getImageObserver()
    {
        return this;
    }
    
    //--------------------- Canvas Implementation

    @Override
    public void paint(Graphics g)
    {
        System.out.println("MazeCanvas.paint, clip="+g.getClipBounds());
        
        myGraphics = g; // TODO: Pass this only as a parameter.
        painter.paint(g);

        if (false)
        {
            Rectangle rect = new Rectangle(0, 0, this.getWidth(), this.getHeight());
            
            g.setColor(Color.RED);
            g.drawLine(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
            g.drawLine(rect.x, rect.y + rect.height, rect.x + rect.width, rect.y);
        }
        
        painter.renderBufferedGraphics(g);
    }

    /**
     * Reserves a region of the maze covered by the coveringControl.
     * This MazeCanvas and the coveringControl must have the same Parent, i.e. a common coordinate system.
     * @param coveringComponent Another Component within the same Container.
     * @return True if the area below the component was reserved.
     */
    public boolean reserveArea(Component coveringComponent)
    {
        if (coveringComponent.getParent() != this.getParent())
        {
            throw new IllegalArgumentException("The given component must have the same Parent.");
        }

        // Location of the maze, as seen by the painter, in canvas coordinates.
        Point mazeLocation = getDisplayRectangle().getLocation();
        mazeLocation.x += this.getX();
        mazeLocation.y += this.getY();
        
        // Dimensions of the control in square coordinates.
        int x, y, w, h;
        int d = 3;
        x = painter.geometry.xCoordinate(coveringComponent.getX() - d - mazeLocation.x, true);
        y = painter.geometry.yCoordinate(coveringComponent.getY() - d - mazeLocation.y, true);

        w = 1 + painter.geometry.xCoordinate(coveringComponent.getX() + d - mazeLocation.x + coveringComponent.getWidth() - 1, false) - x;
        h = 1 + painter.geometry.yCoordinate(coveringComponent.getY() + d - mazeLocation.y + coveringComponent.getHeight() - 1, false) - y;

        if (0 < x && x + w < getMaze().getXSize())
        {
            w = 1 + (coveringComponent.getWidth() + painter.geometry.wallWidth + 2 * d) / painter.geometry.gridWidth;
        }
        if (0 < y && y + h < getMaze().getYSize())
        {
            h = 1 + (coveringComponent.getHeight() + painter.geometry.wallWidth + 2 * d) / painter.geometry.gridWidth;
        }

        System.out.println(String.format("MazeCanvas.ReserveArea(%d, %d) + (%d, %d)", x, y, w, h));
        boolean result = getMaze().reserveRectangle(x, y, w, h, null);

        // Move the control into the center of the reserved area.
        if (result)
        {
            int cx = coveringComponent.getX();
            int cy = coveringComponent.getY();

            if (0 < x && x + w < getMaze().getXSize())
            {
                cx = mazeLocation.x + painter.geometry.getSquareX(x);
                cx += 1 + (w * painter.geometry.gridWidth - painter.geometry.wallWidth - coveringComponent.getWidth()) / 2;
            }
            if (0 < y && y + h < getMaze().getYSize())
            {
                cx = mazeLocation.y + painter.geometry.getSquareY(y);
                cy += 1 + (h * painter.geometry.gridWidth - painter.geometry.wallWidth - coveringComponent.getHeight()) / 2;
            }
            
            // Adjust the control's location
            coveringComponent.setLocation(new Point(cx, cy));
            componentLocations.add(coveringComponent.getBounds());
        }

        return result;
    }
}
