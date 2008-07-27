using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Gui.Mazes;

namespace SWA.Ariadne.Ctrl
{
    public class MultiSolverController
        : ISolverController
    {
        #region Member variables

        /// <summary>
        /// The mazeForm has a status line and a caption.
        /// </summary>
        private IMazeForm mazeForm;

        private List<ISolverController> list;

        /// <summary>
        /// Number of executed steps.
        /// </summary>
        private long countSteps;

        public long CountSteps
        {
            get { return countSteps; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiSolverController(IMazeForm mazeForm)
        {
            this.mazeForm = mazeForm;
            this.list = new List<ISolverController>();
        }

        #endregion

        #region List behavior

        public void Add(ISolverController item)
        {
            list.Add(item);
        }

        public void Remove(ISolverController item)
        {
            list.Remove(item);
        }

        #endregion

        #region Setup methods

        public void Reset()
        {
            foreach (SolverController item in list)
            {
                item.Reset();
            }
        }

        public void ReleaseResources()
        {
            foreach (SolverController item in list)
            {
                item.ReleaseResources();
            }
        }

        public void ResetCounters()
        {
            foreach (SolverController item in list)
            {
                item.ResetCounters();
            }
            countSteps = 0;
        }

        public void PrepareForStart()
        {
            foreach (SolverController item in list)
            {
                item.PrepareForStart();
            }
        }

        public void Start()
        {
            foreach (SolverController item in list)
            {
                item.Start();
            }
        }

        #endregion

        #region Solver methods

        public void DoStep()
        {
            foreach (SolverController item in list)
            {
                item.DoStep();
            }
            ++countSteps;
        }

        public void FinishPath()
        {
            foreach (SolverController item in list)
            {
                item.FinishPath();
            }
        }

        #endregion

        #region Status methods

        /// <summary>
        /// Displays information about the running MazeSolver in the status line.
        /// </summary>
        public void UpdateStatusLine()
        {
            mazeForm.UpdateStatusLine();

            foreach (SolverController item in list)
            {
                item.UpdateStatusLine();
            }
        }

        public void FillStatusMessage(StringBuilder message)
        {
            if (countSteps > 0)
            {
                string steps = (countSteps == 1 ? "step" : "steps");
                message.Append(countSteps.ToString("#,##0") + " " + steps);
            }
        }

        public string StrategyName
        {
            get
            {
                return "(multiple)";
            }
        }

        #endregion

        #region MazeControl methods

        public int BlinkingCounter
        {
            get
            {
                return list[0].BlinkingCounter;
            }
            set
            {
                foreach (SolverController item in list)
                {
                    item.BlinkingCounter = value;
                }
            }
        }

        #endregion
    }
}