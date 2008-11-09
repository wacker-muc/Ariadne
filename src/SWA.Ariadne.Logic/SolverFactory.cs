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
            typeof(OpposedBacktracker),
            typeof(ProximityBacktracker),
            typeof(RandomBacktracker),
            typeof(MasterSolver),
            typeof(RightHandWalker),
            typeof(LeftHandWalker),
            typeof(RandomWalker),
            typeof(RoundRobinFlooder),
            typeof(CloseFlooder),
            typeof(FarFlooder),
            typeof(OpposedFlooder),
            typeof(ProximityFlooder),
            typeof(HesitatingFlooder),
            typeof(CenterFlooder),
            typeof(CornerFlooder),
            typeof(ForwardFlooder),
            typeof(BackwardFlooder),
            typeof(ThickestBranchFlooder),
            typeof(ThinnestBranchFlooder),
            typeof(SpreadingFlooder),
            typeof(RandomFlooder),
        };

        /// <summary>
        /// Returns true if a DeadEndChecker may be installed in the given solverType.
        /// </summary>
        /// <param name="solverType"></param>
        /// <returns></returns>
        public static bool HasEfficientVariant(System.Type solverType)
        {
            bool result = (solverType.IsSubclassOf(typeof(SolverBase)));
            foreach (System.Type t in noEfficientSolverTypes)
            {
                result &= (solverType != t);
            }
            return result;
        }
        private static System.Type[] noEfficientSolverTypes = new System.Type[] {
            typeof(MasterSolver),
            typeof(RandomWalker),
        };
        public static string EfficientPrefix = "Efficient";

        private static Type SolverType(string name)
        {
            foreach (Type t in SolverTypes)
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a new MazeSolver of the given Type.
        /// </summary>
        /// <param name="solverType"></param>
        /// <param name="maze"></param>
        /// <returns></returns>
        private static IMazeSolver CreateSolver(Type solverType, Maze maze, IMazeDrawer mazeDrawer)
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
        /// A (reasonably) intelligent strategy is chosen randomly.
        /// </summary>
        /// <param name="maze"></param>
        /// <returns></returns>
        private static IMazeSolver CreateSolver(Maze maze, IMazeDrawer mazeDrawer)
        {
            Random r = SWA.Utilities.RandomFactory.CreateRandom();

            while (true)
            {
                Type t = solverTypes[r.Next(solverTypes.Length)];
                bool shouldBeEfficient = (r.Next(2) == 0);
                shouldBeEfficient &= RegisteredOptions.GetBoolSetting(RegisteredOptions.OPT_EFFICIENT_SOLVERS);

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

                IMazeSolver result = CreateSolver(t, maze, mazeDrawer);

                if (shouldBeEfficient && HasEfficientVariant(t))
                {
                    result.MakeEfficient();
                }

                return result;
            }
        }

        /// <summary>
        /// Returns a new MazeSolver.
        /// If the strategyName is valid, that type is created; otherwise a random type is returned.
        /// </summary>
        public static IMazeSolver CreateSolver(string strategyName, Maze maze, IMazeDrawer mazeDrawer)
        {
            IMazeSolver result;
            bool isEfficient = false;

            if (strategyName != null)
            {
                if (strategyName.StartsWith(EfficientPrefix))
                {
                    strategyName = strategyName.Substring(EfficientPrefix.Length);
                    isEfficient = true;
                }
            }

            Type strategy = SolverType(strategyName);
            if (strategy != null)
            {
                // If strategyName is a valid solver type name:
                result = CreateSolver(strategy, maze, mazeDrawer);
                if (isEfficient)
                {
                    result.MakeEfficient();
                }
            }
            else
            {
                // Otherwise (strategy name is "any"):
                result = CreateSolver(maze, mazeDrawer);
            }

            return result;
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

        /// <summary>
        /// Returns the list of squares on the solution path from start to end square.
        /// </summary>
        /// <param name="maze"></param>
        /// <returns></returns>
        public static List<MazeSquare> SolutionPath(Maze maze)
        {
            // Note: This code must not modify the given maze.
            MasterSolver m = new MasterSolver(maze, null);
            return m.Path;
        }
    }
}
