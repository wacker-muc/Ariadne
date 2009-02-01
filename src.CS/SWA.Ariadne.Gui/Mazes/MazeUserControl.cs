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
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Mazes
{
    public partial class MazeUserControl : UserControl
        , IMazeControl
        , IMazePainterClient
    {
        #region Member variables

        /// <summary>
        /// The MazePainter responsible for all painting activities.
        /// </summary>
        public MazePainter MazePainter
        {
            get { return this.painter; }
        }
        private MazePainter painter;

        /// <summary>
        /// The Maze that is displayed in this control.
        /// </summary>
        public Maze Maze
        {
            get { return painter.Maze; }
        }

        private AriadneSettingsData settingsData;

        public bool RandomizeWallVisibility
        {
            set { painter.RandomizeWallVisibility = value; }
        }

        // These properties provide a simplified access to MazePainter properties.
        private int wallWidth { get { return painter.WallWidth; } }
        private int gridWidth { get { return painter.GridWidth; } }
        private int xOffset { get { return painter.XOffset; } }
        private int yOffset { get { return painter.YOffset; } }

        /// <summary>
        /// A source of images to be displayed.
        /// </summary>
        public ImageLoader ImageLoader
        {
            get { return imageLoader; }
            set { imageLoader = value; }
        }
        private ImageLoader imageLoader;

        /// <summary>
        /// A list of (scaled) images that will be painted in reserved areas of the maze.
        /// </summary>
        private List<ContourImage> images = new List<ContourImage>();

        /// <summary>
        /// A list of locations (in graphics coordinates) where the images will be painted.
        /// Note: Specifying a Point is not sufficient if the image's resolution differs from the graphics resolution.
        /// </summary>
        private List<Rectangle> imageLocations = new List<Rectangle>();

        /// <summary>
        /// A list of locations (in graphics coordinates) where other controls will be painted.
        /// </summary>
        private List<Rectangle> controlLocations = new List<Rectangle>();

        /// <summary>
        /// Returns true when the list of prepared images is not empty.
        /// </summary>
        public bool HasPreparedImages
        {
            get { return hasPreparedImages; }
        }
        private bool hasPreparedImages = false;

        /// <summary>
        /// When false, do not update the caption or status bar.
        /// </summary>
        private bool allowUpdates = true;

        public IMazeForm MazeForm
        {
            get { return this.mazeForm; }
            set { this.mazeForm = value; }
        }
        IMazeForm mazeForm;

        #region Saved properties of the previous maze

        // When the current maze is solved and an alternate buffer has been prepared,
        // we need to remember the former maze properties for saving the currently displayed screen.

        /// <summary>
        /// If an alternate buffer exists:
        /// Holds the previous maze's Code property.
        /// </summary>
        private string solvedScreenshotMazeCode;
        
        /// <summary>
        /// If an alternate buffer exists:
        /// Holds the previous maze's XSize and YSize properties.
        /// </summary>

        private Size solvedScreenshotMazeDimensions;
        /// <summary>
        /// If an alternate buffer exists:
        /// X = xOffset,
        /// Y = yOffset,
        /// Width = gridWidth,
        /// Height = wallWidth
        /// </summary>
        private Rectangle solvedScreenshotControlDimensions;

        #endregion

        #endregion

        #region Constructor and Initialization

        public MazeUserControl()
        {
            InitializeComponent();
            this.painter = new MazePainter(this.CreateGraphics(), this.DisplayRectangle, this as IMazePainterClient, false);
        }

        /// <summary>
        /// Set the graphics context in screen saver preview mode.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public void SetGraphics(Graphics g, Rectangle rect)
        {
            this.painter.Reconfigure(g, rect);
        }

        // TODO: Make this method private, then unite painter.Setup() and painter.Setup(s,w,p).
        public void Setup(int squareWidth, int wallWidth, int pathWidth)
        {
            painter.Setup(squareWidth, wallWidth, pathWidth);
            CreateMaze();
            Reset();
        }

        public void Setup()
        {
            if (painter.HasBufferAlternate)
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

            #region Set up the painter.

            painter.Setup();

            // TODO: Move the remaining code to painter.Setup()

            int wallWidth;
            int squareWidth;
            int pathWidth;
            MazePainter.SuggestWidths(painter.GridWidth, painter.VisibleWalls, out squareWidth, out pathWidth, out wallWidth);

            this.Setup(squareWidth, wallWidth, pathWidth);

            #endregion
        }

        /// <summary>
        /// Paint a new maze into an alternate graphics buffer that will be used at the next repetition.
        /// </summary>
        public void PrepareAlternateBuffer()
        {
            // An alternate buffer must only be prepared when the previous maze is solved.
            if (Maze != null && Maze.IsFinished != true)
            {
                return;
            }

            // The alternate buffer method doesn't work properly in the screen saver preview mode.
            if (painter.screenSaverPreviewMode == true)
            {
                return;
            }

            // Prevent updates of the caption or status line caused by the prepared alternate maze.
            this.allowUpdates = false;

            #region Remember current maze attributes, if a screen shot should be requested.

            this.solvedScreenshotMazeCode = Code;
            this.solvedScreenshotMazeDimensions = new Size(XSize, YSize);
            this.solvedScreenshotControlDimensions = new Rectangle(xOffset, yOffset, gridWidth, wallWidth);

            #endregion

            this.Setup();
            painter.PrepareAlternateBuffer(this.PaintImages);
            
            this.allowUpdates = true;
        }

        /// <summary>
        /// Construct a maze that fits into the drawing area.
        /// </summary>
        private void CreateMaze()
        {
            this.imageLocations.Clear();
            this.controlLocations.Clear();

            if (this.MazeForm == null)
            {
                painter.CreateMaze(null);
            }
            else
            {
                painter.CreateMaze(this.MazeForm.MakeReservedAreas);

                // Note: In the designer, the MazeForm property is not valid.
                if (allowUpdates)
                {
                    this.MazeForm.UpdateStatusLine();
                    this.MazeForm.UpdateCaption();
                }
            }
        }

        /// <summary>
        /// Reserves a region of the maze covered by the coveringControl.
        /// This MazeUserControl and the coveringControl must have the same Parent, i.e. a common coordinate system.
        /// </summary>
        /// <param name="coveringControl"></param>
        /// <exception cref="ArgumentException">The given Control has a differnent Parent.</exception>
        public bool ReserveArea(Control coveringControl)
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

            if (0 < x && x + w < Maze.XSize)
            {
                w = 1 + (coveringControl.Width + wallWidth + 6) / gridWidth;
            }
            if (0 < y && y + h < Maze.YSize)
            {
                h = 1 + (coveringControl.Height + wallWidth + 6) / gridWidth;
            }


            bool result = Maze.ReserveRectangle(x, y, w, h, null);

            // Move the control into the center of the reserved area.
            if (result)
            {
                int cx = coveringControl.Left;
                int cy = coveringControl.Top;

                if (0 < x && x + w < Maze.XSize)
                {
                    cx = this.Location.X + xOffset + x * gridWidth;
                    cx += 1 + (w * gridWidth - wallWidth - coveringControl.Width) / 2;
                }
                if (0 < y && y + h < Maze.YSize)
                {
                    cy = this.Location.Y + yOffset + y * gridWidth;
                    cy += 1 + (h * gridWidth - wallWidth - coveringControl.Height) / 2;
                }
                
                // Adjust the control's location
                coveringControl.Location = new Point(cx, cy);
                controlLocations.Add(coveringControl.Bounds);
            }

            return result;
        }

        /// <summary>
        /// Returns the maze X coordinate corresponding to the given control coordinate.
        /// </summary>
        /// <param name="xLocation"></param>
        /// <param name="leftBiased"></param>
        /// <returns></returns>
        private int XCoordinate(int xLocation, bool leftBiased)
        {
            return painter.XCoordinate(xLocation - this.Location.X, leftBiased);
        }

        /// <summary>
        /// Returns the maze Y coordinate corresponding to the given control coordinate.
        /// </summary>
        /// <param name="yLocation"></param>
        /// <param name="leftBiased"></param>
        /// <returns></returns>
        private int YCoordinate(int yLocation, bool topBiased)
        {
            return painter.XCoordinate(yLocation - this.Location.Y, topBiased);
        }

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// </summary>
        public void Reset()
        {
            //this.BackColor = Color.Black;
            painter.Reset();

            painter.BlinkingCounter = 0;

            if (allowUpdates)
            {
                this.Invalidate();
            }

            // If the window is minimized, there will be no OnPaint() event.
            // Therefore we paint the maze directly.
            if (this.ParentForm.WindowState == FormWindowState.Minimized)
            {
                // TODO: Reset() is called twice but should be called only once.
                painter.PaintMaze(this.PaintImages);
            }
        }

        #endregion

        #region Painting methods

        /// <summary>
        /// Paints the contents of this control by rendering the GraphicsBuffer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.wallWidth < 0)
            {
                this.Setup(12, 3, 8);
            }

            painter.OnPaint(this.PaintImages);

            // If a new maze has been painted, the caption needs to be updated.
            if (MazeForm != null)
            {
                MazeForm.UpdateCaption();
            }
        }

        /// <summary>
        /// Paints the images into their reserved areas.
        /// </summary>
        /// <param name="g"></param>
        private void PaintImages(Graphics g)
        {
            Region clip = g.Clip;

            for (int i = 0; i < images.Count; i++)
            {
                ContourImage img = images[i];

                if (img.HasContour)
                {
                    // Set a clipping region around the image.
                    Region r = img.BorderRegion.Clone();
                    r.Translate(imageLocations[i].X, imageLocations[i].Y);
                    g.Clip = r;
                }
                else
                {
                    // Paint a black border around the image, covering any background images.
                    Rectangle border = imageLocations[i];
                    border.Inflate(2, 2);
                    g.Clip = clip;
                    g.FillRectangle(Brushes.Black, border);
                }

                g.DrawImage(img.DisplayedImage, imageLocations[i]);
            }

            // Restore previous clip region (infinite).
            g.Clip = clip;

            for (int i = 0; i < controlLocations.Count; i++)
            {
                // Paint a black border around the control, covering any background images.
                Rectangle border = controlLocations[i];
                border.Inflate(2, 2);
                g.FillRectangle(Brushes.Black, border);
            }
        }

        #endregion

        #region Support for saving image files

        /// <summary>
        /// Returns a Bitmap image of the maze.
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            // The relevant dimensions should represent what is currently displayed,
            // not what has been prepared for the next run.
            //
            int xOffset, yOffset, gridWidth, wallWidth;
            if (this.painter.HasBufferAlternate)
            {
                xOffset = solvedScreenshotControlDimensions.X;
                yOffset = solvedScreenshotControlDimensions.Y;
                gridWidth = solvedScreenshotControlDimensions.Width;
                wallWidth = solvedScreenshotControlDimensions.Height;
            }
            else
            {
                xOffset = this.xOffset;
                yOffset = this.yOffset;
                gridWidth = this.gridWidth;
                wallWidth = this.wallWidth;
            }

            int margin = 4;
            margin = Math.Max(margin, wallWidth + 2);
            margin = Math.Max(margin, gridWidth / 2);
            margin = Math.Min(margin, 12);

            // Determine the dimensions of the image.
            Point clientUpperLeft = new Point(xOffset - wallWidth / 2 - 1, yOffset - wallWidth / 2 - 1);
            Size clientSize = new Size(XSize * gridWidth + wallWidth + 2, YSize * gridWidth + wallWidth + 2);
            Size imgSize = new Size(clientSize.Width + 2 * (margin - 1), clientSize.Height + 2 * (margin - 1));

            // In full screen mode, copy exactly the whole screen.
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            if (this.Width == screenSize.Width && this.Height == screenSize.Height)
            {
                clientUpperLeft.X = clientUpperLeft.Y = margin = 0;
                imgSize = clientSize = screenSize;
            }

            // Create a fitting Bitmap image.
            System.Drawing.Imaging.PixelFormat pxlFormat = System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
            Bitmap result = new Bitmap(imgSize.Width, imgSize.Height, pxlFormat);

            // Make sure the end square is painted solid.
            painter.BlinkingCounter = 0;

            // Grab the painted maze from the screen.
            // Note: I've found no way to get access to the contents of the BufferedGraphics.
            Graphics g = Graphics.FromImage(result);
            g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), imgSize));
            g.CopyFromScreen(PointToScreen(clientUpperLeft), new Point(margin, margin), clientSize);

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
            painter.FillParametersInto(data);
        }

        /// <summary>
        /// Take all modifyable parameters from the given data object.
        /// </summary>
        /// <param name="data"></param>
        public void TakeParametersFrom(AriadneSettingsData data)
        {
            this.settingsData = data;

            painter.TakeParametersFrom(data);

            // Destroy the current image loader (in case the parameters have changed).
            this.imageLoader = null;

            #region Do the equivalent of Setup() with the modified parameters.

            // CreateMaze()
            MazeForm.MakeReservedAreas(Maze);
            this.ReserveAreasForImages(data);
            this.AddOutlineShape(data);
            Maze.CreateMaze();
            Reset();

            #endregion

            mazeForm.UpdateStatusLine();
            mazeForm.UpdateCaption();
        }

        #endregion

        #region IMazePainterClient implementation

        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool Alive
        {
            get { return true; }
        }

        #endregion

        #region Placement of images

        /// <summary>
        /// Reserves areas for the number of images given in the AriadneSettingsData.
        /// </summary>
        /// <param name="data"></param>
        private void ReserveAreasForImages(AriadneSettingsData data)
        {
            int count = data.ImageNumber;
            int minSize = data.ImageMinSize;
            int maxSize = data.ImageMaxSize;
            string imageFolder = data.ImageFolder;

            ReserveAreaForImages(count, minSize, maxSize, imageFolder);
        }

        /// <summary>
        /// Prepares the given number of images and reserves an area for each of them.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <param name="imageFolder"></param>
        private void ReserveAreaForImages(int count, int minSize, int maxSize, string imageFolder)
        {
            if (this.imageLoader == null)
            {
                this.imageLoader = new ImageLoader(minSize, maxSize, false, imageFolder, 0, null);
            }

            PrepareImages(count);
            ReserveAreaForImages();
        }

        /// <summary>
        /// Puts the given number of images into the this.images list.
        /// </summary>
        /// <param name="count"></param>
        public void PrepareImages(int count)
        {
            //Log.WriteLine("{ PrepareImages()");
            images.Clear();
            imageLocations.Clear();

            bool haveBackgroundImage = this.MazePainter.PrepareBackgroundImage();

            #region Determine number of images to be placed into reserved areas.

            Random r = Maze.Random;
            int n, nMin, nMax = count;

            if (haveBackgroundImage && r.Next(100) < 25)
            {
                nMax = 0;
            }

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
                n = 0;
            }

            #endregion

            for (int i = 0; i < n; i++)
            {
                ContourImage img = imageLoader.GetNext(r);
                if (img != null)
                {
                    images.Add(img);
                }
            }

            this.hasPreparedImages = true;
            //Log.WriteLine("} PrepareImages()");
        }

        /// <summary>
        /// Reserves an area for each of the prepared images (as found in this.images).
        /// If no free area is found for an image, it is discarded.
        /// </summary>
        public void ReserveAreaForImages()
        {
            if (this.HasPreparedImages)
            {
                for (int i = 0; i < images.Count; i++ )
                {
                    ContourImage img = images[i];
                    if (!AddImage(img))
                    {
                        images.RemoveAt(i);
                        i--;
                    }
                }
            }

            this.hasPreparedImages = false;
        }

        /// <summary>
        /// Reserves a rectangular area for the given image in this.Maze.
        /// The chosen location is remembered in this.imageLocations.
        /// Returns true if the reservation was successful.
        /// </summary>
        /// <param name="contourImage"></param>
        /// <returns></returns>
        private bool AddImage(ContourImage contourImage)
        {
            Image img = contourImage.DisplayedImage;
            int sqW = (img.Width + 8 + this.wallWidth) / this.gridWidth + 1;
            int sqH = (img.Height + 8 + this.wallWidth) / this.gridWidth + 1;

            int xOffsetImg = (sqW * gridWidth - img.Width) / 2;
            int yOffsetImg = (sqH * gridWidth - img.Height) / 2;

            OutlineShape shape = (ContourImage.DisplayProcessedImage ? contourImage.GetCoveredShape(gridWidth, wallWidth, xOffsetImg, yOffsetImg) : null);

            Rectangle rect;
            if (Maze.ReserveRectangle(sqW, sqH, 2, shape, out rect))
            {
                // Remember the image data and location.  It will be painted in PaintMaze().
                int x = rect.X * gridWidth + xOffset + xOffsetImg;
                int y = rect.Y * gridWidth + yOffset + yOffsetImg;
                imageLocations.Add(new Rectangle(x, y, img.Width, img.Height));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of (foreground and background) images displayed in the maze control.
        /// </summary>
        public int ImageCount
        {
            get
            {
                int result = this.images.Count;

                if (painter.GetBackgroundImagePath() != null)
                {
                    result++;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Returns the file path of one of the displayed images.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetImagePath(int i)
        {
            if (i < this.images.Count)
            {
                return this.images[i].Path;
            }
            else if (i == this.images.Count && painter.GetBackgroundImagePath() != null)
            {
                return painter.GetBackgroundImagePath();
            }
            else
            {
                return "(no image)";
            }
        }

        #endregion

        #region Placement of outline shapes

        /// <summary>
        /// Returns the shape of a ContourImage or null if none of the images has a real contour.
        /// </summary>
        /// <returns></returns>
        public OutlineShape SuggestOutlineShape(Random r, double offCenter, double size)
        {
            foreach (ContourImage img in this.images)
            {
                if (img.HasContour)
                {
                    // Use img.GetCoveredShape() as an OutlineShapeBuilder.
                    return OutlineShape.RandomInstance(r, img.GetCoveredShape, this.XSize, this.YSize, offCenter, size);
                }
            }

            return null;
        }

        private void AddOutlineShape(AriadneSettingsData data)
        {
            Random r = Maze.Random;

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
                case AriadneSettingsData.OutlineKindEnum.Tiles:
                    shapeBuilderDelegate = OutlineShape.Tiles;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Rectangles:
                    shapeBuilderDelegate = OutlineShape.Rectangles;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Grid:
                    shapeBuilderDelegate = OutlineShape.Grid;
                    break;
                case AriadneSettingsData.OutlineKindEnum.GridElement:
                    shapeBuilderDelegate = OutlineShape.GridElement;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Maze:
                    shapeBuilderDelegate = OutlineShape.Maze;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Lines:
                    shapeBuilderDelegate = OutlineShape.Lines;
                    break;
                case AriadneSettingsData.OutlineKindEnum.Circles:
                    shapeBuilderDelegate = OutlineShape.Circles;
                    break;
            }
            if (shapeBuilderDelegate != null)
            {
                OutlineShape shape = OutlineShape.RandomInstance(r, shapeBuilderDelegate, XSize, YSize, offCenter, size);
                
                if (data.DistortedOutlines)
                {
                    shape = shape.DistortedCopy(r);
                }

                if (data.AsEmbeddedMaze)
                {
                    Maze.AddEmbeddedMaze(shape);
                }
                else
                {
                    Maze.OutlineShape = shape;
                }
            }
        }

        #endregion

        #region IMazeControl implementation

        /// <summary>
        /// Gets the XSize of the currently displayed Maze.
        /// </summary>
        public int XSize
        {
            get
            {
                return (Maze == null ? -1 :
                    painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeDimensions.Width :
                    Maze.XSize);
            }
        }

        /// <summary>
        /// Gets the YSize of the currently displayed Maze.
        /// </summary>
        public int YSize
        {
            get
            {
                return (Maze == null ? -1 :
                    painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeDimensions.Height :
                    Maze.YSize);
            }
        }

        /// <summary>
        /// Gets the Code of the currently displayed Maze.
        /// </summary>
        public string Code
        {
            get
            {
                return (Maze == null ? "---" :
                    painter != null && painter.HasBufferAlternate ? solvedScreenshotMazeCode :
                    Maze.Code);
            }
        }

        #endregion
    }
}
