using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Outlines
{
    /// <summary>
    /// An OutlineShape with a pattern based on repeating rectangular tiles.
    /// </summary>
    internal class TilesOutlineShape : OutlineShape
    {
        #region Member variables and Properties

        /// <summary>
        /// The pattern is stored in an ExplicitOutlineShape.
        /// </summary>
        private ExplicitOutlineShape tile;

        /// <summary>
        /// Each column in the tile can be repeated more than once.
        /// Dimensions: tile.XSize, tile.YSize.
        /// </summary>
        private int[] xRepetitions, yRepetitions;

        /// <summary>
        /// The effective tile size, resulting from the repetitions.
        /// </summary>
        private int xTileSize, yTileSize;

        /// <summary>
        /// Mapping of shape coordinates to tile coordinates.
        /// Dimensions: xTileSize, yTileSize.
        /// </summary>
        private int[] xMap, yMap;

        public override bool this[int x, int y]
        {
            get
            {
                return tile[xMap[x % xTileSize], yMap[y % yTileSize]];
            }
        }
        public void SetValue(int x, int y, bool value)
        {
            tile.SetValue(x, y, value);
        }

        #endregion

        #region Constructor

         TilesOutlineShape(int xSize, int ySize, int xTileSize, int yTileSize)
            : base(xSize, ySize)
        {
            this.tile = new ExplicitOutlineShape(xTileSize, yTileSize);

            this.xRepetitions = new int[xTileSize];
            this.yRepetitions = new int[yTileSize];

            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xRepetitions[i] = 1;
            }
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yRepetitions[i] = 1;
            }

            UpdateTileSize();
        }

        #endregion

        #region Static methods for creating OutlineShapes

        public static OutlineShape RandomInstance(Random r, int xSize, int ySize)
        {
            // TODO: more patterns
            switch (r.Next(1))
            {
                default:
                case 0:
                    return StripesOrGrid(r, xSize, ySize);
                case 1:
                    return Ribbons(r, xSize, ySize);
            }
        }

        private static OutlineShape StripesOrGrid(Random r, int xSize, int ySize)
        {
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, 2, 2);

            int k = r.Next(1, 4); // 1, 2, 3

            result.SetValue(0, 0, (k != 3 || r.Next(2) == 0));  // maybe a disconnected grid
            result.SetValue(0, 1, ((k & 1) != 0));              // horizontal lines
            result.SetValue(1, 0, ((k & 2) != 0));              // vertical lines
            result.SetValue(1, 1, false);

            result.SetRepetitions(0, 1);                        // one-square pinstripes
            if (r.Next(0) == 0)
            {
                result.SetRepetitions(1, r.Next(4, 16));        // same x and y width
            }
            else
            {
                while (Math.Abs(result.xRepetitions[1] - result.yRepetitions[1]) < 5)
                {
                    result.SetXRepetitions(1, r.Next(4, 21));   // different x and y width
                    result.SetYRepetitions(1, r.Next(4, 21));
                }
            }

            if (result.tile[0, 0] == false && result.xRepetitions[1] > 7 && result.yRepetitions[1] > 7)
            {
                result.SetRepetitions(0, r.Next(1, 3));         // thin stripes: one or two squares
            }

            return result;
        }

        /// <summary>
        /// Builds a pattern of interwoven ribbons.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <returns></returns>
        private static OutlineShape Ribbons(Random r, int xSize, int ySize)
        {
            TilesOutlineShape result = new TilesOutlineShape(xSize, ySize, 8, 8);

            #region Start with filled squares in all but the (odd,odd) positions.

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    result.SetValue(x, y, (x % 2 == 0 || y % 2 == 0));
                }
            }

            // result: a dense grid
            /*
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             *   x x x x x x x x
             *   x o x o x o x o
             */

            #endregion

            int p = 5, q = 3;

            #region Erase four pairs of squares

            result.SetValue(p + 2, q - 3, false);
            result.SetValue(p + 2, q + 3, false);

            result.SetValue(p - 2, q - 1, false);
            result.SetValue(p - 2, q + 1, false);

            result.SetValue(q - 3, p - 2, false);
            result.SetValue(q + 3, p - 2, false);

            result.SetValue(q - 1, p + 2, false);
            result.SetValue(q + 1, p + 2, false);

            // result: four interwoven ribbons
            /*
             *   x x x x x x x O
             *   x o x o x o x o
             *   x x x O x x x x
             *   O o x o x o O o
             *   x x x O x x x x
             *   x o x o x o x o
             *   x x x x x x x O
             *   x o O o O o x o
             */

            #endregion

            #region Optionally, erase more pairs of squares; the shape will no longer be totally connected

            if (r.Next(2) == 0)
            {
                for (int i = 0; i < 8; i += 2)
                {
                    result.SetValue(q - 2, i, false);
                    result.SetValue(q + 2, i, false);
                }
            }
            if (r.Next(2) == 0)
            {
                for (int i = 0; i < 8; i += 2)
                {
                    result.SetValue(i, q - 2, false);
                    result.SetValue(i, q + 2, false);
                }
            }

            // with one set erased: rectangular frames
            // with two sets erased: disconnected bars
            /*
             *   x a x x x a x o
             *   b o b o b o b o
             *   x a x o x a x x
             *   o o x o x o o o
             *   x a x o x a x x
             *   b o b o b o b o
             *   x a x x x a x o
             *   x o o o o o x o
             */

            #endregion

            #region Extend the shape width.

            int r3 = r.Next(6, 21);                 // the broad ribbon width
            int r2 = 1 + r.Next(2);                 // the ribbon border
            int r1 = r3 / 6 + (r.Next(3) - 1);      // the distance between parallel ribbons

            for (int i = 2; i < 8; i += 4)
            {
                result.SetRepetitions(i - 1, r1);
                result.SetRepetitions(i + 1, r3);
            }

            for (int i = 0; i < 8; i += 2)
            {
                result.SetRepetitions(i, r2);
            }

            #endregion

            return result;
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// Set the repetition number of the given tile position (x coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetXRepetitions(int pos, int num)
        {
            xRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        /// <summary>
        /// Set the repetition number of the given tile position (y coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetYRepetitions(int pos, int num)
        {
            yRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        /// <summary>
        /// Set both repetition numbers of the given tile position (x and y coordinate).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="num"></param>
        private void SetRepetitions(int pos, int num)
        {
            xRepetitions[pos] = Math.Max(0, num);
            yRepetitions[pos] = Math.Max(0, num);
            UpdateTileSize();
        }

        private void UpdateTileSize()
        {
            xTileSize = yTileSize = 0;

            for (int i = 0; i < xRepetitions.Length; i++)
            {
                xTileSize += xRepetitions[i];
            }
            for (int i = 0; i < yRepetitions.Length; i++)
            {
                yTileSize += yRepetitions[i];
            }

            UpdateCoordinateMapping();
        }

        private void UpdateCoordinateMapping()
        {
            xMap = new int[xTileSize];
            yMap = new int[yTileSize];

            // Determine the origin of a tile centered in the drawing area.
            int x0 = (XSize - xTileSize) / 2, y0 = (YSize - yTileSize) / 2;

            #region Derive the xMap and yMap of a (wrapped) tile in the top left corner of the shape.

            for (int xt = 0, xm = 0; xt < tile.XSize; xt++)
            {
                for (int i = 0; i < xRepetitions[xt]; i++, xm++)
                {
                    xMap[(xm + x0) % xTileSize] = xt;
                }
            }
            for (int yt = 0, ym = 0; yt < tile.YSize; yt++)
            {
                for (int i = 0; i < xRepetitions[yt]; i++, ym++)
                {
                    yMap[(ym + y0) % yTileSize] = yt;
                }
            }

            #endregion
        }

        #endregion
    }
}
