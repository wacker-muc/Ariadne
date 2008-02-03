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

namespace SWA.Ariadne.App
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
        /// Number of executed steps.
        /// </summary>
        protected long countSteps
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

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AriadneFormBase()
        {
            InitializeComponent();

            #region Initialize the stepsPerSecTextBox

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
                }
                stepTimer = null;
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
            AboutBox form = new AboutBox();
            form.ShowDialog();
        }

        private void OnOpenArena(object sender, EventArgs e)
        {
            ArenaForm arena = new ArenaForm();
            arena.Show();
        }

        #endregion

        #region Solver controls

        /// <summary>
        /// Start a solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    // State is Finished and may become Ready if someone creates a new Maze.
                }

                SolverController.UpdateStatusLine();
            }
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
            stepsBeforeRateChange = countSteps;
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
        public void UpdateStatusLine()
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
        protected void FillStatusMessage(StringBuilder message)
        {
            SolverController.FillStatusMessage(message);

            if (countSteps > 0)
            {
                message.Append(" / ");
                double totalSeconds = accumulatedSeconds + lapSeconds;
                message.Append(totalSeconds.ToString("#,##0.00") + " sec");

                double sps = (countSteps - stepsBeforeRateChange) / (totalSeconds - secondsBeforeRateChange);
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
        public void UpdateCaption()
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
                    return this.strategyComboBox.SelectedItem.ToString();
                }
                catch (NullReferenceException)
                {
                    return "...";
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
        private void ResetCounters()
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

            // TimeSpan since last Start or Continue.
            TimeSpan lap = System.DateTime.Now - lapStartTime;
            this.lapSeconds = lap.TotalSeconds;

            // Duration for which the desired step rate should be achieved.
            double relevantSeconds = (accumulatedSeconds + lapSeconds) - secondsBeforeRateChange;

            // Number of steps that should have been achieved.
            double scheduledSteps = (relevantSeconds * stepsPerSecond) + stepsBeforeRateChange;

            return (countSteps < 1 + scheduledSteps);
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
                // While there is no timer, we are Ready to create one and start it.
                if (stepTimer == null)
                {
                    return SolverState.Ready;
                }

                // If the maze is solved, we are Finished.
                if (MazeControlProperties.IsSolved)
                {
                    return SolverState.Finished;
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