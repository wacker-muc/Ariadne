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
    public partial class PolygonTestForm : MazeTestForm
    {
        public PolygonTestForm()
        {
            InitializeComponent();

            this.mazeConfigurator = this.PolygonConfigurator;
        }

        private void PolygonConfigurator(Maze maze)
        {
            int n = (int)this.cornersNumericUpDown.Value;
            int w = Math.Min((n-1)/2, (int)this.windingsNnumericUpDown.Value);
            double s = (double)this.slantNumericUpDown.Value / n * 2.0 * Math.PI;
            maze.OutlineShape = SWA_Ariadne_Outlines_PolygonOutlineShapeAccessor.CreatePrivate(n, w, s, maze.XSize, maze.YSize, 0.5, 0.5, 1.0);
            if (this.distortedCheckBox.Checked)
            {
                maze.OutlineShape = maze.OutlineShape.DistortedCopy(new Random());
            }
        }
    }
}