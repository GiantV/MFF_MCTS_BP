using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    /// <summary>
    /// not commented, fast, final?
    /// </summary>
    class GameTests : ITests
    {
        private static List<List<Action<Random>>> Tests = new List<List<Action<Random>>>()
        {
            new List<Action<Random>>()
            {
                r => PopulateTable1_Hry(r),
                r => PopulateTable1_Hry_Fast(r),
                r => PopulateTable2_Hry(r),
                r => PopulateTable1_Hry_Fast(r)
            },
            new List<Action<Random>>()
            {
                r => PopulateTable3_0_Hry(r),
                r => PopulateTable3_1_Hry(r),
                r => PopulateTable3_2_Hry(r),
                r => PopulateTable3_Hry_Fast(r),              
                r => PopulateTable4_Hry(r),
                r => PopulateTable4_Hry_Fast(r)
            },
            new List<Action<Random>>()
            {
                r => PopulateTable5_1_Hry(r),
                r => PopulateTable5_2_Hry(r),
                r => PopulateTable5_3_Hry(r),
                r => PopulateTable5_Hry_Fast(r),
                r => PopulateTable6_Hry(r),
                r => PopulateTable6_Hry_Fast(r)
            },
            new List<Action<Random>>()
            {
                r => PopulateTable7_Hry(r),
                r => PopulateTable7_Hry_Fast(r),
                r => PopulateTable8_Hry(r),
                r => PopulateTable8_Hry_Fast(r)
            },
            new List<Action<Random>>()
            {
                r => PopulateImage1_Hry_Round1(r),
                r => PopulateImage1_Hry_Round2(r),
                r => PopulateImage1_Hry_Round3(r),
                r => PopulateImage2_Hry_Round1(r),
                r => PopulateImage2_Hry_Round2(r),
                r => PopulateImage12_Hry_Fast(r)
            }
        };


        #region Hry Tables 1+2, 2048 Heurstic 1 parameter testing, fast, final?
        public static void PopulateTable1_Hry(Random r)
        {
            double[] TileVal = new double[] { 1, 2, 3, 5 };

            Game2048 game = new Game2048(r, (int)Game2048.Heuristic2048.Adjacency);

            PopulateTable1_Hry_HelpFunction("Hry_Table1.txt", TileVal, game, 1000);
        }

        public static void PopulateTable1_Hry_Fast(Random r)
        {
            double[] TileVal = new double[] { 1, 2, 3, 5 };

            Game2048 game = new Game2048(r, (int)Game2048.Heuristic2048.Adjacency);

            PopulateTable1_Hry_HelpFunction("Hry_Table1_Fast.txt", TileVal, game, 1);
        }

        public static void PopulateTable2_Hry(Random r)
        {
            double[] TileVal = new double[] { 1.5, 1.9, 2, 2.1, 2.5 };

            Game2048 game = new Game2048(r, (int)Game2048.Heuristic2048.Adjacency);

            PopulateTable1_Hry_HelpFunction("Hry_Table2.txt", TileVal, game, 1000);
        }

        public static void PopulateTable2_Hry_Fast(Random r)
        {
            double[] TileVal = new double[] { 1.5, 1.9, 2, 2.1, 2.5 };

            Game2048 game = new Game2048(r, (int)Game2048.Heuristic2048.Adjacency);

            PopulateTable1_Hry_HelpFunction("Hry_Table2_Fast.txt", TileVal, game, 1);
        }

        private static void PopulateTable1_Hry_HelpFunction(string name, double[] TileVal, Game2048 game, int iterations)
        {
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Simulations per parameter: {0}", iterations);
                foreach (double i in TileVal)
                {
                    game.HEURTILEVAL = i;

                    int totalDepth = 0;

                    for (int iter = 0; iter < iterations; iter++)
                    {
                        GameState currentState = game.DefaultState(0);

                        while (!game.IsTerminal(currentState))
                        {
                            GameState nextState = game.GetRandomValidMove(currentState);
                            currentState = nextState;
                        }

                        totalDepth += currentState.Depth;
                    }
                    sw.WriteLine("TileVal {0} average depth: {1}", i, (double)totalDepth / (double)iterations);
                }
            }
        }
        #endregion

        #region Hry Table 3+4, 2048 UCT parameter testing, fast, final?
        public static void PopulateTable3_0_Hry(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_0.txt", new Game2048(r, (int)Game2048.Heuristic2048.None), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable3_0_Hry_Fast(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_0.txt", new Game2048(r, (int)Game2048.Heuristic2048.None), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable3_1_Hry(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_1.txt", new Game2048(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable3_1_Hry_Fast(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_1_Fast.txt", new Game2048(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable3_2_Hry(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_2.txt", new Game2048(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable3_2_Hry_Fast(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_2_Fast.txt", new Game2048(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable3_Hry_Fast(Random r)
        {
            PopulateTable3_0_Hry_Fast(r);
            PopulateTable3_1_Hry_Fast(r);
            PopulateTable3_2_Hry_Fast(r);
        }

        private static void PopulateTable3_Hry_HelpFunction(string name, Game2048 game, double[] param, int iterations, Random r)
        {
            double[] UCTParams = param;

            StopPolicyTime stp = new StopPolicyTime(200);

            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit: 200 ms");
                sw.WriteLine("Iterations per combination: " + iterations);
                foreach (double uct in UCTParams)
                {
                    MCTS AI = new MCTS(game, new UCTSelectionPolicy(game, uct), stp);
                    int totalDepth = 0;


                    for (int i = 0; i < iterations; i++)
                    {
                        #region Print progress
                        switch (game.HEURSIM)
                        {
                            case 0:
                                Console.WriteLine("Random simulation + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                            case 1:
                                Console.WriteLine("Heuristic 1 + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                            case 2:
                                Console.WriteLine("Heuristic 2 + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                        }
                        #endregion

                        GameState result = PlayControl.Play2048AI(AI, game, false, r);
                        totalDepth += result.Depth;
                    }
                    switch (game.HEURSIM)
                    {
                        case 0:
                            sw.WriteLine("Random simulation + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                        case 1:
                            sw.WriteLine("Heuristic 1 + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                        case 2:
                            sw.WriteLine("Heuristic 2 + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                    }

                }
            }
        }

        public static void PopulateTable4_Hry(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table4.txt", new Game2048(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.3, 0.7, 1, 3, 7 }, 40, r);
        }

        public static void PopulateTable4_Hry_Fast(Random r)
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table4_Fast.txt", new Game2048(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.3, 0.7, 1, 3, 7 }, 1, r);
        }
        #endregion

        #region Hry Table 5+6, 2048D UCT parameter testing, fast, final?
        public static void PopulateTable5_1_Hry(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_1.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.None), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable5_1_Hry_Fast(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_1_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.None), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable5_2_Hry(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_2.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable5_2_Hry_Fast(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_2_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable5_3_Hry(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_3.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.1, 1, 10, 100, 1000 }, 20, r);
        }

        public static void PopulateTable5_3_Hry_Fast(Random r)
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_3_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Greedy), new double[] { 0.1, 1, 10, 100, 1000 }, 1, r);
        }

        public static void PopulateTable5_Hry(Random r)
        {
            PopulateTable5_1_Hry(r);
            PopulateTable5_2_Hry(r);
            PopulateTable5_3_Hry(r);
        }

        public static void PopulateTable5_Hry_Fast(Random r)
        {
            PopulateTable5_1_Hry_Fast(r);
            PopulateTable5_2_Hry_Fast(r);
            PopulateTable5_3_Hry_Fast(r);
        }

        public static void PopulateTable6_Hry(Random r, bool parallel = false)
        {
            if (!parallel)
            {
                PopulateTable5_Hry_HelpFunction("Hry_Table6_1.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 50, 75 }, 40, r, true);
                PopulateTable5_Hry_HelpFunction("Hry_Table6_2.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 100, 250, 500 }, 40, r, true);
            }
            else
            {
                Parallel.Invoke(
                    () => PopulateTable5_Hry_HelpFunction("Hry_Table6_1.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 50, 75 }, 40, r, true),
                    () => PopulateTable5_Hry_HelpFunction("Hry_Table6_2.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 100, 250, 500 }, 40, r, true));
            }
        }

        public static void PopulateTable6_Hry_Fast(Random r, bool parallel = false)
        {
            if (!parallel)
            {
                PopulateTable5_Hry_HelpFunction("Hry_Table6_1_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 50, 75 }, 1, r, true);
                PopulateTable5_Hry_HelpFunction("Hry_Table6_2_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 100, 250, 500 }, 1, r, true);
            }
            else
            {
                Parallel.Invoke(
                    () => PopulateTable5_Hry_HelpFunction("Hry_Table6_1_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 50, 75 }, 1, r, true),
                    () => PopulateTable5_Hry_HelpFunction("Hry_Table6_2_Fast.txt", new Game2048Derandomized(r, (int)Game2048.Heuristic2048.Adjacency), new double[] { 100, 250, 500 }, 1, r, true));
            }
        }

        private static void PopulateTable5_Hry_HelpFunction(string name, Game2048 game, double[] param, int iterations, Random r, bool resetTree = false)
        {
            double[] UCTParams = param;

            StopPolicyTime stp = new StopPolicyTime(100);

            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Time limit: 100 ms");
                sw.WriteLine("Iterations per combination: " + iterations);
                foreach (double uct in UCTParams)
                {
                    MCTS AI = new MCTS(game, new UCTSelectionPolicy(game, uct), stp);
                    int totalDepth = 0;


                    for (int i = 0; i < iterations; i++)
                    {
                        #region Print progress
                        switch (game.HEURSIM)
                        {
                            case 0:
                                Console.WriteLine("Random simulation + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                            case 1:
                                Console.WriteLine("Heuristic 1 + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                            case 2:
                                Console.WriteLine("Heuristic 2 + UCT parameter {0} iteration: {1}", uct, i);
                                break;
                        }
                        #endregion

                        GameState result = PlayControl.Play2048DAI(AI, game, false, r, resetTree);  //Play2048AI(AI, game, false);
                        totalDepth += result.Depth;
                    }
                    switch (game.HEURSIM)
                    {
                        case 0:
                            sw.WriteLine("Random simulation + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                        case 1:
                            sw.WriteLine("Heuristic 1 + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                        case 2:
                            sw.WriteLine("Heuristic 2 + UCT parameter {0} average depth: {1}", uct, (double)totalDepth / (double)iterations);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Hry Table 7+8, Reversi eval func / heur sim testing, fast, final?
        public static void PopulateTable7_Hry(Random r)
        {
            GameReversi linEval = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi disEval = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;

            PopulateTable7_Hry_IntermediaryFunction(iter, linEval, disEval, "linear evaluation function", "discrete evaluation function", "Hry_Table7.txt");
        }

        public static void PopulateTable7_Hry_Fast(Random r)
        {
            GameReversi linEval = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi disEval = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 1;

            PopulateTable7_Hry_IntermediaryFunction(iter, linEval, disEval, "linear evaluation function", "discrete evaluation function", "Hry_Table7_Fast.txt");
        }

        public static void PopulateTable8_Hry(Random r)
        {
            GameReversi randomSim = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi heurSim = new GameReversi(r, (int)GameReversi.HeuristicReversi.QualityGroups, (int)GameReversi.EvaluationTypeReversi.Linear);

            int iter = 100;

            PopulateTable7_Hry_IntermediaryFunction(iter, randomSim, heurSim, "random simulation", "heuristic simulation", "Hry_Table8.txt");
        }

        public static void PopulateTable8_Hry_Fast(Random r)
        {
            GameReversi randomSim = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi heurSim = new GameReversi(r, (int)GameReversi.HeuristicReversi.QualityGroups, (int)GameReversi.EvaluationTypeReversi.Linear);

            int iter = 1;

            PopulateTable7_Hry_IntermediaryFunction(iter, randomSim, heurSim, "random simulation", "heuristic simulation", "Hry_Table8_Fast.txt");
        }

        private static void PopulateTable7_Hry_IntermediaryFunction(int iter, GameReversi game1, GameReversi game2, string text1, string text2, string name)
        {
            double UCTParam = 0.7;
            int time = 1000;

            MCTS AI1 = new MCTS(game1, new UCTSelectionPolicy(game1, UCTParam), new StopPolicyTime(time));
            MCTS AI2 = new MCTS(game2, new UCTSelectionPolicy(game2, UCTParam), new StopPolicyTime(time));

            PopulateTable7_Hry_HelpFunction(name,
                AI1, AI2, game1,
                text1,
                text2,
                "iterations: " + iter + ", UCT parameter: " + UCTParam + ", time per move: " + time + " ms",
                iter
                );
        }

        private static void PopulateTable7_Hry_HelpFunction(string name, MCTS AI1, MCTS AI2, GameReversi game, string ai1desc, string ai2desc, string intro, int iter)
        {
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine(intro);
                sw.WriteLine("AI 1: " + ai1desc);
                sw.WriteLine("AI 2: " + ai2desc);
                int ai1Wins = 0;
                int ai2Wins = 0;
                int draws = 0;

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);

                    GameState currentState = game.DefaultState((byte)(i % 2));

                    while (!game.IsTerminal(currentState))
                    {
                        if (currentState.PlayedBy == 0)
                            currentState = AI2.BestMove(currentState, 1);
                        else
                            currentState = AI1.BestMove(currentState, 0);

                        currentState.Parent.ExploredMoves = null;
                        currentState.Parent = null;
                    }

                    double res = game.GameResult(currentState);

                    if (res == 0)
                        draws++;
                    else if (res == 1)
                        ai1Wins++;
                    else if (res == -1)
                        ai2Wins++;
                }

                sw.WriteLine("AI1 won: {0} times", ai1Wins);
                sw.WriteLine("AI2 won: {0} times", ai2Wins);
                sw.WriteLine("AIs tied: {0} times", draws);
            }
        }
        #endregion

        #region Hry Image 1+2, Reversi UCT parameter testing, fast, final?
        public static void PopulateImage1_Hry(Random r)
        {
            PopulateImage1_Hry_Round1(r);
            PopulateImage1_Hry_Round2(r);
            PopulateImage1_Hry_Round3(r);
        }

        public static void PopulateImage2_Hry(Random r)
        {
            PopulateImage2_Hry_Round1(r);
            PopulateImage2_Hry_Round2(r);
        }

        public static void PopulateImage2_Hry_Round1(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;
            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);

            MCTS AI03 = new MCTS(game, new UCTSelectionPolicy(game, 0.3), stp.Clone());
            MCTS AI07 = new MCTS(game, new UCTSelectionPolicy(game, 0.7), stp.Clone());

            MCTS AI1 = new MCTS(game, new UCTSelectionPolicy(game, 1), stp.Clone());
            MCTS AI5 = new MCTS(game, new UCTSelectionPolicy(game, 5), stp.Clone());




            Console.WriteLine("0,3 vs 0,7");
            PopulateTable7_Hry_HelpFunction("Hry_Image2_1_A.txt",
                AI03, AI07, game,
                "UCT 0,3",
                "UCT 0,7",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
            Console.WriteLine("3 vs 7");
            PopulateTable7_Hry_HelpFunction("Hry_Image2_1_B.txt",
                AI1, AI5, game,
                "UCT 1",
                "UCT 5",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
        }

        public static void PopulateImage2_Hry_Round2(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;
            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);
            MCTS AI07 = new MCTS(game, new UCTSelectionPolicy(game, 0.7), stp.Clone());

            MCTS AI5 = new MCTS(game, new UCTSelectionPolicy(game, 5), stp.Clone());



            Console.WriteLine("0,7 vs 5");
            PopulateTable7_Hry_HelpFunction("Hry_Image2_2_A.txt",
                AI07, AI5, game,
                "UCT 0,7",
                "UCT 5",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );

        }

        public static void PopulateImage1_Hry_Round1(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;
            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);

            MCTS AI0001 = new MCTS(game, new UCTSelectionPolicy(game, 0.001), stp.Clone());
            MCTS AI001 = new MCTS(game, new UCTSelectionPolicy(game, 0.01), stp.Clone());
            MCTS AI01 = new MCTS(game, new UCTSelectionPolicy(game, 0.1), stp.Clone());
            MCTS AI1 = new MCTS(game, new UCTSelectionPolicy(game, 1), stp.Clone());
            MCTS AI10 = new MCTS(game, new UCTSelectionPolicy(game, 10), stp.Clone());
            MCTS AI100 = new MCTS(game, new UCTSelectionPolicy(game, 100), stp.Clone());
            MCTS AI1000 = new MCTS(game, new UCTSelectionPolicy(game, 1000), stp.Clone());
            MCTS AI10000 = new MCTS(game, new UCTSelectionPolicy(game, 10000), stp.Clone());


            Console.WriteLine("0.001 vs 0.01");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_1_A.txt",
                AI0001, AI001, game,
                "UCT 0,001",
                "UCT 0,01",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
            Console.WriteLine("0.1 vs 1");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_1_B.txt",
                AI01, AI1, game,
                "UCT 0,1",
                "UCT 1",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
            Console.WriteLine("10 vs 100");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_1_C.txt",
                AI10, AI100, game,
                "UCT 10",
                "UCT 100",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
            Console.WriteLine("1000 vs 10000");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_1_D.txt",
                AI1000, AI10000, game,
                "UCT 1000",
                "UCT 10000",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
        }

        public static void PopulateImage1_Hry_Round2(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;
            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);

            MCTS AI0001 = new MCTS(game, new UCTSelectionPolicy(game, 0.001), stp.Clone());


            MCTS AI1 = new MCTS(game, new UCTSelectionPolicy(game, 1), stp.Clone());
            MCTS AI10 = new MCTS(game, new UCTSelectionPolicy(game, 10), stp.Clone());

            MCTS AI1000 = new MCTS(game, new UCTSelectionPolicy(game, 1000), stp.Clone());



            Console.WriteLine("0.001 vs 1");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_2_A.txt",
                AI0001, AI1, game,
                "UCT 0,001",
                "UCT 1",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );
            Console.WriteLine("10 vs 1000");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_2_B.txt",
                AI10, AI1000, game,
                "UCT 10",
                "UCT 1000",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );

        }

        public static void PopulateImage1_Hry_Round3(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int iter = 100;
            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);


            MCTS AI1 = new MCTS(game, new UCTSelectionPolicy(game, 1), stp.Clone());
            MCTS AI10 = new MCTS(game, new UCTSelectionPolicy(game, 10), stp.Clone());



            Console.WriteLine("1 vs 10");
            PopulateTable7_Hry_HelpFunction("Hry_Image1_3_A.txt",
                AI1, AI10, game,
                "UCT 1",
                "UCT 10",
                "iterations: " + iter + ", time per move: " + time + " ms",
                iter
                );

        }

        public static void PopulateImage12_Hry_Fast(Random r)
        {
            GameReversi game = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Discrete);

            int time = 500;

            StopPolicyTime stp = new StopPolicyTime(time);

            MCTS AI0001 = new MCTS(game, new UCTSelectionPolicy(game, 0.001), stp.Clone());
            MCTS AI001 = new MCTS(game, new UCTSelectionPolicy(game, 0.01), stp.Clone());
            MCTS AI01 = new MCTS(game, new UCTSelectionPolicy(game, 0.1), stp.Clone());
            MCTS AI1 = new MCTS(game, new UCTSelectionPolicy(game, 1), stp.Clone());
            MCTS AI10 = new MCTS(game, new UCTSelectionPolicy(game, 10), stp.Clone());
            MCTS AI100 = new MCTS(game, new UCTSelectionPolicy(game, 100), stp.Clone());
            MCTS AI1000 = new MCTS(game, new UCTSelectionPolicy(game, 1000), stp.Clone());
            MCTS AI10000 = new MCTS(game, new UCTSelectionPolicy(game, 10000), stp.Clone());

            double result_1 = PlayControl.PlayReversiAIAI(AI0001, AI001, game, r, 0, false);
            double result_2 = PlayControl.PlayReversiAIAI(AI01, AI1, game, r, 0, false);
            double result_3 = PlayControl.PlayReversiAIAI(AI10, AI100, game, r, 0, false);
            double result_4 = PlayControl.PlayReversiAIAI(AI1000, AI10000, game, r, 0, false);

            MCTS winner1 = (result_1 == 1) ? AI0001 : AI001;
            MCTS winner2 = (result_1 == 1) ? AI01 : AI1;
            MCTS winner3 = (result_1 == 1) ? AI10 : AI100;
            MCTS winner4 = (result_1 == 1) ? AI1000 : AI10000;

            double result_5 = PlayControl.PlayReversiAIAI(winner1, winner2, game, r, 0, false);
            double result_6 = PlayControl.PlayReversiAIAI(winner3, winner4, game, r, 0, false);

            MCTS winner5 = (result_5 == 1) ? winner1 : winner2;
            MCTS winner6 = (result_6 == 1) ? winner3 : winner4;

            double finalResult = PlayControl.PlayReversiAIAI(winner5, winner6, game, r, 0, false);
            MCTS finalWinner = (finalResult == 1) ? winner5 : winner6;

            

            using (StreamWriter sw = new StreamWriter("Hry_Image12_Fast.txt"))
            {
                sw.WriteLine("iterations: 1, time per move: 500 ms");
                sw.WriteLine("winning UCT param: {0}", ((UCTSelectionPolicy)finalWinner.selectionPolicy).UCT);
            }
        }
        #endregion

        public static void PopulateMiscValues_Hry(Random r)
        {

            using (StreamWriter sw = new StreamWriter("Hry_Misc.txt"))
            {
                int iter = 1000;
                double adj = Test2048HeuristicAvgDepth(r, (int)Game2048.Heuristic2048.Adjacency, iter);
                double grd = Test2048HeuristicAvgDepth(r, (int)Game2048.Heuristic2048.Greedy, iter);
                double none = Test2048HeuristicAvgDepth(r, (int)Game2048.Heuristic2048.None, iter);
                sw.WriteLine("Average depth reached for: ");
                sw.WriteLine($"No heuristic: {none}");
                sw.WriteLine($"Heuristic 1: {adj}");
                sw.WriteLine($"Heuristic 2: {grd}");
            }
        }

        private static double Test2048HeuristicAvgDepth(Random r, int heuristic, int iter)
        {
            int depthreached = 0;
            Game2048 game = new Game2048(r, heuristic);

            for (int i = 0; i < iter; i++)
            {
                GameState currentState = game.DefaultState(0);

                while (!game.IsTerminal(currentState)) // while terminal state is not reached
                {
                    GameState nextState = game.GetRandomValidMove(currentState); // select next state
                    currentState = nextState;
                }
                depthreached += currentState.Depth;
            }
            return (double)depthreached / (double)iter;
        }

        public List<string> GenerateMenu()
        {
            List<string> menu = new List<string>();

            menu.Add("1) Graph 1 & 2  - Heuristic 1 parameter testing");
            menu.Add("2) Graph 3 & 4  - 2048 UCT param. testing");
            menu.Add("3) Graph 5 & 6  - 2048 Derandomizer UCT param. testing");
            menu.Add("4) Graph 7 & 8  - Reversi evaluation function / heurstic simulation testing");
            menu.Add("5) Graph 9 & 10 - Reversi UCT param. testing");

            return menu;
        }

        public List<string> GenerateSubmenu(string option)
        {
            List<string> menu = new List<string>();

            switch(option)
            {
                case "1":
                    menu.Add("1) Parameters: 1, 2, 3, 5 (1000 iterations)");
                    menu.Add("2) Parameters: 1, 2, 3, 5 (1 iteration)");
                    menu.Add("3) Parameters: 1.5, 1.9, 2, 2.1, 2.5 (1000 iterations)");
                    menu.Add("4) Parameters: 1.5, 1.9, 2, 2.1, 2.5 (1 iteration)");
                    break;
                case "2":
                    menu.Add("1) UCT Param testing, random simulation (20 simulations)");
                    menu.Add("2) UCT Param testing, heuristic 1 simulation (20 simulations)");
                    menu.Add("3) UCT Param testing, heuristic 2 simulation (20 simulations)");
                    menu.Add("4) UCT Param testing, all types of simulation (1 simulation)");
                    menu.Add("5) UCT Param specification, heuristic 2 simulation (40 simulations)");
                    menu.Add("6) UCT Param specification, heuristic 2 simulation (1 simulation)");
                    break;
                case "3":
                    menu.Add("1) UCT Param testing, random simulation (20 simulations)");
                    menu.Add("2) UCT Param testing, heuristic 1 simulation (20 simulations)");
                    menu.Add("3) UCT Param testing, heuristic 2 simulation (20 simulations)");
                    menu.Add("4) UCT Param testing, all types of simulation (1 simulation)");
                    menu.Add("5) UCT Param specification, heuristic 2 simulation (40 simulations)");
                    menu.Add("6) UCT Param specification, heuristic 2 simulation (1 simulation)");
                    break;
                case "4":
                    menu.Add("1) Linear vs discrete evaluation function test (100 games)");
                    menu.Add("2) Linear vs discrete evaluation function test (1 game)");
                    menu.Add("3) Random simulation vs Heuristic simulation testing (100 games)");
                    menu.Add("4) Random simulation vs Heuristic simulation testing (1 game)");
                    break;
                case "5":
                    menu.Add("1) UCT param testing, tournament 1, round 1 (100 games)");
                    menu.Add("2) UCT param testing, tournament 1, round 2 (100 games)");
                    menu.Add("3) UCT param testing, tournament 1, round 3 (100 games)");
                    menu.Add("4) UCT param testing, tournament 2, round 1 (100 games)");
                    menu.Add("5) UCT param testing, tournament 2, round 1 (100 games)");
                    menu.Add("6) UCT param testing, tournament 1&2 (1 game)");
                    break;
                default:
                    break;
            }

            return menu;
        }

        public void RunTest(string id, Random r)
        {
            string[] arg = id.Split(';');
            int graph = Int32.Parse(arg[0]);
            int test = Int32.Parse(arg[1]);
            Tests[graph - 1][test - 1](r);
        }
    }
}
