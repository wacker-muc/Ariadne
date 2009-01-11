package swa.ariadne.ctrl;

import java.util.List;

import swa.ariadne.gui.maze.*;
import swa.ariadne.logic.IMazeSolver;
import swa.ariadne.logic.factory.SolverFactory;
import swa.ariadne.model.Maze;
import swa.ariadne.model.MazeSquare;

/**
 * A controller for a {@link IMazeSolver MazeSolver} and a {@link MazeCanvas}.
 * 
 * @author Stephan.Wacker@web.de
 */
public
class SolverController
implements ISolverController
{
    //--------------------- Member Variables and Properties

    /** The object knowing the selected solver strategy. */
    private IMazeCanvasClient mazeForm; // TODO: canvasClient

    /** The MazePainter. */
    private MazePainter mazePainter;

    /**
     * The maze solver algorithm.
     * This is only set (not null) while we are in a solving state.
     */
    private IMazeSolver solver;
    
    /** The path that will be highlighted when the solution is complete. */
    private List<MazeSquare> solutionPath;

    /**
     *  @return The maze we are working with.
     *  @see MazePainter#getMaze()
     */
    public Maze Maze()
    {
        return mazePainter.getMaze();
    }
    
    //--------------------- Constructor and Initialization
    
    /**
     * Constructor.
     * @param mazeForm The container of the MazeCanvas.
     * @param mazePainter The MazePainter for that MazeCanvas.
     */
    public SolverController(IMazeCanvasClient mazeForm, MazePainter mazePainter /* TODO: , ProgressBar visitedProgressBar */)
    {
        this.mazeForm = mazeForm;
        this.mazePainter = mazePainter;
        // TODO: this.visitedProgressBar = visitedProgressBar;
    }

    //--------------------- ISolverController Implementation

    @Override
    public void Reset()
    {
        ReleaseResources();

        /* TODO
        if (visitedProgressBar != null)
        {
            visitedProgressBar.Value = 0;
        }
        */
    }

    @Override
    public void ReleaseResources()
    {
        /* TODO
        solver = null;
        solutionPath = null;
        embeddedControllers.Clear();
        */
    }

    @Override
    public void advanceBlinkingCounter()
    {
        mazePainter.advanceBlinkingCounter();
    }

    @Override
    public void resetBlinkingCounter() 
    {
        mazePainter.resetBlinkingCounter();
    }

    @Override
    public void PrepareForStart()
    {
        // Prepare the solver.
        if (mazeForm != null)
        {
            solver = SolverFactory.createSolver(mazeForm.getStrategyName(), this.Maze(), this.mazePainter);
        }
        else
        {
            solver = SolverFactory.createSolver(null, this.Maze(), mazePainter);
        }

        // Prepare the solution path.
        solutionPath = SolverFactory.getSolutionPath(this.Maze());

        /* TODO
        // If our Maze has embedded mazes, we need to supply them with embedded solvers.
        CreateEmbeddedSolvers();

        // Prepare embedded solvers.
        for (EmbeddedSolverController item : embeddedControllers)
        {
            item.PrepareForStart();
        }

        // After all controllers have been created, some resources need to be shared with the master controller.
        if (this.Maze.MazeId == MazeSquare.PrimaryMazeId)
        {
            CoordinateEmbeddedControllers(this);
        }
        */
    }

    @Override
    public void Start()
    {
        if (solver == null)
        {
            PrepareForStart();
        }
        if (mazeForm != null)
        {
            mazeForm.updateCaption();
        }
    }

    @Override
    public boolean IsFinished()
    {
        /* TODO
        if (mazePainter.HasBufferAlternate)
        {
            // When the mazePainter has prepared an alternate buffer, its maze is already the new maze.
            // TODO: Avoid this quirk.
            return true;
        }
        else */ if (Maze() != null)
        {
            return Maze().isFinished();
        }
        else
        {
            return false;
        }
    }

    @Override
    public void UpdateStatusLine()
    {
        if (mazeForm != null)
        {
            mazeForm.updateStatusLine();
        }
    }

    @Override
    public void FillStatusMessage(StringBuilder message)
    {
        /* TODO
        long totalSteps = this.CountSteps;
        long ownSteps = this.countSteps;
        long embeddedSteps = totalSteps - ownSteps;

        if (totalSteps > 0)
        {
            string steps = (totalSteps == 1 ? "step" : "steps");
            message.Append(ownSteps.ToString("#,##0"));
            if (embeddedSteps > 0)
            {
                message.Append(" + " + embeddedSteps.ToString("#,##0"));
            }
            message.Append(" " + steps);

            if (countBackward > 0)
            {
                message.Append(", "
                    + countForward.ToString("#,##0") + " forward, "
                    + countBackward.ToString("#,##0") + " backward"
                    );
            }

            if (solver != null)
            {
                solver.FillStatusMessage(message);
            }
        }
        */
    }

    @Override
    public String StrategyName()
    {
        String result = null;

        /* TODO
        if (this.solver != null)
        {
            result = (this.solver.IsEfficientSolver ? SolverFactory.EfficientPrefix : "") + this.solver.GetType().Name;
        }
        */

        return result;
    }
}
