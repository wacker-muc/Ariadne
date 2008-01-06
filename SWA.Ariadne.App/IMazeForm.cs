using System;

namespace SWA.Ariadne.App
{
    /// <summary>
    /// A MazeUserControl will try to call these methods of its ParentForm.
    /// </summary>
    internal interface IMazeForm
    {
        void MakeReservedAreas(SWA.Ariadne.Model.Maze maze);
        void UpdateStatusLine();
        void UpdateCaption();
    }
}
