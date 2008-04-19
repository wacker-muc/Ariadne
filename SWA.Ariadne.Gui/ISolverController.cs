using System;
namespace SWA.Ariadne.Gui
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
        string StrategyName { get; }
        void UpdateStatusLine();
        int BlinkingCounter { get; set; }
    }
}
