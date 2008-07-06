using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// Provides Maze dimension limits based on the desired Maze.Code length.
    /// </summary>
    public class MazeDimensions
    {
        private const double XYRatio = (4.0/3.0); // e.g. 1024x768

        /// <summary>
        /// Minimum width or height: number of squares.
        /// </summary>
        public static readonly int MinSize = 4;

        /// <summary>
        /// Maximum width: number of squares.
        /// </summary>
        public static int MaxXSize
        {
            get
            {
                return MinSize + xRange;
            }
        }
        private static readonly int xRange = 337; // expected value, manually calculated

        /// <summary>
        /// Maximum height: number of squares.
        /// </summary>
        public static int MaxYSize
        {
            get
            {
                return MinSize + yRange;
            }
        }
        private static readonly int yRange = 251; // expected value, manually calculated

        /// <summary>
        /// Maximum distance of start/end point from border.
        /// </summary>
        public static readonly int MaxBorderDistance = 16;

        /// <summary>
        /// Calculate maximum x and y dimensions, based on the desired Maze.Code length.
        /// </summary>
        static MazeDimensions()
        {
            double codeLimit = Math.Pow(MazeCode.CodeDigitRange, MazeCode.CodeLength);

            if (codeLimit > long.MaxValue)
            {
                throw new Exception("Maze.Code is too large to be represented as a 64 bit integer");
            }

            codeLimit /= MazeCode.SeedLimit;
            //           (MaxXSize - MinSize + 1)
            //           (MaxYSize - MinSize + 1)
            codeLimit /= MazeSquare.WP_NUM;
            codeLimit /= (MaxBorderDistance + 1);
            codeLimit /= (MaxBorderDistance + 1);
            //           (MaxXSize + 1)
            //           (MaxXSize + 1)

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

            double x = Math.Truncate(Math.Pow(codeLimit * XYRatio, 0.25));

            while ((x - MinSize) * (x / XYRatio - MinSize) * (x) * (x) < codeLimit)
            {
                x = x + 1;
            }

            /* Now, x is 1 greater than acceptable, i.e.
             *          x-1  =  MaxXSize + 1  =  MinSize + xRange + 1
             *          MinSize + yRange  =  MaxYSize  =  MaxXSize / XYRatio
             */
            xRange = (int)(x - MinSize - 2);
            yRange = (int)(MaxXSize / XYRatio - MinSize); // Note: MaxXSize is valid after xRange has been assigned
        }
    }
}
