using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// A MazeSolver will use these methods to draw the path while it is working on a solution.
    /// </summary>
    public interface IMazeDrawer
    {
        /// <summary>
        /// Draws a section of the path between the given (adjoining) squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <param name="forward"></param>
        void DrawStep(MazeSquare sq1, MazeSquare sq2, bool forward);

        /// <summary>
        /// Draws the path between the given squares.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="forward"></param>
        void DrawPath(List<MazeSquare> path, bool forward);

        /// <summary>
        /// Paints a dot (in the forward color) at the given square.
        /// Renders the GraphicsBuffer.
        /// </summary>
        /// <param name="sq">when null, no dot is drawn</param>
        void FinishPath(MazeSquare sq);
    }
}
