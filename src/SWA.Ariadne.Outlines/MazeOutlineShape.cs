using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Outlines
{
    internal class MazeOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        private int wallWidth;
        private int gridWidth;
        private int xOffset;
        private int yOffset;
        private IMazeShape maze;
        private ExplicitOutlineShape baseShape;

        public override bool this[int x, int y]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a MazeOutlineShape using the given mazeBuilder.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="gridWidth"></param>
        /// <param name="mazeBuilder"></param>
        private MazeOutlineShape(int xSize, int ySize, int wallWidth, int gridWidth, MazeShapeBuilder mazeBuilder)
            : base(xSize, ySize)
        {
            this.wallWidth = wallWidth;
            this.gridWidth = gridWidth;

            // Determine dimensions of a maze shape that fits tightly around the real maze.
            int mazeWidth = (XSize - wallWidth + gridWidth - 1) / gridWidth;
            int mazeHeight = (YSize - wallWidth + gridWidth - 1) / gridWidth;
            int mazeAreaX = mazeWidth * gridWidth + wallWidth;
            int mazeAreaY = mazeHeight * gridWidth + wallWidth;

            this.xOffset = -(mazeAreaX - XSize) / 2;
            this.yOffset = -(mazeAreaY - YSize) / 2;

            // Adjust width if the left (and maybe also the right) border would lie on the real maze border.
            if (wallWidth + xOffset > 0)
            {
                mazeWidth += 1;
                mazeAreaX += gridWidth;
                xOffset = -(mazeAreaX - XSize) / 2;
            }
            if (wallWidth + yOffset > 0)
            {
                mazeHeight += 1;
                mazeAreaY += gridWidth;
                yOffset = -(mazeAreaY - YSize) / 2;
            }

            this.maze = mazeBuilder(mazeWidth, mazeHeight);

            this.baseShape = new ExplicitOutlineShape(XSize, YSize);

            PaintBorder();
            PaintWalls();
        }

        /// <summary>
        /// Implements the east and south border of the maze shape.
        /// </summary>
        private void PaintBorder()
        {
            int width = maze.XSize * gridWidth;
            int height = maze.YSize * gridWidth;

            for (int w = 0; w < this.wallWidth; w++)
            {
                DrawWall(xOffset + width + w, yOffset, 0, 1, height + wallWidth);
                DrawWall(xOffset, yOffset + height + w, 1, 0, width + wallWidth);
            }
        }

        /// <summary>
        /// Implements the north and west walls of all squares in the maze shape.
        /// </summary>
        private void PaintWalls()
        {
            // Draw the west and north walls of every square.
            for (int x = 0; x < maze.XSize; x++)
            {
                int cx = xOffset + x * this.gridWidth;

                for (int y = 0; y < maze.YSize; y++)
                {
                    int cy = yOffset + y * this.gridWidth;

                    // Draw the west wall.
                    if (maze.WallIsClosed(x, y, WallPosition.WP_W))
                    {
                        DrawWall(cx, cy, 0, 1);
                    }

                    // Draw the north wall.
                    if (maze.WallIsClosed(x, y, WallPosition.WP_N))
                    {
                        DrawWall(cx, cy, 1, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Implements a single square wall starting at the given coordinates.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="dx">0 for a vertical wall, 1 for a horizontal wall</param>
        /// <param name="dy">1 for a vertical wall, 0 for a horizontal wall</param>
        private void DrawWall(int x0, int y0, int dx, int dy)
        {
            for (int w = 0; w < this.wallWidth; w++)
            {
                DrawWall(x0 + dy * w, y0 + dx * w, dx, dy, this.gridWidth + this.wallWidth);
            }
        }

        /// <summary>
        /// Implements a wall with the given length starting at the given coordinates.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="dx">0 for a vertical wall, 1 for a horizontal wall</param>
        /// <param name="dy">1 for a vertical wall, 0 for a horizontal wall</param>
        /// <param name="len"></param>
        private void DrawWall(int x0, int y0, int dx, int dy, int len)
        {
            for (int x = x0, y = y0, i = 0; i < len; x += dx, y += dy, i++)
            {
                if (0 <= x && x < baseShape.XSize && 0 <= y && y < baseShape.YSize)
                {
                    this.baseShape.SetValue(x, y, true);
                }
            }
        }

        #endregion

        #region Static methods for creating OutlineShapes

        /// <summary>
        /// Returns an outline shape based on another maze.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        public static OutlineShape CreateInstance(Random r, int xSize, int ySize)
        {
            int wallWidth, gridWidth;

            if (r.Next(2) == 0)
            {
                // One square wide walls, wide squares.
                wallWidth = 1;
                gridWidth = 1 + r.Next(4, 8 + 1);
            }
            else
            {
                // Walls and squares of equal width.
                wallWidth = r.Next(1, 4 + 1);
                gridWidth = wallWidth * 2;
            }
            MazeOutlineShape result = new MazeOutlineShape(xSize, ySize, wallWidth, gridWidth, MazeBuilder.Instance);

            // The shape is implemented in the underlying ExplicitOulineShape.
            return result.baseShape;
        }

        #endregion
    }

    /// <summary>
    /// A static class that holds a MazeShapeBuilder instance.
    /// </summary>
    /// Note: We can't get access to the SWA.Ariaden.Gui.Mazes project.
    ///       Therefore, the instance must be supplied externally before it may be used.
    public class MazeBuilder
    {
        public static MazeShapeBuilder Instance
        {
            get { return MazeBuilder.instance; }
            set { MazeBuilder.instance = value; }
        }
        private static MazeShapeBuilder instance;
    }
}
