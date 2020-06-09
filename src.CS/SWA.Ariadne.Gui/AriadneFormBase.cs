using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Ctrl;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;
using SWA.Ariadne.Gui.Dialogs;

namespace SWA.Ariadne.Gui
{
    /// <summary>
    /// Base class for Ariadne Form windows.
    /// </summary>
    public partial class AriadneFormBase : Form
        , SWA.Ariadne.Gui.Mazes.IMazeForm
        , SWA.Ariadne.Ctrl.IAriadneEventHandler
    {
        #region Properties that need to be implemented by derived classes

        /// <summary>
        /// The object that accepts the MazeControl commands.
        /// </summary>
        protected virtual SWA.Ariadne.Gui.Mazes.IMazeControlProperties MazeControlProperties
        {
            get { return null; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        /// TODO: eliminate this property
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

        #endregion

        #region Member variables

        /// <summary>
        /// The object controlling a running solver.
        /// </summary>
        protected AriadneController ariadneController;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AriadneFormBase()
        {
            InitializeComponent();

            // Hide some controls that are not used in all base classes
            strategyComboBox.Visible = false;
            visitedProgressBar.Visible = false;
        }

        private void AriadneFormBase_Load(object sender, EventArgs e)
        {
            // Create an AriadneController.
            ariadneController = new AriadneController(this, this.SolverController);

            // Initialize the stepsPerSecTextBox
            stepsPerSecTextBox.Text = ariadneController.StepsPerSecond.ToString();

            // Create a new maze.
            this.OnNew(null, null);
        }

        private void AriadneFormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            // When the form is closed, stop the controller.
            ariadneController.Stop();
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
            if (State == SolverState.Paused)
            {
                this.OnPause(sender, e);
            }

            ariadneController.Reset();
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnNew(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                OnReset(sender, e);
            }

            // Subclasses must add an implementation.
        }

        /// <summary>
        /// Opens a Details dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDetails(object sender, EventArgs e)
        {
            // This is not allowed while the Solver is busy.
            if (State != SolverState.Ready && State != SolverState.Finished)
            {
                return;
            }

            DetailsDialog form = new DetailsDialog(this.AriadneSettingsSource);
            form.ShowDialog(this);
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

        /// <summary>
        /// Opens an Arena form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenArena(object sender, EventArgs e)
        {
            ArenaForm form = new ArenaForm();
            form.Icon = this.Icon;
            form.Show();
        }

        /// <summary>
        /// Opens a form that behaves like in screen saver mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenScreenSaver(object sender, EventArgs e)
        {
            ScreenSaverForm form = new ScreenSaverForm(false, null);
            form.Icon = this.Icon;
            form.Bounds = new Rectangle(form.Bounds.Location, this.Bounds.Size);
            form.Show();
        }

        private void OnOpenScreenSaverPreview(object sender, EventArgs e)
        {
            ScreenSaverPreviewController.Run();
        }

        private void OnOpenScreenSaverOptions(object sender, EventArgs e)
        {
            Form dialog = new OptionsDialog();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Saves the currently displayed maze to a PNG file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveImage(object sender, EventArgs e)
        {
            System.Drawing.Imaging.ImageFormat imgFormat = System.Drawing.Imaging.ImageFormat.Png;
            int mazeWidth = MazeControlProperties.XSize, mazeHeigth = MazeControlProperties.YSize;

            Image img = GetImage();
            if (img == null)
            {
                return;
            }

            StringBuilder filename = new StringBuilder(200);
            filename.Append("Ariadne_");
            filename.Append(MazeControlProperties.Code.Replace(".", ""));
            filename.Append("_" + mazeWidth.ToString() + "x" + mazeHeigth.ToString());
            if (State == SolverState.Ready || State == SolverState.ReadyAndScheduled)
            {
                filename.Append("_empty");
            }
            else
            {
                filename.Append("_" + this.StrategyName);
                if (State == SolverState.Finished || State == SolverState.FinishedAndScheduled)
                {
                    filename.Append("_solved");
                }
                else
                {
                    filename.Append("_partial-");
                    filename.Append(SolverController.CountSteps);
                }
            }
            filename.Append(".png");

            string imgPath = System.IO.Path.Combine(CreateScreenshotsDirectory(), filename.ToString());
            img.Save(imgPath, imgFormat);
        }

        #endregion

        #region Solver controls

        /// <summary>
        /// Start a solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnStart(object sender, EventArgs e)
        {
            ariadneController.Start();
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

            ariadneController.Pause();

            if (State == SolverState.Running)
            {
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
            if (State == SolverState.Ready || State == SolverState.ReadyAndScheduled)
            {
                this.OnStart(sender, e);
                this.OnPause(sender, e);
            }

            if (State != SolverState.Paused)
            {
                return;
            }

            ariadneController.SingleStep();
        }

        /// <summary>
        /// Toggle the repeat mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRepeat(object sender, EventArgs e)
        {
            // Switch between Running and Paused.
            ariadneController.RepeatMode = !ariadneController.RepeatMode;

            if (ariadneController.RepeatMode == false)
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
        /// Display the drop down menu (when pressing the split button).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowDropDownMenu(object sender, EventArgs e)
        {
            this.menuButton.ShowDropDown();
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
                case (char)Keys.R:
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
                    OnShowDropDownMenu(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.S:
                    OnSaveImage(sender, e);
                    e.Handled = true;
                    break;
#if true
                case (char)Keys.D8:
                    OnOpenScreenSaver(sender, e);
                    e.Handled = true;
                    break;
                case (char)Keys.D9:
                    OnOpenScreenSaverPreview(sender, e);
                    e.Handled = true;
                    break;
#endif
                case (char)Keys.Q:
                    this.Close();
                    e.Handled = true;
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
            int value = ariadneController.StepsPerSecond;

            try
            {
                value = Int32.Parse(stepsPerSecTextBox.Text);
            }
            catch (Exception) { }

            ariadneController.StepsPerSecond = value;
            UpdateCaption();
        }

        private void strategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCaption();
        }

        #endregion

        #endregion

        #region Support for early preparations for the next start

        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool Alive { get { return true; } }

        /// <summary>
        /// Executes some time consuming preparations before the next solver run is started.
        /// </summary>
        public void PrepareForNextStart()
        {
            PrepareForNextStart(true);
            PrepareForNextStart(false);
        }

        protected virtual void PrepareForNextStart(bool baseFirst)
        {
            if (baseFirst)
            {
                // base.PrepareForNextStart(baseFirst);
                // more code
            }
            else
            {
                // more code
                // base.PrepareForNextStart(baseFirst);
            }
        }

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

            ariadneController.FillStatusMessage(message);

            this.statusLabel.Text = message.ToString();
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

            if (ariadneController != null)
            {
                caption.Append(" - ");
                caption.Append(ariadneController.StepsPerSecond.ToString());
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
                string result = null;

                if (result == null && ariadneController != null)
                {
                    result = ariadneController.StrategyName;
                }
                if (result == null && strategyComboBox != null && this.strategyComboBox.SelectedItem != null)
                {
                    result = this.strategyComboBox.SelectedItem.ToString();
                }
                if (result == null)
                {
                    result = "???";
                }

                return result;
            }
        }

        /// <summary>
        /// Called when the controller's state has changed.
        /// <seealso cref="OnDetails(object, EventArgs)"/>
        /// </summary>
        public void NotifyControllerStateChanged()
        {
            this.detailsMenuItem.Enabled = (State == SolverState.Ready || State == SolverState.Finished);
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        public virtual void ResetCounters()
        {
            // Subclasses may add an implementation.
        }

        /// <summary>
        /// Gets the current SolverState.
        /// </summary>
        /// TODO: use ariadneController.State directly
        protected SolverState State
        {
            get { return ariadneController.State; }
        }

        /// <summary>
        /// Returns an image of the current maze, without the Form border.
        /// </summary>
        /// <returns></returns>
        protected virtual Image GetImage()
        {
            // Subclasses may add an implementation.
            return null;
        }

        private static string CreateScreenshotsDirectory()
        {
            string result = System.IO.Path.Combine(SWA.Utilities.Directory.ApplicationDirectory, "Screenshots");
            System.IO.Directory.CreateDirectory(result);
            return result;
        }

        #endregion
    }
}
