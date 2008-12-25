using System;
using System.Collections.Generic;
using System.Text;

/* Note: This namespace is part of the SWA.Ariadne.Settings project.
 *       Creating a separate project for these few definitions would not be adequate.
 */
namespace SWA.Ariadne.Model.Interfaces
{
    #region Enumeration types

    /// <summary>
    /// Positions of the walls around a square: East, North, West, South.
    /// </summary>
    public enum WallPosition : int
    {
        /// <summary>
        /// East.
        /// </summary>
        WP_E = 0,
        /// <summary>
        /// North.
        /// </summary>
        WP_N = 1,
        /// <summary>
        /// West.
        /// </summary>
        WP_W = 2,
        /// <summary>
        /// South.
        /// </summary>
        WP_S = 3,

        WP_MIN = WP_E,
        WP_MAX = WP_S,
        WP_NUM = WP_MAX + 1,
    }

    /// <summary>
    /// States of a wall: Open, Closed, Not determined.
    /// </summary>
    public enum WallState : byte
    {
        /// <summary>
        /// Undetermined, needs to be initialized.
        /// </summary>
        WS_MAYBE = 0,
        /// <summary>
        /// Open wall, may be passed.
        /// </summary>
        WS_OPEN = 1,
        /// <summary>
        /// Closed wall, may not be passed.
        /// </summary>
        WS_CLOSED = 2,
        /// <summary>
        /// Undetermined, should be part of an outlined shape.
        /// </summary>
        WS_OUTLINE = 3,
    }

    #endregion

    #region Interfaces and Delegates

    /// <summary>
    /// Comprises the methods required to build an outline shape from a Maze.
    /// </summary>
    /// TODO: Consider underlying maze's reserved areas.
    public interface IMazeShape
    {
        int XSize { get; }
        int YSize { get; }
        bool WallIsClosed(int x, int y, WallPosition p);
    }

    public delegate IMazeShape MazeShapeBuilder(int width, int height);

    #endregion
}
