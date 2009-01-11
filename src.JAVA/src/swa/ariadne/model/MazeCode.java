package swa.ariadne.model;

/**
 * This class is used to encode/decode the characteristic {@link Maze} parameters
 * in a short identifying string.
 * <p>
 * The encoding schemas are:
 * <ul><li>
 *    Version 0: 12 characters 'A'..'Z'
 * </li><li>
 *    Version 1:  6 characters '0'..'9', 'a'..'z'
 * </li></ul>
 * 
 * @author Stephan.Wacker@web.de
 */
public class MazeCode
{
    /**
     * Range of the initial seed value: v0: 2^13, v1: 2^14.
     * This value and the MazeDimension.Max... values are chosen so that the maze code
     * may be represented with [v0: 12 / v1: 6] characters.
     */
    private int _seedLimit = 8192;
    
    /**
     * @return Range of the initial seed value: v0: 2^13, v1: 2^14.
     */
    public int getSeedLimit()
    {
        return this._seedLimit;
    }

    /**
     * Number of significant characters in the code string.
     * The string may contain additional separator characters.
     */
    private int _codeLength = 12;

    /**
     * @return Number of significant characters in the code string.
     */
    public int getCodeLength()
    {
        return this._codeLength;
    }

    /**
     * Number of different letters/digits in the code.  
     */
    private int _codeDigitRange = 26; // 'A' .. 'Z'
    
    /**
     * @return Number of different letters/digits in the code.  
     */
    public int getCodeDigitRange()
    {
        return this._codeDigitRange;
    }
    
    /**
     * Version 0: [A-Z]{12}
     * Version 1: [0-9a-z]{6}
     */
    public static final int DefaultCodeVersion = 1;

    /**
     * Maze encoding version.
     */
    private int _codeVersion;
    
    /**
     * @return Maze encoding version.
     */
    public int getCodeVersion()
    {
        return this._codeVersion;
    }
    
    /**
     * Private Constructor.
     * @param version 0 or 1
     */
    private MazeCode(int version)
    {
        this._codeVersion = version;

        switch (version)
        {
            case 0:
                this._codeLength = 12;
                this._codeDigitRange = 26; // 'A' .. 'Z'
                this._seedLimit = 8 * 1024;
                break;
            case 1:
                this._codeLength = 6;
                this._codeDigitRange = 36; // '0' .. '9', 'a' .. 'z'
                this._seedLimit = 16 * 1024;
                break;
        }
    }
    
