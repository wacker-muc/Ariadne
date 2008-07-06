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
        public readonly int MinSize = 4;

        /// <summary>
        /// Maximum width: number of squares.
        /// </summary>
        public int MaxXSize
        {
            get
            {
                return MinSize + xRange;
            }
        }
        private readonly int xRange = 337; // expected value, manually calculated

        /// <summary>
        /// Maximum height: number of squares.
        /// </summary>
        public int MaxYSize
        {
            get
            {
                return MinSize + yRange;
            }
        }
        private readonly int yRange = 251; // expected value, manually calculated

        /// <summary>
        /// Maximum distance of start/end point from border.
        /// </summary>
        public readonly int MaxBorderDistance = 16;

        private MazeDimensions()
        {
            this.CalculateDimensions(out this.xRange, out this.yRange);
        }

        /// <summary>
        /// Calculate maximum x and y dimensions, based on the desired Maze.Code length.
        /// </summary>
        protected void CalculateDimensions(out int xRange, out int yRange)
        {
            MazeCode codeObj = MazeCode.Instance();
            double codeLimit = Math.Pow(codeObj.CodeDigitRange, codeObj.CodeLength);

            if (codeLimit > long.MaxValue)
            {
                throw new Exception("Maze.Code is too large to be represented as a 64 bit integer");
            }

            codeLimit /= codeObj.SeedLimit;
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

        /// <summary>
        /// Returns a singleton MazeDimensions instance.
        /// </summary>
        /// <returns></returns>
        public static MazeDimensions Instance()
        {
            if (instance == null)
            {
                instance = new MazeDimensions();
            }
            return instance;
        }

        private static MazeDimensions instance = null;
    }
}
