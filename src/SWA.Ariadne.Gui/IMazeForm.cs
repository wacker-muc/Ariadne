using System;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A MazeUserControl will try to call these methods of its ParentForm.
    /// </summary>
    public interface IMazeForm
    {
        void MakeReservedAreas(Maze maze);
        void UpdateStatusLine();
        void UpdateCaption();
        string StrategyName { get; }
        void FixStateDependantControls(AriadneFormBase.SolverState state);
    }
}
