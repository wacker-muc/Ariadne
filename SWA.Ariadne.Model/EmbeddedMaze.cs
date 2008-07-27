using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Outlines;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// An EmbeddedMaze occupies a contiguous part of its host maze.
    /// It may be solved independently of its host.
    /// </summary>
    internal class EmbeddedMaze : Maze
    {
        #region Member variables

        private Maze hostMaze;

        /// <summary>
        /// Embedded mazes have an ID > 1.
        /// </summary>
        protected override int MazeId
        {
            get { return mazeId; }
        }
        private readonly int mazeId;

        /// <summary>
        /// Returns a square which is shared with the embedded maze.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MazeSquare this[int x, int y]
        {
            get { return hostMaze[x, y]; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a maze that is embedded in the hostMaze.
        /// The maze is formed by all squares inside the given shape.
        /// </summary>
        /// <param name="hostMaze"></param>
        /// <param name="mazeId"></param>
        /// <param name="shape"></param>
        public EmbeddedMaze(Maze hostMaze, int mazeId, OutlineShape shape)
            : base(hostMaze.XSize, hostMaze.YSize)
        {
            if (mazeId <= MazeSquare.PrimaryMazeId || mazeId > MazeSquare.MaxMazeId)
            {
                throw new Exception("invalid maze ID: " + mazeId.ToString());
            }

            this.hostMaze = hostMaze;
            this.mazeId = mazeId;

            // We want to share the host's Random generator.
            this.random = hostMaze.Random;

            this.InstallInHost(shape);
        }

        /// <summary>
        /// Overwrites the mazeId of every square inside the shape.
        /// </summary>
        /// <param name="shape"></param>
        private void InstallInHost(OutlineShape shape)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (shape[x, y] && !this[x, y].isReserved)
                    {
                        this[x, y].MazeId = this.mazeId;
                    }
                }
            }
        }

        #endregion
    }
}
