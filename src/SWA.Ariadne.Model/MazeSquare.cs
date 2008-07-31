using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Model
{
    sealed public class MazeSquare
    {
        #region Enumeration types

        /// <summary>
        /// Positions of the walls around a square: East, North, West, South.
        /// </summary>
        public enum WallPosition : int
        {
            /// <summary>
            /// East.
            /// </summary>
            WP_E = 0,
            /// <summary>
            /// North.
            /// </summary>
            WP_N = 1,
            /// <summary>
            /// West.
            /// </summary>
            WP_W = 2,
            /// <summary>
            /// South.
            /// </summary>
            WP_S = 3,
        }
        public const WallPosition WP_MIN = WallPosition.WP_E;
        public const WallPosition WP_MAX = WallPosition.WP_S;
        public const int WP_NUM = 4;

        /// <summary>
        /// States of a wall: Open, Closed, Not determined.
        /// </summary>
        public enum WallState : byte
        {
            /// <summary>
            /// Undetermined, needs to be initialized.
            /// </summary>
            WS_MAYBE = 0,
            /// <summary>
            /// Open wall, may be passed.
            /// </summary>
            WS_OPEN = 1,
            /// <summary>
            /// Closed wall, may not be passed.
            /// </summary>
            WS_CLOSED = 2,
            /// <summary>
            /// Undetermined, should be part of an outlined shape.
            /// </summary>
            WS_OUTLINE = 3,
        }

        #endregion

        #region Member variables and Properties

        internal WallState[] walls = new WallState[WP_NUM];

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
        internal bool isReserved
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
        private MazeSquare[] neighbors = new MazeSquare[WP_NUM];

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

            for (int i = 0; i < WP_NUM; i++)
            {
                this.walls[i] = WallState.WS_MAYBE;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Setup method: Define the adjoining sqare on the other side of a wall.
        /// </summary>
        /// <param name="side"></param>
        /// <param name="neighbor"></param>
        internal void SetNeighbor(MazeSquare.WallPosition side, MazeSquare neighbor)
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
