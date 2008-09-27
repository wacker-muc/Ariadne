using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Threading;
using SWA.Utilities;

namespace SWA.Ariadne.Gui.Mazes
{
    public class ImageLoader
    {
        #region Member variables.

        /// <summary>
        /// This directory and its subdirectories are searched for images to be loaded.
        /// </summary>
        private readonly string imageFolder;

        /// <summary>
        /// Minimum and maximum size (in each dimension) the images should be scaled to.
        /// </summary>
        private readonly int minSize, maxSize;

        /// <summary>
        /// Synchronized queue for placing the loaded images.
        /// </summary>
        private readonly Queue queue;

        /// <summary>
        /// Desired number of images that should be in the queue.
        /// </summary>
        private readonly int queueLength;

        /// <summary>
        /// Background thread responsible for filling the queue.
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        /// Used for regulating the buffered queue length.
        /// </summary>
        private readonly Semaphore queueEmptySemaphore, queueFullSemaphore;

#if true
        /// <summary>
        /// A list of recently used images.  We'll try to avoid using the same images in rapid succession.
        /// </summary>
        private List<string> recentlyUsedImages = new List<string>();
#endif

        #endregion

        #region Constructor.

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <param name="imageFolder"></param>
        /// <param name="queueLength">Number of images to be pre-loaded into a queue.  If positive, the loader uses a background thread.</param>
        public ImageLoader(int minSize, int maxSize, string imageFolder, int queueLength)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.imageFolder = imageFolder;
            this.queueLength = queueLength;

            if (queueLength > 0)
            {
                this.queue = Queue.Synchronized(new Queue(queueLength));
                this.thread = new Thread(new ThreadStart(LoadImages));
                this.queueEmptySemaphore = new Semaphore(0, queueLength);
                this.queueFullSemaphore = new Semaphore(queueLength, queueLength);

                thread.Priority = ThreadPriority.BelowNormal;
                thread.Start();
            }
        }

        /// <summary>
        /// Call this method when the application is going to terminate.
        /// </summary>
        public void Shutdown()
        {
            if (this.thread != null)
            {
                thread.Abort();
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

            if (queueLength > 0)
            {
                queueEmptySemaphore.WaitOne();
                result = queue.Dequeue() as ContourImage;
                queueFullSemaphore.Release();
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
            }

            return result;
        }

        #endregion

        #region Image provider thread.

        /// <summary>
        /// Background thread:
        /// Fills the queue with images.
        /// </summary>
        private void LoadImages()
        {
            // Parameters for first iteration.
            int chunkSize = 2;
            bool quickSearch = true;

            Random r = RandomFactory.CreateRandom();

            while (true)
            {
                List<string> imagePaths = FindImages(imageFolder, chunkSize, quickSearch, r);
                int loadedImagesCount = 0;

                foreach (string imagePath in imagePaths)
                {
                    // Wait while the queue contains enough items.
                    queueFullSemaphore.WaitOne();

                    ContourImage img = LoadImage(imagePath, r);
                    
                    if (img != null)
                    {
                        queue.Enqueue(img);
                        queueEmptySemaphore.Release();
                        ++loadedImagesCount;
                    }
                    else
                    {
                        // Return the semaphore to the state before the try block.
                        queueFullSemaphore.Release();
                    }
                }

                // If no image was found or loaded successfully, put a null value into the queue.
                if (loadedImagesCount == 0)
                {
                    queueFullSemaphore.WaitOne();
                    queue.Enqueue(null);
                    queueEmptySemaphore.Release();
                }

                // Parameters for following iterations.
                chunkSize = 100;
                quickSearch = false;
            }
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
                Image img = new Bitmap(imagePath);

                #region Scale img so that its larger dimension is between the desired min and max size.

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

                #region Remove a uniform background color.

                ContourImage result = new ContourImage(img);

                #endregion

                return result;
            }
            catch (Exception e)
            {
                System.Console.Out.WriteLine("failed loading image [{0}]: {1}", imagePath, e.ToString());
                return null;
            }
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
            if (folderPath == null || count < 1)
            {
                return new List<string>();
            }

            List<string> availableImages = new List<string>();

            availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.jpg", true));

            if (quickSearch == false || availableImages.Count < 100)
            {
                availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.gif", true));
                availableImages.AddRange(SWA.Utilities.Directory.Find(folderPath, "*.png", true));
            }

            List<string> result = new List<string>(count);

#if true
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
#endif

            // Select the required number of images.
            // Avoid recently used images.
            while (result.Count < count && availableImages.Count > 0)
            {
                int p = r.Next(availableImages.Count);
                string imagePath = availableImages[p];

#if true
                if (!recentlyUsedImages.Contains(imagePath))
                {
                    result.Add(imagePath);
                    recentlyUsedImages.Add(imagePath);
                }
#else
                result.Add(imagePath);
#endif

                availableImages.RemoveAt(p);
            }

            return result;
        }

        #endregion
    }
}