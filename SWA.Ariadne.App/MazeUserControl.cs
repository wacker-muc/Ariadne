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

        /// <summary>
        /// Paints the contents of this control by rendering the GraphicsBuffer.
        /// On first time, the buffer is created and the maze (without any path) is painted.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

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
            //gBuffer.Render(e.Graphics);
        }

        /// <summary>
        /// Paints a border around the maze.
        /// </summary>
        /// <param name="g"></param>
        private void PaintBorder(Graphics g)
        {
            g.DrawRectangle(wallPen, new Rectangle(xOffset, yOffset, maze.XSize * gridWidth, maze.YSize * gridWidth));
        }

        /// <summary>
        /// Paints the closed inner walls.
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

        /// <summary>
        /// Paints the start and end point.
        /// </summary>
        /// <param name="g"></param>
        private void PaintEndpoints(Graphics g)
        {
            int x, y;
            maze.GetStartCoordinates(out x, out y);
            PaintSquare(g, Brushes.Red, x, y);
            maze.GetEndCoordinates(out x, out y);
            PaintSquare(g, Brushes.Red, x, y);
        }

        /// <summary>
        /// Fills one square with the given color.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void PaintSquare(Graphics g, Brush b, int x, int y)
        {
            float cx = xOffset + wallWidth/2.0F + x * gridWidth;
            float cy = yOffset + wallWidth/2.0F + y * gridWidth;
            g.FillRectangle(b, cx, cy, squareWidth, squareWidth);
        }

        /// <summary>
        /// Paints a section of the path between the given (adjoining) squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <param name="forward"></param>
        internal void PaintPath(MazeSquare sq1, MazeSquare sq2, bool forward)
        {
            float cx1 = xOffset + gridWidth / 2.0F + sq1.XPos * gridWidth;
            float cy1 = yOffset + gridWidth / 2.0F + sq1.YPos * gridWidth;
            float cx2 = xOffset + gridWidth / 2.0F + sq2.XPos * gridWidth;
            float cy2 = yOffset + gridWidth / 2.0F + sq2.YPos * gridWidth;

            // Draw a line from sq1 to sq2.
            Graphics g = gBuffer.Graphics;
            Pen p = (forward ? this.forwardPen : this.backwardPen);
            g.DrawLine(p, cx1, cy1, cx2, cy2);

            // Maybe redraw the end point.
            if (sq1 == maze.StartSquare || sq2 == maze.StartSquare || sq1 == maze.EndSquare || sq2 == maze.EndSquare)
            {
                this.PaintEndpoints(g);
            }
        }

        /// <summary>
        /// Paints a dot (in the forward color) at the given square.
        /// Renders the GraphicsBuffer.
        /// </summary>
        /// <param name="sq">when null, no dot is drawn</param>
        internal void FinishPath(MazeSquare sq)
        {
            if (sq != null && sq != maze.EndSquare)
            {
                float cx = xOffset + gridWidth / 2.0F + sq.XPos * gridWidth;
                float cy = yOffset + gridWidth / 2.0F + sq.YPos * gridWidth;

                // Draw a dot at sq2.
                Graphics g = gBuffer.Graphics;
                g.FillRectangle(this.forwardPen.Brush, cx - pathWidth / 2.0F, cy - pathWidth / 2.0F, pathWidth, pathWidth);
            }

            gBuffer.Render();
        }

        #endregion
    }
}
