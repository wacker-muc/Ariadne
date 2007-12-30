using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWA.Ariadne.App
{
    public partial class MazeUserControl : UserControl
    {
        #region Member variables

        private int squareWidth;
        private int wallWidth;
        private int gridWidth;
        private int pathWidth;
        private int xOffset, yOffset;

        Color wallColor = Color.Yellow;
        Color forwardColor = Color.Thistle;
        Color backwardColor = Color.Plum;

        Pen wallPen;
        Pen forwardPen;
        Pen backwardPen;

        private int xSize, ySize;
        private int xCur, yCur;
        private int xStart, yStart;
        private int xEnd, yEnd;

        private Random random;

        /// <summary>
        /// Number of times a square has been visited.
        /// 0: not yet visited
        /// 1: visited in forward direction, current path
        /// 2: visited in backward direction, dead end
        /// </summary>
        private byte[,] visited;

        #endregion

        #region Constructor and Initialization

        public MazeUserControl()
        {
            InitializeComponent();

            this.random = new Random();
        }

        public void Setup(int squareWidth, int wallWidth, int pathWidth)
        {
            this.squareWidth = squareWidth;
            this.wallWidth = wallWidth;
            this.gridWidth = squareWidth + wallWidth;
            this.pathWidth = pathWidth;

            CreateMaze();
            PlaceEndpoints();
            ClearMaze();
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
            // TODO: determine xSize and ySize
            xSize = (this.Width  - this.wallWidth - 4) / this.gridWidth;
            ySize = (this.Height - this.wallWidth - 4) / this.gridWidth;

            xOffset = (this.Width - xSize * gridWidth) / 2;
            yOffset = (this.Height - ySize * gridWidth) / 2;

            // create two-dimensional data sturctures
            visited = new byte[xSize, ySize];
        }

        private void PlaceEndpoints()
        {
            // the travel direction (one of four)
            int direction = this.random.Next(4);

            // a small portion of the maze size (in trave direction)
            int edgeWidth = 0;
            switch (direction)
            {
                case 0:
                case 2:
                    // vertical
                    edgeWidth = 1 + ySize * 2/100;
                    break;
                case 1:
                case 3:
                    // horizontal
                    edgeWidth = 1 + xSize * 2/100;
                    break;
            }

            // distance of start and end point from the maze border
            int edgeDistStart = 0
                + random.Next(edgeWidth)
                + random.Next(edgeWidth)
                + random.Next(edgeWidth)
                ;
            int edgeDistEnd = 0
                + random.Next(edgeWidth)
                + random.Next(edgeWidth)
                + random.Next(edgeWidth)
                ;

            switch (direction)
            {
                case 0:
                    // start at top, end at bottom
                    xStart = random.Next(xSize);
                    yStart = ySize - 1 - edgeDistStart;
                    xEnd = random.Next(xSize);
                    yEnd = edgeDistEnd;
                    break;
                case 1:
                    // start at left, end at right
                    xStart = edgeDistEnd;
                    yStart = random.Next(ySize);
                    xEnd = xSize - 1 - edgeDistStart;
                    yEnd = random.Next(ySize);
                    break;
                case 2:
                    // start at bottom, end at top
                    xStart = random.Next(xSize);
                    yStart = edgeDistEnd;
                    xEnd = random.Next(xSize);
                    yEnd = ySize - 1 - edgeDistStart;
                    break;
                case 3:
                    // start at right, end at left
                    xStart = xSize - 1 - edgeDistStart;
                    yStart = random.Next(ySize);
                    xEnd = edgeDistEnd;
                    yEnd = random.Next(ySize);
                    break;
            }
        }

        public void ClearMaze()
        {
            // clear the visited region
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    visited[x,y] = 0;
                }
            }
            xCur = xStart;
            yCur = yStart;

            this.BackColor = Color.Black;
            this.wallPen = new Pen(wallColor, wallWidth);
            this.forwardPen = new Pen(forwardColor, pathWidth);
            this.backwardPen = new Pen(backwardColor, pathWidth);

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
            //PaintWalls(g);
            //PaintPath(g);
        }

        private void PaintBorder(Graphics g)
        {
            g.DrawRectangle(wallPen, new Rectangle(xOffset, yOffset, xSize * gridWidth, ySize * gridWidth));
        }

        #endregion

        public void Step()
        {
            if (Solved())
            {
                return;
            }

            // possible choices
            int[] xNext = new int[4];
            int[] yNext = new int[4];
            int numNext = 0;

            // minimum of visited count of the current square's neighbors
            byte minVisited = 2;

            // TODO: find most promising neighbor squares

            // select one of the neighbor squares
            int iNext = random.Next(numNext);

            // TODO: draw a path from xyCur to xyNext[iNext]

            xCur = xNext[iNext];
            yCur = yNext[iNext];
        }

        public bool Solved()
        {
            return (xCur == xEnd) && (yCur == yEnd);
        }
    }
}
