using System;
namespace SWA.Ariadne.Ctrl
{
    public interface ISolverController
    {
        int BlinkingCounter { get; set; }
        long CountSteps { get; }
        int DoStep();
        void FillStatusMessage(System.Text.StringBuilder message);
        void FinishPath();
        void PrepareForStart();
        void ReleaseResources();
        void Reset();
        void ResetCounters();
        void Start();
        string StrategyName { get; }
        void UpdateStatusLine();
    }
}
