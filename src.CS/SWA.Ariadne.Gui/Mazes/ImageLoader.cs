using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Threading;
using SWA.Ariadne.Settings;
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Mazes
{
    /// <summary>
    /// The ImageLoader can be used to pre-fetch several images in a background thread and convert them to ContourImages.
    /// The GetNext() method will immediately return the next available image, or wait until the next conversion is finished.
    /// </summary>
    public class ImageLoader
        : IImageLoader
    {
        private const char PathSeparator = ';';

        #region Member variables.

        /// <summary>
        /// This directory and its subdirectories are searched for images to be loaded.
        /// </summary>
        private readonly string imageFolder;

        /// <summary>
        /// Minimum and maximum size (in each dimension) the images should be scaled to.
        /// </summary>
        private int minSize, maxSize;

        /// <summary>
        /// When true, small images are enlarged to the defined size range.
        /// </summary>
        private readonly bool minSizeRequired = false;

        /// <summary>
        /// Probability for returning no image from the GetNext() method.
        /// </summary>
        public int YieldNullPercentage
        {
            set { yieldNullPercentage = value; }
        }
        private int yieldNullPercentage = 0;

        /// <summary>
        /// Synchronized queue for placing the loaded images.
        /// </summary>
        private readonly Queue queue;

        /// <summary>
        /// Desired number of images that should be in the queue.
        /// </summary>
        private readonly int queueLength;

        /// <summary>
        /// Similar to queue; contains the paths of all currently queued images.
        /// Will regularly be written to the Windows Registry.
        /// Note: This will be null in all but the ScreenSaver's ImageLoader(s).
        /// </summary>
        private readonly List<string> queuedImagePaths;

        /// <summary>
        /// Background thread responsible for filling the queue.
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        /// Used for regulating the buffered queue length.
        /// </summary>
        private readonly Semaphore queueEmptySemaphore, queueFullSemaphore;

        /// <summary>
        /// A list of recently used images.  We'll try to avoid using the same images in rapid succession.
        /// This static list is shared among all instances.
        /// </summary>
        private static readonly List<string> sharedRecentlyUsedImages = new List<string>();

        /// <summary>
        /// A private copy of the shared list, used in an ArenaForm context, when all
        /// ArenaItems shall use the same images.
        /// </summary>
        private readonly List<string> privateRecentlyUsedImages;

        /// <summary>
        /// A list of recently used images.  We'll try to avoid using the same images in rapid succession.
        /// </summary>
        private List<string> RecentlyUsedImages {
            get
            {
                if (privateRecentlyUsedImages != null) { return privateRecentlyUsedImages; }
                return sharedRecentlyUsedImages;
            }
        }

        /// <summary>
        /// Image paths that failed to load.
        /// </summary>
        private static Dictionary<string, bool> badImages = new Dictionary<string, bool>();

        #endregion

        #region Constructor.

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <param name="minSizeRequired"></param>
        /// <param name="imageFolder"></param>
        /// <param name="queueLength">Number of images to be pre-loaded into a queue.  If positive, the loader uses a background thread.</param>
        /// <param name="threadName">Unique name of this image loader thread.  Only used for positive queueLength.</param>
        /// <param name="isArena">If true, this ImageLoader will use a <see cref="privateRecentlyUsedImages"/>.</param>
        public ImageLoader(int minSize, int maxSize, bool minSizeRequired, string imageFolder, int queueLength,
            string threadName, bool isArena = false)
        {
            this.minSize = Math.Min(minSize, maxSize); // prevent abnormal behaviour
            this.maxSize = maxSize;
            this.minSizeRequired = minSizeRequired;
            this.imageFolder = imageFolder;
            this.queueLength = queueLength;

            if (isArena)
            {
                // Create a private copy of the current shared List.
                privateRecentlyUsedImages = new List<string>(sharedRecentlyUsedImages);
            }

            if (queueLength > 0)
            {
                this.queue = Queue.Synchronized(new Queue(queueLength));
                this.thread = new Thread(new ThreadStart(LoadImages));
                this.queueEmptySemaphore = new Semaphore(0, queueLength);
                this.queueFullSemaphore = new Semaphore(queueLength, queueLength);

                if (threadName != null)
                {
                    this.queuedImagePaths = new List<string>(queueLength);
                }

                // The background thread should run with high priority until a list of image filenames
                // has been loaded (see FindImages()).
                thread.Priority = ThreadPriority.AboveNormal;
                thread.IsBackground = true;
                thread.Name = threadName;
                thread.Start();
            }
        }

        /// <summary>
        /// Returns an ImageLoader configured with the screen saver parameters from the registry
        /// or null if the "image number" option is 0.
        /// </summary>
        /// <returns></returns>
        public static ImageLoader GetScreenSaverImageLoader(Rectangle screenBounds)
        {
            int count = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
            int minSize = ImageSize(screenBounds, RegisteredOptions.OPT_IMAGE_MIN_SIZE, RegisteredOptions.OPT_IMAGE_MIN_SIZE_PCT);
            int maxSize = ImageSize(screenBounds, RegisteredOptions.OPT_IMAGE_MAX_SIZE, RegisteredOptions.OPT_IMAGE_MAX_SIZE_PCT);
            string imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
            bool backgroundImages = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BACKGROUND_IMAGES);

            if (count > 0)
            {
                ImageLoader result = new ImageLoader(minSize, maxSize, false, imageFolder, count + 2, "FGIL");
                result.YieldNullPercentage = (backgroundImages ? 5 : 0);
                return result;
            }
            else
            {
                return null;
            }
        }

        private static int ImageSize(Rectangle screenBounds, string optNamePx, string optNamePct)
        {
            // Before version 3.5: image sizes are given in pixels
            int result = RegisteredOptions.GetIntSetting(optNamePx);

            // Since version 3.5: image sizes are given as a percentage of the screen size
            int pct = RegisteredOptions.GetIntSetting(optNamePct);
            if (pct > 0)
            {
                int screenSize = Math.Min(screenBounds.Width, screenBounds.Height);
                result = pct * screenSize / 100;
            }

            return result;
        }

        /// <summary>
        /// Sets new limits on the desired image size.
        /// </summary>
        /// <param name="bounds">The current maze size.</param>
        /// <remarks>
        /// The currently enqueued images need to be filtered in the GetNext() method.
        /// </remarks>
        public void UpdateImageSize(Rectangle bounds)
        {
            this.minSize = ImageSize(bounds, RegisteredOptions.OPT_IMAGE_MIN_SIZE, RegisteredOptions.OPT_IMAGE_MIN_SIZE_PCT);
            this.maxSize = ImageSize(bounds, RegisteredOptions.OPT_IMAGE_MAX_SIZE, RegisteredOptions.OPT_IMAGE_MAX_SIZE_PCT);

            try // this might fail on queue.Peek()
            {
                // If the size has changed considerably, many/all queued images will be discarded.
                while (this.queue.Count > 0 && ImageHasImproperSize(this.queue.Peek() as ContourImage, true))
                {
                    this.Dequeue();
                }
            } catch { }
        }

        /// <summary>
        /// Call this method when the application is going to terminate.
        /// </summary>
        public void Shutdown()
        {
            if (this.thread != null)
            {
                thread.Abort();
                //SaveImagePaths(); // this now happens in the Enqueue() method
                thread.Join();
            }
        }

        #endregion

        #region Access methods.

        /// <summary>
        /// Returns the next queued image.
        /// Returns null if no valid images can be found.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public ContourImage GetNext(Random r)
        {
            ContourImage result;

            if (yieldNullPercentage > 0 && r.Next(100) < yieldNullPercentage)
            {
                return null;
            }

            if (queueLength > 0)
            {
                result = Dequeue();

                // Check if the actual image size still fits the desired limits.
                // See UpdateImageSize().
                if (ImageHasImproperSize(result, minSizeRequired)) return null;
            }
            else
            {
                List<string> imagePaths = FindImages(imageFolder, 1, false, r);
                if (imagePaths.Count == 0)
                {
                    return null;
                }
                string imagePath = imagePaths[0];
                result = LoadImage(imagePath, r);
                if (result == null) { return result; }
                result.ProcessImage();
            }

            return result;
        }

        /// <summary>
        /// Compares the given image's dimensions to the desired limits.
        /// Returns true if the image should be discarded.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="testMinSize">May be this.minSizeRequired</param>
        private bool ImageHasImproperSize(ContourImage img, bool testMinSize)
        {
            if (img == null) return false; 

            int h = img.DisplayedImage.Height;
            int w = img.DisplayedImage.Width;
            if (h > maxSize || w > maxSize) return true;
            if (testMinSize && (h < minSize || w < minSize)) return true;

            return false;
        }

        #endregion

        #region Image provider thread.

        /// <summary>
        /// Background thread:
        /// Fills the queue with images.
        /// </summary>
        private void LoadImages()
        {
            Random r = RandomFactory.CreateRandom();

            #region Start with a few images, preferably without a contour.

            // The saved image paths are already ordered (see SaveImagePaths()).
            LoadAndEnqueue(r, true, LoadImagePaths());
            
            if (queue.Count >= queueLength)
            {
                ReduceThreadPriority();
            }
            else
            {
                LoadAndEnqueue(r, false, FindImages(imageFolder, queueLength + 1, true, r));
            }

            #endregion

            #region Continuously load more images, keeping the queue full.

            while (true)
            {
                int loadedImagesCount = 0;

                foreach (string imagePath in FindImages(imageFolder, 100, false, r))
                {
                    ContourImage img = LoadImage(imagePath, r);
                    if (img == null)
                    {
                        continue;
                    }

                    Enqueue(img);
                    ++loadedImagesCount;
                }

                // If no image was loaded successfully, enqueue a null value.
                if (loadedImagesCount == 0)
                {
                    Enqueue(null);
                }
            }

            #endregion
        }

        /// <summary>
        /// Loads and enqueues all images of the given list.
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <param name="preserveOrder">When false, the list is reordered so that images without a contour that need not be processed come first.</param>
        /// <param name="r"></param>
        private void LoadAndEnqueue(Random r, bool preserveOrder, IEnumerable<string> imagePaths)
        {
            //Log.WriteLine("{ " + string.Format("LoadAndEnqueue({0})", imagePaths.GetType().ToString()));
            List<ContourImage> unprocessedImages = new List<ContourImage>(queueLength + 1);

            IEnumerator<string> e = imagePaths.GetEnumerator();
            while(e.MoveNext())
            {
                string imagePath = e.Current;
                ContourImage img = null;
                
                if (imagePath != null)
                {
                    img = LoadImage(imagePath, r);
                }
                if (img == null)
                {
                    continue;
                }
                else if (!preserveOrder && img.HasContour)
                {
                    //Log.WriteLine("- LoadAndEnqueue() save contour image: " + img.ToString());
                    unprocessedImages.Add(img);
                }
                else
                {
                    Enqueue(img);
                }
            }

            foreach (ContourImage img in unprocessedImages)
            {
                Enqueue(img);
            }
            //Log.WriteLine("} LoadAndEnqueue()");
        }

        /// <summary>
        /// Returns an image loaded from the given path and scaled to the known size range.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private ContourImage LoadImage(string imagePath, Random r)
        {
            try
            {
                //Log.WriteLine("{ " + string.Format("LoadImage({0})", imagePath));
                Image img = new Bitmap(imagePath);

                //Log.WriteLine("- LoadImage() scale image");
                #region Scale img so that its larger dimension is between the desired min and max size.

                bool resize = false;

                if (img.Width > maxSize || img.Height > maxSize)
                {
                    resize = true;
                }
                if (minSizeRequired && img.Width < minSize && img.Height < minSize)
                {
                    resize = true;
                }

                if (resize)
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
                    Bitmap tmpImg = new Bitmap(img, new Size(w, h));
                    img.Dispose(); // closes and unlocks the image file
                    img = tmpImg;
                }

                #endregion

                //Log.WriteLine("- LoadImage() create contour image");
                ContourImage result = new ContourImage(img, imagePath);
                
                //Log.WriteLine("} LoadImage()");
                return result;
            }
            catch (Exception ex)
            {
                string msg = string.Format("cannot load image [{0}]: {1}", imagePath, ex.Message);
                Log.WriteLine(msg, true);
                badImages[imagePath] = true;
                return null;
            }
        }

        /// <summary>
        /// Processes and enqueues the given image.
        /// </summary>
        /// <param name="img"></param>
        private void Enqueue(ContourImage img)
        {
            //Log.WriteLine("{ " + string.Format("Enqueue({0})", img.ToString()));

            // Wait while the queue contains enough items.
            queueFullSemaphore.WaitOne();

            if (img != null)
            {
                img.ProcessImage();
            }

            // Enqueue the processed image.
            //Log.WriteLine("- Enqueue() Enqueue");
            queue.Enqueue(img);

            #region Add the image path to a separate list; save that list to the Registry

            if (queuedImagePaths != null)
            {
                lock (queuedImagePaths)
                {
                    if (img.HasContour)
                    {
                        // True contour images should be loaded last.
                        queuedImagePaths.Add(img.Path);
                    }
                    else
                    {
                        // Images without a contour should be loaded first.
                        queuedImagePaths.Insert(0, img.Path);
                    }

                    if (queuedImagePaths.Count >= queueLength)
                    {
                        SaveImagePaths();
                    }
                }
            }

            #endregion

            queueEmptySemaphore.Release();

#if true
            /* Continue at the lower thread priority as soon as the first image has been loaded.
             * This assumes that a) we need only one image or b) we can easily load more images on demand.
             * This works fine if enough images can be loaded from the saved images list.
             * See LoadImagePaths().
             */
            ReduceThreadPriority();
#endif
            //Log.WriteLine("} Enqueue()");
        }

        private ContourImage Dequeue()
        {
            //Log.WriteLine("{ Dequeue()");
            queueEmptySemaphore.WaitOne();
            //Log.WriteLine("- Dequeue() Dequeue");
            ContourImage result = queue.Dequeue() as ContourImage;
            if (queuedImagePaths != null)
            {
                lock (queuedImagePaths)
                {
                    queuedImagePaths.Remove(result.Path);
                }
            }
            queueFullSemaphore.Release();
            //Log.WriteLine("} Dequeue()");
            return result;
        }

        /// <summary>
        /// Returns a list of file names in or below the given path.
        /// Only the following image file types are considered: JPG, GIF, PNG.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="count"></param>
        /// <param name="quickSearch">when true, only JPG files are considered (if there are at least 100 of them)</param>
        /// <returns></returns>
        private List<string> FindImages(string folderPath, int count, bool quickSearch, Random r)
        {
            //Log.WriteLine("{ FindImages()");
            if (folderPath == null || folderPath == "" || count < 1)
            {
                return new List<string>();
            }

            List<string> availableImages = new List<string>();

            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.jpg", true));

            if (quickSearch == false || availableImages.Count < 100)
            {
                availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.gif", true));
                availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.png", true));

                if (!Platform.IsWindows)
                {
                    availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.JPG", true));
                    availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.GIF", true));
                    availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.PNG", true));
                }
            }

            // Initially, the thread was started with high priority (see the Constructor).
            // After the list has been loaded, it shall continue with low priority.
            ReduceThreadPriority();

            List<string> result = new List<string>(count);

            // Shorten the list of recently used images.
            // Make sure the list does not get too short.
            while (this.RecentlyUsedImages.Count > 0
                && RecentlyUsedImages.Count > Math.Min(availableImages.Count - count, availableImages.Count * 3 / 4)
                )
            {
                // Remove an item near the beginning of the list (recently added items are at the end).
                RecentlyUsedImages.RemoveAt(r.Next(RecentlyUsedImages.Count / 3 + 1));
            }

            // Select the required number of images.
            // Avoid recently used images.
            while (result.Count < count && availableImages.Count > 0)
            {
                int p = r.Next(availableImages.Count);
                string imagePath = availableImages[p];

                if (!RecentlyUsedImages.Contains(imagePath) && !badImages.ContainsKey(imagePath))
                {
                    result.Add(imagePath);
                    RecentlyUsedImages.Add(imagePath);
                }

                availableImages.RemoveAt(p);
            }

            //Log.WriteLine("} FindImages()");
            return result;
        }

        /// <summary>
        /// Sets the background thread priority to a minimum.
        /// </summary>
        private void ReduceThreadPriority()
        {
            ThreadPriority prio = ThreadPriority.Lowest;

            if (this.thread != null && thread.Priority != prio)
            {
                //Log.WriteLine("{ ReduceThreadPriority() set priority");
                thread.Priority = prio;
                Thread.Sleep(0);
                //Log.WriteLine("} ReduceThreadPriority()");
            }
        }

        #endregion

        #region Persistent memory of image paths.

        /// <summary>
        /// Writes the paths of all currently queued images to the Windows registry.
        /// These paths will be used to initialize the queue on the following run.
        /// </summary>
        private void SaveImagePaths()
        {
            var paths = this.queuedImagePaths;
            if (paths == null) return;

            #region Concatenate all paths.

            StringBuilder s = new StringBuilder(2048);

            foreach (string path in paths)
            {
                //Log.WriteLine("- SaveImagePaths() " + path);
                if (s.Length > 0) { s.Append(PathSeparator); }
                s.Append(path.Substring(imageFolder.Length));
            }

            #endregion

            Microsoft.Win32.RegistryKey key = RegisteredOptions.AppRegistryKey();
            if (key != null)
            {
                key.SetValue(thread.Name + " " + RegisteredOptions.SAVE_IMAGE_PATHS, s.ToString(), Microsoft.Win32.RegistryValueKind.String);
                //Log.WriteLine("SaveImagePaths[" + this.thread.Name + "]: " + s, true);
            }
        }

        private IEnumerable<string> LoadImagePaths()
        {
            string s = RegisteredOptions.GetStringSetting(thread.Name + " " + RegisteredOptions.SAVE_IMAGE_PATHS);
            if (s.Length == 0) { return new List<string>(); }

            string[] result = s.Split(new char[] { PathSeparator });

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = imageFolder + result[i];
            }

            RecentlyUsedImages.AddRange(result);

            return result;
        }

        #endregion
    }
}
