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
        const int MinGridWidth = 6, MaxGridWidth = 12;

        #endregion

        #region Member variables

        public Maze Maze
        {
            get { return maze; }
        }
        private Maze maze;

        private int squareWidth;
        private int wallWidth;
        private int gridWidth;
        private int pathWidth;
        private int xOffset, yOffset;

        private Color wallColor = Color.Gray;
        private Color forwardColor = Color.GreenYellow;
        private Color backwardColor = Color.Brown;

        private Pen wallPen;
        private Pen forwardPen;
        private Pen backwardPen;

        private BufferedGraphics gBuffer;

        public MazeForm MazeForm
        {
            get { return (MazeForm)this.ParentForm; }
        }

        #endregion

        #region Constructor and Initialization

        public MazeUserControl()
        {
            InitializeComponent();

            /*
            // Use double buffered drawing.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
             * */
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

        public void Setup(int gridWidth)
        {
            int wallWidth = (int)(0.3 * gridWidth);
            if (wallWidth < 1) { wallWidth = 1; }
            int squareWidth = gridWidth - wallWidth;
            int pathWidth = (int)(0.7 * squareWidth);

            this.Setup(squareWidth, wallWidth, pathWidth);
        }

        internal void Setup()
        {
            Random r = new Random();
            int gridWidth = r.Next(MinGridWidth, MaxGridWidth);
            
            this.Setup(gridWidth);
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

            try
            {
                this.MazeForm.StatusLine = "Size = " + xSize + "x" + ySize;
            }
            catch (InvalidCastException) { }
        }

        private void PlaceEndpoints()
        {
            maze.PlaceEndpoints();
        }

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
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

            // Destroy the current buffer; it will be re-created in the OnPaint() method.
            if (gBuffer != null)
            {
                gBuffer.Dispose();
                gBuffer = null;
            }

            this.Invalidate();
        }

        #endregion

        #region Painting methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //MazeForm.StatusLine = "called OnPaint()";
            

            if (this.wallWidth == 0)
            {
                this.Setup(12, 3, 8);
            }

            // On first time, create a graphics buffer and draw the static maze.
            //
            if (gBuffer == null)
            {
                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                gBuffer = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

                Graphics g = gBuffer.Graphics;

                // The PaintWalls() method fails in design mode.
                try
                {
                    PaintBorder(g);
                    PaintWalls(g);
                    PaintEndpoints(g);
                    //PaintPath(g);
                }
                catch (MissingMethodException) { }
            }

            gBuffer.Render();
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

        private void PaintEndpoints(Graphics g)
        {
            int x, y;
            maze.GetStartCoordinates(out x, out y);
            PaintSquare(g, Brushes.Red, x, y);
            maze.GetEndCoordinates(out x, out y);
            PaintSquare(g, Brushes.Red, x, y);
        }

        private void PaintSquare(Graphics g, Brush b, int x, int y)
        {
            float cx = xOffset + wallWidth/2.0F + x * gridWidth;
            float cy = yOffset + wallWidth/2.0F + y * gridWidth;
            g.FillRectangle(b, cx, cy, squareWidth, squareWidth);
        }

        internal void PaintPath(MazeSquare sq1, MazeSquare sq2, bool forward)
        {
            float cx1 = xOffset + gridWidth / 2.0F + sq1.XPos * gridWidth;
            float cy1 = yOffset + gridWidth / 2.0F + sq1.YPos * gridWidth;
            float cx2 = xOffset + gridWidth / 2.0F + sq2.XPos * gridWidth;
            float cy2 = yOffset + gridWidth / 2.0F + sq2.YPos * gridWidth;

            Graphics g = gBuffer.Graphics;
            Pen p = (forward ? this.forwardPen : this.backwardPen);
            g.DrawLine(p, cx1, cy1, cx2, cy2);

            if (sq1 == maze.StartSquare || sq2 == maze.StartSquare || sq1 == maze.EndSquare || sq2 == maze.EndSquare)
            {
                this.PaintEndpoints(g);
            }
            gBuffer.Render();
        }

        #endregion
    }
}
