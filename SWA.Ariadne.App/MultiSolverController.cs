using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.App
{
    public class MultiSolverController
        : ISolverController
    {
        #region Member variables

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
        public MultiSolverController()
        {
            this.list = new List<ISolverController>();
        }

        #endregion

        #region List behavior

        internal void Add(ISolverController item)
        {
            list.Add(item);
        }

        internal void Remove(ISolverController item)
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

        public void ResetCounters()
        {
            foreach (SolverController item in list)
            {
                item.ResetCounters();
            }
            countSteps = 0;
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

        public void UpdateStatusLine()
        {
            foreach (SolverController item in list)
            {
                item.UpdateStatusLine();
            }
        }

        public void FillStatusMessage(StringBuilder message)
        {
            if (countSteps > 0)
            {
                message.Append(countSteps.ToString("#,##0") + " steps");
            }
        }

        #endregion
    }
}
