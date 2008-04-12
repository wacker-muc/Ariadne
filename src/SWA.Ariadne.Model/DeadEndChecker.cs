using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Model
{
    public class DeadEndChecker
    {
        #region Member variables and Properties

        #region Class MazeSquareExtension

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

        #endregion

        /// <summary>
        /// For every MazeSquare: an extension that helps identifying dead ends.
        /// </summary>
        private MazeSquareExtension[,] mazeExtension;

        #region Public Properties

        /// <summary>
        /// Returns a measure of the distance of the given square from the maze's end square.
        /// The distance is the length of a trjectory that passes only through "alive" squares.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns>-1 if the given square is dead; >=0 otherwise</returns>
        public int Distance(MazeSquare sq)
        {
            if (mazeExtension[sq.XPos, sq.YPos].isDeadEnd)
            {
                return -1;
            }
            else
            {
                return mazeExtension[sq.XPos, sq.YPos].trajectoryDistance;
            }
        }

        /// <summary>
        /// Returns true if the given square is marked as dead.
        /// A square is dead if
        ///  a) it is a reserved area in the maze
        ///  b) it has already been visited
        ///  c) there is no trajectory leading to the maze's end square
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public bool IsDead(MazeSquare sq)
        {
            return mazeExtension[sq.XPos, sq.YPos].isDeadEnd;
        }

        #endregion

        /// <summary>
        /// Ordered collection of squares whose trajectoryDistance is uncertain.
        /// The trajectoryDistance is a negative number (the original value negated).
        /// Squares with a positive trajectoryDistance have been revived and may be discarded.
        /// The list is sorted by increasing (absolute) value of (uncertain) trajectoryDistance.
        /// </summary>
        private List<MazeSquareExtension> uncertainSquares = new List<MazeSquareExtension>();

        /// <summary>
        /// Ordered collection of squares whose trajectoryDistance has been confirmed.
        /// The list is sorted by increasing value of trajectoryDistance.
        /// </summary>
        private List<MazeSquareExtension> confirmedSquares = new List<MazeSquareExtension>();

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
                        sqe.trajectoryDistance = -1;
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
            mazeExtension[maze.StartSquare.XPos, maze.StartSquare.YPos].trajectoryDistance = -1;
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
                        if (sqe1.trajectoryDistance - 1 > sqe2.trajectoryDistance && !sqe2.isDeadEnd)
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

            // Don't process squares that have been visited before.
            if (sqe.isDeadEnd)
            {
                return result;
            }

            // The visited square is marked as dead.
            // TODO: Remove sqe from all its neighbors' neighbor lists.  Further tests for dead neighbors are obsolete.
            sqe.isDeadEnd = true;
            int d = sqe.trajectoryDistance;
            if (d > 0)
            {
                sqe.trajectoryDistance *= -1;
            }
            else
            {
                d *= -1;
            }

            if (d == 0)
            {
                // This is the end square.  No need to kill any more squares...
                return result;
            }

            // Add neighbors to the list of uncertainSquares if their trajectory passed through the visited square.
            // That is the case if their distance is greater by one than the visited square's distance.
            for (int i = 0; i < sqe.neighbors.Count; i++)
            {
                MazeSquareExtension sqe2 = sqe.neighbors[i];

                if (sqe2.trajectoryDistance == d + 1)
                {
                    AddUncertainSquare(sqe2, -1);
                }
            }

#if false
            // TODO: enable this enhancement: check for harmless constellation
            if (HarmlessConstellation(sqe))
            {
                return result;
            }
#endif

            // Add all squares whose trajectory depends on the ones already inserted.
            CollectUncertainSquares();

            // The areas next to all confirmedSquares will receive an adjusted trajectoryDistance.
            ReviveConfirmedSquaresNeighbors();

            // The remaining uncertainSquares with negative (invalid) trajectoryDistance are dead.
            for (int j = 0; j < uncertainSquares.Count; j++)
            {
                MazeSquareExtension sqe2 = uncertainSquares[j];
                if (sqe2.trajectoryDistance < 0 && !sqe2.isDeadEnd)
                {
                    sqe2.isDeadEnd = true;
                    result.Add(sqe2.extendedSquare);
                }
            }

#if false
            // TODO: disable this debug code
            Console.Out.WriteLine("Visited {0} : {1} uncertain and {2} confirmed squares.",
                                  sq.ToString(), uncertainSquares.Count, confirmedSquares.Count);
#endif

            // Empty the internal lists.
            uncertainSquares.Clear();
            confirmedSquares.Clear();

            return result;
        }

        /// <summary>
        /// Adjust trajectory distances of the uncertainSquares.
        /// </summary>
        private void CollectUncertainSquares()
        {
            // Note: Initially, uncertainSquares is an ordered list of the neighbors of a visited square.
            //       As our search for a new trajectory continues, more squares will be marked uncertain
            //       and added to this list.  The sort order will be preserved.

            for (int i = 0; i < uncertainSquares.Count; i++)
            {
                MazeSquareExtension sqe1 = uncertainSquares[i];

                if (sqe1.trajectoryDistance > 0)
                {
                    // This square's trajectory has been confirmed.  It is not uncertain any more.
                    continue;
                }

                if (sqe1.isDeadEnd)
                {
                    // No chance to revive an already dead square!
                    continue;
                }

                // We need to find a neighbor that gives this square a new trajectory.
                int requiredNeighborDistance = -sqe1.trajectoryDistance - 1;

                for (int j = 0; j < sqe1.neighbors.Count; j++)
                {
                    MazeSquareExtension sqe2 = sqe1.neighbors[j];
                    
                    if (sqe2.trajectoryDistance == requiredNeighborDistance)
                    {
                        // We have confirmed that sqe1 has a neighbor sqe2 giving it a new trajectory.
                        AddConfirmedSquare(sqe1);

                        break; // from for (j)
                    }
                    else if (sqe2.trajectoryDistance > requiredNeighborDistance)
                    {
                        // Add this square to the list of uncertainSquares.
                        AddUncertainSquare(sqe2, i);
                    }
                }
            }
        }

        /// <summary>
        /// Add the given square to the ordered list of uncertain squares.
        /// </summary>
        /// <param name="sqe"></param>
        private void AddUncertainSquare(MazeSquareExtension sqe, int behindPosition)
        {
            // Mark sqe's trajectoryDistance as uncertain.
            if (sqe.trajectoryDistance > 0)
            {
                sqe.trajectoryDistance *= -1;
            }

            AddSquare(sqe, behindPosition, uncertainSquares);
        }

        /// <summary>
        /// Add the given square to the ordered list of uncertain squares.
        /// </summary>
        /// <param name="sqe"></param>
        private void AddConfirmedSquare(MazeSquareExtension sqe)
        {
            // Mark sqe's trajectoryDistance as confirmed.
            if (sqe.trajectoryDistance < 0)
            {
                sqe.trajectoryDistance *= -1;
            }

            AddSquare(sqe, -1, confirmedSquares);
        }

        private static void AddSquare(MazeSquareExtension sqe, int behindPosition, List<MazeSquareExtension> list)
        {
            if (list.Count > 8 * Maze.MaxXSize * Maze.MaxYSize)
            {
                throw new Exception("internal list grows too long");
            }

            int key = sqe.trajectoryDistance; if (key < 0) key = -key;

            // The key needs to be compared with the squares in the index range a..b
            int a = behindPosition + 1, b = list.Count - 1;

            // Empty region.
            if (b < a)
            {
                list.Add(sqe);
                return;
            }

            int keyA = list[a].trajectoryDistance; if (keyA < 0) keyA = -keyA;
            int keyB = list[b].trajectoryDistance; if (keyB < 0) keyB = -keyB;

            // The square fits at the end of the list.
            if (keyB <= key)
            {
                list.Add(sqe);
                return;
            }

            // The square fits at the start of the list.
            if (keyA >= key)
            {
                list.Insert(a, sqe);
                return;
            }

            // Find a position for insertion into the list.
            // keyA < key < keyB
            while (a < b)
            {
                int m = (a + b) / 2;
                int keyM = list[m].trajectoryDistance; if (keyM < 0) keyM = -keyM;

                if (keyM > key)
                {
                    b = m;
                }
                else if (keyM < key)
                {
                    a = m + 1;
                }
                else
                {
                    a = m + 1;
                    break;
                }

            }

            list.Insert(a, sqe);
        }

        /// <summary>
        /// Assign valid (positve) trajectory distances to all uncertain neighbors of the confirmedSquares.
        /// </summary>
        private void ReviveConfirmedSquaresNeighbors()
        {
            // Note: The confirmedSquares list is ordered by increasing trajectoryDistance.
            //       Additional items will be added while we traverse the adjoining area.

            for (int i = 0; i < confirmedSquares.Count; i++)
            {
                MazeSquareExtension sqe1 = confirmedSquares[i];
                for (int j = 0; j < sqe1.neighbors.Count; j++ )
                {
                    MazeSquareExtension sqe2 = sqe1.neighbors[j];
                    if (sqe2.trajectoryDistance < 0 && !sqe2.isDeadEnd)
                    {
                        sqe2.trajectoryDistance = sqe1.trajectoryDistance + 1;
                        confirmedSquares.Add(sqe2);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if no squares could have possibly been killed by visiting the given square.
        /// </summary>
        /// <param name="sqe"></param>
        /// <returns></returns>
        /// 
        /// +---+---+---+   +---+---+---+ 
        /// |   |   |   |   |   | o | x |   In these diagrams, sqe is the center square.
        /// +---+---+---+   +---+---+---+   "x" are dead squares; "o" are alive squares; the rest is irrelevant
        /// | x | x | x |   | x | x | o |   These two constellations are critical (not harmless).
        /// +---+---+---+   +---+---+---+
        /// |   |   |   |   |   |   |   |   The straight constellation is harmless if one of the parallel lines
        /// +---+---+---+   +---+---+---+   is completely dead.
        /// 
        private bool HarmlessConstellation(MazeSquareExtension sqe)
        {
            int x = sqe.extendedSquare.XPos, y = sqe.extendedSquare.YPos;
            int w = mazeExtension.GetUpperBound(0);
            int h = mazeExtension.GetUpperBound(1);

            #region Identify straight lines.
            
            bool deadW = (x == 0 || mazeExtension[x - 1, y].isDeadEnd);
            bool deadE = (x == w || mazeExtension[x + 1, y].isDeadEnd);
            bool deadN = (y == 0 || mazeExtension[x, y - 1].isDeadEnd);
            bool deadS = (y == h || mazeExtension[x, y + 1].isDeadEnd);

            bool deadNW = (x == 0 || y == 0 || mazeExtension[x - 1, y - 1].isDeadEnd);
            bool deadNE = (x == w || y == 0 || mazeExtension[x + 1, y - 1].isDeadEnd);
            bool deadSW = (x == 0 || y == h || mazeExtension[x - 1, y + 1].isDeadEnd);
            bool deadSE = (x == w || y == h || mazeExtension[x + 1, y + 1].isDeadEnd);

            if (deadW && deadE)
            {
                if (deadN || deadS)
                {
                    return true;
                }
                return false;
            }

            if (deadN && deadS)
            {
                if (deadW || deadE)
                {
                    return true;
                }
                return false;
            }

            #endregion

            #region Identify angled lines.

            if (deadNW && !deadN && !deadW && (deadS || deadE))
            {
                return false;
            }

            if (deadNE && !deadN && !deadE && (deadS || deadW))
            {
                return false;
            }

            if (deadSW && !deadS && !deadW && (deadN || deadE))
            {
                return false;
            }

            if (deadSE && !deadS && !deadE && (deadN || deadW))
            {
                return false;
            }

            #endregion

            return true;
        }

        #endregion
    }
}
