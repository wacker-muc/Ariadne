using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
{
    public partial class MazeUserControl : UserControl
        , IAriadneSettingsSource
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

        internal IMazeForm MazeForm
        {
            get { return (IMazeForm)this.ParentForm; }
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

            AdjustPathWidth();
            CreateMaze();
            PlaceEndpoints();
            Reset();
        }

        public void Setup(int gridWidth)
        {
            int wallWidth;
            int squareWidth;
            int pathWidth;
            SuggestWidths(gridWidth, out wallWidth, out squareWidth, out pathWidth);

            this.Setup(squareWidth, wallWidth, pathWidth);
        }

        private static void SuggestWidths(int gridWidth, out int wallWidth, out int squareWidth, out int pathWidth)
        {
            wallWidth = (int)(0.3 * gridWidth);
            if (wallWidth < 1) { wallWidth = 1; }
            squareWidth = gridWidth - wallWidth;
            pathWidth = (int)(0.7 * squareWidth);
        }

        internal void Setup()
        {
            Random r = new Random();
            int gridWidth = r.Next(MinGridWidth, MaxGridWidth);
            
            this.Setup(gridWidth);
        }

        /// <summary>
        /// Make (squareWidth - pathWidth) an even number.
        /// That will make sure that the path is centered nicely between the walls.
        /// </summary>
        /// <returns></returns>
        private void AdjustPathWidth()
        {
            if ((squareWidth - pathWidth) % 2 != 0)
            {
                pathWidth -= 1;
            }
            if (pathWidth < 2)
            {
                pathWidth = squareWidth;
            }
        }

        /// <summary>
        /// Construct a maze 
        /// </summary>
        private void CreateMaze()
        {
            // Determine dimensions of a maze that fits into the drawing area.
            int xSize, ySize;
            FitMazeWidth(out xSize, out this.xOffset);
            FitMazeHeight(out ySize, out this.yOffset);

            // Create a maze.
            this.maze = new Maze(xSize, ySize);
            try
            {
                this.MazeForm.MakeReservedAreas(maze);
            }
            catch (InvalidCastException) { }
            maze.CreateMaze();

            try
            {
                this.MazeForm.UpdateStatusLine();
            }
            catch (InvalidCastException) { }
        }

        private void FitMazeWidth(out int width, out int offset)
        {
            width = (this.Width - this.wallWidth - 4) / this.gridWidth;
            offset = (this.Width - width * this.gridWidth) / 2;
        }

        private void FitMazeHeight(out int height, out int offset)
        {
            height = (this.Height - this.wallWidth - 4) / this.gridWidth;
            offset = (this.Height - height * this.gridWidth) / 2;
        }

        /// <summary>
        /// Reserves a region of the maze covered by the coveringControl.
        /// This MazeUserControl and the coveringControl must have the same Parent, i.e. a common coordinate system.
        /// </summary>
        /// <param name="coveringControl"></param>
        /// <exception cref="ArgumentException">The given Control has a differnent Parent.</exception>
        internal bool ReserveArea(Control coveringControl)
        {
            if (coveringControl.Parent != this.Parent)
            {
                throw new ArgumentException("Must have the same Parent.", "coveringControl");
            }

            // Dimensions of the control in square coordinates.
            int x, y, w, h;
            x = XCoordinate(coveringControl.Left);
            y = YCoordinate(coveringControl.Top);
            w = 1 + XCoordinate(coveringControl.Right) - x;
            h = 1 + XCoordinate(coveringControl.Bottom) - y;

            bool result = maze.ReserveRectangle(x, y, w, h);

            // Move the control into the center of the reserved area.
            if (result)
            {
                int cx = coveringControl.Location.X;
                int cy = coveringControl.Location.Y;

                if (0 < x && x + w < maze.XSize - 1)
                {
                    cx = this.Location.X + xOffset + x * gridWidth;
                    cx += (w * gridWidth - coveringControl.Width) / 2;
                }
                if (0 < y && x + w < maze.XSize - 1)
                {
                    cy = this.Location.Y + yOffset + y * gridWidth;
                    cy += (h * gridWidth - coveringControl.Height) / 2;
                }
                
                coveringControl.Location = new Point(cx, cy);
            }

            return result;
        }

        private int XCoordinate(int xLocation)
        {
            int result = (xLocation - this.Location.X);
            result -= xOffset;
            result /= gridWidth;
            
            return result;
        }

        private int YCoordinate(int yLocation)
        {
            int result = (yLocation - this.Location.Y);
            result -= yOffset;
            result /= gridWidth;

            return result;
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
                PaintMaze();
            }

            gBuffer.Render();
            //gBuffer.Render(e.Graphics);
        }

        /// <summary>
        /// Creates the GraphicsBuffer and draws the static maze.
        /// </summary>
        internal void PaintMaze()
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

        /// <summary>
        /// Paints a border around the maze.
        /// </summary>
        /// <param name="g"></param>
        private void PaintBorder(Graphics g)
        {
            /* Actually, we only paint the east and south wall of every
             * square on the respective border.
             * We don't optimize by drawing long lines or a rectangle
             * because there may be reserved areas on the border
             * and the walls may be open instead of closed.
             */

            // Draw the south walls of every square on the southern border.
            for (int x = 0; x < maze.XSize; x++)
            {
                int cx = xOffset + x * gridWidth;
                int cy = yOffset + maze.YSize * gridWidth;
                MazeSquare sq = maze[x, maze.YSize-1];

                if (sq[MazeSquare.WallPosition.WP_S] == MazeSquare.WallState.WS_CLOSED)
                {
                    g.DrawLine(wallPen, cx, cy, cx + gridWidth, cy);
                }
            }

            // Draw the east walls of every square on the eastern border.
            for (int y = 0; y < maze.YSize; y++)
            {
                int cy = yOffset + y * gridWidth;
                int cx = xOffset + maze.XSize * gridWidth;
                MazeSquare sq = maze[maze.XSize-1, y];

                if (sq[MazeSquare.WallPosition.WP_E] == MazeSquare.WallState.WS_CLOSED)
                {
                    g.DrawLine(wallPen, cx, cy, cx, cy + gridWidth);
                }
            }
        }

        /// <summary>
        /// Paints the closed inner walls.
        /// </summary>
        /// <param name="g"></param>
        private void PaintWalls(Graphics g)
        {
            // We'll only draw the west and north walls of every square.
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

        #region IAriadneSettingsSource implementation

        /// <summary>
        /// Fill all modifyable parameters into the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void FillParametersInto(AriadneSettingsData data)
        {
            data.GridWidth = this.gridWidth;
            data.PathCapStyle = System.Drawing.Drawing2D.LineCap.Square;
            data.PathWidth = this.pathWidth;
            data.SquareWidth = this.squareWidth;
            data.WallWidth = this.wallWidth;

            this.maze.FillParametersInto(data);
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            #region Take parameters concerning this MazeUserControl

            int maxAcceptedGridWidth = MaxGridWidth * 3;
            int maxAcceptedWallWidth = maxAcceptedGridWidth / 2;

            if (!data.AutoGridWidth)
            {
                this.gridWidth = Math.Max(2, Math.Min(maxAcceptedGridWidth, data.GridWidth));

                SuggestWidths(gridWidth, out wallWidth, out squareWidth, out pathWidth);
            }
            else if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
            {
                this.wallWidth = Math.Max(1, Math.Min(maxAcceptedWallWidth, data.WallWidth));
                this.squareWidth = Math.Max(1, Math.Min(maxAcceptedGridWidth - wallWidth, data.SquareWidth));
                this.pathWidth = Math.Max(1, Math.Min(squareWidth, data.PathWidth));

                this.gridWidth = squareWidth + wallWidth;
            }
            else
            {
                Random r = new Random();
                this.gridWidth = r.Next(MinGridWidth, MaxGridWidth);
                SuggestWidths(gridWidth, out wallWidth, out squareWidth, out pathWidth);
            }

            #endregion

            #region Adjust automatic parameters of the underlying Maze

            if (data.AutoMazeWidth)
            {
                int width;
                FitMazeWidth(out width, out this.xOffset);
                data.MazeWidth = width;
            }
            if (data.AutoMazeHeight)
            {
                int height;
                FitMazeHeight(out height, out this.yOffset);
                data.MazeHeight = height;
            }

            #endregion

            maze.TakeParametersFrom(data);

            #region Do the equivalent of Setup() with the modified parameters.

            // CreateMaze()
            AdjustPathWidth();
            MazeForm.MakeReservedAreas(maze);
            maze.CreateMaze();
            MazeForm.UpdateStatusLine();

            PlaceEndpoints();

            Reset();

            #endregion
        }

        #endregion
    }
}