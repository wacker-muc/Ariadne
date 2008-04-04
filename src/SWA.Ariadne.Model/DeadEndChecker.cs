using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Model
{
    public class DeadEndChecker
    {
        #region Member variables and Properties

        /// <summary>
        /// This extension allows an estimation if a square is reachable from the end square.
        /// A "trajectory" is like a path through unvisited squares regardless of closed walls.
        /// If there is no such trajectory (i.e. if every path would have to pass through visited
        /// squares), we may conclude that the square is not reachable and therefore useless.
        /// </summary>
        private class MazeSquareExtension
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public MazeSquareExtension()
            {
            }

            /// <summary>
            /// The MazeSquare which is extended.
            /// </summary>
            public MazeSquare extendedSquare;

            /// <summary>
            /// When true, this square is not reachable from the end square.
            /// </summary>
            public bool isDeadEnd;

            /// <summary>
            /// Length of the shortest trajectory to the end square.
            /// </summary>
            public int trajectoryDistance;

            /// <summary>
            /// All alive neighbors of this square.
            /// Note: At least one of them must have a smaller trajectoryDistance.
            ///       (unless this is the end square itself and trajectoryDistance is 0)
            /// </summary>
            public List<MazeSquareExtension> neighbors;

            public override string ToString()
            {
                string result = string.Format("({0}/{1}): d={2}", extendedSquare.XPos, extendedSquare.YPos, trajectoryDistance);
                result += ", n=[";
                foreach (MazeSquareExtension sqe in neighbors)
                {
                    result += string.Format("({0}/{1}), ", sqe.extendedSquare.XPos, sqe.extendedSquare.YPos);
                }
                if (neighbors.Count > 0)
                {
                    result = result.Remove(result.Length - 2);
                }
                result += "]";
                if (isDeadEnd)
                {
                    result += " (dead)";
                }
                return result;
            }
        }
        /// <summary>
        /// For every MazeSquare: an extension that helps identifying dead ends.
        /// </summary>
        private MazeSquareExtension[,] mazeExtension;

        public int Distance(int x, int y)
        {
            if (mazeExtension[x, y].isDeadEnd)
            {
                return -1;
            }
            else
            {
                return mazeExtension[x, y].trajectoryDistance;
            }
        }

        public bool IsDead(MazeSquare sq)
        {
            return mazeExtension[sq.XPos, sq.YPos].isDeadEnd;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a DeadEndChecker, based on the given Maze.
        /// </summary>
        /// <param name="maze"></param>
        public DeadEndChecker(Maze maze)
        {
            this.mazeExtension = new MazeSquareExtension[maze.XSize, maze.YSize];
            #region Create mazeExtension[i, j]
            for (int i = 0; i < maze.XSize; i++)
            {
                for (int j = 0; j < maze.YSize; j++)
                {
                    mazeExtension[i, j] = new MazeSquareExtension();
                }
            }
            #endregion

            // Set up the mazeExtension
            InitializeMazeExtension(maze);
            InitializeTrajectoryDistances(maze);
        }

        /// <summary>
        /// Initialize all squares in the mazeExtension (everything but trajectoryDistance).
        /// </summary>
        /// <param name="maze"></param>
        private void InitializeMazeExtension(Maze maze)
        {
            for (int i = 0; i < maze.XSize; i++)
            {
                for (int j = 0; j < maze.YSize; j++)
                {
                    MazeSquare sq = maze[i, j];
                    MazeSquareExtension sqe = mazeExtension[i, j];

                    // extendedSquare:
                    sqe.extendedSquare = sq;

                    if (sq.isReserved)
                    {
                        // isDeadEnd:
                        sqe.isDeadEnd = true;
                    }
                    else
                    {
                        // neighbors:
                        sqe.neighbors = new List<MazeSquareExtension>(4);

                        for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
                        {
                            MazeSquare sq2 = sq.NeighborSquare(wp);

                            if (sq2 != null && !sq2.isReserved)
                            {
                                sqe.neighbors.Add(mazeExtension[sq2.XPos, sq2.YPos]);
                            }
                        }

                        // Negative values mean: not initialized
                        sqe.trajectoryDistance = int.MinValue;
                    }
                }
            }

            // Mark start square as visited.
            mazeExtension[maze.StartSquare.XPos, maze.StartSquare.YPos].isDeadEnd = true;
        }

        /// <summary>
        /// Initialize trajectoryDistance of all squares in the mazeExtension.
        /// </summary>
        /// <param name="maze"></param>
        private void InitializeTrajectoryDistances(Maze maze)
        {
            MazeSquareExtension sqe = mazeExtension[maze.EndSquare.XPos, maze.EndSquare.YPos];
            sqe.trajectoryDistance = 0;

            List<MazeSquareExtension> list = new List<MazeSquareExtension>();
            list.Add(sqe);

            while (list.Count > 0)
            {
                // Take the first item from the list.
                MazeSquareExtension sqe1 = list[0];
                list.RemoveAt(0);

                if (sqe1.trajectoryDistance <= 0)
                {
                    for (int i = 0; i < sqe1.neighbors.Count; i++)
                    {
                        MazeSquareExtension sqe2 = sqe1.neighbors[i];

                        // Set the potential (negative) trajectory distance:
                        //  * sqe2's distance is one greater than sqe1's
                        //  * but only if this is better than the current potential
                        if (sqe1.trajectoryDistance - 1 > sqe2.trajectoryDistance)
                        // && sqe2.trajectoryDistance < 0 -- 
                        {
                            // When first encountered: add sqe2 to the list.
                            if (sqe2.trajectoryDistance == int.MinValue)
                            {
                                list.Add(sqe2);
                                sqe2.trajectoryDistance = sqe1.trajectoryDistance - 1;
                            }
                            else
                            {
                                sqe2.trajectoryDistance = sqe1.trajectoryDistance - 1;
                                list.Remove(sqe2);
                                #region Find a new position p in the list
                                int p;
                                for (p = 0; p < list.Count; p++)
                                {
                                    // The potential distance must be at least as good...
                                    if (sqe2.trajectoryDistance >= list[p].trajectoryDistance)
                                    {
                                        break;
                                    }
                                }
                                #endregion
                                list.Insert(p, sqe2);
                            }
                        }
                    }

                    // Convert a potential distance to an actual distance.
                    sqe1.trajectoryDistance *= -1;
                }
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Registers the given square as visited.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>a list of dead end squares</returns>
        public List<MazeSquare> Visit(MazeSquare sq)
        {
            List<MazeSquare> result = new List<MazeSquare>();
            MazeSquareExtension sqe = mazeExtension[sq.XPos, sq.YPos];

            sqe.isDeadEnd = true;

            if (sqe.trajectoryDistance == 0)
            {
                // This is the end square.  No need to kill any more squares...
                return result;
            }

            // Re-calculate trajectories of neighbors if they passed through the visited square.
            for (int i = 0; i < sqe.neighbors.Count; i++)
            {
                MazeSquareExtension sqe2 = sqe.neighbors[i];

                if (sqe2.trajectoryDistance > sqe.trajectoryDistance && !sqe2.isDeadEnd)
                {
                    List<MazeSquareExtension> theseDeadSquares = CalculateTrajectory(sqe2);
                    if (theseDeadSquares != null)
                    {
                        for (int j = 0; j < theseDeadSquares.Count; j++)
                        {
                            MazeSquareExtension deadSqe = theseDeadSquares[j];
                            deadSqe.isDeadEnd = true;
                            result.Add(deadSqe.extendedSquare);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Adjust trajectory distances.  If no new trajectory is found, collect dead end squares.
        /// </summary>
        /// <param name="sqe"></param>
        /// <returns>a list of dead end squares</returns>
        private List<MazeSquareExtension> CalculateTrajectory(MazeSquareExtension sqe)
        {
            // We need to find a square whose distance is smaller than this square's distance.
            int curDistance = sqe.trajectoryDistance;
            List<MazeSquareExtension> trajectoryBases = new List<MazeSquareExtension>();

            // We'll collect the squares surrounding sqe in a List.
            // If no new trajectory is found, that is the result: a list of dead end squares.
            List<MazeSquareExtension> result = new List<MazeSquareExtension>();
            sqe.trajectoryDistance *= -1;
            result.Add(sqe);

            for (int i = 0; i < result.Count; i++)
            {
                MazeSquareExtension sqe1 = result[i];
                for (int j = sqe1.neighbors.Count; j-- > 0; )
                {
                    MazeSquareExtension sqe2 = sqe1.neighbors[j];
                    if (sqe2.isDeadEnd)
                    {
                        // Dead squares may be discarded.
                        sqe1.neighbors.RemoveAt(j);
                        continue;
                    }
                    else if (sqe2.trajectoryDistance < 0)
                    {
                        // Marked squares have already been collected.
                        continue;
                    }
                    else if (sqe2.trajectoryDistance <= curDistance)
                    {
                        // This is the base of a new trajectory.
                        trajectoryBases.Add(sqe2);
                    }
                    else
                    {
                        // Mark this square as visited.
                        sqe2.trajectoryDistance *= -1;
                        result.Add(sqe2);
                    }
                }
            }

            if (trajectoryBases.Count > 0)
            {
                ReviveNeighbors(trajectoryBases);
                result.Clear();
            }

            return result;
        }

        /// <summary>
        /// Assign valid (positve) trajectory distances to all marked neighbors of the given list.
        /// </summary>
        /// <param name="list">A list of squares with equal (positive) trajectoryDistance.</param>
        private void ReviveNeighbors(List<MazeSquareExtension> list)
        {
            // Additional items may be added to the list.
            // The list is ordered by increasing trajectoryDistance.

            for (int i = 0; i < list.Count; i++)
            {
                MazeSquareExtension sqe1 = list[i];
                for (int j = sqe1.neighbors.Count; j-- > 0; )
                {
                    MazeSquareExtension sqe2 = sqe1.neighbors[j];
                    if (sqe2.trajectoryDistance >= 0)
                    {
                        // This square's distance needs not be adjusted.
                        continue;
                    }
                    else
                    {
                        sqe2.trajectoryDistance = sqe1.trajectoryDistance + 1;
                        list.Add(sqe2);
                    }
                }
            }
        }

        #endregion
    }
}
