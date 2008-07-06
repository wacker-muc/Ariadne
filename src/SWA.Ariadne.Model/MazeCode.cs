using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    public class MazeCode
    {
        #region Member variables

        /// <summary>
        /// Maximum initial seed value: 2^13-1.
        /// This value and the other Max... values are chosen so that the maze Code may be represented with 12 characters.
        /// </summary>
        public readonly int SeedLimit = 8192;

        public readonly int CodeLength = 12;
        public readonly int CodeDigitRange = 26; // 'A' .. 'Z'

        #endregion

        #region Constructor

        MazeCode()
        {
        }

        #endregion

        /// <summary>
        /// A string of twelve characters (A..Z) that encodes the maze parameters.
        /// This code can be used to construct an identical maze.
        /// </summary>
        public string Code(Maze maze)
        {
            long nCode = 0;

            #region Encode the relevant parameters into a numeric code

            // Items are encoded in reverse order of decoding.
            // Some items must be encoded before others if decoding requires them.

            // Encode the start and end points.
            // Instead of the direct coordinates we will use the following information:
            // * travel direction
            // * distance from the border (instead of coordinate)
            // * other coordinate
            // The scaling factor is always MazeDimensions.MaxXSize, as it is greater than MazeDimensions.MaxYSize.

            int d1, d2, c1, c2;

            switch (maze.Direction)
            {
                case MazeSquare.WallPosition.WP_E:
                    d1 = maze.StartSquare.XPos;
                    d2 = maze.XSize - 1 - maze.EndSquare.XPos;
                    c1 = maze.StartSquare.YPos;
                    c2 = maze.EndSquare.YPos;
                    break;
                case MazeSquare.WallPosition.WP_W:
                    d1 = maze.EndSquare.XPos;
                    d2 = maze.XSize - 1 - maze.StartSquare.XPos;
                    c1 = maze.EndSquare.YPos;
                    c2 = maze.StartSquare.YPos;
                    break;
                case MazeSquare.WallPosition.WP_S:
                    d1 = maze.StartSquare.YPos;
                    d2 = maze.YSize - 1 - maze.EndSquare.YPos;
                    c1 = maze.StartSquare.XPos;
                    c2 = maze.EndSquare.XPos;
                    break;
                case MazeSquare.WallPosition.WP_N:
                    d1 = maze.EndSquare.YPos;
                    d2 = maze.YSize - 1 - maze.StartSquare.YPos;
                    c1 = maze.EndSquare.XPos;
                    c2 = maze.StartSquare.XPos;
                    break;
                default:
                    d1 = d2 = c1 = c2 = -1;
                    break;
            }

            nCode *= (maze.DimensionsObj.MaxBorderDistance + 1);
            nCode += d1;

            nCode *= (maze.DimensionsObj.MaxBorderDistance + 1);
            nCode += d2;

            nCode *= (maze.DimensionsObj.MaxXSize + 1);
            nCode += c1;

            nCode *= (maze.DimensionsObj.MaxXSize + 1);
            nCode += c2;

            nCode *= MazeSquare.WP_NUM;
            nCode += (int)maze.Direction;

            // Encode maze dimension.

            nCode *= (maze.DimensionsObj.MaxYSize - maze.DimensionsObj.MinSize + 1);
            nCode += (maze.YSize - maze.DimensionsObj.MinSize);

            nCode *= (maze.DimensionsObj.MaxXSize - maze.DimensionsObj.MinSize + 1);
            nCode += (maze.XSize - maze.DimensionsObj.MinSize);

            // Encode initial seed.

            nCode *= SeedLimit;
            nCode += maze.Seed;

            #endregion

            // The resulting nCode is less than 26^12.  See SWA.Ariadne.Model.Tests unit tests.

            #region Convert the numeric code into a character code (base 26)

            StringBuilder result = new StringBuilder(7);

            for (int p = CodeLength; p-- > 0; )
            {
                int digit = (int)(nCode % CodeDigitRange);
                nCode /= CodeDigitRange;
                result.Insert(0, (char)(digit + 'A'));
            }

            result.Insert(8, '.');
            result.Insert(4, '.');

            #endregion

            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="seed"></param>
        /// <param name="maze.XSize"></param>
        /// <param name="maze.YSize"></param>
        /// <param name="direction"></param>
        /// <param name="maze.StartSquare.XPos"></param>
        /// <param name="maze.StartSquare.YPos"></param>
        /// <param name="maze.EndSquare.XPos"></param>
        /// <param name="maze.EndSquare.YPos"></param>
        /// <exception cref="ArgumentOutOfRangeException">decoded parameters are invalid</exception>
        public void Decode(string code
            , out int seed
            , out int xSize, out int ySize
            , out MazeSquare.WallPosition direction
            , out int xStart, out int yStart
            , out int xEnd, out int yEnd
            )
        {
            long nCode = 0;
            MazeDimensions dimensionsObj = MazeDimensions.Instance();

            #region Convert the character code (base 26) into a numeric code

            char[] a = code.Replace(".","").ToCharArray();

            if (!(a.Length == CodeLength))
            {
                throw new ArgumentOutOfRangeException("code", code, "Must be " + CodeLength.ToString() + " characters.");
            }

            for (int p = 0; p < a.Length; p++)
            {
                int digit = a[p] - 'A';
                if (!(0 <= digit && digit < CodeDigitRange))
                {
                    throw new ArgumentOutOfRangeException("code", code, "Allowed characters are 'A'..'Z' only.");
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

            int d1, d2, c1, c2;

            itemRange = MazeSquare.WP_NUM;
            direction = (MazeSquare.WallPosition)(nCode % itemRange);
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
                case MazeSquare.WallPosition.WP_E:
                    xStart = d1;
                    xEnd = xSize - 1 - d2;
                    yStart = c1;
                    yEnd = c2;
                    break;
                case MazeSquare.WallPosition.WP_W:
                    xEnd = d1;
                    xStart = xSize - 1 - d2;
                    yEnd = c1;
                    yStart = c2;
                    break;
                case MazeSquare.WallPosition.WP_S:
                    yStart = d1;
                    yEnd = ySize - 1 - d2;
                    xStart = c1;
                    xEnd = c2;
                    break;
                case MazeSquare.WallPosition.WP_N:
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

            #region Verify the validity of the decoded items

            if (!(nCode == 0))
            {
                throw new ArgumentOutOfRangeException("remainder(code)", seed, "Must be a zero.");
            }
            ValidateCodeItemRange("seed", seed, 0, SeedLimit-1);
            ValidateCodeItemRange("xSize", xSize, dimensionsObj.MinSize, dimensionsObj.MaxXSize);
            ValidateCodeItemRange("ySize", ySize, dimensionsObj.MinSize, dimensionsObj.MaxYSize);
            ValidateCodeItemRange("xStart", xStart, 0, xSize-1);
            ValidateCodeItemRange("yStart", yStart, 0, ySize-1);
            ValidateCodeItemRange("xEnd", xEnd, 0, xSize-1);
            ValidateCodeItemRange("yEnd", yEnd, 0, ySize-1);

            #endregion
        }

        private static void ValidateCodeItemRange(string item, int value, int min, int max)
        {
            if (!(min <= value && value <= max))
            {
                throw new ArgumentOutOfRangeException(item + "(code)", value, "Must be between " + min.ToString() + " and " + max.ToString() + ".");
            }
        }

        /// <summary>
        /// Returns a singleton MazeCode instance.
        /// </summary>
        /// <returns></returns>
        public static MazeCode Instance()
        {
            if (instance == null)
            {
                instance = new MazeCode();
            }
            return instance;
        }

        private static MazeCode instance = null;
    }
}
