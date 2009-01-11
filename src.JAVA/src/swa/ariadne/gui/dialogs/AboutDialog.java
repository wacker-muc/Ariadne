package swa.ariadne.gui.dialogs;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Window;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;

import javax.swing.JButton;
import javax.swing.JDesktopPane;
import javax.swing.JFrame;
import javax.swing.JLayeredPane;
import javax.swing.SwingUtilities;

import swa.ariadne.gui.maze.IMazeCanvasClient;
import swa.ariadne.gui.maze.MazeCanvas;
import swa.ariadne.gui.maze.MazeGeometry;
import swa.ariadne.logic.IMazeSolver;
import swa.ariadne.logic.factory.SolverFactory;
import swa.ariadne.model.Maze;
import swa.util.RandomFactory;

/**
 * The Ariadne application's "About" dialog.
 * <p>
 * This code was edited or generated using CloudGarden's Jigloo
 * SWT/Swing GUI Builder, which is free for non-commercial
 * use. If Jigloo is being used commercially (ie, by a corporation,
 * company or business for any purpose whatever) then you
 * should purchase a license for each developer using Jigloo.
 * Please visit www.cloudgarden.com for details.
 * Use of Jigloo implies acceptance of these licensing terms.
 * A COMMERCIAL LICENSE HAS NOT BEEN PURCHASED FOR
 * THIS MACHINE, SO JIGLOO OR THIS CODE CANNOT BE USED
 * LEGALLY FOR ANY CORPORATE OR COMMERCIAL PURPOSE.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class AboutDialog
extends javax.swing.JDialog
implements IMazeCanvasClient
{
    //--------------------- Constants
    
    /** Required attribute of a serializable class. */
    private static final long serialVersionUID = 4390160174089433404L;

    //--------------------- Member Variables and Properties

    /** A common container for all other components. */
    private JDesktopPane jDesktopPane1;

    /** The Canvas displaying a Maze. */
    private MazeCanvas mazeCanvas;

    /** Will open another panel with detailed information. */
    private JButton moreButton;
    /** Will close the panel. */
    private JButton okButton;

    /** When true, the authorButton is displayed, otherwise the aboutPanel. */
    private boolean displayAuthorButton = false;

    //--------------------- Main Method

    /**
     * Auto-generated main method to display this JDialog.
     * @param args Ignored. 
     */
    public static void main(String[] args)
    {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                JFrame frame = new JFrame();
                AboutDialog inst = new AboutDialog(frame);
                inst.setVisible(true);
            }
        });
    }
    
    //--------------------- Constructor and Initialization

    /**
     * Constructor.
     * @param frame The root window of this Dialog.
     */
    public AboutDialog(JFrame frame)
    {
        super(frame);
        initGUI();

        /* TODO
        this.Text = String.Format("About {0}", AssemblyProduct);
        this.labelProductName.Text = AssemblyProduct;
        this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
        this.labelCopyright.Text = AssemblyCopyright;
        */
    }
    
    /**
     * Initialize the attributes of this component.
     */
    private void initGUI()
    {
        System.out.println("AboutDialog.initGUI");

        try
        {
            this.setSize(369, 206);
            this.setLocale(new java.util.Locale("en", "US"));
            this.setFocusableWindowState(false);
            this.setModal(true);
            BorderLayout thisLayout = new BorderLayout();
            getContentPane().setLayout(thisLayout);
            this.setResizable(false);
            //this.setUndecorated(true);
            this.addWindowListener(new WindowAdapter() {
                public void windowOpened(WindowEvent evt) {
                    thisWindowOpened(evt);
                }
            });
            {
                jDesktopPane1 = new JDesktopPane();
                getContentPane().add(jDesktopPane1, BorderLayout.CENTER);
                jDesktopPane1.setBounds(8, 8, 315, 162);
                {
                    mazeCanvas = new MazeCanvas();
                    mazeCanvas.setClient(this);
                    jDesktopPane1.add(mazeCanvas, JLayeredPane.DEFAULT_LAYER);
                    mazeCanvas.setBounds(12, 12, 305, 154);
                    mazeCanvas.setLocation(new java.awt.Point(12, 12));
                    mazeCanvas.addMouseListener(new MouseAdapter() {
                        public void mouseClicked(MouseEvent evt) {
                            mazeCanvasMouseClicked(evt);
                        }
                    });
                }
                {
                    moreButton = new JButton();
                    jDesktopPane1.add(moreButton, JLayeredPane.PALETTE_LAYER);
                    moreButton.setText("More");
                    moreButton.setBounds(282, 12, 70, 20);
                }
                {
                    okButton = new JButton();
                    jDesktopPane1.add(okButton, JLayeredPane.PALETTE_LAYER);
                    okButton.setText("OK");
                    okButton.setBounds(282, 146, 70, 20);
                    okButton.addActionListener(new ActionListener() {
                        public void actionPerformed(ActionEvent evt) {
                            okButtonActionPerformed(evt);
                        }
                    });
                }
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    //--------------------- Behavior

    /**
     * Creates a new maze.
     * The maze will automatically be painted from within the {@link MazeCanvas#setup(MazeGeometry)} method.
     * From there, the {@link #afterMazePainted()} will be called.
     */
    private void makeNewMaze()
    {
        System.out.println("AboutDialog.paintNewMaze");

        // Create a maze with a fixed geometry.
        MazeGeometry geometry = new MazeGeometry(5, 2, 3);
        geometry = new MazeGeometry(5 + RandomFactory.nextInt(12 - 5 + 1), true);
        //geometry = new MazeGeometry(11, true);
        mazeCanvas.setup(geometry);
    }
    
    //--------------------- Event Handlers
    
    
    /**
     * Paints a different maze.
     * @param evt
     */
    private void mazeCanvasMouseClicked(MouseEvent evt)
    {
        System.out.println("AboutDialog.mazeCanvas.mouseClicked, event="+evt);
        this.makeNewMaze();
    }
    
    /**
     * Closes the dialog.
     * @param evt
     */
    private void okButtonActionPerformed(ActionEvent evt)
    {
        System.out.println("AboutDialog.okButton.actionPerformed, event="+evt);
        
        Container w = this.okButton.getTopLevelAncestor();
        if (w != null && Window.class.isInstance(w))
        {
            // @see http://forums.java.net/jive/thread.jspa?messageID=254885
            w.getToolkit().getSystemEventQueue().postEvent(new WindowEvent((Window)w, WindowEvent.WINDOW_CLOSING));
        }
    }
    
    /**
     * Is called after the window has been opened, but before the components have been painted.
     * <p>
     * Here we create a new Maze.
     * @param evt
     */
    private void thisWindowOpened(WindowEvent evt)
    {
        System.out.println("AboutDialog.windowOpened, event="+evt);

        this.makeNewMaze();
    }
    
    //--------------------- IMazeCanvasClient Implementation
    
    @Override
    public void makeReservedAreas(Maze maze)
    {
        mazeCanvas.reserveArea(this.okButton);
        mazeCanvas.reserveArea(this.moreButton);
        
        /* TODO
        if (displayAuthorButton)
        {
            this.outerAboutPanel.SendToBack();
            this.authorButton.BringToFront();
            mazeCanvas.ReserveArea(this.authorButton);
        }
        else
        {
            this.authorButton.SendToBack();
            this.outerAboutPanel.BringToFront();
            mazeCanvas.ReserveArea(this.outerAboutPanel);
        }
        */
    }

    @Override
    public String getStrategyName()
    {
        // TODO Auto-generated method stub
        return null;
    }

    @Override
    public void updateCaption()
    {
        // TODO Auto-generated method stub
        
    }

    @Override
    public void updateStatusLine() {
        // TODO Auto-generated method stub
        
    }

    @Override
    public void afterMazePainted()
    {
        System.out.println("AboutDialog.afterMazePainted");

        // Solve the maze.
        IMazeSolver solver = SolverFactory.createDefaultSolver(mazeCanvas.getMaze(), mazeCanvas.getMazePainter());
        solver.reset();
        solver.solve();
     
        mazeCanvas.repaint();
    }
}
