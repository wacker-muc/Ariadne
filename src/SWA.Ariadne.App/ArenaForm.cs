using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A Windows Form that displays a grid of MazeUserControls.
    /// </summary>
    public partial class ArenaForm : AriadneFormBase
        , IMazeControlProperties
        , IAriadneSettingsSource
    {
        #region Member variables and properties

        /// <summary>
        /// The object that accepts the MazeControlProperties commands.
        /// </summary>
        protected override IMazeControlProperties MazeControlProperties
        {
            get { return this as IMazeControlProperties; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        protected override ISolverController SolverController
        {
            get { return (this.solverController); }
        }

        /// <summary>
        /// The object that accepts the AriadneSettingsSource commands.
        /// </summary>
        protected override IAriadneSettingsSource AriadneSettingsSource
        {
            get { return (this as IAriadneSettingsSource); }
        }

        /// <summary>
        /// Number of mazes.
        /// </summary>
        private int nX = 2, nY = 2;

        /// <summary>
        /// Gets the List of ArenaItems (a subset of the Controls).
        /// </summary>
        private System.Collections.Generic.List<ArenaItem> Items
        {
            get
            {
                List<ArenaItem> result = new List<ArenaItem>(nX * nY);

                foreach (Control item in this.Controls)
                {
                    if (typeof(ArenaItem).IsInstanceOfType(item))
                    {
                        result.Add(item as ArenaItem);
                    }
                }

                return result;
            }
        }

        private ArenaItem TemplateItem
        {
            get { return Items[0]; }
        }

        private MazeUserControl TemplateMazeUserControl
        {
            get { return TemplateItem.MazeUserControl; }
        }

        /// <summary>
        /// The SolverController.
        /// </summary>
        private MultiSolverController solverController;

        #endregion

        #region Constructor

        public ArenaForm()
        {
            InitializeComponent();

            this.solverController = new MultiSolverController();

            // Start with an empty Items list.  It will be created in OnLayout().
            foreach (ArenaItem item in Items)
            {
                this.Controls.Remove(item);
            }

            OnLayout();
        }

        #endregion

        #region Event handlers

        #region Arena controls

        private void OnLayout()
        {
            List<ArenaItem> items = Items;

            #region Adjust the number of ArenaItems and SolverControllers

            while (items.Count < nX * nY)
            {
                ArenaItem item = new ArenaItem();
                this.Controls.Add(item);
                items.Add(item);
                solverController.Add(item.SolverController);
            }
            while(items.Count > nX * nY)
            {
                ArenaItem item = items[items.Count - 1];
                this.Controls.Remove(item);
                items.RemoveAt(items.Count - 1);
                solverController.Remove(item.SolverController);
            }

            #endregion

            for (int x = 0; x < nX; x++)
            {
                for (int y = 0; y < nY; y++)
                {
                    int cw = (this.Width - 6) / nX - 6;
                    int cx = 6 + x * (cw + 6);
                    int ch = (this.statusStrip.Location.Y - 2 - 6) / nX - 6;
                    int cy = 6 + y * (ch + 6);

                    ArenaItem item = items[y * nX + x];
                    item.Location = new Point(cx, cy);
                    item.Size = new Size(cw, ch);
                }
            }
        }

        #endregion

        #region Maze controls

        /// <summary>
        /// Stops the solvers and returns the mazes to their original (unsolved) state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnReset(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                base.OnReset(sender, e);
            }

            foreach (ArenaItem item in Items)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// Creates a new (different) maze.
        /// Every item will be created with the same parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnNew(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                OnReset(sender, e);
            }

            OnLayout();

            #region Setup one item and use its parameters for all other items.

            TemplateItem.Setup();

            AriadneSettingsData data = new AriadneSettingsData();
            TemplateMazeUserControl.FillParametersInto(data);

            data.AutoSeed = false;
            data.AutoGridWidth = data.AutoPathWidth = false;
            data.AutoMazeHeight = data.AutoMazeWidth = false;

            foreach (ArenaItem item in Items)
            {
                item.MazeUserControl.TakeParametersFrom(data);
            }

            #endregion
        }

        #endregion

        #region Solver controls

#if false
        /// <summary>
        /// Start the solver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStart(object sender, EventArgs e)
        {
            if (State != SolverState.Ready)
            {
                //OnReset(sender, e);
                return;
            }

            foreach (ArenaItem item in Items)
            {
                item.SolverController.Start();
            }

            base.OnStart(sender, e);
        }
#endif

#if false
        protected void DoStep()
        {
            foreach (ArenaItem item in Items)
            {
                item.SolverController.DoStep();
            }
        }

        protected void FinishPath()
        {
            foreach (ArenaItem item in Items)
            {
                item.SolverController.FinishPath();
            }
        }
#endif

        #endregion

        #endregion

        #region IAriadneSettingsSource implementation

        /// <summary>
        /// Fill all modifyable parameters into the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void FillParametersInto(AriadneSettingsData data)
        {
            TemplateMazeUserControl.FillParametersInto(data);
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            foreach (ArenaItem item in Items)
            {
                item.MazeUserControl.TakeParametersFrom(data);
            }
        }

        #endregion

        #region IMazeControlProperties implementation

        public bool IsSolved
        {
            get 
            {
                foreach (ArenaItem item in Items)
                {
                    if (!item.MazeUserControl.IsSolved)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int XSize
        {
            get { return (Items.Count == 0 ? -1 : TemplateMazeUserControl.XSize); }
        }

        public int YSize
        {
            get { return (Items.Count == 0 ? -1 : TemplateMazeUserControl.YSize); }
        }

        public string Code
        {
            get { return (Items.Count == 0 ? "---" : TemplateMazeUserControl.Code); }
        }

        public Maze Maze
        {
            get { return this.MazeControlProperties.Maze; }
        }

        #endregion
    }
}