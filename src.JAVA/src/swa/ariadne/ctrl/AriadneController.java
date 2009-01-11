package swa.ariadne.ctrl;

import java.util.Date;
import java.util.Timer;
import java.util.TimerTask;

/**
 * Controls the timing and other activities of the running solver and painter.
 * 
 * @author Stephan.Wacker@web.de
 */
public class AriadneController
{
    //--------------------- Member Variables and Properties

    /** The object that is in charge of the application. */
    private IAriadneEventHandler client;

    /** The object that controls the MazeSolver. */
    private ISolverController solverController;
    
    //--------------------- Timers

    /**
     * A timer that causes single solver steps.
     * It is created in the Start() method and deleted when the maze is finished. 
     */
    private Timer stepTimer;
    
    /** True if the stepTimer is temporarily paused. */
    private boolean stepTimerPaused = false;

    //--------------------- Timing and Step Rate

    /** Time when start or pause button was pressed. */
    private Date lapStartTime;

    //--------------------- Constructor and Initialization

    /**
     * Constructor.
     * @param client
     * @param solverController
     */
    public AriadneController(IAriadneEventHandler client, ISolverController solverController)
    {
        this.client = client;
        this.solverController = solverController;

        /* TODO
        stepsPerSecond = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND);
        stepsPerSecond = Math.min(40000, Math.max(1, stepsPerSecond));
        */
    }

    //--------------------- Control commands corresponding to AriadneFrameBase events

    /**
     * Stops the solver and returns the maze to its original (unsolved) state.
     */
    public void Reset()
    {
        Stop();
        ResetCounters();

        solverController.Reset();
        solverController.UpdateStatusLine();
    }

    /**
     * Stops all timers.
     * Call this method before a window is closed.
     */
    public void Stop()
    {
        if (stepTimer != null)
        {
            stepTimer.cancel();
            stepTimer = null;
        }
        /* TODO
        if (blinkTimer != null)
        {
            blinkTimer.Stop();
            blinkTimer = null;
        }
        if (repeatTimer != null)
        {
            repeatTimer.Stop();
            repeatTimer = null;
        }
        */
        
        solverController.resetBlinkingCounter();
    }
    
    /**
     * Starts a solver.
     */
    public void Start()
    {
        //Log.WriteLine("{ Start()");
        stepTimer = new Timer();
        stepTimerPaused = false;
        /* TODO
        stepTimer.Interval = (1000/60); // 60 frames per second
        stepTimer.Tick += new EventHandler(this.OnStepTimer);
        stepTimer.Start();
        */

        /* TODO
        blinkTimer = new Timer();
        blinkTimer.Interval = 600; // ms
        blinkTimer.Tick += new EventHandler(this.OnBlinkTimer);
        if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING))
        {
            blinkTimer.Start();
        }
        */

        ResetCounters();

        solverController.Start();
        // TODO this.finishedStrategyName = solverController.StrategyName;

        lapStartTime = new Date(); // TODO: now
        //Log.WriteLine("} Start()");
    }

    /**
     * Halt or continue the solver.
     */
    public void Pause()
    {
        // Switch between Running and Paused.
        stepTimerPaused = !stepTimerPaused;

        if (State() == SolverState.Running)
        {
            /* TODO
            accumulatedSeconds += lapSeconds;
            lapSeconds = 0;
            lapStartTime = System.DateTime.Now;
            */
        }
    }
    
    /**
     * Reset step and runtime counters.
     */
    protected void ResetCounters()
    {
        /* TODO
        solverController.ResetCounters();
        client.ResetCounters();

        lapSeconds = accumulatedSeconds = 0;
        stepsBeforeRateChange = 0;
        secondsBeforeRateChange = 0;
        */
    }
    
    //--------------------- Solver State

    /**
     * @return The current SolverState.
     */
    public SolverState State()
    {
        // If the maze is solved, we are Finished.
        if (solverController != null && solverController.IsFinished())
        {
            return SolverState.Finished;
        }

        // While there is no timer, we are Ready to create one and start it.
        if (stepTimer == null)
        {
            return SolverState.Ready;
        }

        // So we are either running or paused.
        if (stepTimerPaused)
        {
            return SolverState.Paused;
        }
        else
        {
            return SolverState.Running;
        }
    }

    //--------------------- Auxiliary methods

    /**
     * Write status information to the given StringBuilder.
     * @param message Where the string is collected.
     */
    public void FillStatusMessage(StringBuilder message)
    {
        solverController.FillStatusMessage(message);

        /* TODO
        if (CountSteps > 0)
        {
            message.append(" / ");
            double totalSeconds = accumulatedSeconds + lapSeconds;
            message.Append(totalSeconds.ToString("#,##0.00") + " sec");

            double sps = (CountSteps - stepsBeforeRateChange) / (totalSeconds - secondsBeforeRateChange);
            if (stepsBeforeRateChange > 0)
            {
                message.Append(" = [" + sps.ToString("#,##0") + "] steps/sec");
            }
            else
            {
                message.Append(" = " + sps.ToString("#,##0") + " steps/sec");
            }
        }
        */
    }
    
    /** @return The name of the current solver strategy. */
    public String StrategyName()
    {
        String result = null;

        if (result == null && solverController != null)
        {
            result = this.solverController.StrategyName();
        }
        /* TODO
        if (result == null && State() == SolverState.Finished)
        {
            result = this.finishedStrategyName;
        }
        */

        return result;
    }
}
