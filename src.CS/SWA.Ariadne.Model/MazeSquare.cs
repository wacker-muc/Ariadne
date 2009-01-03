using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model.Interfaces;

namespace SWA.Ariadne.Model
{
    /// <summary>
    /// MazeSquares are the building blocks of a Maze.
    /// They have open or closed walls on all four sides.
    /// </summary>
    sealed public class MazeSquare
    {
        #region Member variables and Properties

        internal WallState[] walls = new WallState[(int)WallPosition.WP_NUM];

        #region Properties
        public WallState this[WallPosition side]
        {
            get { return walls[(int)side]; }
            set { walls[(int)side] = value; }
        }
        #endregion

        #region Maze ID: Constants variables and properties

        /// <summary>
        /// Used to distinguish a primary (host) maze and several embedded mazes.
        /// 0: This square is reserved.
        /// 1: Belongs to the primary maze.
        /// >1: Belongs to an embedded maze.
        /// </summary>
        public int MazeId
        {
            get
            {
                return this.mazeId;
            }
            set
            {
                if (value >= PrimaryMazeId && value <= MaxMazeId)
                {
                    this.mazeId = value;
                }
                else
                {
                    throw new Exception("invalid MazeId value" + value.ToString());
                }
            }
        }

        private int mazeId;

        /// <summary>
        /// Number of valid maze IDs, including the reserved ID.
        /// </summary>
        public const int MazeIdRange = 8;

        /// <summary>
        /// Maximum maze ID (primary and embedded).
        /// </summary>
        public const int MaxMazeId = MazeIdRange - 1;

        /// <summary>
        /// Maze ID assigned to reserved squares.
        /// </summary>
        public const int ReservedMazeId = 0;

        /// <summary>
        /// Maze ID of the primary maze.
        /// </summary>
        public const int PrimaryMazeId = 1;

        #endregion

        /// <summary>
        /// Used while building: Square is connected to the maze.
        /// </summary>
        internal bool isConnected = false;

        /// <summary>
        /// Used while building: Square will not be used.
        /// </summary>
        public bool isReserved
        {
            get { return (mazeId == ReservedMazeId); }
            set
            {
                if (value == true)
                {
                    this.mazeId = ReservedMazeId;
                }
                else
                {
                    throw new Exception("The reserved state cannot be cleared.");
                }
            }
        }

        /// <summary>
        /// Used while solving: Square has been visited.
        /// </summary>
        public bool isVisited = false;

        /// <summary>
        /// Adjoining squares in the four directions.
        /// </summary>
        private MazeSquare[] neighbors = new MazeSquare[(int)WallPosition.WP_NUM];

        #region Maze coordinates

        private int xPos, yPos;
        
        public int XPos
        {
            get { return this.xPos; }
        }
        public int YPos
        {
            get { return this.yPos; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MazeSquare(int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.mazeId = PrimaryMazeId;

            for (WallPosition wp = WallPosition.WP_MIN; wp <= WallPosition.WP_MAX; wp++)
            {
                this.walls[(int)wp] = WallState.WS_MAYBE;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Setup method: Define the adjoining sqare on the other side of a wall.
        /// </summary>
        /// <param name="side"></param>
        /// <param name="neighbor"></param>
        internal void SetNeighbor(WallPosition side, MazeSquare neighbor)
        {
            this.neighbors[(int)side] = neighbor;
        }

        /// <summary>
        /// Returns the WallPosition on the opposite side of a square.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static WallPosition OppositeWall(WallPosition p)
        {
            switch (p)
            {
                case WallPosition.WP_E: return WallPosition.WP_W;
                case WallPosition.WP_N: return WallPosition.WP_S;
                case WallPosition.WP_W: return WallPosition.WP_E;
                case WallPosition.WP_S: return WallPosition.WP_N;
                default: throw new ArgumentOutOfRangeException("p");
            }
        }

        /// <summary>
        /// Returns the square on the other side of a wall.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public MazeSquare NeighborSquare(WallPosition side)
        {
            return this.neighbors[(int)side];
        }

        #endregion

        #region ToString customization

        public override string ToString()
        {
            return string.Format("{0}/{1}: {2}", XPos.ToString(), YPos.ToString(), MazeId);
        }

        #endregion 
    }
}
