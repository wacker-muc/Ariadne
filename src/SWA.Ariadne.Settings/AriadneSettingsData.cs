using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Settings
{
    public class AriadneSettingsData
    {
        #region Data binding properties

        private const int M = 0; // modifyable value
        private const int S = 1; // shadow value

        #region Shape tab

        public int MazeWidth
        {
            get { return mazeWidth[M]; }
            set { mazeWidth[M] = value; }
        }
        public bool MazeWidthModified
        {
            get { return (mazeWidth[M] != mazeWidth[S]); }
        }
        private int[] mazeWidth = new int[2];

        public int MazeHeight
        {
            get { return mazeHeight[M]; }
            set { mazeHeight[M] = value; }
        }
        public bool MazeHeightModified
        {
            get { return (mazeHeight[M] != mazeHeight[S]); }
        }
        private int[] mazeHeight = new int[2];

        public int Seed
        {
            get { return seed[M]; }
            set { seed[M] = value; }
        }
        public bool SeedModified
        {
            get { return (seed[M] != seed[S]); }
        }
        private int[] seed = new int[2];

        public bool AutoMazeWidth
        {
            get { return autoMazeWidth[M]; }
            set { autoMazeWidth[M] = value; }
        }
        public bool AutoMazeWidthModified
        {
            get { return (autoMazeWidth[M] != autoMazeWidth[S]); }
        }
        private bool[] autoMazeWidth = new bool[2];

        public bool AutoMazeHeight
        {
            get { return autoMazeHeight[M]; }
            set { autoMazeHeight[M] = value; }
        }
        public bool AutoMazeHeightModified
        {
            get { return (autoMazeHeight[M] != autoMazeHeight[S]); }
        }
        private bool[] autoMazeHeight = new bool[2];

        public bool AutoSeed
        {
            get { return autoSeed[M]; }
            set { autoSeed[M] = value; }
        }
        public bool AutoSeedModified
        {
            get { return (autoSeed[M] != autoSeed[S]); }
        }
        private bool[] autoSeed = new bool[2];

        public string Code
        {
            get { return code[M]; }
            set { code[M] = value; }
        }
        public bool CodeModified
        {
            get { return (code[M] != code[S]); }
        }
        private string[] code = new string[2];
        

        #endregion

        #region Layout tab

        public int SquareWidth
        {
            get { return squareWidth[M]; }
            set { squareWidth[M] = value; }
        }
        public bool SquareWidthModified
        {
            get { return (squareWidth[M] != squareWidth[S]); }
        }
        private int[] squareWidth = new int[2];

        public int PathWidth
        {
            get { return pathWidth[M]; }
            set { pathWidth[M] = value; }
        }
        public bool PathWidthModified
        {
            get { return (pathWidth[M] != pathWidth[S]); }
        }
        private int[] pathWidth = new int[2];

        public int WallWidth
        {
            get { return wallWidth[M]; }
            set { wallWidth[M] = value; }
        }
        public bool WallWidthModified
        {
            get { return (wallWidth[M] != wallWidth[S]); }
        }
        private int[] wallWidth = new int[2];

        public int GridWidth
        {
            get { return gridWidth[M]; }
            set { gridWidth[M] = value; }
        }
        public bool GridWidthModified
        {
            get { return (gridWidth[M] != gridWidth[S]); }
        }
        private int[] gridWidth = new int[2];

        public bool AutoSquareWidth
        {
            get { return autoSquareWidth[M]; }
            set { autoSquareWidth[M] = value; }
        }
        public bool AutoSquareWidthModified
        {
            get { return (autoSquareWidth[M] != autoSquareWidth[S]); }
        }
        private bool[] autoSquareWidth = new bool[2] { true, true };

        public bool AutoPathWidth
        {
            get { return autoPathWidth[M]; }
            set { autoPathWidth[M] = value; }
        }
        public bool AutoPathWidthModified
        {
            get { return (autoPathWidth[M] != autoPathWidth[S]); }
        }
        private bool[] autoPathWidth = new bool[2] { true, true };

        public bool AutoWallWidth
        {
            get { return autoWallWidth[M]; }
            set { autoWallWidth[M] = value; }
        }
        public bool AutoWallWidthModified
        {
            get { return (autoWallWidth[M] != autoWallWidth[S]); }
        }
        private bool[] autoWallWidth = new bool[2] { true, true };

        public bool AutoGridWidth
        {
            get { return autoGridWidth[M]; }
            set { autoGridWidth[M] = value; }
        }
        public bool AutoGridWidthModified
        {
            get { return (autoGridWidth[M] != autoGridWidth[S]); }
        }
        private bool[] autoGridWidth = new bool[2] { true, true };

        public string ResultingArea
        {
            get { return resultingArea; }
            set { resultingArea = value; }
        }
        private string resultingArea;

        public List<string> CapStyleList
        {
            get { return capStyleList; }
            set { capStyleList = value; }
        }
        private List<string> capStyleList;

        /*
        public string CapStyle
        {
            get { return capStyle; }
            set { capStyle = value; }
        }
        private string capStyle;
         * */

        public System.Drawing.Drawing2D.LineCap PathCapStyle
        {
            get { return pathCapStyle[M]; }
            set { pathCapStyle[M] = value; }
        }
        public bool PathCapStyleModified
        {
            get { return (pathCapStyle[M] != pathCapStyle[S]); }
        }
        private System.Drawing.Drawing2D.LineCap[] pathCapStyle = new System.Drawing.Drawing2D.LineCap[2];

        #endregion

        #endregion

        #region Constructor

        public AriadneSettingsData()
        {
        }

        #endregion

        public void ClearModifedFlags()
        {
            squareWidth[S] = squareWidth[M];
            pathWidth[S] = pathWidth[M];
            wallWidth[S] = wallWidth[M];
            gridWidth[S] = gridWidth[M];

            autoSquareWidth[S] = autoSquareWidth[M];
            autoPathWidth[S] = autoPathWidth[M];
            autoWallWidth[S] = autoWallWidth[M];
            autoGridWidth[S] = autoGridWidth[M];
            
            pathCapStyle[S] = pathCapStyle[M];
        }
    }
}
