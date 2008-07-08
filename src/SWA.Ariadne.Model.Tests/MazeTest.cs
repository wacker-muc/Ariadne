﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Model.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Model.Maze and is intended
    ///to contain all SWA.Ariadne.Model.Maze Unit Tests
    ///</summary>
    [TestClass()]
    public class MazeTest
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

        #region Constructor Tests

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_size_01()
        {
            string testObject = "Maze.Constructor(size)-01-small";
            int xSize = 40;
            int ySize = 40;

            ConstructorTestInsideBounds(testObject, xSize, ySize);
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_size_02()
        {
            string testObject = "Maze.Constructor(size)-02-max";
            int version = 0;
            SWA_Ariadne_Model_MazeDimensionsAccessor dimensionsObj = SWA_Ariadne_Model_MazeDimensionsAccessor.Instance(version);
            int xSize = int.MaxValue;
            int ySize = int.MaxValue;
            int xSizeExpected = dimensionsObj.MaxXSize;
            int ySizeExpected = dimensionsObj.MaxYSize;

            ConstructorTestOutsideBounds(testObject, version, xSize, ySize, xSizeExpected, ySizeExpected);
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_size_03()
        {
            string testObject = "Maze.Constructor(size)-03-large";

            // screen dimensions: 1400 x 1050
            int xSize = (1400 - 7) / 5;
            int ySize = (1050 - 7) / 5;

            ConstructorTestInsideBounds(testObject, xSize, ySize);
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_size_04()
        {
            string testObject = "Maze.Constructor(size)-03-huge";
            int xSize = ((2 * 1024 - 7) / 6);
            int ySize = ((2 * 768 - 7) / 6);

            ConstructorTestInsideBounds(testObject, xSize, ySize);
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_size_05()
        {
            string testObject = "Maze.Constructor(size)-05-min";
            int version = 0;
            SWA_Ariadne_Model_MazeDimensionsAccessor dimensionsObj = SWA_Ariadne_Model_MazeDimensionsAccessor.Instance(version);
            int xSize = 0;
            int ySize = 0;
            int xSizeExpected = dimensionsObj.MinSize;
            int ySizeExpected = dimensionsObj.MinSize;

            ConstructorTestOutsideBounds(testObject, version, xSize, ySize, xSizeExpected, ySizeExpected);
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_code_01()
        {
            string testObject = "Maze.Constructor(string)-01";
            int version = 0;

            Maze template = new Maze(87, 34, version);
            template.CreateMaze();

            string templateCode = template.Code;

            Maze target = new Maze(templateCode);
            target.CreateMaze();

            string targetCode = target.Code;

            Assert.AreEqual(templateCode, targetCode, testObject + ": Code differs");
            Assert.IsTrue(template.StartSquare.XPos == target.StartSquare.XPos
                && template.StartSquare.YPos == target.StartSquare.YPos,
                testObject + ": StartSquare differs");
            Assert.IsTrue(template.EndSquare.XPos == target.EndSquare.XPos
                && template.EndSquare.YPos == target.EndSquare.YPos,
                testObject + ": EndSquare differs");
        }

        /// <summary>
        ///A test for Maze (string)
        ///</summary>
        [TestMethod()]
        public void M_ConstructorTest_code_02()
        {
            string testObject = "Maze.Constructor(string)-02";

            // These are a few codes of actual mazes.
            CreateMazeTest(testObject, "FCGC.OIUA.JZRX");
            CreateMazeTest(testObject, "KWOF.WDEI.TGGD");
            CreateMazeTest(testObject, "FNYK.QJEA.AJFL");
            CreateMazeTest(testObject, "OKRQ.YJFO.KQUN");
        }

        private static void CreateMazeTest(string testObject, string mazeCode)
        {
            Maze maze = new Maze(mazeCode);
            maze.CreateMaze();

            Assert.AreEqual(mazeCode, maze.Code, testObject + ": wrong code");
        }

        #endregion

        #endregion

        #region Auxiliary methods

        private static void ConstructorTestInsideBounds(string testObject, int xSize, int ySize)
        {
            Maze target = new Maze(xSize, ySize);

            Assert.AreEqual(xSize, target.XSize, testObject + ": wrong XSize");
            Assert.AreEqual(ySize, target.YSize, testObject + ": wrong YSize");
        }

        private static void ConstructorTestOutsideBounds(string testObject, int version, int xSize, int ySize, int xSizeExpected, int ySizeExpected)
        {
            Maze target = new Maze(xSize, ySize, version);

            Assert.AreEqual(xSizeExpected, target.XSize, testObject + ": wrong XSize");
            Assert.AreEqual(ySizeExpected, target.YSize, testObject + ": wrong YSize");
        }

        #endregion
    }
}
