using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Outlines;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// Base class for a set of strategies that construct non-uniform maze shapes.
    /// </summary>
    internal abstract class IrregularMazeShape
    {
        #region Member variables.

        internal enum Kind
        {
            Mixed = -1,
            Neutral = 0,
            Straights,
            Circles,
            Zigzags,
        }

        protected readonly Kind kind;

        #endregion

        #region Constructor

        protected IrregularMazeShape(Kind kind)
        {
            this.kind = kind;
        }

        #endregion

        #region Abstract methods, implemented by derived classes.

        /// <summary>
        /// Returns an array four boolean values, indexed by the MazeSquare.WallPosition constants.
        /// The value is true when that wall is a preferred direction for paths from the given square.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public abstract bool[] PreferredDirections(MazeSquare sq);

        #endregion

        #region Static methods.

        /// <summary>
        /// Returns a randomly chosen instance.
        /// Some patterns are "simple", applied to the whole maze area.
        /// Some patterns are "mixed", with two different patterns on the inside/outside of an OutlineShape.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static IrregularMazeShape RandomInstance(Random r, Maze maze)
        {
            if (maze.OutlineShape != null && r.Next(100) < 10)
            {
                // Build a mixed instance, using the OutlineShape laid into the maze.

                if (r.Next(100) < 50)
                {
                    // two different patterns inside and outside of the OutlineShape
                    return MixedInstance(r, maze, maze.OutlineShape);
                }
                else if (r.Next(100) < 66)
                {
                    // the outside of the OutlineShape is regular
                    return ConfinedInstance(r, maze, maze.OutlineShape, true);
                }
                else
                {
                    // the inside of the OutlineShape is regular
                    return ConfinedInstance(r, maze, maze.OutlineShape, false);
                }
            }
            else if (maze.OutlineShape == null && r.Next(100) < 20)
            {
                // Build a mixed instance, using a new OutlineShape.
                
                OutlineShape outline = OutlineShape.RandomInstance(r, maze.XSize, maze.YSize, 0.3, 0.9);

                if (r.Next(100) < 20)
                {
                    // two different patterns inside and outside of the OutlineShape
                    return MixedInstance(r, maze, outline);
                }
                else if (r.Next(100) < 50)
                {
                    // the outside of the OutlineShape is regular
                    return ConfinedInstance(r, maze, outline, true);
                }
                else
                {
                    // the inside of the OutlineShape is regular
                    return ConfinedInstance(r, maze, outline, false);
                }
            }
            else
            {
                return SimpleInstance(r, maze);
            }
        }
            
        /// <summary>
        /// Returns a randomly chosen (simple) instance.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static IrregularMazeShape SimpleInstance(Random r, Maze maze)
        {
            int choice = r.Next(20);

            //choice = 17 + r.Next(3);

            // For PreferPathsRelativeToReferenceSquare: number of x and y partitions.
            int p = (choice % 2 == 0 ? 1 : 4), q = (choice % 2 == 0 ? 1 : 3);

            switch (choice)
            {
                default:
                case 0:
                    return new PreferStraightPaths();
                case 1:
                    return new PreferAngledPaths();
                case 2:
                    return new PreferUndulatingPaths();

                case 3:
                case 4:
                    // Circles
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, true, true, true);
                case 5:
                case 6:
                    // Squares
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, true, false, true);
                case 7:
                case 8:
                    // Squares with indeted corners
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, true, false, false);
                case 9:
                case 10:
                    // Four quadrants
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, false, false, true);
                case 11:
                case 12:
                    // Radial lines
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, false, true, true);

                case 13:
                    // Off-Center circles
                    return new PreferPathsRelativeToReferenceSquare(maze, 1, 1, true, true, true, 1.5, r);
                case 14:
                case 15:
                    // Off-center squares
                    return new PreferPathsRelativeToReferenceSquare(maze, p, q, true, false, true, 1.2, r);
                case 16:
                    // Off-center squares with indeted corners
                    return new PreferPathsRelativeToReferenceSquare(maze, 1, 1, true, false, false, 1.2, r);

                case 17:
                    // Scattered circles
                    return new PreferPathsRelativeToReferenceSquare(maze, (maze.XSize * maze.YSize) / (20 * 20), true, true, true, r);
                case 18:
                    // Scattered squares
                    return new PreferPathsRelativeToReferenceSquare(maze, (maze.XSize * maze.YSize) / (16 * 16), true, false, true, r);
                case 19:
                    // Scattered four quadrants
                    return new PreferPathsRelativeToReferenceSquare(maze, (maze.XSize * maze.YSize) / (16 * 16), false, false, true, r);
            }
        }

        /// <summary>
        /// Returns a combination of two different patterns on the inside/outside of the given OutlineShape.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="maze"></param>
        /// <param name="outline"></param>
        /// <returns></returns>
        private static IrregularMazeShape MixedInstance(Random r, Maze maze, OutlineShape outline)
        {
            IrregularMazeShape inside = null, outside = null;
            do
            {
                inside = SimpleInstance(r, maze);
                outside = SimpleInstance(r, maze);
            } while (inside.kind <= Kind.Neutral
                || outside.kind <= Kind.Neutral
                || inside.kind == outside.kind
                || (inside.kind != Kind.Zigzags && outside.kind != Kind.Zigzags)
                );

            return new MixedIrregularMazeShape(outline, inside, outside);
        }

        /// <summary>
        /// Returns an irregular pattern that is only applied to the inside/outside of the given OutlineShape.
        /// The remainder is a regular pattern (without preferred directions).
        /// </summary>
        /// <param name="r"></param>
        /// <param name="maze"></param>
        /// <param name="outline"></param>
        /// <param name="confinedToInside">
        /// When true: only the inside is irregular.
        /// When false: only the outside is irregular.
        /// </param>
        /// <returns></returns>
        private static IrregularMazeShape ConfinedInstance(Random r, Maze maze, OutlineShape outline, bool confinedToInside)
        {
            IrregularMazeShape regular = new PreferNothing();
            IrregularMazeShape irregular = SimpleInstance(r, maze);

            if (confinedToInside)
            {
                return new MixedIrregularMazeShape(outline, irregular, regular);
            }
            else
            {
                return new MixedIrregularMazeShape(outline, regular, irregular);
            }
        }

        #endregion

        #region Internal derived classes, implementing different patterns.

        private class PreferNothing : IrregularMazeShape
        {
            public PreferNothing()
                : base(Kind.Neutral)
            {
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                bool[] result = new bool[4] { false, false, false, false };

                return result;
            }
        }

        /// <summary>
        /// Prefer walls opposite of already open walls.
        /// Results in long straight paths and rectangular spirals.
        /// </summary>
        private class PreferStraightPaths : IrregularMazeShape
        {
            public PreferStraightPaths()
                : base(Kind.Straights)
            {
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                bool[] result = new bool[4];

                for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
                {
                    result[(int)wp] = (sq.walls[(int)MazeSquare.OppositeWall(wp)] == MazeSquare.WallState.WS_OPEN);
                }

                return result;
            }
        }

        /// <summary>
        /// Reject walls opposite of already walls.
        /// Results in angled, undulating paths.
        /// <summary>
        private class PreferAngledPaths : IrregularMazeShape
        {
            public PreferAngledPaths()
                : base(Kind.Zigzags)
            {
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                bool[] result = new bool[4];

                for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
                {
                    result[(int)wp] = (sq.walls[(int)MazeSquare.OppositeWall(wp)] != MazeSquare.WallState.WS_OPEN);
                }

                return result;
            }
        }

        /// <summary>
        /// Prefer a certain neighbor in a 2x2 section of the square.
        /// Results in straight undulating paths (e.g. ENESENES...)
        /// </summary>
        private class PreferUndulatingPaths : IrregularMazeShape
        {
            public PreferUndulatingPaths()
                : base(Kind.Zigzags)
            {
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                bool[] result = new bool[4] { false, false, false, false };

                switch (1 * (sq.XPos % 2) + 2 * (sq.YPos % 2)) // 0..3
                {
                    case 0: // (0,0) -> (1,0)
                        result[(int)MazeSquare.WallPosition.WP_E] = true;
                        break;
                    case 1: // (1,0) -> (1,1)
                        result[(int)MazeSquare.WallPosition.WP_S] = true;
                        break;
                    case 2: // (0,1) -> (0,0)
                        result[(int)MazeSquare.WallPosition.WP_N] = true;
                        break;
                    case 3: // (1,1) -> (0,1)
                        result[(int)MazeSquare.WallPosition.WP_W] = true;
                        break;
                }

                return result;
            }
        }

        protected class PreferPathsRelativeToReferenceSquare : IrregularMazeShape
        {
            private int xSize, ySize;
            private int xPartitions, yPartitions;
            private bool approximateCircles; 
            private bool minimizeDistance; 
            private bool followDiagonals;

            /// <summary>
            /// When not null: 
            /// For every partition, the x and y offset to be applied when selecting the reference square.
            /// </summary>
            private int[,] xOffCenter, yOffCenter;

            /// <summary>
            /// When not null: 
            /// Instead of partitions, use the closest of these squares as a reference.
            /// </summary>
            private MazeSquare[] vertices;

            /// <summary>
            /// Constructor for regular grids with reference point at the center of a grid partition.
            /// </summary>
            /// <param name="maze"></param>
            /// <param name="xPartitions">Number of grid partitions in X direction.</param>
            /// <param name="yPartitions">Number of grid partitions in Y direction.</param>
            /// <param name="minimizeDistance">Selects concentric or radial shapes.</param>
            /// <param name="approximateCircles">Selects a round or square basic shape.</param>
            /// <param name="followDiagonals">Selects shape in the four corner octants.</param>
            public PreferPathsRelativeToReferenceSquare(Maze maze, int xPartitions, int yPartitions, bool minimizeDistance, bool approximateCircles, bool followDiagonals)
                : base(approximateCircles ? Kind.Circles : Kind.Straights)
            {
                this.xSize = maze.XSize;
                this.ySize = maze.YSize;
                this.xPartitions = xPartitions;
                this.yPartitions = yPartitions;
                this.approximateCircles = approximateCircles;
                this.minimizeDistance = minimizeDistance;
                this.followDiagonals = followDiagonals;
            }

            /// <summary>
            /// Constructor for grids with excentric reference points.
            /// </summary>
            /// <param name="maze"></param>
            /// <param name="xPartitions">Number of grid partitions in X direction.</param>
            /// <param name="yPartitions">Number of grid partitions in Y direction.</param>
            /// <param name="minimizeDistance">Selects concentric or radial shapes.</param>
            /// <param name="approximateCircles">Selects a round or square basic shape.</param>
            /// <param name="followDiagonals">Selects shape in the four corner octants.</param>
            /// <param name="offCenter">Shape's excentricity: 0.0 .. 1.0</param>
            /// <param name="r">Used for selecting the off-center distance.</param>
            public PreferPathsRelativeToReferenceSquare(Maze maze, int xPartitions, int yPartitions, bool minimizeDistance, bool approximateCircles, bool followDiagonals, double offCenter, Random r)
                : this(maze, xPartitions, yPartitions, minimizeDistance, approximateCircles, followDiagonals)
            {
                this.xOffCenter = new int[xPartitions, yPartitions];
                this.yOffCenter = new int[xPartitions, yPartitions];

                for (int i = 0; i < xPartitions; i++)
                {
                    for (int j = 0; j < yPartitions; j++)
                    {
                        xOffCenter[i, j] = (int)(offCenter * (r.NextDouble() - 0.5) * maze.XSize / xPartitions);
                        yOffCenter[i, j] = (int)(offCenter * (r.NextDouble() - 0.5) * maze.YSize / yPartitions);
                    }
                }
            }

            /// <summary>
            /// Constructor for irregular grids.
            /// The reference point's influence areas form a Voronoi cell pattern.
            /// </summary>
            /// <param name="maze"></param>
            /// <param name="nVertices">Number of randomly placed reference squares.</param>
            /// <param name="minimizeDistance">Selects concentric or radial shapes.</param>
            /// <param name="approximateCircles">Selects a round or square basic shape.</param>
            /// <param name="followDiagonals">Selects shape in the four corner octants.</param>
            /// <param name="r">Used for selecting the vertex coordinates.</param>
            public PreferPathsRelativeToReferenceSquare(Maze maze, int nVertices, bool minimizeDistance, bool approximateCircles, bool followDiagonals, Random r)
                : this(maze, 1, 1, minimizeDistance, approximateCircles, followDiagonals)
            {
                this.vertices = new MazeSquare[nVertices];

                for (int i = 0; i < nVertices; i++)
                {
                    int x = (int)((0.5 + 1.2 * r.NextDouble() - 0.6) * xSize);
                    int y = (int)((0.5 + 1.2 * r.NextDouble() - 0.6) * ySize);
                    vertices[i] = new MazeSquare(x, y);
                }
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                bool[] result = new bool[4] { false, false, false, false };

                #region Collect the distances of all neighbor squares (including diagonals) from the reference square.

                // Actually, this is the (absolute) difference between the distances of a neighbor square and sq itself.
                double[,] distances = new double[3, 3];

                // The distance of the current square to the reference square should be approximated.
                MazeSquare sq0 = this.ReferenceSquare(sq);
                double d0 = Maze.Distance(sq, sq0);
                // Rounding to the next integer will approximate circles in 1 unit steps.
                if (approximateCircles)
                {
                    d0 = Math.Round(d0);
                }

                for (int i = -1; i <= +1; i++)
                {
                    for (int j = -1; j <= +1; j++)
                    {
                        // Note: this[x,y] is not defined outside the maze size.
                        MazeSquare sq1 = new MazeSquare(sq.XPos + i, sq.YPos + j);
                        distances[i + 1, j + 1] = (minimizeDistance ? +1 : -1) * Math.Abs(Maze.Distance(sq1, sq0) - d0);
                    }
                }

                #endregion

                #region Find a range of distances including the current square and two of the neighbors.

                double d1 = double.MaxValue, d2 = double.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == 1 && j == 1)       // Discard the current square.
                        {
                            continue;
                        }
                        if (distances[i, j] >= d2)  // Discard distances greater than the current second best.
                        {
                            continue;
                        }
                        if (distances[i, j] >= d1)  // This is a new second best distance.
                        {
                            d2 = distances[i, j];
                        }
                        else                        // This is a new best distance.
                        {
                            d2 = d1;
                            d1 = distances[i, j];
                        }
                    }
                }

                #endregion

                #region Add the directions to the two best neighbors to the result.

                if (distances[1, 0] <= d2) // straight north
                {
                    result[(int)MazeSquare.WallPosition.WP_N] = true;
                }
                if (distances[0, 1] <= d2) // straight west
                {
                    result[(int)MazeSquare.WallPosition.WP_W] = true;
                }
                if (distances[1, 2] <= d2) // straight south
                {
                    result[(int)MazeSquare.WallPosition.WP_S] = true;
                }
                if (distances[2, 1] <= d2) // straight east
                {
                    result[(int)MazeSquare.WallPosition.WP_E] = true;
                }

                // For approximating a diagonal connection, we follow the closer path (unless this logic is inverted).
                if (distances[0, 0] <= d2) // north-west
                {
                    result[(int)((distances[1, 0] < distances[0, 1] == followDiagonals) ? MazeSquare.WallPosition.WP_N : MazeSquare.WallPosition.WP_W)] = true;
                }
                if (distances[2, 0] <= d2) // north-east
                {
                    result[(int)((distances[1, 0] < distances[2, 1] == followDiagonals) ? MazeSquare.WallPosition.WP_N : MazeSquare.WallPosition.WP_E)] = true;
                }
                if (distances[0, 2] <= d2) // south-west
                {
                    result[(int)((distances[1, 2] < distances[0, 1] == followDiagonals) ? MazeSquare.WallPosition.WP_S : MazeSquare.WallPosition.WP_W)] = true;
                }
                if (distances[2, 2] <= d2) // south-east
                {
                    result[(int)((distances[1, 2] < distances[2, 1] == followDiagonals) ? MazeSquare.WallPosition.WP_S : MazeSquare.WallPosition.WP_E)] = true;
                }

                #endregion
                return result;
            }

            /// <summary>
            /// Returns the center square of the given sqaure's partition.
            /// </summary>
            /// <param name="sq"></param>
            /// <returns></returns>
            private MazeSquare ReferenceSquare(MazeSquare sq)
            {
                // x and y index of the current square's partition
                int px = sq.XPos * xPartitions / xSize, py = sq.YPos * yPartitions / ySize;

                // x and y coordinates of the selected partition's center square
                int xCenter = (2 * px + 1) * xSize / (2 * xPartitions);
                int yCenter = (2 * py + 1) * ySize / (2 * yPartitions);

                // Apply this partition's offset.
                if (xOffCenter != null && yOffCenter != null)
                {
                    xCenter += xOffCenter[px, py];
                    yCenter += yOffCenter[px, py];
                }

                // If defined: Find the vertex closest to the given square.
                if (vertices != null)
                {
                    MazeSquare closestVertex = vertices[0];
                    double closestDistance = Maze.Distance(sq, closestVertex);

                    for (int i = 1; i < vertices.Length; i++)
                    {
                        double d = Maze.Distance(sq, vertices[i]);
                        if (d < closestDistance)
                        {
                            closestVertex = vertices[i];
                            closestDistance = d;
                        }
                    }

                    xCenter = closestVertex.XPos;
                    yCenter = closestVertex.YPos;
                }

                return new MazeSquare(xCenter, yCenter);
            }
        }

        protected class MixedIrregularMazeShape : IrregularMazeShape
        {
            #region Member variables

            private readonly OutlineShape outline;
            private readonly IrregularMazeShape inside;
            private readonly IrregularMazeShape outside;

            #endregion

            public MixedIrregularMazeShape(OutlineShape outline, IrregularMazeShape inside, IrregularMazeShape outside)
                : base(Kind.Mixed)
            {
                this.outline = outline;
                this.inside = inside;
                this.outside = outside;
            }

            public override bool[] PreferredDirections(MazeSquare sq)
            {
                if (outline[sq.XPos, sq.YPos] == true)
                {
                    return inside.PreferredDirections(sq);
                }
                else
                {
                    return outside.PreferredDirections(sq);
                }
            }
        }

        #endregion
    }
}
