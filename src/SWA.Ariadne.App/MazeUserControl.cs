using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.App
{
    public partial class MazeUserControl : UserControl
    {
        #region Constants

        /// <summary>
        /// Minimum and maximum grid width.
        /// </summary>
        const int MinGridWidth = 4, MaxGridWidth = 12;

        #endregion

        #region Member variables

        private Maze maze;

        private int squareWidth;
        private int wallWidth;
        private int gridWidth;
        private int pathWidth;
        private int xOffset, yOffset;

        private Color wallColor = Color.Yellow;
        private Color forwardColor = Color.Thistle;
        private Color backwardColor = Color.Plum;

        private Pen wallPen;
        private Pen forwardPen;
        private Pen backwardPen;

        #endregion

        #region Constructor and Initialization

        public MazeUserControl()
        {
            InitializeComponent();
        }

        public void Setup(int squareWidth, int wallWidth, int pathWidth)
        {
            this.squareWidth = squareWidth;
            this.wallWidth = wallWidth;
            this.gridWidth = squareWidth + wallWidth;
            this.pathWidth = pathWidth;

            CreateMaze();
            PlaceEndpoints();
            Reset();
        }

        public void Setup(int squareWidth)
        {
            this.Setup(squareWidth, 1, (int)(0.7 * squareWidth));
        }

        /// <summary>
        /// Construct a maze 
        /// </summary>
        private void CreateMaze()
        {
            // Determine dimensions of a maze that fits into the drawing area.
            int xSize = (this.Width  - this.wallWidth - 4) / this.gridWidth;
            int ySize = (this.Height - this.wallWidth - 4) / this.gridWidth;

            // Determine offset for centering the maze in the drawing area.
            this.xOffset = (this.Width - xSize * gridWidth) / 2;
            this.yOffset = (this.Height - ySize * gridWidth) / 2;

            // Create a maze.
            this.maze = new Maze(xSize, ySize);
            maze.CreateMaze();
        }

        private void PlaceEndpoints()
        {
            maze.PlaceEndpoints();
        }

        public void Reset()
        {
            maze.Reset();

            this.BackColor = Color.Black;
            this.wallPen = new Pen(wallColor, wallWidth);
            this.forwardPen = new Pen(forwardColor, pathWidth);
            this.backwardPen = new Pen(backwardColor, pathWidth);

            wallPen.StartCap = wallPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            forwardPen.StartCap = forwardPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            backwardPen.StartCap = backwardPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;

            // TODO: draw maze
            // TODO: draw start and end point
        }

        #endregion

        #region Painting methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //if (this.wallWidth == 0)
            {
                this.Setup(12, 3, 8);
            }

            Graphics g = e.Graphics;

            PaintBorder(g);
            PaintWalls(g);
            //PaintPath(g);
        }

        /// <summary>
        /// Paint a border around the maze.
        /// </summary>
        /// <param name="g"></param>
        private void PaintBorder(Graphics g)
        {
            g.DrawRectangle(wallPen, new Rectangle(xOffset, yOffset, maze.XSize * gridWidth, maze.YSize * gridWidth));
        }

        /// <summary>
        /// Paint the closed inner walls.
        /// </summary>
        /// <param name="g"></param>
        private void PaintWalls(Graphics g)
        {
            // We'll only draw the west and east walls of every square.
            for (int x = 0; x < maze.XSize; x++)
            {
                int cx = xOffset + x * gridWidth;
                for (int y = 0; y < maze.YSize; y++)
                {
                    int cy = yOffset + y * gridWidth;
                    MazeSquare sq = maze[x, y];

                    // Draw the west wall.
                    if (sq[MazeSquare.WallPosition.WP_W] == MazeSquare.WallState.WS_CLOSED)
                    {
                        g.DrawLine(wallPen, cx, cy, cx, cy + gridWidth);
                    }

                    // Draw the north wall.
                    if (sq[MazeSquare.WallPosition.WP_N] == MazeSquare.WallState.WS_CLOSED)
                    {
                        g.DrawLine(wallPen, cx, cy, cx + gridWidth, cy);
                    }
                }
            }
        }

        #endregion
    }
}
