﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using SWA.Ariadne.Model;
using SWA.Ariadne.Outlines;

namespace SWA.Ariadne.Outlines.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Outlines.DistortedOutlineShape and is intended
    ///to contain all SWA.Ariadne.Outlines.DistortedOutlineShape Unit Tests
    ///</summary>
    [TestClass()]
    public class DistortedOutlineShapeTest
    {
        #region Test Context

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Unit tests

        /// <summary>
        ///A test for DistortedOutlineShape (int, int, SmoothOutlineShape, Distortion)
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_ManualTest_01()
        {
            int corners = 3, windings = 1;
            double slant = 0, centerX = 0.5, centerY = 0.5, shapeSize = 0.8;
            double distortionWinding = 0.25;

            TestDistortedPolygon(corners, windings, slant, centerX, centerY, shapeSize, distortionWinding);
        }

#if false
        /// <summary>
        ///A test for SpiralDistortion (Random, double, double, double, double)
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void SpiralDistortionTest()
        {
            Random r = null; // TODO: Initialize to an appropriate value

            double xCenter = 0; // TODO: Initialize to an appropriate value

            double yCenter = 0; // TODO: Initialize to an appropriate value

            double size = 0; // TODO: Initialize to an appropriate value

            double maxWindings = 0; // TODO: Initialize to an appropriate value

            SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor expected = null;
            SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor actual;

            actual = SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.SpiralDistortion(r, xCenter, yCenter, size, maxWindings);

            Assert.AreEqual(expected, actual, "SWA.Ariadne.Outlines.DistortedOutlineShape.SpiralDistortion did not return the ex" +
                    "pected value.");
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
#endif

#if false
        /// <summary>
        ///A test for this[double x, double y]
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void ItemTest()
        {
            int xSize = 0; // TODO: Initialize to an appropriate value

            int ySize = 0; // TODO: Initialize to an appropriate value

            SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_SmoothOutlineShapeAccessor baseShape = null; // TODO: Initialize to an appropriate value

            SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor distortion = null; // TODO: Initialize to an appropriate value

            OutlineShape target = SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.CreatePrivate(xSize, ySize, baseShape, distortion);

            bool val = false; // TODO: Assign to an appropriate value for the property

            SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor accessor = new SWA.Ariadne.Outlines.Tests.SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor(target);

            double x = 0; // TODO: Initialize to an appropriate value

            double y = 0; // TODO: Initialize to an appropriate value


            Assert.AreEqual(val, accessor[x, y], "SWA.Ariadne.Outlines.DistortedOutlineShape.this was not set correctly.");
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
#endif

        /// <summary>
        /// A test for RadialWaveDistortion (double, double, int, double, double)
        /// Four corners, pointing east.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_RadialWaveDistortionTest_01()
        {
            int n = 4;
            double s = 0;
            double m = 0.5;
            TestRadialWaveDistortion(n, s, m);
        }

        /// <summary>
        /// A test for RadialWaveDistortion (double, double, int, double, double)
        /// Four corners, pointing north.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_RadialWaveDistortionTest_02()
        {
            int n = 4;
            double s = 0.25;
            double m = 0.5;
            TestRadialWaveDistortion(n, s, m);
        }

        /// <summary>
        /// A test for RadialWaveDistortion (double, double, int, double, double)
        /// Four corners, pointing east.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_RadialWaveDistortionTest_03()
        {
            int n = 3;
            double s = 0;
            double m = 0.5;
            TestRadialWaveDistortion(n, s, m);
        }

        /// <summary>
        /// A test for RadialWaveDistortion (double, double, int, double, double)
        /// Four corners, pointing north.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_RadialWaveDistortionTest_04()
        {
            int n = 3;
            double s = 0.25;
            double m = 0.5;
            TestRadialWaveDistortion(n, s, m);
        }

        /// <summary>
        /// A test for RadialWaveDistortion (double, double, int, double, double)
        /// One edge is in the south.  Test the eastern corner on that edge.
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Outlines.dll")]
        [TestMethod()]
        public void DOS_RadialWaveDistortionTest_05()
        {
            double m = 0.5;

            for (int n = 3; n <= 12; n++)
            {
                double s = (n % 2 == 1 ? 0.25 : (n % 4 == 2 ? 0.00 : 0.5 / n));
                string testObject = string.Format("{0}_<{1};{2:0.00}>", "RadialWaveDistortion", n, s);

                SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor target;
                target = SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.RadialWaveDistortion(0, 0, n, s, m);

                double x, y;

                TestDistortion(testObject, target, 0, -1, 0, -2);

                SWA.Utilities.Geometry.PolarToRect(1, 2.0 * Math.PI * (0.75 + 0.5 / n), out x, out y);
                TestDistortionFixpoint(testObject, target, x, y);
            }
        }

        #endregion

        #region Auxiliary methods

        private static void TestDistortedPolygon(int corners, int windings, double slant, double centerX, double centerY, double shapeSize, double distortionWinding)
        {
            string testObject = string.Format(
                "DistortedOutlineShape: Polygon({0}) @ ({1:0.##}, {2:0.##}), Spiral({3:0.##})",
                corners, centerX, centerY, distortionWinding);

            MazeTestForm.MazeConfiguratorDelegate mazeConfigurator = DistortedPolygonConfiguratorDelegate(corners, windings, slant, centerX, centerY, shapeSize, distortionWinding);

            MazeTestForm form = new MazeTestForm(mazeConfigurator);
            form.Text = testObject;
            DialogResult result = form.ShowDialog();

            Assert.AreEqual(DialogResult.OK, result, testObject);
        }

        public static MazeTestForm.MazeConfiguratorDelegate DistortedPolygonConfiguratorDelegate(int corners, int windings, double slant, double centerX, double centerY, double shapeSize, double distortionWinding)
        {
            MazeTestForm.MazeConfiguratorDelegate mazeConfigurator = delegate(Maze maze)
            {
                int xSize = maze.XSize, ySize = maze.YSize;
                OutlineShape baseShape = SWA_Ariadne_Outlines_PolygonOutlineShapeAccessor.CreatePrivate(corners, windings, slant, xSize, ySize, centerX, centerY, shapeSize);

                SWA_Ariadne_Outlines_SmoothOutlineShapeAccessor baseShapeAccessor = new SWA_Ariadne_Outlines_SmoothOutlineShapeAccessor(baseShape);

                double xCenter, yCenter, size;
                SWA_Ariadne_Outlines_OutlineShapeAccessor.ConvertParameters(xSize, ySize, centerX, centerY, shapeSize, out xCenter, out yCenter, out size);
                SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor distortion = SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.SpiralDistortion(xCenter, yCenter, size, distortionWinding);

                OutlineShape targetShape = SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.CreatePrivate(xSize, ySize, baseShapeAccessor, distortion);

                maze.OutlineShape = targetShape;
            };
            return mazeConfigurator;
        }

        private static void TestRadialWaveDistortion(int n, double s, double m)
        {
            double xCenter = 0;
            double yCenter = 0;
            string testObject = string.Format("{0}_<{1};{2:0.00}>", "RadialWaveDistortion", n, s);

            SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor target;
            target = SWA_Ariadne_Outlines_DistortedOutlineShapeAccessor.RadialWaveDistortion(xCenter, yCenter, n, s, m);

            double x, y;

            TestDistortionFixpoint(testObject, target, 0.0, 0.0);

            SWA.Utilities.Geometry.PolarToRect(1, 2.0 * Math.PI * (s + 0.0 / n), out x, out y);
            TestDistortionFixpoint(testObject, target, x, y);

            SWA.Utilities.Geometry.PolarToRect(1, 2.0 * Math.PI * (s + 1.0 / n), out x, out y);
            TestDistortionFixpoint(testObject, target, x, y);

            SWA.Utilities.Geometry.PolarToRect(1, 2.0 * Math.PI * (s + 0.5 / n), out x, out y);
            TestDistortion(testObject, target, x, y, x / m, y / m);

            SWA.Utilities.Geometry.PolarToRect(1, 2.0 * Math.PI * (s - 0.5 / n), out x, out y);
            TestDistortion(testObject, target, x, y, x / m, y / m);
        }

        private static void TestDistortionFixpoint(string testObject, SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor target, double x, double y)
        {
            TestDistortion(testObject + "_Fixpoint", target, x, y, x, y);
        }

        private static void TestDistortion(string testObject, SWA_Ariadne_Outlines_DistortedOutlineShape_DistortionAccessor target, double x, double y, double xExpected, double yExpected)
        {
            double xActual = x, yActual = y;
            target(ref xActual, ref yActual);
            Assert.AreEqual(xExpected, xActual, 1e-6, string.Format("{0}({1:0.00},{2:0.00}).x", testObject, x, y));
            Assert.AreEqual(yExpected, yActual, 1e-6, string.Format("{0}({1:0.00},{2:0.00}).y", testObject, x, y));
        }

        #endregion
    }
}
