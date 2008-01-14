using System;
namespace SWA.Ariadne.App
{
    public interface ISolverController
    {
        long CountSteps { get; }
        void DoStep();
        void FillStatusMessage(System.Text.StringBuilder message);
        void FinishPath();
        void Reset();
        void ResetCounters();
        void Start();
        void UpdateStatusLine();
    }
}
