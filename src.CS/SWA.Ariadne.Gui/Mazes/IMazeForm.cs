using System;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.Gui.Mazes
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
    }
}
