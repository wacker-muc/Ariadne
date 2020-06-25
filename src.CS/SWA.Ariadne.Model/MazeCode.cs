using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// This class is used to encode/decode the Maze parameters in a short identifying string.
    /// The encoding schemas are:
    ///    Version 0: 12 characters 'A'..'Z'
    ///    Version 1:  6 characters '0'..'9', 'a'..'z'
    /// </summary>
    public class MazeCode
    {
        #region Member variables

        public readonly string Description;

        /// <summary>
        /// Range of the initial seed value: v0: 2^13, v1: 2^14.
        /// This value and the MazeDimension.Max... values are chosen so that the maze Code
        /// may be represented with [v0: 12 / v1: 6] characters.
        /// </summary>
        public readonly int SeedLimit = 8192;

        public readonly int CodeLength = 12;
        public readonly int CodeDigitRange = 26; // 'A' .. 'Z'

        private readonly string CodeDigits; // 'A' .. 'Z'

        public readonly string Separator = ".";

        // Version 0: [A-Z]{12}
        // Version 1: [0-9a-z]{6}
        // Version 2: [0-9a-zA..Z]{6} // except some similar letters: 'l', 'I', '0', 'O'
        public const int DefaultCodeVersion = 2;

        public const int MaxCodeVersion = 2;

        /// <summary>
        /// Maze encoding version.
        /// </summary>
        public readonly int codeVersion;

        #endregion

        #region Constructor

        /// <summary>
        /// Private Constructor.
        /// </summary>
        /// <param name="version"></param>
        private MazeCode(int version)
        {
            this.codeVersion = version;

            switch (version)
            {
                case 0:
                    Description = "v0: 12 uppercase letters";
                    CodeLength = 12;
                    CodeDigitRange = 26; // 'A' .. 'Z'
                    Separator = ".";
                    SeedLimit = 8 * 1024;
                    break;
                case 1:
                    Description = "v1: 6 lowercase letters or digits";
                    CodeLength = 6;
                    CodeDigitRange = 36; // '0' .. '9', 'a' .. 'z'
                    Separator = ".";
                    SeedLimit = 16 * 1024;
                    break;
                case 2:
                    Description = "v2: 6 letters or digits, but not 'I', 'O', 'l' or '0'";
                    CodeLength = 6;
                    CodeDigitRange = 9 + 25 + 24; // digits and letters with a few exceptions
                    Separator = "-";
                    SeedLimit = 32 * 1024;
                    break;
            }

            #region Calculate the valid CodeDigits
            StringBuilder s = new StringBuilder(CodeDigitRange);
            int v = this.codeVersion;

            for (char c = '0'; c <= 'z'; c++)
            {
                if (v == 0 && !char.IsUpper(c)) continue;
                if (v == 1 && !char.IsDigit(c) && !char.IsLower(c)) continue;
                if (v == 2 && !char.IsDigit(c) && !char.IsLetter(c)) continue;
                if (v == 2 && "IOl0".IndexOf(c) >= 0) continue;
                s.Append(c);
            }
            this.CodeDigits = s.ToString();
            #endregion
        }

        #endregion

        #region Encoding methods

        /// <summary>
        /// A string that encodes the maze parameters.
        /// This code can be used to construct an identical maze.
        /// </summary>
        public string Code(Maze maze)
        {
            long nCode = 0;
            MazeDimensions dimensionsObj = MazeDimensions.Instance(codeVersion);

            #region Encode the relevant parameters into a numeric code

            // Items are encoded in reverse order of decoding.
            // Some items must be encoded before others if decoding requires them.

            if (codeVersion == 0)
            {
                // Encode the start and end points.
                // Instead of the direct coordinates we will use the following information:
                // * travel direction
                // * distance from the border (instead of coordinate)
                // * other coordinate
                // The scaling factor is always MazeDimensions.MaxXSize, as it is greater than MazeDimensions.MaxYSize.

                int d1, d2, c1, c2;

                switch (maze.Direction)
                {
                    case WallPosition.WP_E:
                        d1 = maze.StartSquare.XPos;
                        d2 = maze.XSize - 1 - maze.EndSquare.XPos;
                        c1 = maze.StartSquare.YPos;
                        c2 = maze.EndSquare.YPos;
                        break;
                    case WallPosition.WP_W:
                        d1 = maze.EndSquare.XPos;
                        d2 = maze.XSize - 1 - maze.StartSquare.XPos;
                        c1 = maze.EndSquare.YPos;
                        c2 = maze.StartSquare.YPos;
                        break;
                    case WallPosition.WP_S:
                        d1 = maze.StartSquare.YPos;
                        d2 = maze.YSize - 1 - maze.EndSquare.YPos;
                        c1 = maze.StartSquare.XPos;
                        c2 = maze.EndSquare.XPos;
                        break;
                    case WallPosition.WP_N:
                        d1 = maze.EndSquare.YPos;
                        d2 = maze.YSize - 1 - maze.StartSquare.YPos;
                        c1 = maze.EndSquare.XPos;
                        c2 = maze.StartSquare.XPos;
                        break;
                    default:
                        d1 = d2 = c1 = c2 = -1;
                        break;
                }

                nCode *= (dimensionsObj.MaxBorderDistance + 1);
                nCode += d1;

                nCode *= (dimensionsObj.MaxBorderDistance + 1);
                nCode += d2;

                nCode *= (dimensionsObj.MaxXSize + 1);
                nCode += c1;

                nCode *= (dimensionsObj.MaxXSize + 1);
                nCode += c2;

                nCode *= (int)WallPosition.WP_NUM;
                nCode += (int)maze.Direction;
            }

            // Encode maze dimension.

            nCode *= (dimensionsObj.MaxYSize - dimensionsObj.MinSize + 1);
            nCode += (maze.YSize - dimensionsObj.MinSize);

            nCode *= (dimensionsObj.MaxXSize - dimensionsObj.MinSize + 1);
            nCode += (maze.XSize - dimensionsObj.MinSize);

            // Encode initial seed.

            nCode *= SeedLimit;
            nCode += maze.Seed;

            #endregion

            // v0: The resulting nCode is less than 26^12.  See SWA.Ariadne.Model.Tests unit tests.
            // v1: The resulting nCode is less than 36^6.  See SWA.Ariadne.Model.Tests unit tests.

            #region Convert the numeric code into a character code (base 26)

            StringBuilder result = new StringBuilder(7);

            for (int p = CodeLength; p-- > 0; )
            {
                int digit = (int)(nCode % CodeDigitRange);
                nCode /= CodeDigitRange;
                char c = CodeDigits[digit];
                result.Insert(0, c);
            }

            switch (codeVersion)
            {
                case 0:
                    result.Insert(8, this.Separator);
                    result.Insert(4, this.Separator);
                    break;
                case 1:
                    result.Insert(3, this.Separator);
                    break;
                case 2:
                    result.Insert(3, this.Separator);
                    break;
            }

            #endregion

            return result.ToString();
        }

        #endregion

        #region Decoding methods

        /// <summary>
        /// Determine the encoding schema version of the given code string.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int GetCodeVersion(string code)
        {
            string separator = "?";
            if (code.Contains(".")) separator = ".";
            if (code.Contains("-")) separator = "-";

            char[] a = code.Replace(separator.ToString(), "").ToCharArray();
            int result = -1;
            int nVersions = instance.Length;

            for (int v = 0; v < nVersions; v++)
            {
                if (Instance(v).CodeLength == a.Length && Instance(v).Separator == separator)
                {
                    result = v;
                }
            }

            if (result < 0)
            {
                StringBuilder s = new StringBuilder();
                s.Append(Instance(0).CodeLength);
                for (int v = 1; v < nVersions; v++)
                {
                    s.Append(Environment.NewLine);
                    s.Append(Instance(v).Description);
                }
                throw new ArgumentOutOfRangeException("code", code, "Can only be one of the following:" + s);
            }

            return result;
        }

        /// <summary>
        /// Extract the Maze parameters from an encoded string.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="seed"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <exception cref="ArgumentOutOfRangeException">decoded parameters are invalid</exception>
        public void Decode(string code
            , out int seed
            , out int xSize, out int ySize
            )
        {
            long nCode = 0;
            MazeDimensions dimensionsObj = MazeDimensions.Instance(codeVersion);

            #region Convert the character code (base 26) into a numeric code

            char[] a = code.Replace(this.Separator,"").ToCharArray();

            if (!(a.Length == CodeLength))
            {
                throw new ArgumentOutOfRangeException("code", code,
                    "length = " + a.Length + " / Must be " + Description);
            }

            for (int p = 0; p < a.Length; p++)
            {
                int digit = 0;
                digit = CodeDigits.ToString().IndexOf(a[p]);

                if (digit < 0)
                {
                    throw new ArgumentOutOfRangeException("code", code,
                        "c = '" + a[p] + "' / Must be " + Description);
                }

                nCode *= CodeDigitRange;
                nCode += digit;
            }

            #endregion

            #region Decode items in the code in reverse order of encoding

            long nCodeOriginal = nCode; // for debugging

            long itemRange;

            itemRange = SeedLimit;
            seed = (int)(nCode % itemRange);
            nCode /= itemRange;

            itemRange = dimensionsObj.MaxXSize - dimensionsObj.MinSize + 1;
            xSize = (int)(nCode % itemRange) + dimensionsObj.MinSize;
            nCode /= itemRange;

            itemRange = dimensionsObj.MaxYSize - dimensionsObj.MinSize + 1;
            ySize = (int)(nCode % itemRange) + dimensionsObj.MinSize;
            nCode /= itemRange;

            if (codeVersion == 0)
            {
                #region Decoding of obsolete Version 0 parameters

                WallPosition direction;
                int xStart, yStart;
                int xEnd, yEnd;
                int d1, d2, c1, c2;

                itemRange = (int)WallPosition.WP_NUM;
                direction = (WallPosition)(nCode % itemRange);
                nCode /= itemRange;

                itemRange = dimensionsObj.MaxXSize + 1;
                c2 = (int)(nCode % itemRange);
                nCode /= itemRange;

                itemRange = dimensionsObj.MaxXSize + 1;
                c1 = (int)(nCode % itemRange);
                nCode /= itemRange;

                itemRange = dimensionsObj.MaxBorderDistance + 1;
                d2 = (int)(nCode % itemRange);
                nCode /= itemRange;

                itemRange = dimensionsObj.MaxBorderDistance + 1;
                d1 = (int)(nCode % itemRange);
                nCode /= itemRange;

                switch (direction)
                {
                    case WallPosition.WP_E:
                        xStart = d1;
                        xEnd = xSize - 1 - d2;
                        yStart = c1;
                        yEnd = c2;
                        break;
                    case WallPosition.WP_W:
                        xEnd = d1;
                        xStart = xSize - 1 - d2;
                        yEnd = c1;
                        yStart = c2;
                        break;
                    case WallPosition.WP_S:
                        yStart = d1;
                        yEnd = ySize - 1 - d2;
                        xStart = c1;
                        xEnd = c2;
                        break;
                    case WallPosition.WP_N:
                        yEnd = d1;
                        yStart = ySize - 1 - d2;
                        xEnd = c1;
                        xStart = c2;
                        break;
                    default:
                        xStart = yStart = xEnd = yEnd = -1;
                        break;
                }

                #endregion
            }

            #endregion

            #region Verify the validity of the decoded items

            if (!(nCode == 0))
            {
                throw new ArgumentOutOfRangeException("remainder(code)", nCode, "Must be a zero.");
            }
            ValidateCodeItemRange("seed", seed, 0, SeedLimit-1);
            ValidateCodeItemRange("xSize", xSize, dimensionsObj.MinSize, dimensionsObj.MaxXSize);
            ValidateCodeItemRange("ySize", ySize, dimensionsObj.MinSize, dimensionsObj.MaxYSize);

            #endregion
        }

        private static void ValidateCodeItemRange(string item, int value, int min, int max)
        {
            if (!(min <= value && value <= max))
            {
                throw new ArgumentOutOfRangeException(item + " of code", value, "Must be between " + min.ToString() + " and " + max.ToString() + ".");
            }
        }

        #endregion

        #region Singleton behavior

        /// <summary>
        /// Returns a singleton MazeCode instance.
        /// </summary>
        /// <returns></returns>
        public static MazeCode Instance(int version)
        {
            if (instance[version] == null)
            {
                instance[version] = new MazeCode(version);
            }
            return instance[version];
        }

        private static MazeCode[] instance = new MazeCode[MaxCodeVersion+1];

        #endregion
    }
}
