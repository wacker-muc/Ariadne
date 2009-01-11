package swa.ariadne.ctrl;

/**
 * Comprises the {@link SolverController} methods that an {@link AriadneController} requires. 
 * 
 * @author Stephan.Wacker@web.de
 */
public interface ISolverController
{
    void resetBlinkingCounter();
    void advanceBlinkingCounter();
    //long CountSteps();
    //int DoStep();

    /**
     * Write status information to the given StringBuilder.
     * @param message Where the string is collected.
     */
    void FillStatusMessage(StringBuilder message);
    
    //void FinishPath();

    /**
     * @return True if the maze has been solved.
     */
    boolean IsFinished();

    /**
     * Complex and memory consuming resources may be prepared before the controller is started.
     */
    void PrepareForStart();
    
    /**
     * When the controller is Ready or Finished, memory consuming resources should be deallocated.
     */
    void ReleaseResources();
    
    /**
     * Stops the solver and returns the maze to its original (unsolved) state.
     */
    void Reset();
    //void ResetCounters();

    /**
     * Starts a solver.
     */
    void Start();

    /**
     *  @return The name of the current solver strategy.
     */
    String StrategyName();
    
    /**
     *  Displays information about the running MazeSolver in the status line.
     */
    void UpdateStatusLine();
}
