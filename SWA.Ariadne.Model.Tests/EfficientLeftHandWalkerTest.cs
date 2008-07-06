﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.Model.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Logic.EfficientLeftHandWalker and is intended
    ///to contain all SWA.Ariadne.Logic.EfficientLeftHandWalker Unit Tests
    ///</summary>
    [TestClass()]
    public class EfficientLeftHandWalkerTest
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
        ///A test for EfficientLeftHandWalker.Solve()
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Logic.dll")]
        [TestMethod()]
        public void ELHW_SolveTest_01()
        {
            string mazeCode = "FCGC.OIUA.JZRX";
            string testObject = "ELHW_SolveTest (" + mazeCode + ")";

            SolveTest(testObject, mazeCode);
        }

        /// <summary>
        ///A test for EfficientLeftHandWalker.Solve()
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Logic.dll")]
        [TestMethod()]
        public void ELHW_SolveTest_02()
        {
            string mazeCode = "KWOF.WDEI.TGGD";
            string testObject = "ELHW_SolveTest (" + mazeCode + ")";

            SolveTest(testObject, mazeCode);
        }

        /// <summary>
        ///A test for EfficientLeftHandWalker.Solve()
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Logic.dll")]
        [TestMethod()]
        public void ELHW_SolveTest_03()
        {
            string mazeCode = "FNYK.QJEA.AJFL";
            string testObject = "ELHW_SolveTest (" + mazeCode + ")";

            SolveTest(testObject, mazeCode);
        }

        /// <summary>
        ///A test for EfficientLeftHandWalker.Solve()
        ///</summary>
        [DeploymentItem("SWA.Ariadne.Logic.dll")]
        [TestMethod()]
        public void ELHW_SolveTest_04()
        {
            string mazeCode = "OKRQ.YJFO.KQUN";
            string testObject = "ELHW_SolveTest (" + mazeCode + ")";

            SolveTest(testObject, mazeCode);
        }

        private static void SolveTest(string testObject, string mazeCode)
        {
            Maze maze = new Maze(mazeCode);
            maze.CreateMaze();
            //maze.PlaceEndpoints();

            Assert.AreEqual(mazeCode, maze.Code, testObject + ": wrong code");

            object target = SWA_Ariadne_Logic_LeftHandWalkerAccessor.CreatePrivate(maze, null);
            SWA_Ariadne_Logic_LeftHandWalkerAccessor accessor = new SWA_Ariadne_Logic_LeftHandWalkerAccessor(target);
            accessor.MakeEfficient();

            try
            {
                accessor.Reset();
                accessor.Solve();
            }
            catch (Exception e)
            {
                Assert.Fail(testObject + ": failed with exception: " + e.ToString());
            }
        }

        #endregion
    }
}
