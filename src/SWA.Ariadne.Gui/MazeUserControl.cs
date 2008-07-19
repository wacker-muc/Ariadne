using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
{
    public partial class MazeUserControl : UserControl
        , IMazeControl
    {
        #region Constants

        /// <summary>
        /// Minimum and maximum grid width.
        /// </summary>
        public const int MinGridWidth = 2, MaxGridWidth = 40;

        /// <summary>
        /// Minimum and maximum grid width when using automatic settings.
        /// </summary>
        public const int MinAutoGridWidth = 6, MaxAutoGridWidth = 12;

        /// <summary>
        /// Miniumum and maximum square width.
        /// </summary>
        public const int MinSquareWidth = 1, MaxSquareWidth = MaxGridWidth - 1;

        /// <summary>
        /// Miniumum and maximum square width.
        /// </summary>
        public const int MinPathWidth = 1, MaxPathWidth = MaxSquareWidth;

        /// <summary>
        /// Miniumum and maximum wall width.
        /// </summary>
        public const int MinWallWidth = 1, MaxWallWidth = MaxGridWidth / 2;

        /// <summary>
        /// Two reference Colors for deriving forward and backward path colors.
        /// </summary>
        private static readonly Color MinColor = Color.DarkSlateBlue, MaxColor = Color.Gold;

        #endregion

        #region Member variables

        public Maze Maze
        {
            get { return maze; }
        }
        private Maze maze;

        private AriadneSettingsData settingsData;

        private int squareWidth;
        private int wallWidth;
        private int gridWidth;
        private int pathWidth;
        private int xOffset, yOffset;

        private static Color wallColor = Color.Gray;
        private Color forwardColor = Color.GreenYellow;
        private Color backwardColor = Color.Brown;
        private static Color deadEndColor = Color.FromArgb(64, 64, 64); // 25% dark gray

        private Pen wallPen;
        private Pen forwardPen;
        private Pen backwardPen;
        private Brush deadEndBrush = new SolidBrush(deadEndColor);

        /// <summary>
        /// A counter that switches the end square between two states:
        /// When it is 0 or another even number, it is painted normally (red).
        /// When it is an odd number, it is painted invisible (black).
        /// </summary>
        public int BlinkingCounter
        {
            get
            {
                return blinkingCounter;
            }
            set {
                blinkingCounter = value;
                if (gBuffer != null)
                {
                    PaintEndpoints(gBuffer.Graphics);
                    gBuffer.Render();
                    this.Update();
                }
            }
        }
        private int blinkingCounter = 0;

        private Brush StartSquareBrush
        {
            get { return Brushes.Red; }
        }

        private Brush EndSquareBrush
        {
            get
            {
                if (Maze.IsSolved)                 return StartSquareBrush;
                if (this.BlinkingCounter % 2 == 0) return StartSquareBrush;
                return Brushes.Black;
            }
        }

        /// <summary>
        /// A list of (scaled) images that will be painted in reserved areas of the maze.
        /// </summary>
        private List<Image> images = new List<Image>();

        /// <summary>
        /// A list of locations (in graphics coordinates) where the images will be painted.
        /// Note: Specifying a Point is not sufficient if the image's resolution differs from the graphics resolution.
        /// </summary>
        private List<Rectangle> imageLocations = new List<Rectangle>();

        /// <summary>
        /// A list of recently used images.  We'll try to avoid using the same images in rapid succession.
        /// </summary>
        private List<string> recentlyUsedImages = new List<string>();

        /// <summary>
        /// Returns true when the list of prepared images is not empty.
        /// </summary>
        public bool HasPreparedImages
        {
            get { return (images.Count > 0 && imageLocations.Count == 0); }
        }

        /// <summary>
        /// This buffer holds the graphics and is rendered in the control.
        /// </summary>
        private BufferedGraphics gBuffer;

        /// <summary>
        /// This buffer is used to prepare a new maze in Repeat Mode.
        /// </summary>
        private BufferedGraphics gBufferAlternate;

        /// <summary>
        /// When false, do not update the caption or status bar.
        /// </summary>
        private bool allowUpdates = true;

        internal IMazeForm MazeForm
        {
            get { return this.mazeForm; }
            set { this.mazeForm = value; }
        }
        IMazeForm mazeForm;

        #endregion

        #region Constructor and Initialization

        public MazeUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the graphics context in screen saver preview mode.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        internal void SetGraphics(Graphics g, Rectangle rect)
        {
            this.externalGraphics = g;
            //MessageBox.Show("Setting new drawing rectangle: " + rect.ToString(), "Debugging...", MessageBoxButtons.OK);
            this.externalRect = rect;
            this.Size = new Size(rect.Width, rect.Height);
            this.Location = rect.Location;
        }
        private Graphics externalGraphics = null;
        private Rectangle externalRect;

        public void Setup(int squareWidth, int wallWidth, int pathWidth)
        {
            this.squareWidth = squareWidth;
            this.wallWidth = wallWidth;
            this.gridWidth = squareWidth + wallWidth;
            this.pathWidth = pathWidth;

            AdjustPathWidth(squareWidth, ref pathWidth);
            ColorBuilder.SuggestColors(MinColor, MaxColor, out forwardColor, out backwardColor);
            CreateMaze();
            Reset();
        }

        public void Setup(int gridWidth)
        {
            int wallWidth;
            int squareWidth;
            int pathWidth;
            SuggestWidths(gridWidth, out squareWidth, out pathWidth, out wallWidth);

            this.Setup(squareWidth, wallWidth, pathWidth);
        }

        internal static void SuggestWidths(int gridWidth, out int squareWidth, out int pathWidth, out int wallWidth)
        {
            wallWidth = Math.Max(MinWallWidth, Math.Min(MaxWallWidth, (int)(0.3 * gridWidth)));
            squareWidth = gridWidth - wallWidth;
            pathWidth = (int)(0.7 * squareWidth);
            
            AdjustPathWidth(squareWidth, ref pathWidth);
        }

        public void Setup()
        {
            if (gBufferAlternate != null)
            {
                // the Setup() method was already executed for creating the alternate buffer
                this.Invalidate();
                return;
            }

            if (settingsData != null)
            {
                this.TakeParametersFrom(settingsData);
                return;
            }

            Random r = RandomFactory.CreateRandom();
            int gridWidth = r.Next(MinAutoGridWidth, MaxAutoGridWidth);

            if (externalGraphics != null)
            {
                gridWidth /= 2;
            }
            
            this.Setup(gridWidth);
        }

        /// <summary>
        /// Paint a new maze into an alternate graphics buffer that will be used at the next repetition.
        /// </summary>
        public void PrepareAlternateBuffer()
        {
            // An alternate buffer must only be prepared when the previous maze is solved.
            if (maze != null && maze.IsSolved != true)
            {
                return;
            }

            // The alternate buffer method doesn't work properly in the screen saver preview mode.
            if (externalGraphics != null)
            {
                return;
            }

            this.allowUpdates = false;
            
            this.Setup();

            gBufferAlternate = this.CreateGraphicsBuffer();
            Graphics g = gBufferAlternate.Graphics;
            PaintMaze(g);
            
            this.allowUpdates = true;
        }

        /// <summary>
        /// Make (squareWidth - pathWidth) an even number.
        /// That will make sure that the path is centered nicely between the walls.
        /// </summary>
        /// <returns></returns>
        private static void AdjustPathWidth(int squareWidth, ref int pathWidth)
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
                // Note: In the designer, the MazeForm property is not valid.
                this.MazeForm.MakeReservedAreas(maze);
            }
            catch { }

            maze.CreateMaze();

            try
            {
                // Note: In the designer, the MazeForm property is not valid.
                if (allowUpdates)
                {
                    this.MazeForm.UpdateStatusLine();
                    this.MazeForm.UpdateCaption();
                }
            }
            catch { }
        }

        /// <summary>
        /// Calculate width and xOffset.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="yOffset"></param>
        private void FitMazeWidth(out int width, out int xOffset)
        {
            int w = (externalGraphics != null ? externalRect.Width : this.Width);
            width = (w - this.wallWidth - 4) / this.gridWidth;
            xOffset = (w - width * this.gridWidth) / 2;
        }

        /// <summary>
        /// Calculate height and yOffset.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="xOffset"></param>
        private void FitMazeHeight(out int height, out int yOffset)
        {
            int h = (externalGraphics != null ? externalRect.Height : this.Height);
            height = (h - this.wallWidth - 4) / this.gridWidth;
            yOffset = (h - height * this.gridWidth) / 2;
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
            x = XCoordinate(coveringControl.Left, true);
            y = YCoordinate(coveringControl.Top, true);

            w = 1 + XCoordinate(coveringControl.Right - 1, false) - x;
            h = 1 + YCoordinate(coveringControl.Bottom - 1, false) - y;

            if (0 < x && x + w < maze.XSize)
            {
                w = 1 + (coveringControl.Width + wallWidth + 6) / gridWidth;
            }
            if (0 < y && y + h < maze.YSize)
            {
                h = 1 + (coveringControl.Height + wallWidth + 6) / gridWidth;
            }


            bool result = maze.ReserveRectangle(x, y, w, h);

            // Move the control into the center of the reserved area.
            if (result)
            {
#if false
                // Adjust the control's size to make it fit symmetrically into the given space
                coveringControl.Width += coveringControl.Width % 2;
                coveringControl.Width -= (w * gridWidth - wallWidth - coveringControl.Width) % 2;
                coveringControl.Height += coveringControl.Height % 2;
                coveringControl.Height -= (h * gridWidth - wallWidth - coveringControl.Height) % 2;
#endif

                int cx = coveringControl.Left;
                int cy = coveringControl.Top;

                if (0 < x && x + w < maze.XSize)
                {
                    cx = this.Location.X + xOffset + x * gridWidth;
                    cx += 1 + (w * gridWidth - wallWidth - coveringControl.Width) / 2;
                }
                if (0 < y && y + h < maze.YSize)
                {
                    cy = this.Location.Y + yOffset + y * gridWidth;
                    cy += 1 + (h * gridWidth - wallWidth - coveringControl.Height) / 2;
                }
                
                // Adjust the control's location
                coveringControl.Location = new Point(cx, cy);
            }

            return result;
        }

        private int XCoordinate(int xLocation, bool leftBiased)
        {
            int result = (xLocation - this.Location.X);
            result -= xOffset;
            result += (leftBiased ? -1 : +1) * wallWidth;
            result /= gridWidth;
            
            return result;
        }

        private int YCoordinate(int yLocation, bool topBiased)
        {
            int result = (yLocation - this.Location.Y);
            result -= yOffset;
            result += (topBiased ? -1 : +1) * wallWidth;
            result /= gridWidth;

            return result;
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

            if (allowUpdates)
            {
                this.Invalidate();
            }

            // If the window is minimized, there will be no OnPaint() event.
            // Therefore we Paint the maze directly.
            if (this.ParentForm.WindowState == FormWindowState.Minimized)
            {
                // TODO: Reset() is called twice but should be called only once.
                this.PaintMaze();
            }
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
                // Use the previously prepared alternate buffer, if possible.
                if (gBufferAlternate != null)
                {
                    // For a brief moment, display a black screen.
                    Graphics g = this.CreateGraphics();
                    g.FillRectangle(Brushes.Black, this.DisplayRectangle);
                    g.Flush();
                    System.Threading.Thread.Sleep(120); // milliseconds

                    gBuffer = gBufferAlternate;
                    gBufferAlternate = null;

                    // An update of the status line and caption has been delayed until now.
                    //MazeForm.UpdateStatusLine();
                    MazeForm.UpdateCaption();
                }
                else
                {
                    PaintMaze();
                }
            }

            gBuffer.Render();
        }

        /// <summary>
        /// Creates the GraphicsBuffer and draws the static maze.
        /// </summary>
        internal void PaintMaze()
        {
            gBuffer = CreateGraphicsBuffer();
            Graphics g = gBuffer.Graphics;
            PaintMaze(g);
        }

        /// <summary>
        /// Creates a new GraphicsBuffer associated with the current graphics context.
        /// </summary>
        /// <returns></returns>
        private BufferedGraphics CreateGraphicsBuffer()
        {
            BufferedGraphics result;
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            if (externalGraphics != null)
            {
                //MessageBox.Show("Allocating an external graphics buffer: " + externalRect.ToString(), "Debugging...", MessageBoxButtons.OK);
                result = currentContext.Allocate(externalGraphics, externalRect);
            }
            else
            {
                result = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            }
            return result;
        }

        /// <summary>
        /// Creates the GraphicsBuffer and draws the static maze.
        /// </summary>
        private void PaintMaze(Graphics g)
        {
            // The PaintWalls() method fails in design mode.
            try
            {
                if (settingsData != null && settingsData.VisibleOutlines)
                {
                    PaintShapes(g);
                }

                PaintBorder(g);
                PaintWalls(g);
                PaintEndpoints(g);
                PaintImages(g);
            }
            catch (MissingMethodException) { }
        }

        private void PaintShapes(Graphics g)
        {
            // Temporarily set zero width walls; thus, the squares will be drawn seamlessly.
            int savedWallWidth = wallWidth;
            wallWidth = 0;
            squareWidth = gridWidth;

            Color shapeColor = Color.FromArgb(0, 0, 50); // dark blue
            Brush shapeBrush = new SolidBrush(shapeColor);
            for (int x = 0; x < XSize; x++)
            {
                for (int y = 0; y < YSize; y++)
                {
                    int n = maze.CountCoveringOutlineShapes(x, y);
                    if (n % 2 == 1)
                    {
                        this.PaintSquare(g, shapeBrush, x, y);
                    }
                }
            }

            wallWidth = savedWallWidth;
            squareWidth = gridWidth - wallWidth;
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
            PaintSquare(g, this.StartSquareBrush, maze.StartSquare.XPos, maze.StartSquare.YPos);
            PaintSquare(g, this.EndSquareBrush, maze.EndSquare.XPos, maze.EndSquare.YPos);
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
        /// Paints the images into their reserved areas.
        /// </summary>
        /// <param name="g"></param>
        private void PaintImages(Graphics g)
        {
            for (int i = 0; i < images.Count; i++)
            {
                g.DrawImage(images[i], imageLocations[i]);
            }
        }

        /// <summary>
        /// Paints a section of the path between the given (adjoining) squares.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <param name="forward"></param>
        public void DrawStep(MazeSquare sq1, MazeSquare sq2, bool forward)
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
        public void FinishPath(MazeSquare sq)
        {
            if (sq != null && sq != maze.EndSquare)
            {
                this.PaintPathDot(sq);
            }

            // Quit if screen saver preview dialog is dismissed.  Check this periodically.
            if (externalGraphics != null)
            {
                // Quit if screen saver preview dialog is dismissed.  Check this periodically.
                try
                {
                    gBuffer.Render();
                }
                catch (ArgumentException)
                {
                    Application.Exit();
                }
            }
            else
            {
                gBuffer.Render();
            }

            // Finally, update the display.
            this.Update();
        }

        /// <summary>
        /// Paints a dot in forward direction at the square.
        /// Covers up for drawing a backward path into a square on the forward path.
        /// </summary>
        /// <param name="sq"></param>
        private void PaintPathDot(MazeSquare sq)
        {
            PaintPathDot(sq, this.forwardPen.Brush);
        }

        private void PaintPathDot(MazeSquare sq, Brush brush)
        {
            float cx = xOffset + gridWidth / 2.0F + sq.XPos * gridWidth;
            float cy = yOffset + gridWidth / 2.0F + sq.YPos * gridWidth;

            // Draw a dot at sq2.
            Graphics g = gBuffer.Graphics;
            g.FillRectangle(brush, cx - pathWidth / 2.0F, cy - pathWidth / 2.0F, pathWidth, pathWidth);
        }

        /// <summary>
        /// Paints the path between all MazeSquares in the given list in the backward color.
        /// </summary>
        /// <param name="path">List of MazeSquares starting at a dead end and ending at a branching square (not dead)</param>
        /// <param name="forward"></param>
        public void DrawPath(List<MazeSquare> path, bool forward)
        {
            for (int i = 1; i < path.Count; i++)
            {
                this.DrawStep(path[i - 1], path[i], forward);
            }
            
            // Redraw the square where the branching occurred.
            MazeSquare sq = path[path.Count - 1];
            if (sq == maze.StartSquare)
            {
                this.PaintEndpoints(gBuffer.Graphics);
            }
            else
            {
                this.PaintPathDot(sq);
            }
        }

        /// <summary>
        /// Draws a highlighted path between the given squares.
        /// </summary>
        /// <param name="path"></param>
        public void DrawSolvedPath(List<MazeSquare> path)
        {
            float h = forwardColor.GetHue();
            float s = MaxColor.GetSaturation();
            float b = MaxColor.GetBrightness();

            // Make s and b 30% bigger.
            s = 0.7F * s + 0.3F;
            b = 0.7F * b + 0.3F;

            // Make s and b sufficiently different from the forward color.
            s = Math.Max(s, 0.6F * forwardColor.GetSaturation() + 0.4F);
            b = Math.Max(b, 0.6F * forwardColor.GetBrightness() + 0.4F);

            Color highlightColor = ColorBuilder.ConvertHSBToColor(h, s, b);
            Pen p = new Pen(highlightColor, pathWidth);
            p.StartCap = p.EndCap = System.Drawing.Drawing2D.LineCap.Square;

            PointF[] points = new PointF[path.Count];

            for (int i = 0; i < path.Count; i++)
            {
                MazeSquare sq = path[i];

                float cx = xOffset + gridWidth / 2.0F + sq.XPos * gridWidth;
                float cy = yOffset + gridWidth / 2.0F + sq.YPos * gridWidth;

                points[i] = new PointF(cx, cy);
            }

            Graphics g = gBuffer.Graphics;
            g.DrawLines(p, points);

            this.PaintEndpoints(g);
        }

        /// <summary>
        /// Paints a square to mark it as "dead".
        /// </summary>
        /// <param name="sq"></param>
        public void DrawDeadSquare(MazeSquare sq)
        {
            PaintPathDot(sq, deadEndBrush);
        }

        /// <summary>
        /// Paints a square to mark it as "alive".
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="distance"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public void DrawAliveSquare(MazeSquare sq, int distance, bool initialDrawing)
        {
            if (squareWidth >= 10)
            {
                Graphics g = gBuffer.Graphics;
                Font font = new Font("Helvetica", 6);
                int digitHeight = (int)font.GetHeight();
                int digitWidth = (int)(digitHeight * 0.8);
                float cx = xOffset + gridWidth / 2.0F + sq.XPos * gridWidth - (digitWidth / 2.0F);
                float cy = yOffset + gridWidth / 2.0F + sq.YPos * gridWidth - (digitHeight / 2.0F);
                if( distance > 0)
                {
                    Brush digitBrush = (initialDrawing ? Brushes.White : Brushes.Yellow);
                    g.FillRectangle(Brushes.Black, cx, cy, digitWidth, digitHeight);
                    g.DrawString(string.Format("{0}", (distance % 10)), font, digitBrush, new RectangleF(cx, cy, squareWidth, squareWidth));
                }
            }
        }

        #endregion

        #region IAriadneSettingsSource implementation

        /// <summary>
        /// Fill all modifyable parameters into the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void FillParametersInto(AriadneSettingsData data)
        {
            if (settingsData != null)
            {
                data.CopyContentsParameters(settingsData);
            }

            data.GridWidth = this.gridWidth;
            data.PathCapStyle = System.Drawing.Drawing2D.LineCap.Square;
            data.PathWidth = this.pathWidth;
            data.SquareWidth = this.squareWidth;
            data.WallWidth = this.wallWidth;

            data.ReferenceColor1 = MaxColor;
            data.ReferenceColor2 = MinColor;
            data.ForwardColor = this.forwardColor;
            data.BackwardColor = this.backwardColor;

            this.maze.FillParametersInto(data);
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            this.settingsData = data;

            #region Take parameters concerning this MazeUserControl

            #region Layout

            if (!data.AutoGridWidth)
            {
                this.gridWidth = Math.Max(2, Math.Min(MaxGridWidth, data.GridWidth));

                SuggestWidths(gridWidth, out squareWidth, out pathWidth, out wallWidth);
            }
            else if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
            {
                this.wallWidth = Math.Max(MinWallWidth, Math.Min(MaxWallWidth, data.WallWidth));
                this.squareWidth = Math.Max(MinSquareWidth, Math.Min(MaxGridWidth - wallWidth, data.SquareWidth));
                this.pathWidth = Math.Max(MinPathWidth, Math.Min(squareWidth, data.PathWidth));

                this.gridWidth = squareWidth + wallWidth;
            }
            else
            {
                Random r = maze.Random;
                this.gridWidth = r.Next(MinAutoGridWidth, MaxAutoGridWidth);
                SuggestWidths(gridWidth, out squareWidth, out pathWidth, out wallWidth);
            }

            #endregion

            #region Colors

            if (!data.AutoColors)
            {
                this.forwardColor = data.ForwardColor;
                this.backwardColor = data.BackwardColor;
            }
            else
            {
                ColorBuilder.SuggestColors(MinColor, MaxColor, out forwardColor, out backwardColor);
            }

            #endregion

            #endregion

            // Make sure that we have a Maze object.
            if (maze == null)
            {
                maze = new Maze(data.MazeWidth, data.MazeHeight);
            }

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
            AdjustPathWidth(squareWidth, ref pathWidth);
            MazeForm.MakeReservedAreas(maze);
            this.ReserveAreasForImages(data);
            this.AddOutlineShape(data);
            maze.Irregular = data.IrregularMaze;
            maze.Irregularity = data.Irregularity;
            maze.CreateMaze();
            MazeForm.UpdateStatusLine();
            MazeForm.UpdateCaption();

            Reset();

            #endregion

            this.mazeForm.UpdateCaption();
        }

        #endregion

        #region Placement of images

        private void ReserveAreasForImages(AriadneSettingsData data)
        {
            int count = data.ImageNumber;
            int minSize = data.ImageMinSize;
            int maxSize = data.ImageMaxSize;
            string imageFolder = data.ImageFolder;

            ReserveAreaForImages(count, minSize, maxSize, imageFolder);
        }

        public void ReserveAreaForImages(int count, int minSize, int maxSize, string imageFolder)
        {
            PrepareImages(count, minSize, maxSize, imageFolder);
            ReserveAreaForImages();
        }

        public void PrepareImages(int count, int minSize, int maxSize, string imageFolder)
        {
            #region Determine number of images to be placed into reserved areas.

            Random r = maze.Random;
            int n, nMin, nMax = count;

            if (nMax <= 2)
            {
                nMin = nMax;
            }
            else
            {
                nMin = nMax * 2 / 3;
            }

            if (nMax > 0)
            {
                n = r.Next(nMin, nMax);
            }
            else
            {
                return;
            }

            #endregion

            images.Clear();
            imageLocations.Clear();

            foreach (string imagePath in FindImages(imageFolder, n))
            {
                try
                {
                    Image img = new Bitmap(imagePath);

                    #region Scale img so that its larger dimension is between the data's min and max size.

                    if (img.Width > maxSize || img.Height > maxSize)
                    {
                        int d = r.Next(minSize, maxSize);
                        int h = img.Height, w = img.Width;
                        if (h > w)
                        {
                            w = d * w / h;
                            h = d;
                        }
                        else
                        {
                            h = d * h / w;
                            w = d;
                        }
                        img = new Bitmap(img, new Size(w, h));
                    }

                    #endregion

                    images.Add(img);
                }
                catch (Exception e)
                {
                    System.Console.Out.WriteLine("failed loading image [{0}]: {1}", imagePath, e.ToString());
                }
            }
        }

        public void ReserveAreaForImages()
        {
            if (this.HasPreparedImages)
            {
                for (int i = 0; i < images.Count; i++ )
                {
                    Image img = images[i];
                    if (!AddImage(img))
                    {
                        images.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private List<string> FindImages(string folderPath, int count)
        {
            if (folderPath == null || count < 1)
            {
                return new List<string>();
            }

            List<string> availableImages = new List<string>();

            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.jpg", true));
            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.gif", true));
            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.png", true));

            List<string> result = new List<string>(count);
            Random r = maze.Random;

            // Shorten the list of recently used images.
            // Make sure the list does not get too short.
            while (this.recentlyUsedImages.Count > 0
                && this.recentlyUsedImages.Count > availableImages.Count - count
                && this.recentlyUsedImages.Count > availableImages.Count * 3 / 4
                )
            {
                // Remove an item near the beginning of the list (recently added items are at the end).
                this.recentlyUsedImages.RemoveAt(r.Next(this.recentlyUsedImages.Count / 3 + 1));
            }

            // Select the required number of images.
            // Avoid recently used images.
            while (result.Count < count && availableImages.Count > 0)
            {
                int p = r.Next(availableImages.Count);
                string imagePath = availableImages[p];

                if (!recentlyUsedImages.Contains(imagePath))
                {
                    result.Add(imagePath);
                    recentlyUsedImages.Add(imagePath);
                }

                availableImages.RemoveAt(p);
            }

            return result;
        }

        private bool AddImage(Image img)
        {
            int sqW = (img.Width + 8 + this.wallWidth) / this.gridWidth + 1;
            int sqH = (img.Height + 8 + this.wallWidth) / this.gridWidth + 1;

            Rectangle rect;
            if (maze.ReserveRectangle(sqW, sqH, 2, out rect))
            {
                // Remember the image data and location.  It will be painted in PaintMaze().
                int x = rect.X * gridWidth + xOffset + (rect.Width * gridWidth - img.Width) / 2;
                int y = rect.Y * gridWidth + yOffset + (rect.Height * gridWidth - img.Height) / 2;
                imageLocations.Add(new Rectangle(x, y, img.Width, img.Height));
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region Placement of outline shapes

        private void AddOutlineShape(AriadneSettingsData data)
        {
            Random r = maze.Random;

            double offCenter = data.OutlineOffCenter / 100.0;
            double size = data.OutlineSize / 100.0;

            OutlineShape.OutlineShapeBuilder shapeBuilderDelegate = null;

            switch (data.OutlineKind)
            {
                case AriadneSettingsData.OutlineKindEnum.Random:
                    shapeBuilderDelegate = OutlineShape.RandomOutlineShapeBuilder(r);
                    break;
                case AriadneSettingsData.OutlineKindEnum.Circle:
                    shapeBuilderDelegate = OutlineShape.Circle;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Diamond:
                    shapeBuilderDelegate = OutlineShape.Diamond;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Character:
                    shapeBuilderDelegate = OutlineShape.Character;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Symbol:
                    shapeBuilderDelegate = OutlineShape.Symbol;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Polygon:
                    shapeBuilderDelegate = OutlineShape.Polygon;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Function:
                    shapeBuilderDelegate = OutlineShape.Function;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Bitmap:
                    shapeBuilderDelegate = OutlineShape.Bitmap;
                    break;
            }
            if (shapeBuilderDelegate != null)
            {
                OutlineShape shape = OutlineShape.Instance(r, shapeBuilderDelegate, XSize, YSize, offCenter, size);
                AddOutlineShape(shape);
            }
        }

        internal void AddOutlineShape(OutlineShape shape)
        {
            this.maze.AddOutlineShape(shape);
        }

        #endregion

        #region IMazeControl implementation

        public bool IsSolved
        {
            get { return (gBufferAlternate != null ? true : maze == null ? false : maze.IsSolved); }
        }

        public int XSize
        {
            get { return (maze == null ? -1 : maze.XSize); }
        }

        public int YSize
        {
            get { return (maze == null ? -1 : maze.YSize); }
        }

        public string Code
        {
            get { return (maze == null ? "---" : maze.Code); }
        }

        #endregion
    }
}
