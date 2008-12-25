using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Logic
{
    class ThickestBranchFlooder : FlooderBase
    {
        #region Member variables

        float[] branchesLog = { -1, 0, (float)Math.Log(2), (float)Math.Log(3) };

        private struct BranchExtension
        {
            /// <summary>
            /// Length of the path leading from the start square to this square.
            /// </summary>
            public int length;

            /// <summary>
            /// The start square's thickness is 1.
            /// When a branch is split, each branch gets an equal share of the current thickness.
            /// </summary>
            /// 
            /// Note: Instead of the direct thickness, we store its (natural) logarithm.
            ///       Otherwise the range of values would be very limited;
            ///       after approx. 500 splits, the value would be less than float.Epsilon.
            public float thickness;
        }
        /// <summary>
        /// For every MazeSquare: a counter of the open paths that lead away from it.
        /// </summary>
        /// A path is considered "open" if its end point is in the list of active squares.
        private BranchExtension[,] branchExtension;

        /// <summary>
        /// +1 (maximize thickness) or -1 (minimize thickness)
        /// </summary>
        protected int thicknessSign = +1;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="mazeDrawer"></param>
        public ThickestBranchFlooder(Maze maze, IMazeDrawer mazeDrawer)
            : base(maze, mazeDrawer)
        {
            this.branchExtension = new BranchExtension[maze.XSize, maze.YSize];

            MazeSquare sq0 = maze.StartSquare;
            branchExtension[sq0.XPos, sq0.YPos].length = 0;
            branchExtension[sq0.XPos, sq0.YPos].thickness = 0;
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
                // The branch's length is increased by 1.
                branchExtension[sq2.XPos, sq2.YPos].length = branchExtension[sq1.XPos, sq1.YPos].length + 1;

                // The branch's thickness is divided by the number of branches leading away.
                int branches = Math.Max(1, OpenWalls(sq2, false).Count - 1);
                branchExtension[sq2.XPos, sq2.YPos].thickness = branchExtension[sq1.XPos, sq1.YPos].thickness - branchesLog[branches];
            }
        }

        protected override int SelectPathIdx()
        {
            int bestIdx = 0;
            float bestThickness = thicknessSign * float.NegativeInfinity;
            int bestLength = 0;

            for (int i = 0; i < list.Count; i++)
            {
                MazeSquare sq = list[i];
                float thickness = branchExtension[sq.XPos, sq.YPos].thickness;
                int length = branchExtension[sq.XPos, sq.YPos].length;

                if (thicknessSign * thickness > thicknessSign * bestThickness)
                {
                    bestIdx = i;
                    bestThickness = thickness;
                    bestLength = length;
                }
                else if (thickness == bestThickness && thicknessSign * length > thicknessSign * bestLength)
                {
                    bestIdx = i;
                    //bestThickness = thickness;
                    bestLength = length;
                }
            }

            return bestIdx;
        }

        protected override WallPosition SelectDirection(MazeSquare sq1, List<WallPosition> openWalls)
        {
            return openWalls[random.Next(openWalls.Count)];
        }

        #endregion
    }
}
