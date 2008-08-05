﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using SWA.Ariadne.Model;
using SWA.Ariadne.Logic;

namespace SWA.Ariadne.Logic.Tests
{
    /// <summary>
    ///This is a test class for SWA.Ariadne.Logic.SolverFactory and is intended
    ///to contain all SWA.Ariadne.Logic.SolverFactory Unit Tests
    ///</summary>
    [TestClass()]
    public class SolverFactoryTest
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
        ///A test for CreateDefaultSolver (Maze, IMazeDrawer)
        ///</summary>
        [TestMethod()]
        public void SF_CreateDefaultSolverTest_01()
        {
            string testObject = "SWA.Ariadne.Logic.SolverFactory.CreateDefaultSolver";

            Maze maze = NewMaze();
            IMazeDrawer mazeDrawer = null;
            IMazeSolver actual = SolverFactory.CreateDefaultSolver(maze, mazeDrawer);

            Assert.IsInstanceOfType(actual, SolverFactory.DefaultStrategy, testObject + " did not return an instance of the default strategy.");
        }

        /// <summary>
        ///A test for CreateSolver (Maze, IMazeDrawer)
        ///</summary>
        [TestMethod()]
        public void SF_CreateSolverTest_01()
        {
            string testObject = "SWA.Ariadne.Logic.SolverFactory.CreateSolver";

            Maze maze = NewMaze();
            IMazeDrawer mazeDrawer = null;
            IMazeSolver actual = SolverFactory.CreateSolver(null, maze, mazeDrawer);

            Assert.IsInstanceOfType(actual, typeof(IMazeSolver), testObject + " did not return an instanze of IMazeSolver");
        }

        /// <summary>
        ///A test for CreateSolver (Type, Maze, IMazeDrawer)
        ///</summary>
        [TestMethod()]
        public void SF_CreateSolverTest_type_01()
        {
            string testObject = "SWA.Ariadne.Logic.SolverFactory.CreateSolver(type)";

            foreach (Type solverType in SolverFactory.SolverTypes)
            {
                Maze maze = NewMaze();
                IMazeDrawer mazeDrawer = null;
                IMazeSolver actual = SWA_Ariadne_Logic_SolverFactoryAccessor.CreateSolver(solverType, maze, mazeDrawer);

                Assert.IsInstanceOfType(actual, typeof(IMazeSolver), testObject + " did not return an instanze of IMazeSolver");
                Assert.IsInstanceOfType(actual, solverType, testObject + " did not return the given type");
            }
        }

        #endregion

        #region Auxiliary methods

        internal static Maze NewMaze()
        {
            Maze maze = new Maze(2, 2);
            maze.CreateMaze();
            return maze;
        }

        #endregion
    }
}
