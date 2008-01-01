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
    {
        #region Member variables

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

            this.OnNew(null, null);
        }

        #endregion

        #region Event handlers

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
        }

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

            solver = new RandomSolver(mazeUserControl.Maze);
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
                int currentSteps = 0;
                int maxStepsPerTimerEvent = 20;
                while (this.IsBehindSchedule())
                {
                    this.SingleStep();

                    if (++currentSteps > maxStepsPerTimerEvent)
                    {
                        break;
                    }
                }
                UpdateStatusLine();
            }
            stepTimer.Enabled = true;
        }

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

            SingleStep();
            UpdateStatusLine();
        }

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

        private void SingleStep()
        {
            if (mazeUserControl.Maze.IsSolved)
            {
                return;
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
        }

        #endregion

        #region Access to the form's controls

        public string StatusLine
        {
            set
            {
                this.statusLabel.Text = value;
            }
        }

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

            this.StatusLine = message.ToString();
        }

        #endregion
    }
}