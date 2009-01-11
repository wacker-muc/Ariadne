package swa.ariadne.model;

/**
 * Provides {@link Maze} dimension limits based on the desired {@link MazeCode} length.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class MazeDimensions
{
    /**
     * Assumed ratio of screen or window dimensions, e.g. 1024x768
     */
    private static final double XYRatio = (4.0 / 3.0);
    
    /**
     * Minimum width or height: number of squares.
     */
    private static final int MinSize = 4;

    /**
     * Expected value, manually calculated.
     */
    private int _xRange = 337;

    /**
     * @return Maximum width: number of squares.
     */
    public int getMinSize()
    {
        return MinSize;
    }

    /**
     * @return Maximum width: number of squares.
     */
    public int getMaxXSize()
    {
        return MinSize + _xRange;
    }

    /**
     * Expected value, manually calculated.
     */
    private int _yRange = 251;
    
    /**
     * @return Maximum height: number of squares.
     */
    public int getMaxYSize ()
    {
        return MinSize + _yRange;
    }

    /**
     * Maximum distance of start/end point from border.
     */
    public static final int MaxBorderDistance = 16;

    /**
     * Maze encoding version.
     */
    private int _codeVersion;
    
    /**
     * @return Maze encoding version.
     */
    public int getCodeVersion()
    {
        return _codeVersion;
    }

    /**
     * Private Constructor.
     * @param version 0 or 1
     */
    private MazeDimensions(int version)
    {
        this._codeVersion = version;
        this.calculateDimensions();
    }

    /**
     * Calculate maximum x and y dimensions, based on the desired Maze.Code length.
     */
    private void calculateDimensions()
    {
        MazeCode codeObj = MazeCode.getInstance(this._codeVersion);
        double codeLimit = Math.pow(codeObj.getCodeDigitRange(), codeObj.getCodeLength());

        if (codeLimit > Long.MAX_VALUE)
        {
            throw new Error("Maze.Code is too large to be represented as a 64 bit integer");
        }

        codeLimit /= codeObj.getSeedLimit();
        //           (MaxXSize - MinSize + 1)
        //           (MaxYSize - MinSize + 1)

        if (_codeVersion == 0)
        {
            codeLimit /= WallPosition.NUM;
            codeLimit /= (MaxBorderDistance + 1);
            codeLimit /= (MaxBorderDistance + 1);
            //           (MaxXSize + 1)
            //           (MaxXSize + 1)
        }

        double x;

        if (_codeVersion == 0)
        {
            /* We want to find the greatest integer MaxXSize and MaxYSize with the limitation:
             *          (x-m) * (y-m) * x * x < c
             * with:
             *          x = MaxXSize + 1
             *          y = MaxYSize + 1  =  MaxXSize / XYRatio + 1
             *          m = MinSize
             *          c = codeLimit
             *          r = XYRatio
             * 
             * This is approximately equivalent to:
             *          x*x*x*x < c*r
             * or
             *          x = (c*r)^^(1/4)
             * With m>0, that x is even too small.
             */

            x = Math.floor(Math.pow(codeLimit * XYRatio, 0.25));

            while ((x - MinSize) * (x / XYRatio - MinSize) * (x) * (x) < codeLimit)
            {
                x = x + 1;
            }
        }
        else
        {
            /* We want to find the greatest integer MaxXSize and MaxYSize with the limitation:
             *          (x-m) * (y-m) < c
             * with:
             *          x = MaxXSize + 1
             *          y = MaxYSize + 1  =  MaxXSize / XYRatio + 1
             *          m = MinSize
             *          c = codeLimit
             *          r = XYRatio
             * 
             * This is approximately equivalent to:
             *          x*x*x*x < c*r
             * or
             *          x = (c*r)^^(1/2)
             * With m>0, that x is even too small.
             */

            x = Math.floor(Math.pow(codeLimit * XYRatio, 0.5));

            while ((x - MinSize) * (x / XYRatio - MinSize) < codeLimit)
            {
                x = x + 1;
            }
        }

        /* Now, x is 1 greater than acceptable, i.e.
         *          x-1  =  MaxXSize + 1  =  MinSize + xRange + 1
         *          MinSize + yRange  =  MaxYSize  =  MaxXSize / XYRatio
         */
        this._xRange = (int)(x - MinSize - 2);
        this._yRange = (int)(getMaxXSize() / XYRatio - MinSize); // Note: MaxXSize is valid after _xRange has been assigned
    }
    
    /**
     * Singleton objects.
     */
    private static MazeDimensions[] _instance = new MazeDimensions[2];
    
    /**
     * @param version 0 or 1
     * @return A singleton object for the given version.
     */
    public static MazeDimensions getInstance(int version)
    {
        if (_instance[version] == null)
        {
            _instance[version] = new MazeDimensions(version);
        }
        return _instance[version];
    }
}