    /**
     * A string that encodes the maze parameters.
     * This code can be used to construct an identical maze.
     * @param maze A Maze object.
     * @return The code string for the given Maze.
     */
    public String code(Maze maze)
    {
        long nCode = 0;
        MazeDimensions dimensionsObj = MazeDimensions.getInstance(_codeVersion);

        //#region Encode the relevant parameters into a numeric code

        // Items are encoded in reverse order of decoding.
        // Some items must be encoded before others if decoding requires them.

        if (_codeVersion == 0)
        {
            // Encode the start and end points.
            // Instead of the direct coordinates we will use the following information:
            // * travel direction
            // * distance from the border (instead of coordinate)
            // * other coordinate
            // The scaling factor is always MazeDimensions.MaxXSize, as it is greater than MazeDimensions.MaxYSize.

            int d1, d2, c1, c2;

            switch (maze.getDirection())
            {
                case WP_E:
                    d1 = maze.getStartSquare().getXPos();
                    d2 = maze.getXSize() - 1 - maze.getTargetSquare().getXPos();
                    c1 = maze.getStartSquare().getYPos();
                    c2 = maze.getTargetSquare().getYPos();
                    break;
                case WP_W:
                    d1 = maze.getTargetSquare().getXPos();
                    d2 = maze.getXSize() - 1 - maze.getStartSquare().getXPos();
                    c1 = maze.getTargetSquare().getYPos();
                    c2 = maze.getStartSquare().getYPos();
                    break;
                case WP_S:
                    d1 = maze.getStartSquare().getYPos();
                    d2 = maze.getYSize() - 1 - maze.getTargetSquare().getYPos();
                    c1 = maze.getStartSquare().getXPos();
                    c2 = maze.getTargetSquare().getXPos();
                    break;
                case WP_N:
                    d1 = maze.getTargetSquare().getYPos();
                    d2 = maze.getYSize() - 1 - maze.getStartSquare().getYPos();
                    c1 = maze.getTargetSquare().getXPos();
                    c2 = maze.getStartSquare().getXPos();
                    break;
                default:
                    d1 = d2 = c1 = c2 = -1;
                    break;
            }

            nCode *= (MazeDimensions.MaxBorderDistance + 1);
            nCode += d1;

            nCode *= (MazeDimensions.MaxBorderDistance + 1);
            nCode += d2;

            nCode *= (dimensionsObj.getMaxXSize() + 1);
            nCode += c1;

            nCode *= (dimensionsObj.getMaxXSize() + 1);
            nCode += c2;

            nCode *= WallPosition.NUM;
            nCode += maze.getDirection().ordinal();
        }

        // Encode maze dimension.

        nCode *= (dimensionsObj.getMaxYSize() - dimensionsObj.getMinSize() + 1);
        nCode += (maze.getYSize() - dimensionsObj.getMinSize());

        nCode *= (dimensionsObj.getMaxXSize() - dimensionsObj.getMinSize() + 1);
        nCode += (maze.getXSize() - dimensionsObj.getMinSize());

        // Encode initial seed.

        nCode *= getSeedLimit();
        nCode += maze.getSeed();

        //#endregion

        // v0: The resulting nCode is less than 26^12.  See swa.ariadne.model.tests unit tests.
        // v1: The resulting nCode is less than 36^6.  See swa.ariadne.model.tests unit tests.

        //#region Convert the numeric code into a character code (base 26)

        StringBuilder result = new StringBuilder(7);

        for (int p = _codeLength; p-- > 0; )
        {
            int digit = (int)(nCode % _codeDigitRange);
            nCode /= _codeDigitRange;
            char c = '?';
            switch (_codeDigitRange)
            {
                case 26:
                    c = (char)(digit + 'A');
                    break;
                case 36:
                    if (digit < 10)
                    {
                        c = (char)(digit + '0');
                    }
                    else
                    {
                        c = (char)(digit - 10 + 'a');
                    }
                    break;
            }
            result.insert(0, c);
        }

        switch (_codeLength)
        {
            case 12:
                result.insert(8, '.');
                result.insert(4, '.');
                break;
            case 6:
                result.insert(3, '.');
                break;
        }

        //#endregion

        return result.toString();
    }
    
    /**
     * Determine the encoding schema version of the given code string.
     * @param code A maze code string.
     * @return 0 or 1
     */
    public static int getCodeVersion(String code)
    {
        char[] a = code.replace(".", "").toCharArray();
        int result = -1;
        int nVersions = _instance.length;

        for (int v = 0; v < nVersions; v++)
        {
            if (getInstance(v).getCodeLength() == a.length)
            {
                result = v;
            }
        }

        if (result < 0)
        {
            StringBuilder s = new StringBuilder();
            s.append(getInstance(0).getCodeLength());
            for (int v = 1; v < nVersions; v++)
            {
                s.append(" or ");
                s.append(getInstance(v).getCodeLength());
            }
            throw new Error("Code must be " + s.toString() + " characters long:" + code);
        }

        return result;
    }
    
    /**
     * Output parameters of the decode() method.
     */
    private class DecodeOutput {
        /** DecodeParameter.java */
        public int seed;

        /** DecodeParameter.java */
        public int xSize;

        /** DecodeParameter.java */
        public int ySize;

        /**
         * Constructor.
         */
        public DecodeOutput()
        {
            this.seed = this.xSize = this.ySize = -1;
        }
    }

