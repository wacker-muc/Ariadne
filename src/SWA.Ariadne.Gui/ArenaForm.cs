using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Settings;
using SWA.Ariadne.Ctrl;
using SWA.Ariadne.Gui.Mazes;

namespace SWA.Ariadne.Gui
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
        protected override SWA.Ariadne.Gui.Mazes.IMazeControlProperties MazeControlProperties
        {
            get { return this as SWA.Ariadne.Gui.Mazes.IMazeControlProperties; }
        }

        /// <summary>
        /// The object that accepts the SolverController commands.
        /// </summary>
        protected override ISolverController SolverController
        {
            get
            {
                if (this.solverController == null)
                {
                    this.solverController = new MultiSolverController(this as IMazeForm);
                }
                return this.solverController;
            }
        }
        private MultiSolverController solverController;

        /// <summary>
        /// The object that accepts the AriadneSettingsSource commands.
        /// </summary>
        protected override SWA.Ariadne.Settings.IAriadneSettingsSource AriadneSettingsSource
        {
            get { return (this as SWA.Ariadne.Settings.IAriadneSettingsSource); }
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

        #endregion

        #region Constructor

        public ArenaForm()
        {
            InitializeComponent();

            // Start with an empty Items list.  It will be created in OnLayout().
            foreach (ArenaItem item in Items)
            {
                this.Controls.Remove(item);
            }
        }

        #endregion

        #region Event handlers

        #region Arena controls

        /// <summary>
        /// Create and arrange the desired number of ArenaItems.
        /// </summary>
        private void OnLayout()
        {
            List<ArenaItem> items = Items;

            #region Adjust the number of ArenaItems and SolverControllers

            while (items.Count < nX * nY)
            {
                ArenaItem item = new ArenaItem();
                this.Controls.Add(item);
                items.Add(item);
                ((MultiSolverController)SolverController).Add(item.SolverController);
            }
            while(items.Count > nX * nY)
            {
                ArenaItem item = items[items.Count - 1];
                this.Controls.Remove(item);
                items.RemoveAt(items.Count - 1);
                ((MultiSolverController)SolverController).Remove(item.SolverController);
            }

            #endregion

            #region Place the ArenaItems into a regular grid

            const int dx = 6, dy = 6;

            for (int x = 0; x < nX; x++)
            {
                for (int y = 0; y < nY; y++)
                {
                    int cw = (this.ClientSize.Width - dx) / nX - dx;
                    int cx = dx + x * (cw + dx);
                    int ch = (this.statusStrip.Location.Y - dx) / nX - dx;
                    int cy = dy + y * (ch + dy);

                    ArenaItem item = items[y * nX + x];
                    item.Location = new Point(cx, cy);
                    item.Size = new Size(cw, ch);
                }
            }

            #endregion

#if false
            // Note: The maze parameters are still incomplete, so we should not try to update anything. 
            this.UpdateCaption();
            this.UpdateStatusLine();
#endif
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
            base.OnReset(sender, e);

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
        public override void OnNew(object sender, EventArgs e)
        {
            base.OnNew(sender, e);
            OnLayout();

            #region Setup one item and use its parameters for all other items.

            TemplateItem.Setup(true);
            DistributeTemplateParameters();

            #endregion

            // Now that we have created all mazes, refresh the parameter display.
            this.UpdateCaption();
            this.UpdateStatusLine();
        }

        private void DistributeTemplateParameters()
        {
            AriadneSettingsData data = new AriadneSettingsData();
            TemplateMazeUserControl.FillParametersInto(data);

            data.AutoSeed = false;
            data.AutoGridWidth = data.AutoPathWidth = false;
            data.AutoMazeHeight = data.AutoMazeWidth = true;
            data.AutoColors = false;

            // The other MazePainters should not have a local ImageLoader.
            data.LeaveCurrentBackgroundImageLoader = true;

            foreach (ArenaItem item in Items)
            {
                if (item != TemplateItem)
                {
                    item.MazeUserControl.MazePainter.BackgroundImageLoader = TemplateItem.MazeUserControl.MazePainter;
                    item.MazeUserControl.TakeParametersFrom(data);
                    item.Setup(false);
                }
            }
        }

        #endregion

        #endregion

        #region IMazeForm implementation

        public override string StrategyName
        {
            get { return "Arena[" + this.Items.Count.ToString() + "]"; }
        }

        /// <summary>
        /// Reset step and runtime counters.
        /// </summary>
        public override void ResetCounters()
        {
            base.ResetCounters();

            foreach (ArenaItem item in Items)
            {
                item.ConfigureVisitedProgressBar();
            }
        }

        #endregion

        #region IAriadneSettingsSource implementation

        /// <summary>
        /// Fill all modifyable parameters into the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void FillParametersInto(SWA.Ariadne.Settings.AriadneSettingsData data)
        {
            TemplateMazeUserControl.FillParametersInto(data);
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(SWA.Ariadne.Settings.AriadneSettingsData data)
        {
            TemplateMazeUserControl.TakeParametersFrom(data);
            DistributeTemplateParameters();
            UpdateCaption();
        }

        #endregion

        #region IMazeControlProperties implementation

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