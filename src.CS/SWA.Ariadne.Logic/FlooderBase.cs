using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Base class for all MazeSolvers using a flooding strategy.
    /// </summary>
    abstract class FlooderBase : SolverBase
    {
        #region Member variables

        private struct MazeSquareExtension
        {
            /// <summary>
            /// Number of the open paths that lead away from this square.
            /// </summary>
            public int openPathCount;

            /// <summary>
            /// Predecessor of this square on its path.
            /// </summary>
            public MazeSquare previousSquare;

            /// <summary>
            /// A penalty assigned when paths stemming from previous forks are marked as dead ends.
            /// </summary>
            public double deadRelativesPenalty;
        }
        /// <summary>
        /// For every MazeSquare: a counter of the open paths that lead away from it.
        /// </summary>
        /// A path is considered "open" if its end point is in the list of active squares.
        private MazeSquareExtension[,] mazeExtension;

        /// <summary>
        /// All squares passed in forward direction are collected in a list.
        /// </summary>
        protected List<MazeSquare> list = new List<MazeSquare>();

        #region Members for applying a heuristic based on dead paths on neighboring paths

        /// <summary>
        /// A delegate for calculating the heuristic value of a dead branch.
        /// </summary>
        /// <param name="deadBranch"></param>
        /// <returns></returns>
        public delegate double DeadBranchHeuristicDelegate(List<MazeSquare> deadBranch);

        /// <summary>
        /// The currently installed DeadBranchHeuristicDelegate.
        /// </summary>
        private DeadBranchHeuristicDelegate deadBranchHeuristic;

        /// <summary>
        /// If a DeadBranchHeuristicDelegate is installed, this is
        /// the lowest penalty value assigned to any of the currently open paths.
        /// see: IsSelectablePathIdx()
        /// </summary>
        double lowestRelativesPenalty;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public FlooderBase(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.mazeExtension = new MazeSquareExtension[maze.XSize, maze.YSize];
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Reset to the initial state (before the maze is solved).
        /// Subclasses should call their base class' method first.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            list.Clear();

            // Move to the start square.
            MazeSquare sq = maze.StartSquare;

            // Add the start square to the list.
            list.Add(sq);
            sq.isVisited = true;

            // As we may not retract beyond the start square, it needs to have a positive count.
            mazeExtension[sq.XPos, sq.YPos].openPathCount = 1;

            // The start square gets a zero penalty value.
            mazeExtension[sq.XPos, sq.YPos].deadRelativesPenalty = 0;
            UpdateLowestPenalty();
        }

        #endregion

        #region Runtime methods

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// Wrapper. Calls the implementing method StepI().
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            base.Step(out sq1, out sq2, out forward);

            if (forward) // Always true; Flooders only travel forwards.
            {
                mazeExtension[sq1.XPos, sq1.YPos].openPathCount += 1;
                mazeExtension[sq2.XPos, sq2.YPos].previousSquare = sq1;

                // This might also be done in the Reset() method.  But it is not too late here.
                mazeExtension[sq2.XPos, sq2.YPos].openPathCount = 0;
                mazeExtension[sq2.XPos, sq2.YPos].deadRelativesPenalty = mazeExtension[sq1.XPos, sq1.YPos].deadRelativesPenalty;
            }
        }

        /// <summary>
        /// Travel from one visited square to a neighbor square (through an open wall).
        /// </summary>
        /// <param name="sq1">first (previously visited) square</param>
        /// <param name="sq2">next (neighbor) square</param>
        /// <param name="forward">true if the neighbor square was not visited previously</param>
        protected override void StepI(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            if (maze.IsSolved)
            {
                throw new Exception("Maze is already solved.");
            }

            List<WallPosition> openWalls;

            while (true)
            {
                // Get a current square but leave it in the queue.
                int p = SelectPathIdx();
                sq1 = list[p];

                // Possible choices of open walls (not visited).
                openWalls = OpenWalls(sq1, true);

                if (openWalls.Count == 0)
                {
                    list.RemoveAt(p);
                    MarkDeadBranch(sq1);
                }
                else
                {
                    // If this was the last open wall of sq1, it can be removed from the list.
                    if (openWalls.Count == 1)
                    {
                        list.RemoveAt(p);
                    }

                    break;
                }
            }

            // Select one (any) of the neighbor squares.
            WallPosition wp = SelectDirection(sq1, openWalls);

            sq2 = sq1.NeighborSquare(wp);
            forward = true;

            // Add the next square to the list.
            list.Add(sq2);
            sq2.isVisited = true;
        }

        /// <summary>
        /// Select an index within the flooder's list of open paths.
        /// Implementations must consider IsSelectablePathIdx().
        /// </summary>
        /// <returns></returns>
        protected abstract int SelectPathIdx();

        /// <summary>
        /// Select one of the open walls leading away from the given square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="openWalls"></param>
        /// <returns></returns>
        protected abstract WallPosition SelectDirection(MazeSquare sq1, List<WallPosition> openWalls);

        /// <summary>
        /// Lets the mazeDrawer draw the dead branch ending in the given square.
        /// </summary>
        /// <param name="sq">a MazeSquare in a dead end of the Maze</param>
        private void MarkDeadBranch(MazeSquare sq)
        {
            if (OpenWalls(sq, false).Count > 1)
            {
                /* This is a false call.
                 * The subclass MazeSolver regards this square as no longer usable
                 * but it is not a true dead end.
                 */
                return;
            }
            List<MazeSquare> deadBranch = new List<MazeSquare>();

            while (mazeExtension[sq.XPos, sq.YPos].openPathCount == 0   // no more open paths
                && OpenWalls(sq, true).Count == 0                       // no more unvisited neighbors
                )
            {
                deadBranch.Add(sq);                                     // this square is dead
                sq = mazeExtension[sq.XPos, sq.YPos].previousSquare;    // go to previous square
                mazeExtension[sq.XPos, sq.YPos].openPathCount -= 1;     // subtract the dead neighbor
            }

            deadBranch.Add(sq);                                         // last (living) square of the dead branch

            if (deadBranch.Count > 1 && mazeDrawer != null)
            {
                mazeDrawer.DrawPath(deadBranch, false);
            }

            if (deadBranchHeuristic != null)
            {
                double penalty = deadBranchHeuristic(deadBranch);
                AddDeadBranchPenalty(sq, penalty);
                UpdateLowestPenalty();
            }
        }

        /// <summary>
        /// Write state information to the given StringBuilder.
        /// A flooder adds the number of currently open paths.
        /// </summary>
        /// <param name="message"></param>
        public override void FillStatusMessage(StringBuilder message)
        {
            int nPaths = this.list.Count;
            string paths = (nPaths == 1 ? "path" : "paths");
            message.Append(", " + nPaths.ToString() + " " + paths);
        }

        #region Dead branch heuristic

        /// <summary>
        /// Returns true if the given index is an acceptable result of SelectPathIdx().
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected bool IsSelectablePathIdx(int i)
        {
            if (deadBranchHeuristic != null)
            {
                MazeSquare sq = list[i];
                bool result = (mazeExtension[sq.XPos, sq.YPos].deadRelativesPenalty <= lowestRelativesPenalty);
                return result;
            }
            return true;
        }

        /// <summary>
        /// A DeadBranchHeuristic delegate method.
        /// </summary>
        /// <param name="deadBranch"></param>
        /// <returns>The constant 1.</returns>
        public double DeadBranchHeuristicSimple(List<MazeSquare> deadBranch)
        {
            return 1;
        }

        /// <summary>
        /// A DeadBranchHeuristic delegate method.
        /// </summary>
        /// <param name="deadBranch"></param>
        /// <returns>The length of the dead branch.</returns>
        public double DeadBranchHeuristicLength(List<MazeSquare> deadBranch)
        {
            return deadBranch.Count - 1;
        }

        /// <summary>
        /// Enable some solver-specific heuristic that may guide the solver decicions.
        /// A Flooder solver will use a dead branch heuristic.
        /// </summary>
        public override void UseHeuristic()
        {
            base.UseHeuristic();
            this.deadBranchHeuristic = DeadBranchHeuristicSimple;
            //this.deadBranchHeuristic = DeadBranchHeuristicLength;
        }

        /// <summary>
        /// Returns true if this MazeSolver uses some heuristic to guide its decisions.
        /// </summary>
        public override bool IsHeuristicSolver
        {
            get { return (this.deadBranchHeuristic != null); }
        }

        #endregion

        #region Dead branch penalty

        /// <summary>
        /// Add the given penalty to the given square and all already visited paths beyond.
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="penalty"></param>
        private void AddDeadBranchPenalty(MazeSquare sq, double penalty)
        {
            mazeExtension[sq.XPos, sq.YPos].deadRelativesPenalty += penalty;

            foreach (WallPosition wp in OpenWalls(sq, false))
            {
                MazeSquare sq2 = sq.NeighborSquare(wp);
                if (sq2.isVisited && sq2 != mazeExtension[sq.XPos, sq.YPos].previousSquare)
                {
                    AddDeadBranchPenalty(sq2, penalty);
                }
            }
        }

        /// <summary>
        /// Update the lowestRelativesPenalty member variable.
        /// Consider all squares in the list of open paths.
        /// </summary>
        private void UpdateLowestPenalty()
        {
            this.lowestRelativesPenalty = double.MaxValue;

            foreach (MazeSquare sq in list)
            {
                MazeSquareExtension sqe = mazeExtension[sq.XPos, sq.YPos];
                lowestRelativesPenalty = Math.Min(lowestRelativesPenalty, sqe.deadRelativesPenalty);
            }
        }

        #endregion

        #endregion

        #region Support methods for the MasterSolver

        /// <summary>
        /// Given a square (usually, the end square), return the path leading there from the start square.
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        internal List<MazeSquare> PathFromStartSquare(MazeSquare sq)
        {
            List<MazeSquare> result = new List<MazeSquare>();

            result.Add(sq);

            while (sq != this.maze.StartSquare)
            {
                sq = this.mazeExtension[sq.XPos, sq.YPos].previousSquare;
                result.Insert(0, sq);
            }

            return result;
        }

        #endregion
    }
}
