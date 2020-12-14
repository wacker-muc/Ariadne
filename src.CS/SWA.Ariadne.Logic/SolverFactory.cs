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
        private static System.Type[] solverTypes = {
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
            typeof(RandomForwardFlooder),
            typeof(RandomBackwardFlooder),
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
        private static System.Type[] noEfficientSolverTypes = {
            typeof(MasterSolver),
            typeof(RandomWalker),
        };
        public const string EfficientPrefix = "Efficient";

        /// <summary>
        /// Returns true if the given solverType can apply some heuristic to guide its decisions.
        /// Currently, these are all the flooder types.
        /// </summary>
        /// <param name="solverType"></param>
        /// <returns></returns>
        public static bool HasHeuristicVariant(System.Type solverType)
        {
            return solverType.IsSubclassOf(typeof(FlooderBase));
        }
        public const string HeuristicPrefix = "Heuristic";

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
                bool shouldUseHeuristic = (r.Next(2) == 0);
                // Note: There is currently no equivaelnt OPT_HEURISTIC_SOLVERS option.

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
                if (shouldUseHeuristic && HasHeuristicVariant(t))
                {
                    result.UseHeuristic();
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
            bool useHeuristic = false;

            if (strategyName != null)
            {
                if (strategyName.StartsWith(EfficientPrefix, StringComparison.Ordinal))
                {
                    strategyName = strategyName.Substring(EfficientPrefix.Length);
                    isEfficient = true;
                }
                if (strategyName.StartsWith(HeuristicPrefix, StringComparison.Ordinal))
                {
                    strategyName = strategyName.Substring(HeuristicPrefix.Length);
                    useHeuristic = true;
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
                if (useHeuristic)
                {
                    result.UseHeuristic();
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
        /// Adds the names of all solver types and their variants to the given collection.
        /// </summary>
        /// <param name="items"></param>
        public static void FillWithSolverTypes(System.Collections.IList items)
        {
            items.Clear();

            foreach (System.Type t in SolverTypes)
            {
                // Add the solver's name to the combo box.
                items.Add(t.Name);
            }
            foreach (System.Type t in SolverTypes)
            {
                if (HasEfficientVariant(t))
                {
                    // Add the solver's name to the combo box.
                    items.Add(EfficientPrefix + t.Name);
                }
            }
            foreach (System.Type t in SolverTypes)
            {
                if (HasHeuristicVariant(t))
                {
                    // Add the solver's name to the combo box.
                    items.Add(HeuristicPrefix + t.Name);
                }
            }
            foreach (System.Type t in SolverTypes)
            {
                if (HasEfficientVariant(t) && HasHeuristicVariant(t))
                {
                    // Add the solver's name to the combo box.
                    items.Add(EfficientPrefix + HeuristicPrefix + t.Name);
                }
            }
            items.Add("(any)");
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
