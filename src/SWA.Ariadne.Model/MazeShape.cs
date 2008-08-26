using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// Implements the IMazeShape interface.
    /// </summary>
    internal class MazeShape
        : IMazeShape
    {
        #region Member variables

        private Maze maze;

        #endregion

        #region Constructor

        private MazeShape(int width, int height)
        {
            this.maze = new Maze(width, height);
            maze.CreateMaze();
        }

        #endregion

        #region IMazeShape implementation

        public int XSize
        {
            get { return maze.XSize; }
        }

        public int YSize
        {
            get { return maze.YSize; }
        }

        public bool WallIsClosed(int x, int y, WallPosition p)
        {
            return (maze[x, y][p] == WallState.WS_CLOSED);
        }

        #endregion

        #region MazeShapeBuilder delegate

        private static IMazeShape MazeShapeBuilder(int width, int height)
        {
            return new MazeShape(width, height);
        }

        #endregion

        #region Static class constructor

        static MazeShape()
        {
            // Install the MazeShapeBuilder delegate to be used by a MazeOutlineShape.
            SWA.Ariadne.Outlines.MazeBuilder.Instance = MazeShapeBuilder;
        }

        /// <summary>
        /// Triggers the class constructor.
        /// </summary>
        public static void Touch()
        {
        }

        #endregion
    }
}
