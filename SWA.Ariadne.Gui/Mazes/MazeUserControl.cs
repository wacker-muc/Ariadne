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

#if false
        /// <summary>
        /// Besides the main maze, there may be other embedded or included mazes.
        /// Each has its own MazePainter.
        /// </summary>
        private List<MazePainter> painters = new List<MazePainter>();
#endif

        private AriadneSettingsData settingsData;

        public bool RandomizeWallVisibility
        {
            set { painter.RandomizeWallVisibility = value; }
        }

        // TODO: evaluate these properties inline
        private int wallWidth { get { return painter.WallWidth; } }
        private int gridWidth { get { return painter.GridWidth; } }
        private int xOffset { get { return painter.XOffset; } }
        private int yOffset { get { return painter.YOffset; } }

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
        /// When false, do not update the caption or status bar.
        /// </summary>
        private bool allowUpdates = true;

        public IMazeForm MazeForm
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
            this.painter = new MazePainter(this.CreateGraphics(), this.DisplayRectangle, this as IMazePainterClient);
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

            painter.Setup();

            int wallWidth;
            int squareWidth;
            int pathWidth;
            MazePainter.SuggestWidths(painter.GridWidth, painter.VisibleWalls, out squareWidth, out pathWidth, out wallWidth);

            this.Setup(squareWidth, wallWidth, pathWidth);
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

            // TODO: see if this variable is still required
            this.allowUpdates = false;
            
            this.Setup();

            painter.PrepareAlternateBuffer(this.PaintImages);
            
            this.allowUpdates = true;
        }

        /// <summary>
        /// Construct a maze that fits into the drawing area.
        /// </summary>
        private void CreateMaze()
        {
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


            bool result = Maze.ReserveRectangle(x, y, w, h);

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
            }

            return result;
        }

        // TODO: remove remaining methods

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
            //this.BackColor = Color.Black;
            painter.Reset();

            painter.BlinkingCounter = 0;

            if (allowUpdates)
            {
                this.Invalidate();
            }

            // If the window is minimized, there will be no OnPaint() event.
            // Therefore we Paint the maze directly.
            if (this.ParentForm.WindowState == FormWindowState.Minimized)
            {
                // TODO: Reset() is called twice but should be called only once.
                painter.PaintMaze(this.PaintImages);
            }
        }

        #endregion

        #region Painting methods

        // TODO: remove remaining methods

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

            painter.OnPaint(this.PaintImages, true);

            // If a new maze has been painted, the caption needs to be updated.
            if (MazeForm != null)
            {
                MazeForm.UpdateCaption();
            }
        }

#if true
        /// <summary>
        /// Creates the GraphicsBuffer and draws the static maze.
        /// </summary>
        /// TODO: remove this method
        public void PaintMaze()
        {
            painter.PaintMaze(this.PaintImages);
        }
#endif

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

        #endregion

        #region Support for saving image files

        /// <summary>
        /// Returns a Bitmap image of the maze.
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            // TODO: use painter's dimensions

            int margin = 4;
            margin = Math.Max(margin, wallWidth + 2);
            margin = Math.Max(margin, gridWidth / 2);
            margin = Math.Min(margin, 12);

            // Determine the dimensions of the image.
            Point clientUpperLeft = new Point(xOffset - wallWidth / 2 - 1, yOffset - wallWidth / 2 - 1);
            Size clientSize = new Size(XSize * gridWidth + wallWidth + 2, YSize * gridWidth + wallWidth + 2);
            Size imgSize = new Size(clientSize.Width + 2 * (margin - 1), clientSize.Height + 2 * (margin - 1));

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
            images.Clear();
            imageLocations.Clear();

            #region Determine number of images to be placed into reserved areas.

            Random r = Maze.Random;
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
            Random r = Maze.Random;

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
            if (Maze.ReserveRectangle(sqW, sqH, 2, out rect))
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
            }
            if (shapeBuilderDelegate != null)
            {
                OutlineShape shape = OutlineShape.Instance(r, shapeBuilderDelegate, XSize, YSize, offCenter, size);
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

        public int XSize
        {
            get { return (Maze == null ? -1 : Maze.XSize); }
        }

        public int YSize
        {
            get { return (Maze == null ? -1 : Maze.YSize); }
        }

        public string Code
        {
            get { return (Maze == null ? "---" : Maze.Code); }
        }

        #endregion
    }
}
