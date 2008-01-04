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

        #region IAriadneSettingsSource support

        public void FillFrom(IAriadneSettingsSource target)
        {
            target.FillParametersInto(this);
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
