using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
{
    /// <summary>
    /// Base class for Ariadne Form windows.
    /// </summary>
    public partial class AriadneFormBase : Form
        , IMazeForm
    {
        #region Member variables

        /// <summary>
        /// The object that accepts the MazeControl commands.
        /// </summary>
        protected virtual IMazeControlProperties MazeControlProperties
        {
            get { return null; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        protected virtual ISolverController SolverController
        {
            get { return null; }
        }


        /// <summary>
        /// The object that accepts the AriadneSettingsSource commands.
        /// </summary>
        protected virtual IAriadneSettingsSource AriadneSettingsSource
        {
            get { return null; }
        }

        /// <summary>
        /// A timer that causes single solver steps.
        /// </summary>
        protected Timer stepTimer;

        /// <summary>
        /// A timer that creates a new maze when repeatMode is set.
        /// </summary>
        protected Timer repeatTimer;

        /// <summary>
        /// A timer for making the maze's end point blink.
        /// </summary>
        protected Timer blinkTimer;

        /// <summary>
        /// Number of executed steps.
        /// </summary>
        protected long CountSteps
        {
            get { return (SolverController == null ? -1 : SolverController.CountSteps); }
        }

        #region Timing and step rate

        /// <summary>
        /// Desired step rate.
        /// </summary>
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

        protected bool repeatMode = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AriadneFormBase()
        {
            InitializeComponent();

            #region Initialize the stepsPerSecTextBox

            stepsPerSecond = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_STEPS_PER_SECOND, 200);
            stepsPerSecond = Math.Min(40000, Math.Max(20, stepsPerSecond));
            stepsPerSecTextBox.Text = stepsPerSecond.ToString();

            #endregion

            #region Hide some controls that are not used in all base classes

            strategyComboBox.Visible = false;
            visitedProgressBar.Visible = false;

            #endregion
        }

        #endregion

        #region Event handlers

        #region Maze controls

        /// <summary>
        /// Stops the solver and returns the maze to its original (unsolved) state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnReset(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                if (State == SolverState.Paused)
                {
                    this.OnPause(sender, e);
                }
                if (State == SolverState.Running)
                {
                    stepTimer.Stop();
                    blinkTimer.Stop();
                    this.SolverController.BlinkingCounter = 0;
                }
                stepTimer = null;
                blinkTimer = null;
            }

            FixStateDependantControls(this.State);
            ResetCounters();
            SolverController.UpdateStatusLine();
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnNew(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Opens a Details dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDetails(object sender, EventArgs e)
        {
            // This is not allowed while the Solver is busy.
            if (State == SolverState.Running || State == SolverState.Paused)
            {
                return;
            }

            DetailsDialog form = new DetailsDialog(this.AriadneSettingsSource);
            form.ShowDialog(this);

            // What needs to be done if the dialog has caused a State change?
            FixStateDependantControls(this.State);
        }

        /// <summary>
        /// Opens an About box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbout(object sender, EventArgs e)
        {
            Form form = new AboutBox();
            form.ShowDialog();
        }

        private void OnOpenArena(object sender, EventArgs e)
        {
            ArenaForm arena = new ArenaForm();
            arena.Icon = this.Icon;
            arena.Show();
        }

        #endregion

        #region Solver controls

        /// <summary>
        /// Start a solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        protected virtual void OnStart(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                //OnReset(sender, e);
                return;
            }

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

            FixStateDependantControls(this.State);
            ResetCounters();

            SolverController.Start();

            lapStartTime = System.DateTime.Now;
        }

        /// <summary>
        /// Let the solver make enough steps to stay in schedule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStepTimer(object sender, EventArgs e)
        {
            if (State != SolverState.Running)
            {
                return;
            }

            try
            {
                // Stop the timer to prevent additional events while the solver is busy.
                stepTimer.Enabled = false;
                // State looks like Paused but this will be changed back at the end.

                if (!MazeControlProperties.IsSolved)
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
                        for (int steps = 0; steps < maxStepsBetweenRedraw && this.IsBehindSchedule(); ++steps)
                        {
                            SolverController.DoStep();
                        }

                        // Render the executed steps.
                        SolverController.FinishPath();

                        elapsed = DateTime.Now - currentStartTime;
                    }
                }
            }
            finally
            {
                // Either restart or delete the timer.
                if (!MazeControlProperties.IsSolved)
                {
                    stepTimer.Enabled = true;
                    // State is Running.
                }
                else
                {
                    stepTimer = null;
                    blinkTimer.Enabled = false;
                    blinkTimer = null;
                    // State is Finished and may become Ready if someone creates a new Maze.
                }

                SolverController.UpdateStatusLine();

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
                        this.Update();
                        PrepareForNextStart();
                    }

                    // This is a good moment to run the garbage collector.
                    SolverController.ReleaseResources();
                    System.GC.Collect();
                }
            }
        }

        protected virtual void PrepareForNextStart()
        {
            // do nothing
        }

        /// <summary>
        /// Increment the SolverController's BlinkingCounter.
        /// This makes the end square blink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBlinkTimer(object sender, EventArgs e)
        {
            this.SolverController.BlinkingCounter = this.SolverController.BlinkingCounter + 1;
        }

        /// <summary>
        /// Halt or continue the solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPause(object sender, EventArgs e)
        {
            if (State != SolverState.Running && State != SolverState.Paused)
            {
                return;
            }

            // Switch between Running and Paused.
            stepTimer.Enabled = !stepTimer.Enabled;

            if (State == SolverState.Running)
            {
                accumulatedSeconds += lapSeconds;
                lapSeconds = 0;
                lapStartTime = System.DateTime.Now;

                this.pauseLabel.ToolTipText = "Pause";
                this.pauseLabel.BorderStyle = Released;
                this.pauseLabel.Tag = "";
            }
            else
            {
                this.pauseLabel.ToolTipText = "Continue";
                this.pauseLabel.BorderStyle = Pressed;
                this.pauseLabel.Tag = "sunken";
            }

            this.pauseLabel.ToolTipText += " [P]";
        }

        /// <summary>
        /// Execute a single solver step.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStep(object sender, EventArgs e)
        {
            if (State == SolverState.Ready)
            {
                this.OnStart(sender, e);
                this.OnPause(sender, e);
            }

            if (State != SolverState.Paused)
            {
                return;
            }

            SolverController.DoStep();
            SolverController.FinishPath();

            SolverController.UpdateStatusLine();
        }

        /// <summary>
        /// Toggle the repeat mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRepeat(object sender, EventArgs e)
        {
            // Switch between Running and Paused.
            this.repeatMode = !this.repeatMode;

            if (this.repeatMode == false)
            {
                this.repeatLabel.BorderStyle = Released;
                this.repeatLabel.Tag = "";
            }
            else
            {
                this.repeatLabel.BorderStyle = Pressed;
                this.repeatLabel.Tag = "sunken";
            }
        }

        /// <summary>
        /// Automatically create and start a new maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRepeatTimer(object sender, EventArgs e)
        {
            if (State == SolverState.Finished && this.repeatMode)
            {
                repeatTimer.Stop();

                // 1. Create and draw a new maze.
                this.OnNew(null, null); // creates a new maze, invalidates the drawing area
                Application.DoEvents(); // paints the maze

                // 2. Wait a short time before starting the solver.
                repeatTimer.Interval = 1500; // ms
                repeatTimer.Start();

                // 3. Prepare time consuming actions before the SolverController is started.
                SolverController.PrepareForStart();
            }
            else if (State == SolverState.Ready && this.repeatMode)
            {
                repeatTimer.Stop();
                this.OnStart(null, null);
            }
            else
            {
                repeatTimer.Stop();
                repeatTimer = null;
            }
        }

        #endregion

        #region Keyboard controls

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (char.ToUpper(e.KeyChar))
            {
                case (char)Keys.Escape:
                    OnReset(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.Enter:
                    OnStart(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.P:
                    OnPause(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.OemPeriod:
                case '.':
                    OnStep(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.N:
                    OnNew(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.Oemplus:
                case '+':
                    OnRepeat(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.M:
                    this.menuButton.ShowDropDown();
                    break;
            }
        }

        #endregion

        #region Button pressed behavior

        private ToolStripStatusLabel trackedLabel;
        private const Border3DStyle Pressed = Border3DStyle.SunkenOuter;
        private const Border3DStyle Released = Border3DStyle.RaisedInner;

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.trackedLabel = (ToolStripStatusLabel)sender;
                SwitchTrackedLabel(Released, Pressed);
            }
        }

        private void SwitchTrackedLabel(Border3DStyle regular, Border3DStyle alternate)
        {
            bool hasSunkenTag = false;
            try
            {
                hasSunkenTag = ("sunken" == (string)trackedLabel.Tag);
            }
            catch (InvalidCastException) { }

            trackedLabel.BorderStyle = (hasSunkenTag ? regular : alternate);
            this.statusStrip.Refresh();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (sender == trackedLabel)
            {
                SwitchTrackedLabel(Pressed, Released);
            }
            trackedLabel = null;
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (sender == trackedLabel)
            {
                SwitchTrackedLabel(Released, Pressed);
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (sender == trackedLabel)
            {
                SwitchTrackedLabel(Pressed, Released);
                trackedLabel = null;
            }
        }

        #endregion

        #region Parameter settings

        private void stepsPerSec_TextChanged(object sender, EventArgs e)
        {
            int value = this.stepsPerSecond;

            try
            {
                value = Int32.Parse(stepsPerSecTextBox.Text);
            }
            catch (Exception) { }

            value = Math.Max(1, Math.Min(40000,value));

            if (stepsPerSecTextBox.Text != value.ToString())
            {
                //stepsPerSecTextBox.Text = value.ToString();
            }

            this.stepsPerSecond = value;
            UpdateCaption();

            // Adjust scheduling parameters.
            secondsBeforeRateChange = accumulatedSeconds + lapSeconds;
            stepsBeforeRateChange = CountSteps;
        }

        protected virtual void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public virtual void MakeReservedAreas(Maze maze)
        {
            // subclasses might add code here
        }

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public virtual void UpdateStatusLine()
        {
            StringBuilder message = new StringBuilder(200);

            FillStatusMessage(message);

            this.statusLabel.Text = message.ToString();
        }

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// Derived classes should call their base class' method.
        /// </summary>
        /// <param name="message"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.ToString(System.String)")]
        protected void FillStatusMessage(StringBuilder message)
        {
            SolverController.FillStatusMessage(message);

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

        /// <summary>
        /// Displays Maze and Solver characteristics in the window's caption bar.
        /// The maze ID, step rate and solver strategy name.
        /// </summary>
        public virtual void UpdateCaption()
        {
            StringBuilder caption = new StringBuilder(80);

            FillCaption(caption);

            this.Text = caption.ToString();
        }

        /// <summary>
        /// Write a caption text to the given StringBuilder.
        /// Derived classes should call their base class' method.
        /// </summary>
        /// <param name="caption"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        protected virtual void FillCaption(StringBuilder caption)
        {
            caption.Append("Ariadne");

            if (true)
            {
                caption.Append(" - ");
                caption.Append(this.StrategyName);
            }

            if (MazeControlProperties != null)
            {
                caption.Append(" - ");
                caption.Append(MazeControlProperties.XSize.ToString() + "x" + MazeControlProperties.YSize.ToString());
            }

            if (true)
            {
                caption.Append(" - ");
                caption.Append(stepsPerSecond.ToString());
            }

            if (MazeControlProperties != null)
            {
                caption.Append(" - ");
                caption.Append("ID: " + MazeControlProperties.Code);
            }
        }

        public virtual string StrategyName
        {
            get 
            {
                try
                {
                    return this.SolverController.StrategyName;
                }
                catch (NullReferenceException) // in Ready state: there is no solver
                {
                    try
                    {
                        return this.strategyComboBox.SelectedItem.ToString();
                    }
                    catch (NullReferenceException) // in designer mode or on initial startup: there is no combo box, yet
                    {
                        return "...";
                    }
                }
            }
        }

        /// <summary>
        /// Enables or disables some controls depending on whether we are Ready or not.
        /// </summary>
        public virtual void FixStateDependantControls(SolverState state)
        {
            // do nothing
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        protected virtual void ResetCounters()
        {
            SolverController.ResetCounters();

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
        /// The states a SolverController may be in.
        /// </summary>
        public enum SolverState
        {
            Ready,
            Running,
            Paused,
            Finished,
        }

        /// <summary>
        /// Gets the current SolverState.
        /// </summary>
        protected SolverState State
        {
            get
            {
                // If the maze is solved, we are Finished.
                if (MazeControlProperties.IsSolved)
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

        #endregion
    }
}