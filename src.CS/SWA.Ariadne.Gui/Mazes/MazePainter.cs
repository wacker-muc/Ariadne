using System;
using System.Collections.Generic;
using System.Drawing;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;
using SWA.Ariadne.Outlines;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Mazes
{
    /// <summary>
    /// The MazePainter is responsible for the painting operations in a MazeUserControl.
    /// It only needs the control's Graphics object but none of its Control abilities.
    /// </summary>
    public class MazePainter
        : IMazeDrawer
        , IAriadneSettingsSource
        , IImageLoader
    {
        #region Constants

        /// <summary>
        /// Minimum and maximum grid width.
        /// </summary>
        public static readonly int MinGridWidth = 2, MaxGridWidth = 40;

        /// <summary>
        /// Minimum and maximum grid width when using automatic settings.
        /// </summary>
        private static readonly int MinAutoGridWidth = 6, MaxAutoGridWidth = 12;

        /// <summary>
        /// Minimum and maximum grid width when using automatic settings without walls.
        /// </summary>
        private static readonly int MinAutoGridWidthWithoutWalls = 4, MaxAutoGridWidthWithoutWalls = 9;

        /// <summary>
        /// Miniumum and maximum square width.
        /// </summary>
        public static readonly int MinSquareWidth = 1, MaxSquareWidth = MaxGridWidth - 1;

        /// <summary>
        /// Miniumum and maximum path width.
        /// </summary>
        public static readonly int MinPathWidth = 1, MaxPathWidth = MaxSquareWidth;

        /// <summary>
        /// Miniumum and maximum wall width.
        /// </summary>
        public static readonly int MinWallWidth = 1, MaxWallWidth = MaxGridWidth / 2;

        /// <summary>
        /// Two reference Colors for deriving forward and backward path colors.
        /// </summary>
        private static readonly Color MinColor = Color.DarkSlateBlue, MaxColor = Color.Gold;

        public static readonly int Dpi = 96;

        /// <summary>
        /// Static initializer.
        /// All the '...Width' range "constants" are multiplied by a scaling factor.
        /// The scaling compensates for the visual size reduction on high resolution screens.
        /// </summary>
        static MazePainter()
        {
            float scale = 1.0F;
            try
            {
                Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                scale = Math.Max (scale, graphics.DpiX / 96.0F); // never < 1
                Dpi = (int) graphics.DpiX;
            }
            catch (Exception ex)
            {
                Log.WriteLine("cannot get DPI value from Graphics object. " + ex.Message, true);
            }

            MinGridWidth = (int)(MinGridWidth * scale);
            MaxGridWidth = (int)(MaxGridWidth * scale);
            MinAutoGridWidth = (int)(MinAutoGridWidth * scale);
            MaxAutoGridWidth = (int)(MaxAutoGridWidth * scale);
            MinAutoGridWidthWithoutWalls = (int)(MinAutoGridWidthWithoutWalls * scale);
            MaxAutoGridWidthWithoutWalls = (int)(MaxAutoGridWidthWithoutWalls * scale);

            // derived from the above
            MinWallWidth = 1; MaxWallWidth = MaxGridWidth / 2;
            MinSquareWidth = 1; MaxSquareWidth = MaxGridWidth - MinWallWidth;
            MinPathWidth = 1; MaxPathWidth = MaxSquareWidth;
        }

        #endregion

        #region Types

        /// <summary>
        /// A method that is called when a new canvas has been created.
        /// </summary>
        /// <param name="g"></param>
        public delegate void PainterDelegate(Graphics g);

        /// <summary>
        /// A method that is called before the maze is built.
        /// </summary>
        /// <param name="maze"></param>
        public delegate void ConfigureMazeLayoutDelegate(Maze maze);

        #endregion

        #region Member variables

        // TODO: remove this variable
        private IMazePainterClient client;

        public /* TODO: readonly */ bool screenSaverPreviewMode = false;
        private Graphics targetGraphics;
        private Rectangle targetRectangle;

        public Maze Maze
        {
            get { return maze; }
        }
        private Maze maze;

        private AriadneSettingsData settingsData;

        public bool VisibleWalls
        {
            get
            {
                return (WallVisibility != AriadneSettingsData.WallVisibilityEnum.Never);
            }
        }
        private AriadneSettingsData.WallVisibilityEnum WallVisibility
        {
            get
            {
                return (this.settingsData == null ? this.wallVisibility : this.settingsData.WallVisibility);
            }
        }
        private AriadneSettingsData.WallVisibilityEnum wallVisibility = AriadneSettingsData.WallVisibilityEnum.Always;

        public bool RandomizeWallVisibility
        {
            set { randomizeWallVisibility = value; }
        }
        private bool randomizeWallVisibility = false;

        private int squareWidth;
        private int wallWidth = -1;
        private int gridWidth;
        private int pathWidth;

        /// <summary>
        /// Canvas coordinates of the middle of the top left wall.
        /// </summary>
        private int xOffset, yOffset;

        // TODO: Collect the public dimension attributes in a single structure.
        public int WallWidth { get { return this.wallWidth; } }
        public int GridWidth { get { return this.gridWidth; } }
        public int XOffset { get { return this.xOffset; } }
        public int YOffset { get { return this.yOffset; } }

        /// <summary>
        /// Space left around the painted maze inside the client's DisplayRectangle.
        /// In ScreenSaver mode, this should be 0.
        /// </summary>
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        private int padding = 2;

        private static Color wallColor = Color.Gray;
        private Color forwardColor = Color.GreenYellow;
        private Color backwardColor = Color.Brown;
        private static Color deadEndColor = Color.FromArgb(64, 64, 64); // 25% dark gray

        private Pen wallPen;
        private Pen forwardPen;
        private Pen backwardPen;
        private Pen deadEndPen;
        private Pen startSquarePen;
        private Pen blackSquarePen;

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
                // Ignore this message if we have not been properly started.
                if (blinkingCounter < 0 && value != 0)
                {
                    return;
                }

                // Forward this message to the shared painters.
                if (value > 0)
                {
                    foreach (MazePainter item in sharedPainters)
                    {
                        item.BlinkingCounter = value;
                    }
                }

                blinkingCounter = value;

                if (blinkingCounter >= 0 && gBuffer != null)
                {
                    PaintEndpoints(gBuffer.Graphics);
                    if (client == null || client.Alive)
                    {
                        gBuffer.Render();
                    }
                    if (client != null)
                    {
                        client.Update();
                    }
                }
            }
        }
        private int blinkingCounter = 0;

        private Pen StartSquarePen
        {
            get { return startSquarePen; }
        }

        private Pen EndSquarePen
        {
            get
            {
                if (Maze.IsSolved)                 return StartSquarePen;
                if (this.BlinkingCounter % 2 == 0) return StartSquarePen;
                return blackSquarePen;
            }
        }

        /// <summary>
        /// True if the MazePainter has been initialized completely and ready to
        /// receive Draw...() requests.
        /// </summary>
        /// <remarks>
        /// This is the case after the OnPaint() method has created its gBuffer.
        /// </remarks>
        public bool IsReady { get { return (gBuffer != null); } }

        #region Background Image

        /// <summary>
        /// A source of background images.
        /// </summary>
        public IImageLoader BackgroundImageLoader
        {
            get { return backgroundImageLoader; }
            set { backgroundImageLoader = value; }
        }
        private IImageLoader backgroundImageLoader;

        /// <summary>
        /// The ContourImage used to create the current backgroundImage.
        /// </summary>
        private ContourImage backgroundContourImage;

        /// <summary>
        /// A bitmap of the size of the targetRectangle with black (or transparent) background.
        /// One of the available background images is painted at a random location.
        /// Over that image, all (visible) walls are painted, as well.
        /// Initially, this image will not be visible.
        /// Only when a square is visited, the part of the image in that square will be displayed.
        /// </summary>
        private Bitmap backgroundImage;

        /// <summary>
        /// Squares that are covered completely by the background image.
        /// </summary>
        private Rectangle backgroundImageRange;

        /// <summary>
        /// Shape of the background image, if it has a contour.
        /// </summary>
        private OutlineShape backgroundImageShape;

        /// <summary>
        /// True if the square's background has already been displayed.
        /// </summary>
        private bool[,] backgroundPainted;

        delegate void BackgroundImageSetupCompletionDelegate();
        private BackgroundImageSetupCompletionDelegate backgroundImageSetupCompletion;

        #endregion

        /// <summary>
        /// This buffer holds the graphics and is rendered in the control.
        /// </summary>
        private BufferedGraphics gBuffer
        {
            get { return _gBuffer; }
            set
            {
                // move the current _gBuffer to gBufferPrevious
                if (gBufferPrevious != null)
                {
                    gBufferPrevious.Dispose();
                }
                gBufferPrevious = _gBuffer;

                _gBuffer = value;
            }
        }
        private BufferedGraphics _gBuffer;

        /// <summary>
        /// This buffer is used to prepare a new maze in Repeat Mode.
        /// </summary>
        private BufferedGraphics gBufferAlternate;

        /// <summary>
        /// This buffer is used to draw and save the solved maze in Repeat Mode.
        /// </summary>
        private BufferedGraphics gBufferPrevious;

        // TODO: try to remove the HasBufferAlternate property

        /// <summary>
        /// Returns true if an alternate buffer has been prepared but not yet displayed.
        /// </summary>
        public bool HasBufferAlternate { get { return (gBufferAlternate != null); } }

        /// <summary>
        /// These MazePainters paint to the same Graphics object.
        /// </summary>
        private List<MazePainter> sharedPainters = new List<MazePainter>();

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Create a MazePainter that paints into the given Graphics.
        /// </summary>
        /// <param name="client"></param>
        public MazePainter(Graphics graphics, Rectangle rectangle, IMazePainterClient client, bool screenSaverPreviewMode)
        {
            this.client = client;
            this.targetGraphics = graphics;
            this.targetRectangle = rectangle;
            this.screenSaverPreviewMode = screenSaverPreviewMode;
        }

        /// <summary>
        /// Replaces the parameters passed to the constructor with new values.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// TODO: Try to call the constructor with correct values in the first place.
        internal void Reconfigure(Graphics g, Rectangle rect)
        {
            this.client = null;
            this.targetGraphics = g;
            this.targetRectangle = rect;
            this.screenSaverPreviewMode = true;
        }

        /// <summary>
        /// Configure the maze dimension parameters.
        /// This method needs to be called before a maze is created.
        /// </summary>
        /// <param name="squareWidth"></param>
        /// <param name="wallWidth"></param>
        /// <param name="pathWidth"></param>
        public void Setup(int squareWidth, int wallWidth, int pathWidth)
        {
            this.squareWidth = squareWidth;
            this.wallWidth = wallWidth;
            this.gridWidth = squareWidth + wallWidth;
            this.pathWidth = pathWidth;

            AdjustPathWidth(squareWidth, wallWidth, ref pathWidth);
            ColorBuilder.SuggestColors(MinColor, MaxColor, out forwardColor, out backwardColor);
        }

        /// <summary>
        /// Derive reasonable parameter values from the given gridWidth.
        /// </summary>
        /// <param name="gridWidth"></param>
        /// <param name="visibleWalls"></param>
        /// <param name="squareWidth"></param>
        /// <param name="pathWidth"></param>
        /// <param name="wallWidth"></param>
        /// TODO: Move this code into a MazeDimension class.
        public static void SuggestWidths(int gridWidth, bool visibleWalls, out int squareWidth, out int pathWidth, out int wallWidth)
        {
            if (visibleWalls)
            {
                wallWidth = Math.Max(MinWallWidth, Math.Min(MaxWallWidth, (int)(0.3 * gridWidth)));
                squareWidth = gridWidth - wallWidth;
                pathWidth = (int)(0.7 * squareWidth);
            }
            else
            {
                wallWidth = 0;
                squareWidth = gridWidth - wallWidth;
                pathWidth = (int)(0.75 * squareWidth);
            }

            AdjustPathWidth(squareWidth, wallWidth, ref pathWidth);
        }

        /// <summary>
        /// Chooses values for the wallVisibility and gridWidth parameters.
        /// </summary>
        public void Setup()
        {
            Random r = RandomFactory.CreateRandom();

            if (randomizeWallVisibility)
            {
                switch (r.Next(3))
                {
                    case 0:
                        this.wallVisibility = AriadneSettingsData.WallVisibilityEnum.Always;
                        break;
                    case 1:
                        this.wallVisibility = AriadneSettingsData.WallVisibilityEnum.Never;
                        break;
                    case 2:
                        this.wallVisibility = AriadneSettingsData.WallVisibilityEnum.WhenVisited;
                        break;
                }
            }
            else
            {
                this.wallVisibility = AriadneSettingsData.WallVisibilityEnum.Always;
            }

            this.gridWidth = GetRandomGridWidth(r);

            if (screenSaverPreviewMode)
            {
                this.gridWidth /= 2;
            }
        }

        /// <summary>
        /// Return a random grid width between the constant minimum and maximum values.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private int GetRandomGridWidth(Random r)
        {
            int minWidth = (VisibleWalls ? MinAutoGridWidth : MinAutoGridWidthWithoutWalls);
            int maxWidth = (VisibleWalls ? MaxAutoGridWidth : MaxAutoGridWidthWithoutWalls);

            // Use a larger grid width for the first maze.
            if (this.wallWidth < 0)
            {
                minWidth = (minWidth + maxWidth) / 2;
            }

            int result = r.Next(minWidth, maxWidth);

            // Make sure we do not exceed the maximally allowed dimensions.
            MazeDimensions dim = MazeDimensions.Instance(MazeCode.DefaultCodeVersion);
            while (targetRectangle.Width / result > dim.MaxXSize || targetRectangle.Height / result > dim.MaxYSize)
            {
                ++result;
            }

            return result;
        }

        /// <summary>
        /// Paint a new maze into an alternate graphics buffer that will be used at the next repetition.
        /// </summary>
        public void PrepareAlternateBuffer(PainterDelegate painterDelegate)
        {
            gBufferAlternate = this.CreateGraphicsBuffer();
            Graphics g = gBufferAlternate.Graphics;
            PaintMaze(g, painterDelegate);
        }

        /// <summary>
        /// Make (squareWidth - pathWidth) an even number.
        /// That will make sure that the path is centered nicely between the walls.
        /// </summary>
        /// <returns></returns>
        private static void AdjustPathWidth(int squareWidth, int wallWidth, ref int pathWidth)
        {
            if (wallWidth > 0 && (squareWidth - pathWidth) % 2 != 0)
            {
                pathWidth -= 1;
            }
            if (pathWidth < 2)
            {
                pathWidth = squareWidth;
            }
        }

        /// <summary>
        /// Construct a maze that fits into the drawing area. 
        /// </summary>
        public void CreateMaze(ConfigureMazeLayoutDelegate configureDelegate)
        {
            UpdateClientDimensions();

            // Determine dimensions of a maze that fits into the drawing area.
            int xSize, ySize;
            FitMazeWidth(out xSize, out this.xOffset);
            FitMazeHeight(out ySize, out this.yOffset);

            // Create a maze object.
            this.maze = new Maze(xSize, ySize);

            // Configure the maze layout.
            if (configureDelegate != null)
            {
                configureDelegate(maze);
            }

            // Build the maze.
            Maze.CreateMaze();
        }

        /// <summary>
        /// Calculate width and xOffset.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="xOffset"></param>
        private void FitMazeWidth(out int width, out int xOffset)
        {
            FitMazeSize(this, targetRectangle.Width, out width, out xOffset);
        }

        /// <summary>
        /// Calculate height and yOffset.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="yOffset"></param>
        private void FitMazeHeight(out int height, out int yOffset)
        {
            FitMazeSize(this, targetRectangle.Height, out height, out yOffset);
        }

        /// <summary>
        /// Calculate a maze size (width or height) and its offset from the given displaySize.
        /// </summary>
        /// <param name="displaySize"></param>
        /// <param name="mazeSize"></param>
        /// <param name="offset"></param>
        private void FitMazeSize(MazePainter instance, int displaySize, out int mazeSize, out int offset)
        {
            int usableSize = displaySize - 2 * instance.padding;
            
            if (instance.wallWidth > 0)
            {
                usableSize -= instance.wallWidth;
            }
            else
            {
                // Add the unused space between the painted path and the not painted wall.
                // Note: The start or target square may be slightly cut off if it is directly on the border.
                usableSize += (instance.squareWidth - instance.pathWidth);
            }

            mazeSize = usableSize / instance.gridWidth;
            offset = (displaySize - mazeSize * instance.gridWidth) / 2;
        }

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public void Reset()
        {
            if (maze != null)
            {
                maze.Reset();
            }

            this.wallPen = new Pen(wallColor, wallWidth);
            this.forwardPen = new Pen(forwardColor, pathWidth);
            this.backwardPen = new Pen(backwardColor, pathWidth);
            this.deadEndPen = new Pen(backwardColor, pathWidth);
            this.startSquarePen = new Pen(Color.Red, squareWidth);
            this.blackSquarePen = new Pen(Color.Black, squareWidth);

            wallPen.StartCap = wallPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            forwardPen.StartCap = forwardPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            backwardPen.StartCap = backwardPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            deadEndPen.StartCap = deadEndPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            startSquarePen.StartCap = startSquarePen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
            blackSquarePen.StartCap = blackSquarePen.EndCap = System.Drawing.Drawing2D.LineCap.Square;

            // Destroy the current buffer; it will be re-created in the OnPaint() method.
            if (gBuffer != null)
            {
                gBuffer = null;
            }

            // Forward this message to the shared painters.
            foreach (MazePainter item in sharedPainters)
            {
                item.gBuffer = null;
                item.Reset();
            }
        }

        /// <summary>
        /// Update the client dimensions, if it was resized.
        /// </summary>
        private void UpdateClientDimensions()
        {
            if (client != null)
            {
                this.targetGraphics = client.CreateGraphics();
                this.targetRectangle = client.DisplayRectangle;
            }
        }

        #endregion

        #region Support for embedded mazes

        public MazePainter CreateSharedPainter(Maze embeddedMaze)
        {
            // The new MazePainter should not have a client.
            // The client related behavior is a task of the main MazePainter.
            MazePainter result = new MazePainter(this.targetGraphics, this.targetRectangle, null, false);

            // The maze should have the same layout.
            result.Setup(this.squareWidth, this.wallWidth, this.pathWidth);
            result.xOffset = this.xOffset;
            result.yOffset = this.yOffset;
            result.wallVisibility = this.WallVisibility;

            // The shared painter will also share all data related to the background image.
            result.backgroundImage = this.backgroundImage;
            result.backgroundImageRange = this.backgroundImageRange;
            result.backgroundImageShape = this.backgroundImageShape;
            result.backgroundPainted = this.backgroundPainted;

            // Switch path colors until they are sufficiently different.
            while (true)
            {
                float f0 = this.forwardColor.GetHue();
                float f1 = result.forwardColor.GetHue();
                float b0 = this.backwardColor.GetHue();
                float b1 = result.backwardColor.GetHue();

                if (true
                    && ColorBuilder.HueDifference(f0, f1) >= 0.10 * ColorBuilder.MaxHue     // both forward colors
                    && ColorBuilder.HueDifference(f0, b1) >= 0.10 * ColorBuilder.MaxHue     // forward and backward colors
                    && ColorBuilder.HueDifference(b0, f1) >= 0.10 * ColorBuilder.MaxHue     // forward and backward colors
                    && ColorBuilder.HueDifference(b0, b1) >= 0.07 * ColorBuilder.MaxHue     // both backward colors
                    )
                {
                    break;
                }
                else
                {
                    ColorBuilder.SuggestColors(MinColor, MaxColor, out result.forwardColor, out result.backwardColor);
                }
            }

            // Finish the initialization.
            result.Reset();

            // We don't create a new maze but use the given one.
            result.maze = embeddedMaze;

            // Add the shared painter to our list and let it share our BufferedGraphics.
            this.sharedPainters.Add(result);
            result.gBuffer = this.gBuffer;

            return result;
        }

        public void ClearSharedPainters()
        {
            sharedPainters.Clear();
        }

        #endregion

        #region Painting methods

        /// <summary>
        /// Renders the GraphicsBuffer to the output device.
        /// On first time, the buffer is created and the maze (without any path) is painted.
        /// </summary>
        /// <param name="painterDelegate"></param>
        /// 
        public void OnPaint(PainterDelegate painterDelegate)
        {
            // On first time, create a graphics buffer and draw the static maze.
            //
            if (gBuffer == null)
            {
                // Use the previously prepared alternate buffer, if possible.
                if (gBufferAlternate != null)
                {
                    // For a brief moment, display a black screen.
                    targetGraphics.FillRectangle(Brushes.Black, targetRectangle);
                    targetGraphics.Flush();
                    System.Threading.Thread.Sleep(120); // milliseconds

                    gBuffer = gBufferAlternate;
                    gBufferAlternate = null;

                    // Let all shared painters use the new graphics object.
                    foreach (MazePainter item in sharedPainters)
                    {
                        item.gBuffer = this.gBuffer;
                        item.gBufferAlternate = null;
                    }
                }
                else
                {
                    PaintMaze(painterDelegate);
                }
            }

            if (client == null || client.Alive)
            {
                gBuffer.Render();
            }
        }

        /// <summary>
        /// Creates the GraphicsBuffer and draws the static maze.
        /// </summary>
        public void PaintMaze(PainterDelegate painterDelegate)
        {
            //Log.WriteLine("{ PaintMaze()");
            gBuffer = CreateGraphicsBuffer();
            Graphics g = gBuffer.Graphics;
            g.FillRectangle(Brushes.Black, targetRectangle);
            PaintMaze(g, painterDelegate);

            // Let all shared painters use the new graphics object.
            foreach (MazePainter item in sharedPainters)
            {
                item.gBuffer = this.gBuffer;
                item.gBufferAlternate = null;
            }
            //Log.WriteLine("} PaintMaze()");
        }

        /// <summary>
        /// Creates a new GraphicsBuffer associated with the current graphics context.
        /// </summary>
        /// <returns></returns>
        private BufferedGraphics CreateGraphicsBuffer()
        {
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            return currentContext.Allocate(targetGraphics, targetRectangle);
        }

        /// <summary>
        /// Draws the static maze.
        /// </summary>
        private void PaintMaze(Graphics g, PainterDelegate painterDelegate)
        {
            // The PaintWalls() method fails in design mode.
            try
            {
                // If there is a background image, we need to paint the maze into it, as well.
                Graphics bg = null;

                if (backgroundImage != null)
                {
                    bg = Graphics.FromImage(backgroundImage);
                    this.backgroundImageSetupCompletion();
                }

                // Call the painterDelegate first
                // as it may also paint into areas that are later covered by the maze.
                if (painterDelegate != null)
                {
                    painterDelegate(g);
                    if (bg != null)
                    {
                        painterDelegate(bg);
                    }
                }

                if (settingsData != null && settingsData.VisibleOutlines)
                {
                    // Note: These shapes will not be drawn into the background image.
                    PaintOutlineShape(g);
                    PaintEmbeddedMazes(g);
                }

                // Paint the maze into the given Graphics and into the background image.
                switch (this.WallVisibility)
                {
                    default:
                    case AriadneSettingsData.WallVisibilityEnum.Always:

                        PaintBorder(g);
                        PaintWalls(g);

                        if (bg != null)
                        {
                            PaintBorder(bg);
                            PaintWalls(bg);
                        }

                        break;
                    
                    case AriadneSettingsData.WallVisibilityEnum.Never:

                        break;
                    
                    case AriadneSettingsData.WallVisibilityEnum.WhenVisited:

                        PaintWalls(g, maze.StartSquare);

                        if (bg != null)
                        {
                            PaintWalls(bg, maze.StartSquare);
                        }

                        break;
                }

                PaintEndpoints(g);
            }
            catch (MissingMethodException) { }
        }

        /// <summary>
        /// Paint the squares of the outline shape in a dark blue background color.
        /// </summary>
        /// <param name="g"></param>
        private void PaintOutlineShape(Graphics g)
        {
            if (maze.OutlineShape == null)
            {
                return;
            }

            OutlineShape.InsideShapeDelegate test = delegate(int x, int y) { return maze.OutlineShape[x, y]; };
            Color shapeColor = Color.FromArgb(0, 0, 50); // dark blue

            PaintShape(g, test, shapeColor);
        }

        /// <summary>
        /// Paint the squares of the embedded mazes in a dark red background color.
        /// </summary>
        /// <param name="g"></param>
        private void PaintEmbeddedMazes(Graphics g)
        {
            foreach (Maze item in maze.EmbeddedMazes)
            {
                OutlineShape.InsideShapeDelegate test = delegate(int x, int y) { return maze[x, y].MazeId == item.MazeId; };
                Color shapeColor = Color.FromArgb(50, 0, 0); // dark red

                PaintShape(g, test, shapeColor);
            }
        }

        /// <summary>
        /// Paint the shape as defined by the insideShapeTest delegate with the given color.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="insideShapeTest"></param>
        /// <param name="shapeColor"></param>
        private void PaintShape(Graphics g, OutlineShape.InsideShapeDelegate insideShapeTest, Color shapeColor)
        {
            Brush shapeBrush = new SolidBrush(shapeColor);
            Pen shapePen = new Pen(shapeBrush, gridWidth);
            shapePen.StartCap = shapePen.EndCap = System.Drawing.Drawing2D.LineCap.Square;

            for (int x = 0; x < maze.XSize; x++)
            {
                for (int y = 0; y < maze.YSize; y++)
                {
                    if (insideShapeTest(x, y) == true && ! Maze[x, y].isReserved)
                    {
                        this.PaintSquare(g, shapePen, x, y);
                    }
                }
            }
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

                if (sq[WallPosition.WP_S] == WallState.WS_CLOSED)
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

                if (sq[WallPosition.WP_E] == WallState.WS_CLOSED)
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
                    if (sq[WallPosition.WP_W] == WallState.WS_CLOSED)
                    {
                        g.DrawLine(wallPen, cx, cy, cx, cy + gridWidth);
                    }

                    // Draw the north wall.
                    if (sq[WallPosition.WP_N] == WallState.WS_CLOSED)
                    {
                        g.DrawLine(wallPen, cx, cy, cx + gridWidth, cy);
                    }
                }
            }
        }

        /// <summary>
        /// Paints the closed walls around a given square.
        /// This method is called for the visited squares when the walls are initially invisible.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sq"></param>
        private void PaintWalls(Graphics g, MazeSquare sq)
        {
            int cx = xOffset + sq.XPos * gridWidth;
            int cy = yOffset + sq.YPos * gridWidth;

            // Draw the west wall.
            if (sq[WallPosition.WP_W] == WallState.WS_CLOSED)
            {
                g.DrawLine(wallPen, cx, cy, cx, cy + gridWidth);
            }

            // Draw the north wall.
            if (sq[WallPosition.WP_N] == WallState.WS_CLOSED)
            {
                g.DrawLine(wallPen, cx, cy, cx + gridWidth, cy);
            }

            // Draw the east wall.
            if (sq[WallPosition.WP_E] == WallState.WS_CLOSED)
            {
                g.DrawLine(wallPen, cx + gridWidth, cy, cx + gridWidth, cy + gridWidth);
            }

            // Draw the south wall.
            if (sq[WallPosition.WP_S] == WallState.WS_CLOSED)
            {
                g.DrawLine(wallPen, cx, cy + gridWidth, cx + gridWidth, cy + gridWidth);
            }
        }

        /// <summary>
        /// Paints the start and end point.
        /// </summary>
        /// <param name="g"></param>
        private void PaintEndpoints(Graphics g)
        {
            // Activate the blinkingCounter, if necessary.
            if (BlinkingCounter < 0)
            {
                BlinkingCounter = 0;
            }

            PaintSquare(g, this.StartSquarePen, maze.StartSquare.XPos, maze.StartSquare.YPos);
            PaintSquare(g, this.EndSquarePen, maze.EndSquare.XPos, maze.EndSquare.YPos);
        }

        /// <summary>
        /// Fills one square using the given pen.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <remarks>
        /// The pen should have square caps and an appropriate width.
        /// </remarks>
        private void PaintSquare(Graphics g, Pen pen, int x, int y)
        {
            float cx = xOffset + gridWidth/2.0F + x * gridWidth;
            float cy = yOffset + gridWidth/2.0F + y * gridWidth;

            // Drawing from cx to cx has no result at all.
            // Drawing from cx to cx+0.01 paints one pixel too many.
            // Drawing from cx-0.01 to cx seems to do the right thing.
            g.DrawLine(pen, cx-0.01F, cy, cx, cy);
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

            Graphics g = gBuffer.Graphics;

            // Draw the background image in newly visited squares.
            if (backgroundImage != null)
            {
                // Maybe draw walls around the visited square.
                if (forward && this.WallVisibility == AriadneSettingsData.WallVisibilityEnum.WhenVisited)
                {
                    Graphics bg = Graphics.FromImage(backgroundImage);
                    this.PaintWalls(bg, sq2);
                }

                PaintBackgroundSquare(g, sq1);
                PaintBackgroundSquare(g, sq2);
            }

            // Draw a line from sq1 to sq2.
            Pen p = (forward ? this.forwardPen : this.backwardPen);
            g.DrawLine(p, cx1, cy1, cx2, cy2);

            // Maybe redraw the end point.
            if (sq1 == maze.StartSquare || sq2 == maze.StartSquare || sq1 == maze.EndSquare || sq2 == maze.EndSquare)
            {
                this.PaintEndpoints(g);
            }

            // Maybe draw walls around the visited square.
            if (forward && this.WallVisibility == AriadneSettingsData.WallVisibilityEnum.WhenVisited)
            {
                this.PaintWalls(g, sq2);
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

            // Render the buffered graphics to the display.
            if (client == null || client.Alive)
            {
                gBuffer.Render();
            }

            // Finally, update the display.
            if (client != null)
            {
                client.Update();
            }
        }

        /// <summary>
        /// Paints a dot in forward direction at the square.
        /// Covers up for drawing a backward path into a square on the forward path.
        /// </summary>
        /// <param name="sq"></param>
        private void PaintPathDot(MazeSquare sq)
        {
            PaintPathDot(sq, this.forwardPen);
        }

        /// <summary>
        /// Paints a dot with the given pen in the given square.
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="pen"></param>
        /// <remarks>
        /// Previous versions used to fill a rectangle but that proved to be
        /// incompatible with the lines drawn for the path and the maze walls.
        /// Sometimes the dots were one pixel off and left small strokes in the
        /// wrong color. 
        /// </remarks>
        private void PaintPathDot(MazeSquare sq, Pen pen)
        {
            Graphics g = gBuffer.Graphics;

            if (PaintBackgroundSquare(g, sq))
            {
                // Don't paint a dot if the square is part of the background image.
                return;
            }

            this.PaintSquare(g, pen, sq.XPos, sq.YPos);
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
            // Uncover all remaining background squares.
            DrawRemainingBackgroundSquares(maze.MazeId);

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
            PaintPathDot(sq, deadEndPen);
        }

        /// <summary>
        /// Highlights the squares that have not been visited.
        /// </summary>
        public void DrawRemainingSquares()
        {
            for (int x = 0; x < maze.XSize; x++)
            {
                for (int y = 0; y < maze.YSize; y++)
                {
                    if (maze[x, y].MazeId == maze.MazeId && !maze[x, y].isVisited)
                    {
                        PaintPathDot(maze[x, y], backwardPen);
                    }
                }
            }
        }

        #endregion

        #region Support for saving the painted image

        /// <summary>
        /// Tries to copy the image from the BufferedGraphics's private Bitmap field.
        /// This works OK with the Mono version of BufferedGraphic but a
        /// Microsoft BufferedGraphics object has no such member.
        /// </summary>
        /// <returns><c>true</c>, if an image was drawn, <c>false</c> otherwise.</returns>
        /// <param name="targetBitmap"></param>
        /// <param name="sourceUpperLeft"></param>
        /// <param name="targetUpperLeft"></param>
        /// <param name="size"></param>
        public bool DrawImage (Bitmap targetBitmap, Point sourceUpperLeft, Point targetUpperLeft, Size size)
        {
            BufferedGraphics b = gBuffer;
            if (b == null) { b = gBufferPrevious; }
            if (b == null) { return false; }

            Bitmap sourceBitmap = Reflection.GetPrivateField<BufferedGraphics, Bitmap>(b, "membmp");
            if (sourceBitmap == null)
            {
                Log.WriteLine("DrawImage() failed -- cannot access private field BufferedGraphics.membmp", true);
                return false;
            }
            Graphics g = Graphics.FromImage(targetBitmap);
            g.DrawImage(sourceBitmap, new Rectangle(targetUpperLeft, size), new Rectangle(sourceUpperLeft, size), GraphicsUnit.Pixel);
            return true;
        }

        #endregion

        #region Background image related methods

        /// <summary>
        /// Tries to create a background image that will be used in the current iteration.
        /// Returns true if an image was created.
        /// </summary>
        /// <returns></returns>
        internal bool PrepareBackgroundImage()
        {
            // We need a Graphics object defining the resolution of the image to be created.
            Graphics g = targetGraphics;
            return this.CreateBackgroundImage(g);
        }

        /// <summary>
        /// Creates a backgroundImage (and associated data structures) if there is a backgroundImageLoader.
        /// The backgroundImage has the same size as the targetRectangle.
        /// Returns true if an image was created.
        /// </summary>
        /// <returns></returns>
        private bool CreateBackgroundImage(Graphics g)
        {
            this.backgroundContourImage = null;

            if (this.backgroundImageLoader != null)
            {
                backgroundContourImage = backgroundImageLoader.GetNext(maze.Random);
            }

            if (backgroundContourImage != null)
            {
                // Create a new Bitmap with the same resolution as the buffered graphics.
                this.backgroundImage = new Bitmap(targetRectangle.Width, targetRectangle.Height, g);
                Image img = backgroundContourImage.DisplayedImage;

                #region Choose an (x, y) location of the image.

                int w = backgroundImage.Width - img.Width;
                int h = backgroundImage.Height - img.Height;
                int xMin = Math.Min(targetRectangle.Width / 16, w / 2);
                int yMin = Math.Min(targetRectangle.Height / 16, h / 2);
                w -= 2 * xMin;
                h -= 2 * yMin;
                int x = xMin + (w <= 0 ? 0 : maze.Random.Next(w));
                int y = yMin + (h <= 0 ? 0 : maze.Random.Next(h));

                #endregion

                // The remainder of the background image setup can only be executed
                // after the next maze's geometry has been chosen.
                this.backgroundImageSetupCompletion = delegate()
                {
                    #region Determine the maze area that is completely covered by the image.

                    int marginWidth = squareWidth + (wallWidth + 1) / 2;
                    int xSqMin = XCoordinate(x + marginWidth, true);
                    int xSqMax = XCoordinate(x + img.Width - 1 - marginWidth, false);
                    int ySqMin = YCoordinate(y + marginWidth, true);
                    int ySqMax = YCoordinate(y + img.Height - 1 - marginWidth, false);
                    this.backgroundImageRange = new Rectangle(xSqMin, ySqMin, (xSqMax - xSqMin + 1), (ySqMax - ySqMin + 1));

                    int xOffsetImg = x - xSqMin * gridWidth;
                    int yOffsetImg = y - ySqMin * gridWidth;
                    this.backgroundImageShape = backgroundContourImage.GetCoveredShape(gridWidth, wallWidth, xOffsetImg, yOffsetImg);

                    #endregion

                    this.backgroundPainted = new bool[maze.XSize, maze.YSize];
                };

                Graphics bg = Graphics.FromImage(backgroundImage);
                bg.DrawImage(img, new Rectangle(x, y, img.Width, img.Height));
            }
            else
            {
                this.backgroundImage = null;
                this.backgroundImageRange = new Rectangle();
                this.backgroundImageShape = null;
                this.backgroundPainted = null;
            }

            return (backgroundImage != null);
        }

        /// <summary>
        /// Returns the file path of the current background image
        /// or null if no background image is displayed.
        /// </summary>
        /// <returns></returns>
        internal string GetBackgroundImagePath()
        {
            if (this.backgroundContourImage != null)
            {
                return this.backgroundContourImage.Path;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Draws the background image in all squares having the given mazeId.
        /// Squares that have already been drawn will be skipped.
        /// </summary>
        /// <param name="mazeId"></param>
        public void DrawRemainingBackgroundSquares(int mazeId)
        {
            Graphics g = gBuffer.Graphics;

            for (int x = backgroundImageRange.Left; x < backgroundImageRange.Right; x++)
            {
                if (x < 0 || x >= maze.XSize) { continue; }

                for (int y = backgroundImageRange.Top; y < backgroundImageRange.Bottom; y++)
                {
                    if (y < 0 || y >= maze.YSize) { continue; }

                    MazeSquare sq = maze[x, y];
                    if (sq.MazeId == mazeId)
                    {
                        PaintBackgroundSquare(g, sq);
                    }
                }
            }
        }

        /// <summary>
        /// Paints a portion of the background image.
        /// Returns true if the square is part of the background image.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sq"></param>
        /// <returns></returns>
        private bool PaintBackgroundSquare(Graphics g, MazeSquare sq)
        {
            int xSq = sq.XPos, ySq = sq.YPos;

            if (backgroundImage == null)
            {
                return false;
            }
            if (backgroundPainted[xSq, ySq])
            {
                return true;
            }
            if (xSq < backgroundImageRange.Left || xSq >= backgroundImageRange.Right || ySq < backgroundImageRange.Top || ySq >= backgroundImageRange.Bottom)
            {
                return false;
            }
            if (backgroundImageShape != null && !backgroundImageShape[xSq - backgroundImageRange.Left, ySq - backgroundImageRange.Top])
            {
                return false;
            }

            int x = xOffset + xSq * gridWidth;
            int y = yOffset + ySq * gridWidth;
            int w = gridWidth;

            g.DrawImage(backgroundImage, x, y, new Rectangle(x, y, w, w), GraphicsUnit.Pixel);
            backgroundPainted[xSq, ySq] = true;

            return true;
        }

        #endregion

        #region Coordinate system

        /// <summary>
        /// Returns the maze X coordinate corresponding to the given canvas coordinate.
        /// </summary>
        /// <param name="xCanvas"></param>
        /// <param name="leftBiased"></param>
        /// <returns></returns>
        internal int XCoordinate(int xCanvas, bool leftBiased)
        {
            int result = xCanvas - xOffset;
            result += (leftBiased ? -1 : +1) * wallWidth;
            result /= gridWidth;

            return result;
        }

        /// <summary>
        /// Returns the maze Y coordinate corresponding to the given canvas coordinate.
        /// </summary>
        /// <param name="yCanvas"></param>
        /// <param name="topBiased"></param>
        /// <returns></returns>
        internal int YCoordinate(int yCanvas, bool topBiased)
        {
            int result = yCanvas - yOffset;
            result += (topBiased ? -1 : +1) * wallWidth;
            result /= gridWidth;

            return result;
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

            UpdateClientDimensions();

            // The currently installed shared painters are no longer valid.
            // New shared painters may be installed by the caller.
            this.sharedPainters.Clear();

            #region Take parameters concerning this MazePainter.

            #region Layout

            if (!data.AutoGridWidth)
            {
                this.gridWidth = Math.Max(2, Math.Min(MaxGridWidth, data.GridWidth));

                SuggestWidths(gridWidth, VisibleWalls, out squareWidth, out pathWidth, out wallWidth);
            }
            else if (!data.AutoSquareWidth || !data.AutoPathWidth || !data.AutoWallWidth)
            {
                this.wallWidth = (VisibleWalls ? Math.Max(MinWallWidth, Math.Min(MaxWallWidth, data.WallWidth)) : 0);
                this.squareWidth = Math.Max(MinSquareWidth, Math.Min(MaxGridWidth - wallWidth, data.SquareWidth));
                this.pathWidth = Math.Max(MinPathWidth, Math.Min(squareWidth, data.PathWidth));

                this.gridWidth = squareWidth + wallWidth;
            }
            else
            {
                this.gridWidth = GetRandomGridWidth(maze.Random);
                SuggestWidths(gridWidth, VisibleWalls, out squareWidth, out pathWidth, out wallWidth);
            }

            AdjustPathWidth(squareWidth, wallWidth, ref pathWidth);

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

            #region Let the maze take its parameters.

            // Make sure that we have a Maze object.
            if (maze == null)
            {
                maze = new Maze(data.MazeWidth, data.MazeHeight);
            }

            UpdateClientDimensions();

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

            #endregion

            #region Images

            if (!data.LeaveCurrentBackgroundImageLoader)
            {
                if (data.ShowBackgroundImage)
                {
                    if (this.backgroundImageLoader == null)
                    {
                        string imageFolder = data.ImageFolder;
                        CreateBackgroundImageLoader(imageFolder, 0);
                    }
                }
                else
                {
                    this.backgroundImageLoader = null;
                }
            }

            // If a backgroundImageLoader exists, a backgroundImage will be created later
            // in the PaintMaze() method.
            this.backgroundImage = null;

            #endregion
        }

        /// <summary>
        /// Creates an ImageLoader for background images.
        /// </summary>
        /// <param name="imageFolder"></param>
        /// <param name="percentage">when positive: percentage of mazes that will have a background image</param>
        public void CreateBackgroundImageLoader(string imageFolder, int percentage)
        {
            UpdateClientDimensions();

            int minSize = Math.Min(targetRectangle.Width, targetRectangle.Height) * 3 / 4;
            int maxSize = Math.Min(targetRectangle.Width, targetRectangle.Height) * 7 / 8;
            this.backgroundImageLoader = new ImageLoader(minSize, maxSize, true, imageFolder, 1, "BGIL");

            if (0 < percentage && percentage < 100)
            {
                BackgroundImageLoader.YieldNullPercentage = 100 - percentage;
            }
        }

        #endregion

        #region IImageLoader Members

        /// <summary>
        /// In the context of an ArenaForm, one MazePainter may be used as a source of background images
        /// for other MazePainters.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public ContourImage GetNext(Random r)
        {
            return this.backgroundContourImage;
        }

        public void Shutdown()
        {
            // do nothing
        }

        public int YieldNullPercentage
        {
            set { /* do nothing */ }
        }

        #endregion
    }
}
