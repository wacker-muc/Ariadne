using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    public class RandomSolver : SolverBase
    {
        #region Member variables

        /// <summary>
        /// A source of random numbers.
        /// </summary>
        private Random random;

        private Stack<MazeSquare> stack = new Stack<MazeSquare>();

        private bool forward = true;

        #endregion

        #region Constructor

        public RandomSolver(Maze maze)
            : base(maze)
        {
            this.random = new Random();
            this.Reset();
        }

        #endregion

        public void Reset()
        {
            stack.Clear();

            // Move to the start square.
            MazeSquare sq = maze.StartSquare;

            // Push the start square onto the stack.
            stack.Push(sq);
            sq.isVisited = true;
        }

        public override void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward)
        {
            if (maze.IsSolved)
            {
                throw new Exception("Maze is already solved.");
            }

            // Get the current square (at xCur, yCur)
            sq1 = stack.Peek();

            // Possible choices
            List<MazeSquare.WallPosition> openWalls = new List<MazeSquare.WallPosition>(MazeSquare.WP_NUM);

            // Collect the unfixed walls of sq1.
            //
            for (MazeSquare.WallPosition wp = MazeSquare.WP_MIN; wp <= MazeSquare.WP_MAX; wp++)
            {
                if (sq1[wp] == MazeSquare.WallState.WS_OPEN)
                {
                    MazeSquare sq = sq1.NeighborSquare(wp);

                    if (!sq.isVisited)
                    {
                        openWalls.Add(wp);
                    }
                } // if WS_MAYBE
            } // foreach wp

            if (openWalls.Count > 0)
            {
                // select one of the neighbor squares
                MazeSquare.WallPosition wp = openWalls[random.Next(openWalls.Count)];

                sq2 = sq1.NeighborSquare(wp);
                forward = true;

                // Push the next square onto the stack.
                stack.Push(sq2);
                sq2.isVisited = true;

            }
            else
            {
                // Pop the current square from the stack.
                stack.Pop();

                sq2 = stack.Peek();
                forward = false;
            }
        }
    }
}
