using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class PRMCTSTests
    {
        public static void PopulateGraph1_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table1_W3M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table1_W3M" + d + ".txt", r));
        }

        public static void PopulateGraph1_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table1_W2M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table1_W2M" + d + ".txt", r));
        }

        public static void PopulateGraph1_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table1_W1M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table1_W1M" + d + ".txt", r));
        }

        private static void PopulateGraph1_HelpFunction(double _W, double _mult, string _name, Random r, bool fakePrune = true)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            Game2048 game = Game2048.OptimalGame(r);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            int timeLimit = 200;

            StopPolicy stpPolicy = new StopPolicyTime(timeLimit);

            double W = _W;

            double mult = _mult;

            int L = (int)Math.Floor(timeLimit * mult);

            int iterations = 20;

            PRMCTS AI = new PRMCTS(game, selPolicy, stpPolicy, W, mult);



            AI.fakePrune = fakePrune;

            int miss = 0;
            int total = 0;

            int depthTotal = 0;

            string name = _name;
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit, width and prune limit: {0}, {1}, {2}", timeLimit, W, L);
                for (int iter = 0; iter < iterations; iter++)
                {
                    Console.WriteLine("Iteration: {0}", iter);

                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);

                    total++;
                    if (currentState.MiscValue > 0)
                        miss++;

                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048(AI, game, currentState, false, r);
                        total++;
                        if (currentState.MiscValue > 0)
                            miss++;
                    }
                    depthTotal += currentState.Depth;
                }
                if (fakePrune)
                    sw.WriteLine("{0} misses out of {1}", miss, total);
                else
                {
                    sw.WriteLine("Average depth reached: {0}", (double)depthTotal / (double)iterations);
                }
            }
        }

        public static void PopulateGraph2_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table2_W3M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table2_W3M" + d + ".txt", r));
        }

        public static void PopulateGraph2_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table2_W2M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table2_W2M" + d + ".txt", r));
        }

        public static void PopulateGraph2_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table2_W1M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table2_W1M" + d + ".txt", r));
        }

        private static void PopulateGraph2_HelpFunction(double _W, double _mult, string _name, Random r, bool fakePrune = true)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            Game2048Derandomized game = Game2048Derandomized.OptimalGame(r);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            int timeLimit = 100;

            StopPolicy stpPolicy = new StopPolicyTime(timeLimit);

            double W = _W;

            double mult = _mult;

            int L = (int)Math.Floor(timeLimit * mult);

            int iterations = 20;

            PRMCTS AI = new PRMCTS(game, selPolicy, stpPolicy, W, mult);



            AI.fakePrune = fakePrune;

            int miss = 0;
            int total = 0;

            int depthTotal = 0;

            string name = _name;
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit, width and prune limit: {0}, {1}, {2}", timeLimit, W, L);
                for (int iter = 0; iter < iterations; iter++)
                {
                    Console.WriteLine("Iteration: {0}", iter);

                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);

                    total++;
                    if (currentState.MiscValue > 0)
                        miss++;

                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048D(AI, game, currentState, false);
                        total++;
                        if (currentState.MiscValue > 0)
                            miss++;


                        currentState.ExploredMoves.Clear();
                        currentState.SetValidMoves(null);
                    }
                    depthTotal += currentState.Depth;
                }
                if (fakePrune)
                    sw.WriteLine("{0} misses out of {1}", miss, total);
                else
                {
                    sw.WriteLine("Average depth reached: {0}", (double)depthTotal / (double)iterations);
                }
            }
        }

        public static void PopulateGraph3_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph3_HelpFunction(W, mult[i], "Core_Table3_W075M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph3_HelpFunction(W, d, "Core_Table3_W075M" + d + ".txt", r));

        }

        public static void PopulateGraph3_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph3_HelpFunction(W, mult[i], "Core_Table3_W05M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph3_HelpFunction(W, d, "Core_Table3_W05M" + d + ".txt", r));
        }

        public static void PopulateGraph3_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph3_HelpFunction(W, mult[i], "Core_Table3_W25M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph3_HelpFunction(W, d, "Core_Table3_W25M" + d + ".txt", r));
        }

        private static void PopulateGraph3_HelpFunction(double _W, double _mult, string _name, Random r)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            GameReversi game = GameReversi.OptimalGame(r);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            int timeLimit = 500;

            StopPolicy stpPolicy = new StopPolicyTime(timeLimit);

            double W = _W;

            double mult = _mult;

            int L = (int)Math.Floor(timeLimit * mult);

            int iterations = 20;

            PRMCTS AI1 = new PRMCTS(game, selPolicy, stpPolicy, W, L);
            PRMCTS AI2 = new PRMCTS(game, selPolicy, stpPolicy.Clone(), W, L);

            AI1.fakePrune = true;
            AI2.fakePrune = true;

            int miss = 0;
            int total = 0;

            string name = _name;

            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit, width and prune limit: {0}, {1}, {2}", timeLimit, W, L);
                for (int iter = 0; iter < iterations; iter++)
                {
                    Console.WriteLine("Iteration: {0}", iter);

                    int first = iter % 2;

                    GameState currentState = game.DefaultState((byte)first);



                    while (!game.IsTerminal(currentState))
                    {
                        currentState.ExploredMoves = new List<GameState>();
                        currentState.Value = 0;
                        currentState.Visits = 0;
                        currentState.SetValidMoves(game.GetValidMoves(currentState));


                        if (currentState.PlayedBy == 0)
                            currentState = AI2.BestMove(currentState, 1);
                        else
                            currentState = AI1.BestMove(currentState, 0);

                        currentState.Parent.ExploredMoves = null;
                        currentState.Parent = null;

                        total++;
                        if (currentState.MiscValue > 0)
                            miss++;
                    }

                }
                sw.WriteLine("{0} misses out of {1}", miss, total);
            }
        }

        public static void PopulateGraph4_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table4_W3M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table4_W3M" + d + ".txt", r, false));
        }

        public static void PopulateGraph4_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table4_W2M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table4_W2M" + d + ".txt", r, false));
        }

        public static void PopulateGraph4_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph1_HelpFunction(W, mult[i], "Core_Table4_W1M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph1_HelpFunction(W, d, "Core_Table4_W1M" + d + ".txt", r, false));
        }

        public static void PopulateGraph5_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table5_W3M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table5_W3M" + d + ".txt", r, false));
        }

        public static void PopulateGraph5_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table5_W2M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table5_W2M" + d + ".txt", r, false));
        }

        public static void PopulateGraph5_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph2_HelpFunction(W, mult[i], "Core_Table5_W1M" + mult[i] + ".txt", r, false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph2_HelpFunction(W, d, "Core_Table5_W1M" + d + ".txt", r, false));
        }

        public static void PopulateGraph6_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph6_HelpFunction(W, mult[i], "Core_Table6_W3M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph6_HelpFunction(W, d, "Core_Table6_W3M" + d + ".txt", r));

        }

        public static void PopulateGraph6_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph6_HelpFunction(W, mult[i], "Core_Table6_W2M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph6_HelpFunction(W, d, "Core_Table6_W2M" + d + ".txt", r));

        }

        public static void PopulateGraph6_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph6_HelpFunction(W, mult[i], "Core_Table6_W1M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph6_HelpFunction(W, d, "Core_Table6_W1M" + d + ".txt", r));

        }

        private static void PopulateGraph6_HelpFunction(double _W, double _mult, string _name, Random r)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            int iterations = 100;

            int time = 500;

            int L = (int)Math.Floor(time * _mult);

            GameReversi game = GameReversi.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(time);

            PRMCTS AI1 = new PRMCTS(game, selPol, stpPol.Clone(), _W, L);
            MCTS AI2 = new MCTS(game, selPol, stpPol.Clone());

            int win1 = 0;
            int win2 = 0;
            int ties = 0;

            using (StreamWriter sw = new StreamWriter(_name))
            {
                sw.WriteLine("Optimal setup, 100 iterations, time limit 500 ms.");
                sw.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);

                    GameState currentState = game.DefaultState((byte)(i % 2));

                    while (!game.IsTerminal(currentState))
                    {
                        currentState.ExploredMoves = new List<GameState>();
                        currentState.Value = 0;
                        currentState.Visits = 0;
                        currentState.SetValidMoves(game.GetValidMoves(currentState));


                        if (currentState.PlayedBy == 0)
                            currentState = AI2.BestMove(currentState, 1);
                        else
                            currentState = AI1.BestMove(currentState, 0);

                        currentState.Parent.ExploredMoves = null;
                        currentState.Parent = null;
                    }

                    double res = game.GameResult(currentState);

                    if (res == 0)
                        ties++;
                    else if (res == 1)
                        win1++;
                    else if (res == -1)
                        win2++;

                }

                sw.WriteLine("PRMCTS wins: {0}, MCTS wins:{1}, draws: {2}", win1, win2, ties);
            }

        }

        private static void PopulateGraph7_HelpFunction(double _W, double _mult, string _name, Random r)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            Game2048 game = Game2048.OptimalGame(r);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            int timeLimit = 200;

            StopPolicy stpPolicy = new StopPolicyTime(timeLimit);

            double W = _W;

            double mult = _mult;

            int L = (int)Math.Floor(timeLimit * mult);

            int iterations = 20;

            int total = 0;
            double totalMedians = 0;
            double totalAverage = 0;

            PRMCTS AI = new PRMCTS(game, selPolicy, stpPolicy, W, mult);

            string name = _name;

            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit, width and prune limit: {0}, {1}, {2}", timeLimit, W, L);
                for (int iter = 0; iter < iterations; iter++)
                {
                    Console.WriteLine("Iteration: {0}", iter);

                    List<double> totalPruned = new List<double>();

                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);


                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048(AI, game, currentState, false, r);

                        double percentagePruned = (double)AI.totalPruned / (double)AI.statesExpanded;

                        totalPruned.Add(percentagePruned);
                        totalAverage += percentagePruned;
                        total++;
                    }

                    totalPruned.Sort();
                    totalMedians += totalPruned[totalPruned.Count / 2];
                }
                sw.WriteLine("Average of medians of % of states pruned: {0}", totalMedians / iterations);
                sw.WriteLine("Average of % of states pruned: {0}", totalAverage / total);
            }
        }

        public static void PopulateGraph7_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph7_HelpFunction(W, mult[i], "Core_Table7_W3M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph7_HelpFunction(W, d, "Core_Table7_W3M" + d + ".txt", r));
        }

        public static void PopulateGraph7_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph7_HelpFunction(W, mult[i], "Core_Table7_W2M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph7_HelpFunction(W, d, "Core_Table7_W2M" + d + ".txt", r));
        }

        public static void PopulateGraph7_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph7_HelpFunction(W, mult[i], "Core_Table7_W1M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph7_HelpFunction(W, d, "Core_Table7_W1M" + d + ".txt", r));
        }

        private static void PopulateGraph8_HelpFunction(double _W, double _mult, string _name, Random r)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            Game2048Derandomized game = Game2048Derandomized.OptimalGame(r);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            int timeLimit = 100;

            StopPolicy stpPolicy = new StopPolicyTime(timeLimit);

            double W = _W;

            double mult = _mult;

            int L = (int)Math.Floor(timeLimit * mult);

            int iterations = 20;

            PRMCTS AI = new PRMCTS(game, selPolicy, stpPolicy, W, mult);

            int total = 0;
            double totalMedians = 0;
            double totalAverage = 0;

            string name = _name;
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit, width and prune limit: {0}, {1}, {2}", timeLimit, W, L);
                for (int iter = 0; iter < iterations; iter++)
                {
                    Console.WriteLine("Iteration: {0}", iter);

                    List<double> totalPruned = new List<double>();

                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);


                    double percentagePruned = (double)AI.totalPruned / (double)AI.statesExpanded;

                    totalPruned.Add(percentagePruned);
                    totalAverage += percentagePruned;
                    total++;


                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048D(AI, game, currentState, false);

                        percentagePruned = (double)AI.totalPruned / (double)AI.statesExpanded;

                        totalPruned.Add(percentagePruned);
                        totalAverage += percentagePruned;
                        total++;

                        currentState.ExploredMoves.Clear();
                        currentState.SetValidMoves(null);
                    }
                    totalPruned.Sort();
                    totalMedians += totalPruned[totalPruned.Count / 2];
                }
                sw.WriteLine("Average of medians of % of states pruned: {0}", totalMedians / iterations);
                sw.WriteLine("Average of % of states pruned: {0}", totalAverage / total);
            }
        }

        public static void PopulateGraph8_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph8_HelpFunction(W, mult[i], "Core_Table8_W3Ma" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph8_HelpFunction(W, d, "Core_Table8_W3Ma" + d + ".txt", r));
        }

        public static void PopulateGraph8_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph8_HelpFunction(W, mult[i], "Core_Table8_W2Ma" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph8_HelpFunction(W, d, "Core_Table8_W2Ma" + d + ".txt", r));
        }

        public static void PopulateGraph8_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph8_HelpFunction(W, mult[i], "Core_Table8_W1Ma" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph8_HelpFunction(W, d, "Core_Table8_W1Ma" + d + ".txt", r));
        }

        private static void PopulateGraph9_HelpFunction(double _W, double _mult, string _name, Random r)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            int iterations = 100;

            int time = 500;

            int L = (int)Math.Floor(time * _mult);

            GameReversi game = GameReversi.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(time);

            PRMCTS AI1 = new PRMCTS(game, selPol, stpPol.Clone(), _W, _mult);
            MCTS AI2 = new MCTS(game, selPol, stpPol.Clone());

            int win1 = 0;
            int win2 = 0;
            int ties = 0;

            int total = 0;
            double totalMedians = 0;
            double totalAverage = 0;

            using (StreamWriter sw = new StreamWriter(_name))
            {
                sw.WriteLine("Optimal setup, 100 iterations, time limit 500 ms.");
                sw.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

                List<double> totalPruned = new List<double>();

                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);

                    GameState currentState = game.DefaultState((byte)(i % 2));

                    while (!game.IsTerminal(currentState))
                    {
                        currentState.ExploredMoves = new List<GameState>();
                        currentState.Value = 0;
                        currentState.Visits = 0;
                        currentState.SetValidMoves(game.CalcValidMoves(currentState));

                        double percentagePruned = 0;

                        if (currentState.PlayedBy == 0)
                        {
                            currentState = AI2.BestMove(currentState, 1);
                        }
                        else
                        {
                            currentState = AI1.BestMove(currentState, 0);
                            percentagePruned = (double)AI1.totalPruned / (double)AI1.statesExpanded;
                            totalPruned.Add(percentagePruned);
                            totalAverage += percentagePruned;
                            total++;
                        }




                        currentState.Parent.ExploredMoves = null;
                        currentState.Parent = null;
                    }

                    double res = game.GameResult(currentState);

                    if (res == 0)
                        ties++;
                    else if (res == 1)
                        win1++;
                    else if (res == -1)
                        win2++;

                    totalPruned.Sort();
                    totalMedians += totalPruned[totalPruned.Count / 2];
                }
                sw.WriteLine("Average of medians of % of states pruned: {0}", totalMedians / iterations);
                sw.WriteLine("Average of % of states pruned: {0}", totalAverage / total);

                //sw.WriteLine("PRMCTS wins: {0}, MCTS wins:{1}, draws: {2}", win1, win2, ties);
            }

        }

        public static void PopulateGraph9_W3(Random r, bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph9_HelpFunction(W, mult[i], "Core_Table9_W3Ma" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph9_HelpFunction(W, d, "Core_Table9_W3Ma" + d + ".txt", r));

        }

        public static void PopulateGraph9_W2(Random r, bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph9_HelpFunction(W, mult[i], "Core_Table9_W2M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph9_HelpFunction(W, d, "Core_Table9_W2M" + d + ".txt", r));

        }

        public static void PopulateGraph9_W1(Random r, bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateGraph9_HelpFunction(W, mult[i], "Core_Table9_W1M" + mult[i] + ".txt", r);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateGraph9_HelpFunction(W, d, "Core_Table9_W1M" + d + ".txt", r));

        }
    }
}
