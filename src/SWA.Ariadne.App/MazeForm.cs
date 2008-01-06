using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A Windows Form that displays a Maze in a MazeUserControl.
    /// </summary>
    public partial class MazeForm : Form
        , IMazeForm
    {
        #region Member variables

        /// <summary>
        /// The type of solver algorithm we will use.
        /// </summary>
        private System.Type strategy = SolverFactory.DefaultStrategy;

        /// <summary>
        /// A dictionary used by the strategyComboBox.
        /// </summary>
        private readonly Dictionary<string, System.Type> strategies;

        /// <summary>
        /// The maze solver algorithm.
        /// This is only set (not null) while we are in a solving mode.
        /// </summary>
        private IMazeSolver solver;

        /// <summary>
        /// A timer that causes single solver steps.
        /// </summary>
        private Timer stepTimer;

        /// <summary>
        /// Counters for the number of executed steps: total, in forward and backward direction.
        /// </summary>
        private long countSteps, countForward, countBackward;

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
        public MazeForm()
        {
            InitializeComponent();

            #region Initialize the stepsPerSecTextBox

            stepsPerSecTextBox.Text = stepsPerSecond.ToString();

            #endregion

            #region Fill the strategyComboBox with all known MazeSolvers

            strategies = new Dictionary<string, System.Type>();

            strategyComboBox.Items.Clear();

            foreach (System.Type t in SolverFactory.SolverTypes)
            {
                // Add the solver's name to the combo box.
                strategyComboBox.Items.Add(t.Name);

                // Add the solver's type and name to a Dictionary -- see strategy_Validated().
                strategies.Add(t.Name, t);

                // Note: Instead of a string we could add the Type object directly.
                // But the Type's ToString() method returns the FullName instead of the short Name.

                // Pre-select the default solver strategy.
                if (t == strategy)
                {
                    strategyComboBox.SelectedIndex = strategyComboBox.Items.Count - 1;
                }
            }

            #endregion

            this.OnNew(null, null);
        }

        #endregion

        #region Event handlers

        #region Maze controls

        /// <summary>
        /// Stops the solver and returns the maze to its original (unsolved) state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReset(object sender, EventArgs e)
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
                solver = null;
                FixStateDependantControls();
            }

            ResetCounters();

            this.mazeUserControl.Reset();
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNew(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                OnReset(sender, e);
            }

            mazeUserControl.Setup();
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

            DetailsDialog form = new DetailsDialog(this.mazeUserControl);
            form.ShowDialog(this);

            // What needs to be done if the dialog has caused a State change?
            FixStateDependantControls();
        }

        /// <summary>
        /// Opens an About box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbout(object sender, EventArgs e)
        {
            AboutBox form = new AboutBox();
            form.ShowDialog(this);
        }

        #endregion

        #region Solver controls

        /// <summary>
        /// Start a solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStart(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                //OnReset(sender, e);
                return;
            }

            solver = SolverFactory.CreateSolver(strategy, mazeUserControl.Maze);

            stepTimer = new Timer();
            stepTimer.Interval = (1000/60)/2; // 60 frames per second
            stepTimer.Tick += new EventHandler(this.OnStepTimer);
            stepTimer.Start();

            FixStateDependantControls();
            ResetCounters();

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

                if (!mazeUserControl.Maze.IsSolved)
                {
                    MazeSquare sq = null;
                    int currentSteps = 0;
                    int maxStepsPerTimerEvent = 20;
                    while (this.IsBehindSchedule())
                    {
                        sq = SingleStep();

                        if (++currentSteps > maxStepsPerTimerEvent)
                        {
                            break;
                        }
                    }
                    mazeUserControl.FinishPath(sq);
                    UpdateStatusLine();
                }
            }
            finally
            {
                // Either restart or delete the timer.
                if (!mazeUserControl.Maze.IsSolved)
                {
                    stepTimer.Enabled = true;
                    // State is Running.
                }
                else
                {
                    stepTimer = null;
                    // State is Finished and may become Ready if someone creates a new Maze.
                }
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

            mazeUserControl.FinishPath(SingleStep());
            UpdateStatusLine();
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

            if (value < 1)
            {
                value = 1;
            }
            if (value > 9999)
            {
                value = 9999;
            }
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

        private void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.strategy = strategies[(string)strategyComboBox.SelectedItem];
            UpdateCaption();
        }

        #endregion

        #endregion

        #region IMazeForm implementation

        /// <summary>
        /// Place reserved areas into the maze.
        /// This method is called from the MazeUserControl before actually building the maze.
        /// </summary>
        /// <param name="maze"></param>
        public void MakeReservedAreas(Maze maze)
        {
#if false
            Random r = new Random();
            while (maze.ReserveRectangle(r.Next(2, 8), r.Next(2, 8)))
            {
            }
#endif
        }

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public void UpdateStatusLine()
        {
            StringBuilder message = new StringBuilder(200);

            message.Append("Size = " + mazeUserControl.Maze.XSize.ToString() + "x" + mazeUserControl.Maze.YSize.ToString());
            
            if(countSteps > 0)
            {
                message.Append(" / ");
                message.Append(countSteps.ToString() + " steps, " + countForward + " forward, " + countBackward + " backward");

                message.Append(" / ");
                double totalSeconds = accumulatedSeconds + lapSeconds;
                message.Append(totalSeconds.ToString("0.00") + " sec");

                double sps = (countSteps-stepsBeforeRateChange) / (totalSeconds-secondsBeforeRateChange);
                if (stepsBeforeRateChange > 0)
                {
                    message.Append(" = [" + sps.ToString("0") + "] steps/sec");
                }
                else
                {
                    message.Append(" = " + sps.ToString("0") + " steps/sec");
                }
            }

            this.statusLabel.Text = message.ToString();
        }

        /// <summary>
        /// Displays Maze and Solver characteristics in the window's caption bar.
        /// The maze ID, step rate and solver strategy name.
        /// </summary>
        public void UpdateCaption()
        {
            StringBuilder caption = new StringBuilder(80);

            caption.Append("Ariadne");

            if (strategy != null)
            {
                caption.Append(" - ");
                caption.Append(strategy.Name);
            }

            if (true)
            {
                caption.Append(" - ");
                caption.Append(stepsPerSecond.ToString());
            }

            if (mazeUserControl != null && mazeUserControl.Maze != null)
            {
                caption.Append(" - ");
                caption.Append("ID: " + mazeUserControl.Maze.Code);
            }

            this.Text = caption.ToString();
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        private void ResetCounters()
        {
            countSteps = countForward = countBackward = 0;
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

        private enum SolverState
        {
            Ready,
            Running,
            Paused,
            Finished,
        }

        private SolverState State
        {
            get
            {
                // While there is no solver, we are Ready to create one and start it.
                if (solver == null)
                {
                    return SolverState.Ready;
                }

                // If the maze is solved, we are Finished.
                if (mazeUserControl.Maze.IsSolved)
                {
                    return SolverState.Finished;
                }

                // If there is no timer, we should be either Ready or Finished.  But "Finished" was checked above.
                //
                // Explanation:
                // The DetailsDialog can be opened in the Finished state and may create a new, unsolved Maze.
                //
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

        /// <summary>
        /// Enables or disables some controls depending on whether we are Ready or not.
        /// </summary>
        private void FixStateDependantControls()
        {
            bool enabled = (State == SolverState.Ready);

            strategyComboBox.Enabled = enabled;
        }

        /// <summary>
        /// Executes one step in the solver and paints that section of the path.
        /// </summary>
        /// <returns>either null OR the square this step travelled to in backward direction</returns>
        private MazeSquare SingleStep()
        {
            if (State == SolverState.Finished)
            {
                return null;
            }

            MazeSquare sq1, sq2;
            bool forward;

            solver.Step(out sq1, out sq2, out forward);
            mazeUserControl.PaintPath(sq1, sq2, forward);

            if (forward)
            {
                ++countForward;
            }
            else
            {
                ++countBackward;
            }
            ++countSteps;

            return (forward ? null : sq2);
        }

        #endregion
    }
}