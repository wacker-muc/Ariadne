using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

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
            typeof(RandomBacktracker),
            typeof(RightHandWalker),
            typeof(LeftHandWalker),
            typeof(RandomWalker),
            typeof(RoundRobinFlooder),
            typeof(RandomFlooder),
        };

        /// <summary>
        /// Returns a new MazeSolver of the given Type.
        /// </summary>
        /// <param name="solverType"></param>
        /// <param name="maze"></param>
        /// <returns></returns>
        public static IMazeSolver CreateSolver(Type solverType, Maze maze)
        {
            IMazeSolver result = (IMazeSolver)solverType.GetConstructor(
                new Type[1] { typeof(Maze) }).Invoke(
                new object[1] { maze }
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
        public static IMazeSolver CreateSolver(Maze maze)
        {
            Random r = new Random();

            while (true)
            {
                Type t = solverTypes[r.Next(solverTypes.Length)];

                if (t == typeof(RandomWalker))
                {
                    // too dumb
                    continue;
                }
#if false
                if (t == typeof(MasterSolver))
                {
                    // too smart
                    continue;
                }
#endif
                return CreateSolver(t, maze);
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
        public static IMazeSolver CreateDefaultSolver(Maze maze)
        {
            return CreateSolver(DefaultStrategy, maze);
        }
    }
}
