using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Outlines;

namespace SWA.Ariadne.Model
{
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
        /// An embedded maze shares the squares of the hostMaze.
        /// The given coordinates are relative to the embedded maze's bounding box.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override MazeSquare this[int x, int y]
        {
            get { return hostMaze[x + xOffset, y + yOffset]; }
        }

        /// <summary>
        /// Offset of the generating shape's bounding box within the hostMaze.
        /// </summary>
        private int xOffset = 0, yOffset = 0;

        /// <summary>
        /// Returns the number of squares inside the shape that are not reserved.
        /// </summary>
        public virtual int CountSquares
        {
            get
            {
                int result = 0;

                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        if (this[x, y].MazeId == this.mazeId)
                        {
                            ++result;
                        }
                    }
                }

                return result;
            }
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
        /// Adjusts xSize, ySize, xOffset, yOffset to form a tight bounding box around the shape.
        /// </summary>
        /// <param name="shape"></param>
        private void InstallInHost(OutlineShape shape)
        {
            int xMin = xSize, xMax = 0, yMin = ySize, yMax = 0;

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (shape[x, y] && !this[x, y].isReserved)
                    {
                        this[x, y].MazeId = this.mazeId;

                        xMin = Math.Min(xMin, x);
                        xMax = Math.Max(xMax, x);
                        yMin = Math.Min(yMin, y);
                        yMax = Math.Max(yMax, y);
                    }
                }
            }

            // Adjust the dimension parameters to form a tight bounding box.
            this.xOffset = xMin;
            this.yOffset = yMin;
            this.xSize = xMax - xMin + 1;
            this.ySize = yMax - yMin + 1;
        }

        #endregion
    }
}
