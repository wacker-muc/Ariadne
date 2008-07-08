﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;

namespace SWA.Ariadne.Model.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Model.Maze and is intended
    ///to contain all SWA.Ariadne.Model.MazeDimensions Unit Tests
    ///</summary>
    [TestClass()]
    public class MazeDimensionsTest
    {
        #region TestContext

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

        #region Unit Tests

        /// <summary>
        ///A test for MaxXSize and MaxYSize
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Model.dll")]
        [TestMethod()]
        public void MD_MaxSizeTest_01()
        {
            string testObject = "MazeDimensions.MaxXSize and MazeDimensions.MaxYSize (version 0)";
            int version = 0;
            SWA_Ariadne_Model_MazeDimensionsAccessor dimensionsObj = SWA_Ariadne_Model_MazeDimensionsAccessor.Instance(version);
            SWA_Ariadne_Model_MazeCodeAccessor codeObj = SWA_Ariadne_Model_MazeCodeAccessor.Instance(version);

            // simulate the multiplications executed in Maze.Code:

            double f1 = codeObj.SeedLimit;
            double f2 = dimensionsObj.MaxXSize - dimensionsObj.MinSize + 1;
            double f3 = dimensionsObj.MaxYSize - dimensionsObj.MinSize + 1;
            double f4 = dimensionsObj.MaxBorderDistance + 1;
            double f5 = dimensionsObj.MaxBorderDistance + 1;
            double f6 = dimensionsObj.MaxXSize + 1;
            double f7 = dimensionsObj.MaxXSize + 1;
            double f8 = MazeSquare.WP_NUM;

            double maxCodeValue = (f1 * f2 * f3 * f4 * f5 * f6 * f7 * f8) - 1;

            TestMaxCodeValue(testObject, maxCodeValue, codeObj.CodeLength, codeObj.CodeDigitRange);
        }

        /// <summary>
        ///A test for Instance (int)
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Model.dll")]
        [TestMethod()]
        public void MD_InstanceTest_01()
        {
            string testObject = "MazeDimensions.Instance(0)";
            int version = 0;

            SWA_Ariadne_Model_MazeDimensionsAccessor target = SWA_Ariadne_Model_MazeDimensionsAccessor.Instance(version);

            int expected, actual;

            expected = 341; // manually calculated
            actual = target.MaxXSize;
            Assert.AreEqual(expected, actual, testObject + ": wrong MaxXSize value");

            expected = 255; // manually calculated
            actual = target.MaxYSize;
            Assert.AreEqual(expected, actual, testObject + ": wrong MaxYSize value");
        }

        #endregion

        #region Auxiliary methods

        private static void TestMaxCodeValue(string testObject, double maxCodeValue, int codeLength, int codeDigitRange)
        {
            double codeRange = Math.Pow(codeDigitRange, codeLength);

            Assert.IsTrue(codeRange < long.MaxValue,
                testObject
                + ": Code range would exceed an Int64 representation: "
                + codeRange.ToString("#,##0") + " >= "
                + long.MaxValue.ToString("#,##0")
                );

            Assert.IsTrue(maxCodeValue < codeRange,
                testObject
                + ": resulting Code would exceed the Code range: "
                + maxCodeValue.ToString("#,##0") + " >= "
                + codeRange.ToString("#,##0")
                );

            codeRange /= codeDigitRange;
            Assert.IsFalse(maxCodeValue < codeRange,
                testObject
                + ": resulting Code would fit into shorter Code range: "
                + maxCodeValue.ToString("#,##0") + " >= "
                + codeRange.ToString("#,##0")
                );
        }

        #endregion
    }
}