    /**
     * Extract the Maze parameters from an encoded string.
     * @param code The code string to analyze.
     * @param out Output parameters.
     */
    public void decode(String code, DecodeOutput out)
    {
        long nCode = 0;
        MazeDimensions dimensionsObj = MazeDimensions.getInstance(_codeVersion);

        //#region Convert the character code (base 26) into a numeric code

        char[] a = code.replaceAll(".", "").toCharArray();

        if (!(a.length == _codeLength))
        {
            throw new Error("code must be " + _codeLength + " characters long: " + code);
        }

        for (int p = 0; p < a.length; p++)
        {
            int digit = 0;

            switch (_codeDigitRange)
            {
                case 26:
                    digit = a[p] - 'A';
                    break;
                case 36:
                    digit = a[p] - '0';
                    if (digit >= 10)
                    {
                        digit = 10 + a[p] - 'a';
                    }
                    break;
            }

            if (!(0 <= digit && digit < _codeDigitRange))
            {
                String allowed = "???";
                switch (_codeDigitRange)
                {
                    case 26:
                        allowed = "'A'..'Z'";
                        break;
                    case 36:
                        allowed = "'0'..'9' and 'a'..'z'";
                        break;
                }
                throw new Error("Allowed code characters are " + allowed + " only: " + code);
            }

            nCode *= _codeDigitRange;
            nCode += digit;
        }

        //#endregion

        //#region Decode items in the code in reverse order of encoding

        @SuppressWarnings("unused")
        long nCodeOriginal = nCode; // for debugging

        long itemRange;

        itemRange = _seedLimit;
        out.seed = (int)(nCode % itemRange);
        nCode /= itemRange;

        itemRange = dimensionsObj.getMaxXSize() - dimensionsObj.getMinSize() + 1;
        out.xSize = (int)(nCode % itemRange) + dimensionsObj.getMinSize();
        nCode /= itemRange;

        itemRange = dimensionsObj.getMaxYSize() - dimensionsObj.getMinSize() + 1;
        out.ySize = (int)(nCode % itemRange) + dimensionsObj.getMinSize();
        nCode /= itemRange;

        if (_codeVersion == 0)
        {
            //#region Decoding of obsolete Version 0 parameters

            WallPosition direction;
            @SuppressWarnings("unused")
            int xStart, yStart;
            @SuppressWarnings("unused")
            int xEnd, yEnd;
            int d1, d2, c1, c2;

            itemRange = WallPosition.NUM;
            direction = WallPosition.values()[(int)(nCode % itemRange)];
            nCode /= itemRange;

            itemRange = dimensionsObj.getMaxXSize() + 1;
            c2 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = dimensionsObj.getMaxXSize() + 1;
            c1 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MazeDimensions.MaxBorderDistance + 1;
            d2 = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = MazeDimensions.MaxBorderDistance + 1;
            d1 = (int)(nCode % itemRange);
            nCode /= itemRange;

            switch (direction)
            {
                case WP_E:
                    xStart = d1;
                    xEnd = out.xSize - 1 - d2;
                    yStart = c1;
                    yEnd = c2;
                    break;
                case WP_W:
                    xEnd = d1;
                    xStart = out.xSize - 1 - d2;
                    yEnd = c1;
                    yStart = c2;
                    break;
                case WP_S:
                    yStart = d1;
                    yEnd = out.ySize - 1 - d2;
                    xStart = c1;
                    xEnd = c2;
                    break;
                case WP_N:
                    yEnd = d1;
                    yStart = out.ySize - 1 - d2;
                    xEnd = c1;
                    xStart = c2;
                    break;
                default:
                    xStart = yStart = xEnd = yEnd = -1;
                    break;
            }

            //#endregion
        }

        //#endregion

        //#region Verify the validity of the decoded items

        if (!(nCode == 0))
        {
            throw new Error("remainder(code) must be a zero: " + nCode);
        }
        validateCodeItemRange("seed", out.seed, 0, _seedLimit-1);
        validateCodeItemRange("xSize", out.xSize, dimensionsObj.getMinSize(), dimensionsObj.getMaxXSize());
        validateCodeItemRange("ySize", out.ySize, dimensionsObj.getMinSize(), dimensionsObj.getMaxYSize());

        //#endregion
    }

    /**
     * Verify the validity of the decoded items
     * @param item Name of the code item.
     * @param value Value of the code item.
     * @param min Minimum allowed value.
     * @param max Maximum allowed value.
     */
    private static void validateCodeItemRange(String item, int value, int min, int max)
    {
        if (!(min <= value && value <= max))
        {
            throw new Error(item + " of code must be between " + min + " and " + max + ": " + value);
        }
    }
    
    /**
     * Singleton objects.
     */
    private static MazeCode[] _instance = new MazeCode[2];
    
    /**
     * @param version 0 or 1
     * @return A singleton object for the given version.
     */
    public static MazeCode getInstance(int version)
    {
        if (_instance[version] == null)
        {
            _instance[version] = new MazeCode(version);
        }
        return _instance[version];
    }
}
