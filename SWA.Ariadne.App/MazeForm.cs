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
    public partial class MazeForm : Form
        , IMazeForm
    {
        #region Member variables

        /// <summary>
        /// The type of solver algorithm we will use.
        /// </summary>
        private System.Type solverType = typeof(RandomBacktracker);

        /// <summary>
        /// A dictionary used by the strategyComboBox.
        /// </summary>
        private readonly Dictionary<string, System.Type> knownSolvers;

        /// <summary>
        /// The maze solver algorithm.
        /// This is only set (not null) while we are in a solving mode.
        /// </summary>
        private IMazeSolver solver;

        /// <summary>
        /// A timer that causes single solver steps.
        /// </summary>
        private Timer stepTimer;

        private long countSteps, countForward, countBackward;

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

            knownSolvers = new Dictionary<string, System.Type>();

            strategyComboBox.Items.Clear();

            foreach (System.Type t in new System.Type[] {
                typeof(Logic.RandomBacktracker),
                typeof(Logic.RightHandWalker),
                typeof(Logic.LeftHandWalker),
                typeof(Logic.RandomWalker),
            })
            {
                // Add the solver's name to the combo box.
                strategyComboBox.Items.Add(t.Name);

                // Add the solver's type and name to a Dictionary -- see strategy_Validated().
                knownSolvers.Add(t.Name, t);

                // Note: Instead of a string we could add the Type object directly.
                // But the Type's ToString() method returns the FullName instead of the short Name.

                // Pre-select the default solver strategy.
                if (t == solverType)
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
        /// Stop the solver and return the maze to its original (unsolved) state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReset(object sender, EventArgs e)
        {
            if (solver != null)
            {
                if (stepTimer.Enabled == false)
                {
                    this.OnPause(sender, e);
                }
                stepTimer.Stop();
                stepTimer = null;
                solver = null;
                strategyComboBox.Enabled = true;
            }

            countSteps = countForward = countBackward = 0;
            lapSeconds = accumulatedSeconds = 0;

            this.mazeUserControl.Reset();
        }

        /// <summary>
        /// Create a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNew(object sender, EventArgs e)
        {
            if (solver != null)
            {
                OnReset(sender, e);
            }

            mazeUserControl.Setup();

            UpdateCaption();
        }

        private void OnDetails(object sender, EventArgs e)
        {
            if (solver == null)
            {
                DetailsDialog form = new DetailsDialog(this.mazeUserControl);
                form.ShowDialog(this);
            }
        }

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
            if (solver != null)
            {
                //OnReset(sender, e);
                return;
            }

            strategyComboBox.Enabled = false;
            solver = (IMazeSolver) solverType.GetConstructor(
                new Type[1] { typeof(Maze) }).Invoke(
                new object[1] { mazeUserControl.Maze });

            countSteps = countForward = countBackward = 0;
            stepTimer = new Timer();
            stepTimer.Interval = (1000/60)/2; // 60 frames per second
            stepTimer.Tick += new EventHandler(this.OnStepTimer);
            stepTimer.Start();

            lapStartTime = System.DateTime.Now;
            lapSeconds = accumulatedSeconds = 0;
        }

        /// <summary>
        /// Let the solver make one step.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStepTimer(object sender, EventArgs e)
        {
            stepTimer.Enabled = false;
            if (mazeUserControl.Maze.IsSolved)
            {
                stepTimer.Stop();
            }
            else
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
            stepTimer.Enabled = true;
        }

        /// <summary>
        /// Halt or continue the solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPause(object sender, EventArgs e)
        {
            if (solver == null || mazeUserControl.Maze.IsSolved)
            {
                return;
            }

            stepTimer.Enabled = !stepTimer.Enabled;

            if (stepTimer.Enabled == true)
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

        private void OnStep(object sender, EventArgs e)
        {
            if (solver == null)
            {
                this.OnStart(sender, e);
                this.OnPause(sender, e);
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
        }

        private void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.solverType = knownSolvers[(string)strategyComboBox.SelectedItem];
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
        /// Display information about the running MazeSolver in the status line.
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
                double sps = countSteps / totalSeconds;
                message.Append(" = " + sps.ToString("0") + " steps/sec");
            }

            this.statusLabel.Text = message.ToString();
        }

        #endregion

        #region Auxiliary methods

        private bool IsBehindSchedule()
        {
            if (mazeUserControl.Maze.IsSolved)
            {
                return false;
            }

            TimeSpan lap = System.DateTime.Now - lapStartTime;
            lapSeconds = lap.TotalSeconds;
            double scheduledSteps = (accumulatedSeconds + lapSeconds) * stepsPerSecond;

            return (countSteps < 1 + scheduledSteps);
        }

        /// <summary>
        /// Executes one step in the solver and paints that section of the path.
        /// </summary>
        /// <returns>either null OR the square this step travelled to in backward direction</returns>
        private MazeSquare SingleStep()
        {
            if (mazeUserControl.Maze.IsSolved)
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

        /// <summary>
        /// Write the maze ID and solver strategy name into the window's caption bar.
        /// </summary>
        private void UpdateCaption()
        {
            StringBuilder caption = new StringBuilder(80);

            caption.Append("Ariadne");

            if (solverType != null)
            {
                caption.Append(" - ");
                caption.Append(solverType.Name);
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
    }
}