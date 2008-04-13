using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// Knows the various MazeSolver strategies and can create instances of them.
    /// </summary>
    public static class SolverFactory
    {
        /// <summary>
        /// Gets an array with the Types of all implemented MazeSolver strategies.
        /// </summary>
        public static System.Type[] SolverTypes
        {
            get { return solverTypes; }
        }
        private static System.Type[] solverTypes = new System.Type[] {
            typeof(ProximityBacktracker),
            typeof(RandomBacktracker),
            typeof(MasterSolver),
            typeof(RightHandWalker),
            typeof(LeftHandWalker),
            typeof(RandomWalker),
            typeof(RoundRobinFlooder),
            typeof(CloseFlooder),
            typeof(FarFlooder),
            typeof(ProximityFlooder),
            typeof(HesitatingFlooder),
            typeof(RandomFlooder),
            typeof(EfficientProximityBacktracker),
            typeof(EfficientRightHandWalker),
            typeof(EfficientLeftHandWalker),
            typeof(EfficientCloseFlooder),
        };

        public static Type SolverType(string name)
        {
            foreach (Type t in SolverTypes)
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            throw new ArgumentOutOfRangeException("name", name, "No such Solver type.");
        }

        /// <summary>
        /// Returns a new MazeSolver of the given Type.
        /// </summary>
        /// <param name="solverType"></param>
        /// <param name="maze"></param>
        /// <returns></returns>
        public static IMazeSolver CreateSolver(Type solverType, Maze maze, IMazeDrawer mazeDrawer)
        {
            IMazeSolver result = (IMazeSolver)solverType.GetConstructor(
                new Type[2] { typeof(Maze), typeof(IMazeDrawer) }).Invoke(
                new object[2] { maze, mazeDrawer }
                );
            result.Reset();

            return result;
        }

        /// <summary>
        /// Returns a new MazeSolver.
        /// A (reasonably) intelligent strategy.
        /// </summary>
        /// <param name="maze"></param>
        /// <returns></returns>
        public static IMazeSolver CreateSolver(Maze maze, IMazeDrawer mazeDrawer)
        {
            Random r = RandomFactory.CreateRandom();

            while (true)
            {
                Type t = solverTypes[r.Next(solverTypes.Length)];

                if (t == typeof(RandomWalker))
                {
                    // too dumb
                    continue;
                }

                if (t == typeof(MasterSolver))
                {
                    // too smart
                    continue;
                }

                if (!RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS))
                {
                    // Get more information from an instance of this solver type.
                    IMazeSolver foo = (IMazeSolver)t.GetConstructor(
                        new Type[2] { typeof(Maze), typeof(IMazeDrawer) }).Invoke(
                        new object[2] { maze, mazeDrawer }
                        );
                    if (foo.IsEfficientSolver)
                    {
                        // not wanted
                        continue;
                    }
                }

                return CreateSolver(t, maze, mazeDrawer);
            }
        }

        /// <summary>
        /// The default MazeSolver strategy.
        /// </summary>
        public static Type DefaultStrategy
        {
            get { return typeof(RandomBacktracker); }
        }

        /// <summary>
        /// Returns a new MazeSolver.
        /// The default strategy.
        /// </summary>
        /// <param name="maze"></param>
        /// <returns></returns>
        public static IMazeSolver CreateDefaultSolver(Maze maze, IMazeDrawer mazeDrawer)
        {
            return CreateSolver(DefaultStrategy, maze, mazeDrawer);
        }

        public static List<MazeSquare> SolutionPath(Maze maze)
        {
            // Note: This code must not modify the given maze.
            MasterSolver m = new MasterSolver(maze, null);
            return m.Path;
        }
    }
}
