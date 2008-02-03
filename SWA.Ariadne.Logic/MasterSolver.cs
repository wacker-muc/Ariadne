using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    internal class MasterSolver : SolverBase
    {
        #region Member variables

        /// <summary>
        /// The path leading from start to end.
        /// </summary>
        public List<MazeSquare> Path
        {
            get { return path; }
        }
        private readonly List<MazeSquare> path;

        /// <summary>
        /// The current position in the path, while solving.
        /// </summary>
        private int pathPos;

        #endregion

        #region Constructor

        public MasterSolver(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            // Use another MazeSolver to find the path from start to end.
            
            RoundRobinFlooder helper = new RoundRobinFlooder(maze, null);
            helper.Reset();
            helper.Solve();
            this.path = helper.PathFromStartSquare(maze.EndSquare);
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

            // Move to the start square.
            this.pathPos = 0;

            // Mark the start square as visited.
            this.path[pathPos].isVisited = true;
        }

        #endregion

        #region Runtime methods

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

            sq1 = this.path[pathPos];
            sq2 = this.path[++pathPos];
            forward = true;

            // Mark the next square as visited.
            sq2.isVisited = true;
        }

        #endregion
    }
}
