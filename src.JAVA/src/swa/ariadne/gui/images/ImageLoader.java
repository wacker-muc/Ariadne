package swa.ariadne.gui.images;

import java.awt.image.BufferedImage;
import java.io.File;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Iterator;
import java.util.List;
import java.util.Queue;
import java.util.Random;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.Semaphore;

import javax.imageio.ImageIO;

import swa.ariadne.settings.RegisteredOptions;
import swa.util.IParameterFile;
import swa.util.ImageUtil;
import swa.util.IntRange;
import swa.util.RandomFactory;

/**
 * The ImageLoader can be used to pre-fetch several images in a background thread
 * and convert them to {@link ContourImage ContourImages}.
 * <br>
 * The {@link #getNext(Random)} method will immediately return the next available image,
 * or wait until the next conversion is finished.
 *
 * @author Stephan.Wacker@web.de
 */
public final
class ImageLoader
implements IImageLoader
{
    //--------------------- Constants

    /** Used to separate several image paths. */
    private final String PathSeparator = ";";

    //--------------------- Member variables and Properties

    /** This directory and its sub-directories are searched for images to be loaded. */
    private final String imageFolder;

    /** Minimum and maximum size (in each dimension) the images should be scaled to. */
    private final IntRange sizeRange = new IntRange(200, 400);
    /** When true, small images are enlarged to the defined size range. */
    private boolean minSizeRequired = false;

    /** Probability for returning no image from the GetNext() method. */
    private int yieldNullPercentage = 0;

    /** Synchronized queue for placing the loaded images. */
    private Queue<ContourImage> queue;

    /** Desired number of images that should be in the queue. */
    private final int queueLength;

    /** Background thread responsible for filling the queue. */
    private Thread thread;

    /** Used for regulating the buffered queue length. */
    private Semaphore queueEmptySemaphore;

    /** Used for regulating the buffered queue length. */
    private Semaphore queueFullSemaphore;

    /**
     * A list of recently used images.
     * We'll try to avoid using the same images in rapid succession.
     */
    private final List<String> recentlyUsedImages = new ArrayList<String>();

    //--------------------- Constructors

    /**
     * Constructor.
     * @param minSize Minimum size (in each dimension) the images should be scaled to.
     * @param maxSize Maximum size (in each dimension) the images should be scaled to.
     * @param minSizeRequired When true, small images are enlarged to the defined size range.
     * @param imageFolder This directory and its sub-directories are searched for images to be loaded.
     * @param queueLength Number of images to be pre-loaded into a queue.  If positive, the loader uses a background thread.
     * @param threadName Unique name of this image loader thread.  Only used for positive queueLength.
     */
    public ImageLoader(int minSize, int maxSize, boolean minSizeRequired, String imageFolder, int queueLength, String threadName)
    {
        this.sizeRange.min = minSize;
        this.sizeRange.max = maxSize;
        this.sizeRange.includingMax = true;
        this.minSizeRequired = minSizeRequired;
        this.imageFolder = imageFolder;
        this.queueLength = queueLength;

        if (queueLength > 0)
        {
            this.queue = new LinkedBlockingQueue<ContourImage>(queueLength);
            //this.queueEmptySemaphore = new Semaphore(0, queueLength);
            //this.queueFullSemaphore = new Semaphore(queueLength, queueLength);

            this.thread = new Thread(new Runnable()
            {
                @Override
                public void run()
                {
                    LoadImages();
                }
            });

            // The background thread should run with high priority until a list of image filenames
            // has been loaded (see FindImages()).
            thread.setPriority(Thread.MAX_PRIORITY);
            thread.setDaemon(true);
            thread.setName(threadName);
            thread.start();
        }
    }

    /**
     * @return An {@link ImageLoader} configured with the persistent screen saver parameters
     * or null if the "image number" option is 0.
     */
    public static ImageLoader GetScreenSaverImageLoader()
    {
        int count = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_NUMBER);
        int minSize = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MIN_SIZE);
        int maxSize = RegisteredOptions.GetIntSetting(RegisteredOptions.OPT_IMAGE_MAX_SIZE);
        String imageFolder = RegisteredOptions.GetStringSetting(RegisteredOptions.OPT_IMAGE_FOLDER);
        boolean backgroundImages = RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_BACKGROUND_IMAGES);

        if (count > 0)
        {
            ImageLoader result = new ImageLoader(minSize, maxSize, false, imageFolder, count + 2, "FGIL");
            result.setYieldNullPercentage(backgroundImages ? 5 : 0);
            return result;
        }
        else
        {
            return null;
        }
    }

    //--------------------- IImageLoader implementation

    @Override
    public ContourImage getNext(Random r)
    {
        ContourImage result;

        if (yieldNullPercentage > 0 && r.nextInt(100) < yieldNullPercentage)
        {
            return null;
        }

        if (queueLength > 0)
        {
            result = Dequeue();
        }
        else
        {
            List<String> imagePaths = FindImages(imageFolder, 1, false, r);
            if (imagePaths.size() == 0)
            {
                return null;
            }
            String imagePath = imagePaths.get(0);
            result = LoadImage(imagePath, r);
            result.processImage();
        }

        return result;
    }

    @Override
    public void setYieldNullPercentage(int value)
    {
        this.yieldNullPercentage = value;
    }

    @Override
    public void shutdown()
    {
        if (this.thread != null)
        {
            try
            {
                thread.stop(); // TODO: set an indicator variable and interrupt the thread
                SaveImagePaths();
                thread.join();
            }
            catch (InterruptedException e)
            {
                // do nothing
            }
        }
    }

    //--------------------- Image provider thread.

    /**
     *
     */
    protected void LoadImages()
    {
        Random r = RandomFactory.createRandom();

        //--------------------- Start with a few images, preferably without a contour.

        // The saved image paths are already ordered (see SaveImagePaths()).
        LoadAndEnqueue(r, true, LoadImagePaths());

        if (queue.size() >= queueLength)
        {
            ReduceThreadPriority();
        }
        else
        {
            LoadAndEnqueue(r, false, FindImages(imageFolder, queueLength + 1, true, r));
        }

        //--------------------- Continuously load more images, keeping the queue full.

        while (true)
        {
            int loadedImagesCount = 0;

            for (String imagePath : FindImages(imageFolder, 100, false, r))
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
    }

    /**
     * @param r A source of random numbers.
     * @param preserveOrder When false, the list is re-ordered so that images without a contour that need not be processed come first.
     * @param imagePaths A list of fully qualified image file paths.
     */
    private void LoadAndEnqueue(Random r, boolean preserveOrder, Iterable<String> imagePaths)
    {
        //Log.WriteLine("{ " + string.Format("LoadAndEnqueue({0})", imagePaths.GetType().ToString()));
        List<ContourImage> unprocessedImages = new ArrayList<ContourImage>(queueLength + 1);

        Iterator<String> e = imagePaths.iterator();
        while(e.hasNext())
        {
            String imagePath = e.next();
            ContourImage img = null;

            if (imagePath != null)
            {
                img = LoadImage(imagePath, r);
            }

            if (img == null)
            {
                // continue;
            }
            else if (!preserveOrder && img.hasContour())
            {
                //Log.WriteLine("- LoadAndEnqueue() save contour image: " + img.ToString());
                unprocessedImages.add(img);
            }
            else
            {
                Enqueue(img);
            }

            e.remove();
        }

        for (ContourImage img : unprocessedImages)
        {
            Enqueue(img);
        }
        //Log.WriteLine("} LoadAndEnqueue()");
    }


    /**
     * @param imagePath A fully qualified image file path.
     * @param r A source of random numbers.
     * @return An image loaded from the given path and scaled to the known size range.
     */
    private ContourImage LoadImage(String imagePath, Random r)
    {
        try
        {
            //Log.WriteLine("{ " + string.Format("LoadImage({0})", imagePath));
            BufferedImage img = ImageIO.read(new File(imagePath));

            //Log.WriteLine("- LoadImage() scale image");
            // [start] Scale img so that its larger dimension is between the desired min and max size.

            boolean resize = false;

            if (img.getWidth() > sizeRange.max || img.getHeight() > sizeRange.max)
            {
                resize = true;
            }
            if (minSizeRequired && img.getWidth() < sizeRange.min && img.getHeight() < sizeRange.min)
            {
                resize = true;
            }

            if (resize)
            {
                int d = sizeRange.pick(r);
                int h = img.getHeight(), w = img.getWidth();
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

                BufferedImage tmpImg = ImageUtil.resize(img, w, h);
                img = tmpImg;
            }

            // [end]

            //Log.WriteLine("- LoadImage() create contour image");
            ContourImage result = new ContourImage(img, imagePath);

            //Log.WriteLine("} LoadImage()");
            return result;
        }
        catch (Exception e)
        {
            System.out.println(String.format("failed loading image [%s]: %s", imagePath, e.toString()));
            return null;
        }
    }


    /**
     * Processes and enqueues the given image.
     * @param img A {@link ContourImage} that has not yet been processed.
     */
    private void Enqueue(ContourImage img)
    {
        //Log.WriteLine("{ " + string.Format("Enqueue({0})", img.ToString()));

        // Wait while the queue contains enough items.
        queueFullSemaphore.acquireUninterruptibly();

        if (img != null)
        {
            img.processImage();
        }

        // Enqueue the processed image.
        //Log.WriteLine("- Enqueue() Enqueue");
        queue.add(img);
        queueEmptySemaphore.release();

        if (true)
        {
            /* Continue at the lower thread priority as soon as the first image has been loaded.
             * This assumes that a) we need only one image or b) we can easily load more images on demand.
             * This works fine if enough images can be loaded from the saved images list.
             * See LoadImagePaths().
             */
            ReduceThreadPriority();
        }

        //Log.WriteLine("} Enqueue()");
    }


    /**
     * @return The next {@link ContourImage} from the queue.
     */
    private ContourImage Dequeue()
    {
        //Log.WriteLine("{ Dequeue()");
        queueEmptySemaphore.acquireUninterruptibly();
        //Log.WriteLine("- Dequeue() Dequeue");
        ContourImage result = queue.poll();
        queueFullSemaphore.release();
        //Log.WriteLine("} Dequeue()");
        return result;
    }


    /**
     * @param folderPath The full path to a directory containing image files.
     * @param count Number of file names that should be collected.
     * @param quickSearch When true, only JPG files are considered (if there are at least 100 of them).
     * @param r A source of random numbers.
     * @return A list of file names in or below the given path.
     * Only the following image file types are considered: JPG, GIF, PNG.
     */
    private List<String> FindImages(String folderPath, int count, boolean quickSearch, Random r)
    {
        //Log.WriteLine("{ FindImages()");
        if (folderPath == null || count < 1)
        {
            return new ArrayList<String>();
        }

        List<String> availableImages = new ArrayList<String>();

        availableImages.addAll(swa.util.Directory.Find(folderPath, "*.jpg", true));

        if (quickSearch == false || availableImages.size() < 100)
        {
            availableImages.addAll(swa.util.Directory.Find(folderPath, "*.gif", true));
            availableImages.addAll(swa.util.Directory.Find(folderPath, "*.png", true));
        }

        // Initially, the thread was started with high priority (see the Constructor).
        // After the list has been loaded, it shall continue with low priority.
        ReduceThreadPriority();

        ArrayList<String> result = new ArrayList<String>(count);

        // Shorten the list of recently used images.
        // Make sure the list does not get too short.
        while (this.recentlyUsedImages.size() > 0
            && this.recentlyUsedImages.size() > availableImages.size() - count
            && this.recentlyUsedImages.size() > availableImages.size() * 3 / 4
            )
        {
            // Remove an item near the beginning of the list (recently added items are at the end).
            this.recentlyUsedImages.remove(r.nextInt(this.recentlyUsedImages.size() / 3 + 1));
        }

        // Select the required number of images.
        // Avoid recently used images.
        while (result.size() < count && availableImages.size() > 0)
        {
            int p = r.nextInt(availableImages.size());
            String imagePath = availableImages.get(p);

            if (!recentlyUsedImages.contains(imagePath))
            {
                result.add(imagePath);
                recentlyUsedImages.add(imagePath);
            }

            availableImages.remove(p);
        }

        //Log.WriteLine("} FindImages()");
        return result;
    }


    /**
     * Sets the background thread priority to a minimum.
     */
    private void ReduceThreadPriority()
    {
        int prio = Thread.MIN_PRIORITY;

        if (this.thread != null && thread.getPriority() != prio)
        {
            //Log.WriteLine("{ ReduceThreadPriority() set priority");
            thread.setPriority(prio);
            //Thread.sleep(0);
            //Log.WriteLine("} ReduceThreadPriority()");
        }
    }

    //---------------------  Persistent memory of image paths.


    //--------------------- Persistent memory of image paths.

    /**
     * Writes the paths of all currently queued images to the registry.
     * These paths will be used to initialize the queue on the following run.
     */
    private void SaveImagePaths()
    {
        //Log.WriteLine("{ SaveImagePaths()");

        // [start] Get the paths of all currently queued images.

        List<String> paths = new ArrayList<String>(queue.size());

        while (queue.size() > 0)
        {
            ContourImage img = Dequeue();

            if (img.getPath() != null)
            {
                if (img.hasContour())
                {
                    // True contour images should be loaded last.
                    paths.add(img.getPath());
                }
                else
                {
                    // Images without a contour should be loaded first.
                    paths.add(0, img.getPath());
                }
            }
        }

        // [end]

        // [start] Concatenate all paths.

        StringBuilder s = new StringBuilder(2048);

        for (String path : paths)
        {
            //Log.WriteLine("- SaveImagePaths() " + path);
            if (s.length() > 0) { s.append(PathSeparator); }
            s.append(path.substring(imageFolder.length()));
        }

        // [end]

        IParameterFile params = RegisteredOptions.getParameterFile();
        if (params != null)
        {
            params.set(thread.getName() + " " + RegisteredOptions.SAVE_IMAGE_PATHS, s.toString());
        }
        //Log.WriteLine("} SaveImagePaths()");
    }


    /**
     * @return A list of image path names that have been stored in the previous run.
     */
    private Iterable<String> LoadImagePaths()
    {
        //Log.WriteLine("{ LoadImagePaths()");
        String s = RegisteredOptions.GetStringSetting(thread.getName() + " " + RegisteredOptions.SAVE_IMAGE_PATHS);
        String[] paths = s.split(PathSeparator);

        for (int i = 0; i < paths.length; i++)
        {
            paths[i] = imageFolder + paths[i];
        }

        List<String> result = Arrays.asList(paths);
        recentlyUsedImages.addAll(result);

        //Log.WriteLine("} LoadImagePaths()");
        return result;
    }
}
