using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms; // for the Timer; TODO: use System.Threading.Timer
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// Controls the timing and other activities of the running solver and painter.
    /// </summary>
    public class AriadneController
    {
        #region Member variables

        private IAriadneEventHandler client;

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        private ISolverController solverController;

        /// <summary>
        /// A timer that causes single solver steps.
        /// </summary>
        private Timer stepTimer;

        /// <summary>
        /// A timer that creates a new maze when repeatMode is set.
        /// </summary>
        private Timer repeatTimer;

        /// <summary>
        /// A timer for making the maze's end point blink.
        /// </summary>
        private Timer blinkTimer;

        /// <summary>
        /// Number of executed steps.
        /// </summary>
        private long CountSteps
        {
            get { return (solverController == null ? -1 : solverController.CountSteps); }
        }

        #region Timing and step rate

        /// <summary>
        /// Desired step rate.
        /// </summary>
        public int StepsPerSecond
        {
            get { return stepsPerSecond; }
            set
            {
                stepsPerSecond = Math.Max(1, Math.Min(40000, value));

                // Adjust scheduling parameters.
                secondsBeforeRateChange = accumulatedSeconds + lapSeconds;
                stepsBeforeRateChange = CountSteps;
            }
        }
        private int stepsPerSecond = 200;

        /// <summary>
        /// Time when start or pause button was pressed.
        /// </summary>
        private DateTime lapStartTime;

        /// <summary>
        /// Duration since lapStartTime.
        /// </summary>
        private double lapSeconds;

        /// <summary>
        /// Duration of previous laps (Start .. Pause).
        /// </summary>
        private double accumulatedSeconds;

        /// <summary>
        /// Number of steps that don't count in the current rate calculation.
        /// </summary>
        private long stepsBeforeRateChange;

        /// <summary>
        /// Duration that doesn't count in the current rate calculation.
        /// </summary>
        private double secondsBeforeRateChange;

        /// <summary>
        /// Start of a short-term interval.
        /// Used for setting a limit to the current step rate (e.g. 150% of the desired step rate).
        /// </summary>
        private DateTime hopStartTime = new DateTime(0);
        private const double hopDuration = 0.05; // seconds
        private const double maxHopSpeed = 1.20 * 1.50; // relative to desired step rate; 20% for overhead
        private long stepsBeforeThisHop = 0;

        #endregion

        /// <summary>
        /// When true, a new maze is automatically created after the current one has been solved.
        /// </summary>
        public bool RepeatMode
        {
            get { return repeatMode; }
            set { repeatMode = value; }
        }
        private bool repeatMode = false;

        /// <summary>
        /// Remembers the strategy name after the solver has been deleted.
        /// </summary>
        private string finishedStrategyName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AriadneController(IAriadneEventHandler client, ISolverController solverController)
        {
            this.client = client;
            this.solverController = solverController;

            stepsPerSecond = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND);
            stepsPerSecond = Math.Min(40000, Math.Max(1, stepsPerSecond));
        }

        #endregion

        #region Control commands corresponding to AriadneFormBase events

        /// <summary>
        /// Stops the solver and returns the maze to its original (unsolved) state.
        /// </summary>
        public void Reset()
        {
            Stop();
            ResetCounters();

            solverController.Reset();
            solverController.UpdateStatusLine();
        }

        /// <summary>
        /// Stops all timers.
        /// Call this method before a window is closed.
        /// </summary>
        public void Stop()
        {
            if (stepTimer != null)
            {
                stepTimer.Stop();
                stepTimer = null;
            }
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
            solverController.BlinkingCounter = 0;
        }

        /// <summary>
        /// Start a solver.
        /// </summary>
        public void Start()
        {
            //Log.WriteLine("{ Start()");
            stepTimer = new Timer();
            stepTimer.Interval = (1000/60); // 60 frames per second
            stepTimer.Tick += new EventHandler(this.OnStepTimer);
            stepTimer.Start();

            blinkTimer = new Timer();
            blinkTimer.Interval = 600; // ms
            blinkTimer.Tick += new EventHandler(this.OnBlinkTimer);
            if (RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BLINKING))
            {
                blinkTimer.Start();
            }

            ResetCounters();

            solverController.Start();
            this.finishedStrategyName = solverController.StrategyName;

            lapStartTime = System.DateTime.Now;
            //Log.WriteLine("} Start()");
        }

        /// <summary>
        /// Halt or continue the solver.
        /// </summary>
        public void Pause()
        {
            // Switch between Running and Paused.
            stepTimer.Enabled = !stepTimer.Enabled;

            if (State == SolverState.Running)
            {
                accumulatedSeconds += lapSeconds;
                lapSeconds = 0;
                lapStartTime = System.DateTime.Now;
            }
        }

        /// <summary>
        /// Execute a single solver step.
        /// </summary>
        public void SingleStep()
        {
            solverController.DoStep();
            solverController.FinishPath();

            // Each single step is assumed to take exactly the desired time.
            this.accumulatedSeconds += 1.0 / stepsPerSecond;

            solverController.UpdateStatusLine();

            if (solverController.IsFinished)
            {
                Finish();
            }
        }

        /// <summary>
        /// Should be called when the solver is finished.
        /// </summary>
        private void Finish()
        {
            stepTimer.Stop();
            stepTimer = null;
            blinkTimer.Stop();
            blinkTimer = null;
        }

        #endregion

        #region Timer event handlers

        /// <summary>
        /// Let the solver make enough steps to stay in schedule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStepTimer(object sender, EventArgs e)
        {
            // Ignore the message if the timer has been cancelled in the meantime.
            if (stepTimer == null || !stepTimer.Enabled || !client.Alive)
            {
                return;
            }

            if (State != SolverState.Running)
            {
                return;
            }

            try
            {
                // Stop the timer to prevent additional events while the solver is busy.
                // TODO: Leave the timer enabled; further events should be handled or ignored, as appropriate.
                stepTimer.Enabled = false;
                // State looks like Paused but this will be changed back at the end.

                if (!solverController.IsFinished)
                {
                    /* On a small maze or at low step rate, a few steps will be sufficient.
                     * 
                     * On a large maze, painting is slower; therefore, we render the GraphicsBuffer
                     * only every 20 steps: maxStepsBetweenRedraw.
                     * 
                     * The timer ticks only every few milliseconds.  That causes a small idle delay
                     * between the end of one OnStepTimer() event and the start of the next one.
                     * 
                     * For balancing between low responsiveness and high idle times, we keep looping
                     * for up to 1/2 second: maxMillisecondsPerTimerEvent.
                     */

                    // Repetition restrictions.
                    int maxStepsBetweenRedraw = Math.Max(20, stepsPerSecond / 60); // approx. 60Hz
                    int maxMillisecondsPerTimerEvent = 400;

                    DateTime currentStartTime = DateTime.Now;
                    TimeSpan elapsed = new TimeSpan(0);

                    while (elapsed.TotalMilliseconds < maxMillisecondsPerTimerEvent && this.IsBehindSchedule())
                    {
                        for (int steps = 0; steps < maxStepsBetweenRedraw && this.IsBehindSchedule(); )
                        {
                            steps += solverController.DoStep();
                        }

                        // Render the executed steps.
                        solverController.FinishPath();

                        elapsed = DateTime.Now - currentStartTime;
                    }
                }
            }
            finally
            {
                // Either restart or delete the timer.
                if (!solverController.IsFinished)
                {
                    stepTimer.Enabled = true;
                    // State is Running.
                }
                else
                {
                    Finish();
                    // State is Finished and may become Ready if someone creates a new Maze.
                }

                solverController.UpdateStatusLine();

                if (State == SolverState.Finished)
                {
                    // In repeat mode: Start a timer that will create a new maze.
                    if (this.repeatMode)
                    {
                        this.repeatTimer = new Timer();
                        repeatTimer.Interval = 3000; // ms
                        repeatTimer.Tick += new EventHandler(this.OnRepeatTimer);
                        repeatTimer.Start();

                        // Perform time consuming preparations for the next start, but first update the display.
                        client.Update();
                        client.PrepareForNextStart();
                    }

                    // This is a good moment to run the garbage collector.
                    solverController.ReleaseResources();
                    System.GC.Collect();
                }
            }
        }

        /// <summary>
        /// Increment the SolverController's BlinkingCounter.
        /// This makes the end square blink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBlinkTimer(object sender, EventArgs e)
        {
            // Ignore the message if the timer has been cancelled in the meantime.
            if (blinkTimer == null || !blinkTimer.Enabled || !client.Alive)
            {
                return;
            }

            this.solverController.BlinkingCounter = this.solverController.BlinkingCounter + 1;
        }

        /// <summary>
        /// Automatically create and start a new maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRepeatTimer(object sender, EventArgs e)
        {
            // Ignore the message if the timer has been cancelled in the meantime.
            if (repeatTimer == null || !repeatTimer.Enabled || !client.Alive)
            {
                return;
            }

            if (State == SolverState.Finished && this.repeatMode)
            {
                repeatTimer.Stop();

                // Save the current timer in a local variable.
                // Otherwise, it would be discarded in the Reset() method caused by OnNew().
                Timer tmp = repeatTimer;

                // 1. Create and draw a new maze.
                client.OnNew(null, null); // creates a new maze, invalidates the drawing area
                Application.DoEvents(); // paints the maze

                // Re-install the saved timer variable.
                repeatTimer = tmp;

                // 2. Wait a short time before starting the solver.
                repeatTimer.Interval = 1500; // ms
                repeatTimer.Start();

                // 3. Prepare time consuming actions before the SolverController is started.
                solverController.PrepareForStart();
            }
            else if (State == SolverState.Ready && this.repeatMode)
            {
                repeatTimer.Stop();
                this.Start();
            }
            else
            {
                repeatTimer.Stop();
                repeatTimer = null;
            }
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// </summary>
        /// <param name="message"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.ToString(System.String)")]
        public void FillStatusMessage(StringBuilder message)
        {
            solverController.FillStatusMessage(message);

            if (CountSteps > 0)
            {
                message.Append(" / ");
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
        }

        public string StrategyName
        {
            get 
            {
                string result = null;

                if (result == null && solverController != null)
                {
                    result = this.solverController.StrategyName;
                }
                if (result == null && State == SolverState.Finished)
                {
                    result = this.finishedStrategyName;
                }

                return result;
            }
        }

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        protected virtual void ResetCounters()
        {
            solverController.ResetCounters();
            client.ResetCounters();

            lapSeconds = accumulatedSeconds = 0;
            stepsBeforeRateChange = 0;
            secondsBeforeRateChange = 0;
        }

        /// <summary>
        /// Returns true while we have not executed enough steps to achieve the desired step rate.
        /// </summary>
        /// <returns></returns>
        private bool IsBehindSchedule()
        {
            if (State == SolverState.Finished)
            {
                return false;
            }

            DateTime now = System.DateTime.Now;

            #region Limit current short-term speed

            TimeSpan hop = now - hopStartTime;
            if (hop.TotalSeconds > hopDuration)
            {
                hopStartTime = now;
                stepsBeforeThisHop = CountSteps;
            }
            else if (CountSteps - stepsBeforeThisHop > (long)(stepsPerSecond * hopDuration * maxHopSpeed))
            {
                return false;
            }

            #endregion

            // TimeSpan since last Start or Continue.
            TimeSpan lap = now - lapStartTime;
            this.lapSeconds = lap.TotalSeconds;

            // Duration for which the desired step rate should be achieved.
            double relevantSeconds = (accumulatedSeconds + lapSeconds) - secondsBeforeRateChange;

            // Number of steps that should have been achieved.
            double scheduledSteps = (relevantSeconds * stepsPerSecond) + stepsBeforeRateChange;

            return (CountSteps < 1 + scheduledSteps);
        }

        /// <summary>
        /// Gets the current SolverState.
        /// </summary>
        public SolverState State
        {
            get
            {
                // If the maze is solved, we are Finished.
                if (solverController != null && solverController.IsFinished)
                {
                    return SolverState.Finished;
                }

                // While there is no timer, we are Ready to create one and start it.
                if (stepTimer == null)
                {
                    return SolverState.Ready;
                }

                // So we are either running or paused.
                if (stepTimer.Enabled)
                {
                    return SolverState.Running;
                }
                else
                {
                    return SolverState.Paused;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} AriadneController: {1} steps, {2}",
                StrategyName,
                this.CountSteps,
                this.State.ToString());
        }

        #endregion
    }
}