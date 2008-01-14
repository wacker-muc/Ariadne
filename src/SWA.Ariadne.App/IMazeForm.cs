using System;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A MazeUserControl will try to call these methods of its ParentForm.
    /// </summary>
    public interface IMazeForm
    {
        void MakeReservedAreas(SWA.Ariadne.Model.Maze maze);
        void UpdateStatusLine();
        void UpdateCaption();
        string StrategyName { get; }
    }
}
