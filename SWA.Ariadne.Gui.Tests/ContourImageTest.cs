﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SWA.Ariadne.Gui.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Gui.Mazes.ContourImage and is intended
    ///to contain all SWA.Ariadne.Gui.Mazes.ContourImage Unit Tests
    ///</summary>
    [TestClass()]
    public class ContourImageTest
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
        ///A test for CreateFrom (Bitmap, out Bitmap)
        ///</summary>
        [TestMethod()]
        public void CI_ManualTest_01()
        {
            string testObject = "ContourImage";

            TestForm form = new ContourImageTestForm();
            form.Text = testObject;
            DialogResult result = form.ShowDialog();

            Assert.AreEqual(DialogResult.OK, result, testObject);
        }

        /// <summary>
        ///A test for PrepareInfluenceRegions (int)
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_PrepareInfluenceRegionsTest_01()
        {
            string testObject = "Distance from contour pixel";
            int radius = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.ContourDistance;
            int influenceRange = radius + 1;

            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.PrepareInfluenceRegions(influenceRange);

            int dyN = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbDY[SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbN];
            int dyS = -dyN;
            int dxW = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbDX[SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbW];
            int dxE = -dxW;

            int nbL = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbW;
            int nbR = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbS;

            TestInfluence(testObject, nbL, nbR, radius * dxE, 0, +1);   // E
            TestInfluence(testObject, nbL, nbR, radius * dxE, dyN, +1); // E + 1px N
            TestNoInfluence(testObject, nbL, nbR, radius * dxE, dyS);   // E + 1Px S

            TestInfluence(testObject, nbL, nbR, 0, radius * dyN, 0);    // N
            TestInfluence(testObject, nbL, nbR, dxE, radius * dyN, 0);  // N + 1px E
            TestNoInfluence(testObject, nbL, nbR, dxW, radius * dyN);   // N + 1px W
        }

        /// <summary>
        ///A test for PrepareInfluenceRegions (int)
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_PrepareInfluenceRegionsTest_02()
        {
            string testObject = "Distance from contour pixel";
            int radius = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.ContourDistance;
            int influenceRange = radius + 1;

            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.PrepareInfluenceRegions(influenceRange);

            int dyN = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbDY[SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbN];
            int dyS = -dyN;
            int dxW = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbDX[SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbW];
            int dxE = -dxW;

            int nbL = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbW;
            int nbR = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.NbE;

            TestInfluence(testObject, nbL, nbR, 0, radius * dyN, 0);    // N
            TestNoInfluence(testObject, nbL, nbR, dxW, radius * dyN);   // N + 1px W
            TestNoInfluence(testObject, nbL, nbR, dxE, radius * dyN);   // N + 1px E
        }

        #region Unit tests for ScanObject()

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_01_Rectangle()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            g.FillRectangle(fgBrush, rect);
            testObject += "(Rectangle " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_02_Ellipse()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            g.FillEllipse(fgBrush, rect);
            testObject += "(Ellipse " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_03_FlatDiamond()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 12, cy - 4, 24, 8);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            g.FillPolygon(fgBrush, new Point[] { pE, pN, pW, pS });
            testObject += "(Diamond " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_04_SlimDiamond()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 4, cy - 12, 8, 24);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            g.FillPolygon(fgBrush, new Point[] { pE, pN, pW, pS });
            testObject += "(Diamond " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_05_VerticalLine()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            g.DrawLine(fgPen, pN, pS);

            testObject += "(VerticalLine " + pN.ToString() + "-" + pS.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_06_HorizontalLine()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            g.DrawLine(fgPen, pW, pE);

            testObject += "(HorizontalLine " + pW.ToString() + "-" + pE.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_07_UprightCross()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            g.DrawLine(fgPen, pN, pS);
            g.DrawLine(fgPen, pW, pE);

            testObject += "(UprightCross " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 1);
        }

        /// <summary>
        ///A test for ScanObject (Bitmap, int, int, Color, float, int[,], List&lt;int&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_ScanObjectTest_08_DiagonalCross()
        {
            string testObject = "ControurImage.ScanObject";

            Color backgroundColor = Color.Black;
            Color foregroundColor = Color.White;
            Brush fgBrush = new SolidBrush(foregroundColor);
            Pen fgPen = new Pen(fgBrush, 1);

            Bitmap image = new Bitmap(63, 63);
            Graphics g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(backgroundColor), 0, 0, image.Width, image.Height);

            // When painting into the image, leave an outer frame of 16 pixels free.

            int cx = (image.Width + 1) / 2, cy = (image.Height + 1) / 2;
            Rectangle rect = new Rectangle(cx - 8, cy - 4, 16, 8);
            Point pE = new Point(rect.Right - 1, (rect.Top + rect.Bottom) / 2);
            Point pN = new Point((rect.Left + rect.Right) / 2, rect.Top);
            Point pW = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
            Point pS = new Point((rect.Left + rect.Right) / 2, rect.Bottom - 1);
            Point pNW = new Point(rect.Left, rect.Top);
            Point pNE = new Point(rect.Right - 1, rect.Top);
            Point pSW = new Point(rect.Left, rect.Bottom - 1);
            Point pSE = new Point(rect.Right - 1, rect.Bottom - 1);
            g.DrawLine(fgPen, pNE, pSW);
            g.DrawLine(fgPen, pNW, pSE);

            testObject += "(DiagonalCross " + rect.ToString() + ")";

            TestScan(testObject, image, backgroundColor, 2);
        }

        #endregion

        #region Unit tests for EliminateOverlaps()

        /// <summary>
        ///A test for EliminateOverlaps (List&lt;int&gt;[], List&lt;bool&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_EliminateOverlapsTest_01()
        {
            string testObject = "EliminateOverlaps";

            // Prepare data structures used in the test.
            List<int>[] borderXs = new List<int>[1];
            List<bool>[] borderXsLR = new List<bool>[1];
            List<int> borderX = borderXs[0] = new List<int>();
            List<bool> borderLR = borderXsLR[0] = new List<bool>();
            borderX.Add(1000);
            borderLR.Add(true);

            InsertBorderLR(borderX, borderLR, 100, 900);
            InsertBorderLR(borderX, borderLR, 200, 800);

            int[] expectedPoints = { 100, 900 };
            TestEliminateOverlaps(testObject, borderXs, borderXsLR, expectedPoints);
        }

        /// <summary>
        ///A test for EliminateOverlaps (List&lt;int&gt;[], List&lt;bool&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_EliminateOverlapsTest_02()
        {
            string testObject = "EliminateOverlaps";

            // Prepare data structures used in the test.
            List<int>[] borderXs = new List<int>[1];
            List<bool>[] borderXsLR = new List<bool>[1];
            List<int> borderX = borderXs[0] = new List<int>();
            List<bool> borderLR = borderXsLR[0] = new List<bool>();
            borderX.Add(1000);
            borderLR.Add(true);

            InsertBorderLR(borderX, borderLR, 100, 300);
            InsertBorderLR(borderX, borderLR, 500, 700);
            InsertBorderLR(borderX, borderLR, 700, 900);
            InsertBorderLR(borderX, borderLR, 300, 500);

            int[] expectedPoints = { 100, 900 };
            TestEliminateOverlaps(testObject, borderXs, borderXsLR, expectedPoints);
        }

        /// <summary>
        ///A test for EliminateOverlaps (List&lt;int&gt;[], List&lt;bool&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_EliminateOverlapsTest_03()
        {
            string testObject = "EliminateOverlaps";

            // Prepare data structures used in the test.
            List<int>[] borderXs = new List<int>[1];
            List<bool>[] borderXsLR = new List<bool>[1];
            List<int> borderX = borderXs[0] = new List<int>();
            List<bool> borderLR = borderXsLR[0] = new List<bool>();
            borderX.Add(1000);
            borderLR.Add(true);

            InsertBorderLR(borderX, borderLR, 100, 400);
            InsertBorderLR(borderX, borderLR, 500, 900);

            int[] expectedPoints = { 100, 400, 500, 900 };
            TestEliminateOverlaps(testObject, borderXs, borderXsLR, expectedPoints);
        }

        /// <summary>
        ///A test for EliminateOverlaps (List&lt;int&gt;[], List&lt;bool&gt;[])
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Gui.Mazes.dll")]
        [TestMethod()]
        public void CI_EliminateOverlapsTest_04()
        {
            string testObject = "EliminateOverlaps";

            // Prepare data structures used in the test.
            List<int>[] borderXs = new List<int>[1];
            List<bool>[] borderXsLR = new List<bool>[1];
            List<int> borderX = borderXs[0] = new List<int>();
            List<bool> borderLR = borderXsLR[0] = new List<bool>();
            borderX.Add(1000);
            borderLR.Add(true);

            InsertBorderLR(borderX, borderLR, 100, 300);
            InsertBorderLR(borderX, borderLR, 500, 700);
            InsertBorderLR(borderX, borderLR, 700, 900);
            InsertBorderLR(borderX, borderLR, 300, 500);

            InsertBorderLR(borderX, borderLR, 100, 900);
            InsertBorderLR(borderX, borderLR, 400, 600);
            InsertBorderLR(borderX, borderLR, 600, 800);
            InsertBorderLR(borderX, borderLR, 400, 800);
            InsertBorderLR(borderX, borderLR, 550, 850);

            int[] expectedPoints = { 100, 900 };
            TestEliminateOverlaps(testObject, borderXs, borderXsLR, expectedPoints);
        }

        #endregion

        #endregion

        #region Auxiliary methods

        private static void TestInfluence(string testObject, int nbL, int nbR, int x, int y, int btExpected)
        {
            testObject += string.Format("<{0},{1}>: [{2},{3}]", nbL, nbR, x, y);
            int d2 = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.InfluenceD2(nbL, nbR, x, y);

            Assert.IsTrue(d2 < int.MaxValue, testObject + " should be influenced");
            Assert.AreEqual(x * x + y * y, d2, testObject + " is not correct");

            int btActual = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.BorderLimitType(nbL, nbR, x, y);
            Assert.AreEqual(btExpected, btActual, testObject + " has the wrong border type");
        }

        private static void TestNoInfluence(string testObject, int nbL, int nbR, int x, int y)
        {
            testObject += string.Format("<{0},{1}>: [{2},{3}]", nbL, nbR, x, y);
            int d2 = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.InfluenceD2(nbL, nbR, x, y);
            
            Assert.IsFalse(d2 < int.MaxValue, testObject + " should not be influence");
        }

        private static void TestScan(string testObject, Bitmap image, Color backgroundColor, int maxContourScanRegions)
        {
            for (int y = 0; y < image.Height; y++)
            {
                TestScan(testObject, image, backgroundColor, y, maxContourScanRegions);
            }
        }

        private static void TestScan(string testObject, Bitmap image, Color backgroundColor, int y0, int maxContourScanRegions)
        {
            testObject += " @ " + y0.ToString();
            float fuzziness = 0.1F;

            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.PrepareInfluenceRegions(SWA_Ariadne_Gui_Mazes_ContourImageAccessor.GetFrameWidth(backgroundColor));

            #region Find the leftmost object pixel on the scan line at y0.

            int x0 = 0;
            while (SWA_Ariadne_Gui_Mazes_ContourImageAccessor.ColorDistance(image.GetPixel(x0, y0), backgroundColor) <= fuzziness)
            {
                x0++;
                if (x0 >= image.Width)
                {
                    return;
                }
            }

            #endregion

            #region Prepare required data structures.

            int[,] dist2ToImage = new int[image.Width, image.Height];

            #region Initialize dist2ToImage.
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    dist2ToImage[x, y] = int.MaxValue;
                }
            }
            #endregion

            List<int>[] contourXs = new List<int>[image.Height];
            List<int>[] borderXs = new List<int>[image.Height];
            List<bool>[] borderXsLR = new List<bool>[image.Height];

            #region Initialize contourXs and borderXs.
            for (int i = 0; i < image.Height; i++)
            {
                // Create the lists.
                contourXs[i] = new List<int>(5);
                borderXs[i] = new List<int>(5);
                borderXsLR[i] = new List<bool>(5);

                // Add termination points that are border of the scanned image.
                contourXs[i].Add(image.Width);
                borderXs[i].Add(image.Width);
                borderXsLR[i].Add(true);
            }
            #endregion

            #endregion

            bool found = SWA_Ariadne_Gui_Mazes_ContourImageAccessor.ScanObject(image, x0, y0, backgroundColor, fuzziness, dist2ToImage, contourXs, borderXs, borderXsLR);

            #region Test if the contour map is well formed.
            for (int i = 0; i < image.Height; i++)
            {
                int nEntries = contourXs[i].Count;
                int m = nEntries % 2;
                Assert.AreEqual(1, m, testObject + string.Format(" - contourXs[{0}] must be an odd number: {1}", i, nEntries));

                int nRegions = (nEntries - 1) / 2;
                Assert.IsTrue(nRegions <= maxContourScanRegions, testObject + string.Format(" - contourXs[{0}] regions = {1} must be less than {2}", i, nRegions, maxContourScanRegions));
            }
            #endregion

            #region Test if the contour map is complete.
            int imageArea = ImageArea(image, backgroundColor, fuzziness);
            int contourArea = ContourArea(contourXs);
            Assert.AreEqual(imageArea, contourArea, testObject + string.Format(" - contour area and image must be equal"));
            #endregion

            #region Test if the border map is well formed.
            for (int i = 0; i < image.Height; i++)
            {
                string testLine = testObject + string.Format(" - borderXs[{0}]", i);

                int nEntries = borderXs[i].Count;
                int nEntriesLR = borderXsLR[i].Count;
                int m = nEntries % 2;
                Assert.AreEqual(1, m, testLine + string.Format(" must be an odd number: {0}", nEntries));
                Assert.AreEqual(nEntries, nEntriesLR, testLine + " must have the same number of LR entries");

                int n = 0;
                for (int p = 0; p < nEntries - 1; p++)
                {
                    int q = p + 1;
                    int xp = borderXs[i][p], xq = borderXs[i][q];
                    Assert.IsTrue(xp <= xq, testLine + string.Format(" must be sorted: [{0}] = {1}, [{2}] = {3}", p, xp, q, xq));

                    n += (borderXsLR[i][p] == true /*left*/ ? +1 : -1);
                    if (p == nEntries - 2)
                    {
                        Assert.IsTrue(n == 0, testLine + " final LR balance must be zero");
                    }
                    else
                    {
                        Assert.IsTrue(n > 0, testLine + " intermediate LR balance must always be positive");
                    }
                }

                /*
                int nRegions = (nEntries - 1) / 2;
                Assert.IsTrue(nRegions <= maxContourScanRegions, testObject + string.Format(" - contourXs[{0}] regions = {1} must be less than {2}", i, nRegions, maxContourScanRegions));
                 */
            }
            #endregion

        }

        private static int ImageArea(Bitmap image, Color backgroundColor, float fuzziness)
        {
            int result = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (Math.Abs(image.GetPixel(x, y).GetBrightness() - backgroundColor.GetBrightness()) > fuzziness)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        private static int ContourArea(List<int>[] contourXs)
        {
            int result = 0;

            for (int i = 0; i < contourXs.Length; i++)
            {
                for (int p = 0; p + 1 < contourXs[i].Count; p += 2)
                {
                    int w = contourXs[i][p + 1] - contourXs[i][p] + 1;
                    result += w;
                }
            }

            return result;
        }

        private static void InsertBorderLR(List<int> borderX, List<bool> borderLR, int left, int right)
        {
            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.InsertBorderPoint(borderX, borderLR, left, true);
            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.InsertBorderPoint(borderX, borderLR, right, false);
        }

        private static void TestEliminateOverlaps(string testObject, List<int>[] borderXs, List<bool>[] borderXsLR, int[] expectedPoints)
        {
            SWA_Ariadne_Gui_Mazes_ContourImageAccessor.EliminateOverlaps(borderXs, borderXsLR);

            Assert.AreEqual(expectedPoints.Length + 1, borderXs[0].Count, testObject + " left a wrong number of points");
            for (int i = 0; i < expectedPoints.Length; i++)
            {
                Assert.AreEqual(expectedPoints[i], borderXs[0][i], testObject + string.Format(" left a wrong entry at [{0}]", i));
            }
        }

        #endregion
    }
}
