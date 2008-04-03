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
        /// Returns a list of dead end squares.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public void Visit(MazeSquare sq, List<MazeSquare> deadSquares)
        {
            MazeSquareExtension sqe = mazeExtension[sq.XPos, sq.YPos];

            sqe.isDeadEnd = true;

            // Re-calculate trajectories of neighbors if they passed through the visited square.
            for (int i = 0; i < sqe.neighbors.Count; i++)
            {
                MazeSquareExtension sqe2 = sqe.neighbors[i];

                if (sqe2.trajectoryDistance > sqe.trajectoryDistance)
                {
                    List<MazeSquareExtension> theseDeadSquares = new List<MazeSquareExtension>();
                    if (CalculateTrajectory(sqe2, theseDeadSquares))
                    {
                        // ... anything to to?
                    }
                    else
                    {
                        for (int j = 0; j < theseDeadSquares.Count; j++)
                        {
                            MazeSquareExtension deadSqe = theseDeadSquares[j];
                            deadSqe.isDeadEnd = true;
                            deadSquares.Add(deadSqe.extendedSquare);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recursively adjust trajectory distances.  Collect dead end squares.
        /// </summary>
        /// <param name="sqe"></param>
        /// <param name="deadSquares">(potentially) dead ends</param>
        /// <returns>true if a new trajectory has been found</returns>
        private bool CalculateTrajectory(MazeSquareExtension sqe, List<MazeSquareExtension> deadSquares)
        {
            int curDistance = sqe.trajectoryDistance;

            // Mark this square's distance as invalid.
            sqe.trajectoryDistance *= -1;

            // neighbors whose trajectory depends on this square
            List<MazeSquareExtension> dependantNeighbors = new List<MazeSquareExtension>(3);

            for (int i = sqe.neighbors.Count; i-- > 0;)
            {
                MazeSquareExtension sqe2 = sqe.neighbors[i];

                // Dead squares are useless and may be removed from the neighbors list.
                if (sqe2.isDeadEnd)
                {
                    sqe.neighbors.RemoveAt(i);
                    continue;
                }

                // Marked squares cannot be considered as part of the new trajectory.
                if (sqe2.trajectoryDistance < 0)
                {
                    continue;
                }

                // When we encounter a square that is closer than the current square,
                // we have found a new valid trajectory.
                if (sqe2.trajectoryDistance < curDistance)
                {
                    sqe.trajectoryDistance *= -1;
                    ReviveNeighbors(sqe);
                    return true;
                    // Note: Other neighbors of sqe need not be processed because sqe.trajectoryDistance has not changed.
                }
                else
                {
                    dependantNeighbors.Add(sqe2);
                }
            }

            // If we did not return from the previous loop, all dependant neighbors need to be checked.
            int bestNeighborDistance = int.MaxValue;
            for (int i = 0; i < dependantNeighbors.Count; i++)
            {
                MazeSquareExtension sqe2 = dependantNeighbors[i];
                // If we can find a trajectory for the neighbor square,
                // it is also valid for the current square.
                if (CalculateTrajectory(sqe2, deadSquares))
                {
                    // Note: sqe2.trajectoryDistance >= 0
                    bestNeighborDistance = Math.Min(bestNeighborDistance, sqe2.trajectoryDistance);
                }
            }
            if (bestNeighborDistance < int.MaxValue)
            {
                sqe.trajectoryDistance = bestNeighborDistance + 1;
                ReviveNeighbors(sqe);
                return true;
            }

            // Add this square to the (potentially) dead squares.
            // Note: sqe.trajectoryDistance is negative but sqe.deadEnd is still false
            deadSquares.Add(sqe);
            return false;
        }

        /// <summary>
        /// Recursively assign valid (positve) trajectory distances to all neighbors of the given square.
        /// </summary>
        /// <param name="sqe"></param>
        private void ReviveNeighbors(MazeSquareExtension sqe)
        {
            for (int i = 0; i < sqe.neighbors.Count; i++)
            {
                MazeSquareExtension sqe2 = sqe.neighbors[i];
                if (sqe2.trajectoryDistance < 0 && !sqe2.isDeadEnd)
                {
                    sqe2.trajectoryDistance = sqe.trajectoryDistance + 1;
                    ReviveNeighbors(sqe2);
                }
            }
        }

        #endregion
    }
}
