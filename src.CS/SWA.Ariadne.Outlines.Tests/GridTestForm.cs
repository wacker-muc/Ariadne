using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;

namespace SWA.Ariadne.Outlines.Tests
{
    public partial class GridTestForm : MazeTestForm
    {
        public GridTestForm()
        {
            InitializeComponent();

            this.mazeConfigurator = this.GridConfigurator;
        }

        private void GridConfigurator(Maze maze)
        {
            int xSize = maze.XSize;
            int ySize = maze.YSize;
            int width = (int)this.gridShapeWidthNumericUpDown.Value;
            int height = (int)this.gridShapeHeightNumericUpDown.Value;
            double diameter = (double)this.diameterNumericUpDown.Value;
            bool invertEveryOtherTile = this.checkeredCheckBox.Checked;

            Object gridElementObj = null;

            if (this.squaresRadioButton.Checked)
            {
                gridElementObj = SWA_Ariadne_Outlines_BlackGridElementAccessor.CreatePrivate(width, height);
            }
            else if (this.circlesRadioButton.Checked)
            {
                gridElementObj = SWA_Ariadne_Outlines_CircleGridElementAccessor.CreatePrivate(width, height, diameter);
            }

            if (gridElementObj != null)
            {
                SWA_Ariadne_Outlines_TileGridElementAccessor tgea = new SWA_Ariadne_Outlines_TileGridElementAccessor(gridElementObj);
                maze.OutlineShape = (OutlineShape)SWA_Ariadne_Outlines_GridOutlineShapeAccessor.CreateCheckeredInstance(xSize, ySize, tgea, invertEveryOtherTile).Target;
            }
        }
    }
}