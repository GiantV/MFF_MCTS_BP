﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MCTS_Mod
{
    class Analyzer
    {
        Random r;

        public Analyzer(Random _r)
        {
            r = _r;
        }

        public void PopulateTable1_Implementace()
        {
            GameReversi gameNaive = new GameReversi(r, 0, 0);
            GameReversi gameFast = new GameReversi(r, 2, 0, 2);

            GameReversi[] games = new GameReversi[] { gameNaive, gameFast };
            long[] times = new long[] { 0, 0 };


            MCTS AI = new MCTS(gameNaive, new UCTSelectionPolicy(gameNaive, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Table1.txt"))
            {
                sw.WriteLine("Game 1: Standard approach");
                sw.WriteLine("Game 2: Randomly split to 2 groups heuristic");
                sw.WriteLine("Results calculated over the course of 1 game using game board in individual turns as starting point of 5000 random valid state generations.");
                sw.WriteLine("AI used to play the game: MCTS, random simulation, UCT - 15, Stop policy - Time 1000 ms");

                GameState currentState = gameFast.DefaultState(0);

                int totalSimulations = 0;
                int iter = 5000;

                while (!gameNaive.IsTerminal(currentState))
                {

                    currentState.ExploredMoves = new List<GameState>();
                    currentState.Value = 0;
                    currentState.Visits = 0;
                    currentState.SetValidMoves(null);


                    Stopwatch watch = new Stopwatch();
                    for (int i = 0; i < games.Count(); i++)
                    {
                        GameReversi game = games[i];
                        for (int j = 0; j < iter; j++)
                        {
                            watch.Reset();
                            watch.Start();
                            GameState tmp = game.GetRandomValidMove(currentState);
                            currentState.SetValidMoves(null);
                            watch.Stop();
                            times[i] += watch.ElapsedTicks;
                        }
                    }
                    totalSimulations += iter;

                    currentState.SetValidMoves(gameNaive.CalcValidMoves(currentState));

                    if (currentState.PlayedBy == 0)
                        currentState = AI.BestMove(currentState, 1);
                    else
                        currentState = AI.BestMove(currentState, 0);

                    currentState.Parent.ExploredMoves = null;
                    currentState.Parent = null;
                }

                long bil = 1000L * 1000L * 1000L;

                sw.WriteLine("Stopwatch frequency: " + Stopwatch.Frequency);

                for (int i = 0; i < times.Count(); i++)
                {
                    sw.WriteLine("Game" + (i + 1) + " nanoseconds: " + (bil / Stopwatch.Frequency) * (times[i] / totalSimulations));
                }
            }
        }

        public void PopulateGraph1_Implementace()
        {
            GameReversi gameNaive = new GameReversi(r, 0, 0);
            GameReversi gameFast2 = new GameReversi(r, 2, 0, 2);
            GameReversi gameFast4 = new GameReversi(r, 2, 0, 4);
            GameReversi gameFast8 = new GameReversi(r, 2, 0, 8);
            GameReversi gameFast16 = new GameReversi(r, 2, 0, 16);
            GameReversi gameFast32 = new GameReversi(r, 2, 0, 32);
            GameReversi gameFast64 = new GameReversi(r, 2, 0, 64);

            GameReversi[] games = new GameReversi[] { gameNaive, gameFast2, gameFast4, gameFast8, gameFast16, gameFast32, gameFast64 };
            long[] times = new long[] { 0, 0, 0, 0, 0, 0, 0 };
            int iter = 5000;

            MCTS AI = new MCTS(gameNaive, new UCTSelectionPolicy(gameNaive, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Graph1.txt"))
            {
                sw.WriteLine("Game 1: Standard simulation");
                sw.WriteLine("Game 2: Randomly split to 2 groups heuristic");
                sw.WriteLine("Game 3: Randomly split to 4 groups heuristic");
                sw.WriteLine("Game 4: Randomly split to 8 groups heuristic");
                sw.WriteLine("Game 5: Randomly split to 16 groups heuristic");
                sw.WriteLine("Game 6: Randomly split to 32 groups heuristic");
                sw.WriteLine("Game 7: Randomly split to 64 groups heuristic");
                sw.WriteLine("Results calculated over the course of 1 game using game board in individual turns as starting point of 5000 random valid state generations.");
                sw.WriteLine("AI used to play the game: MCTS, random simulation, UCT - 15, Stop policy - Time 1000 ms");

                int totalSimulations = 0;
                GameState currentState = gameFast2.DefaultState(0);



                while (!gameNaive.IsTerminal(currentState))
                {

                    currentState.ExploredMoves = new List<GameState>();
                    currentState.Value = 0;
                    currentState.Visits = 0;
                    currentState.SetValidMoves(null);

                    Stopwatch watch = new Stopwatch();
                    for (int i = 0; i < games.Count(); i++)
                    {
                        GameReversi game = games[i];
                        for (int j = 0; j < iter; j++)
                        {
                            watch.Reset();
                            watch.Start();
                            GameState tmp = game.GetRandomValidMove(currentState);
                            currentState.SetValidMoves(null);
                            watch.Stop();
                            times[i] += watch.ElapsedTicks;
                        }
                    }
                    totalSimulations += iter;

                    currentState.SetValidMoves(gameNaive.CalcValidMoves(currentState));

                    if (currentState.PlayedBy == 0)
                        currentState = AI.BestMove(currentState, 1);
                    else
                        currentState = AI.BestMove(currentState, 0);

                    currentState.Parent.ExploredMoves = null;
                    currentState.Parent = null;
                }

                long bil = 1000L * 1000L * 1000L;

                sw.WriteLine("Stopwatch frequency: " + Stopwatch.Frequency);

                for (int i = 0; i < times.Count(); i++)
                {
                    sw.WriteLine("Game" + (i + 1) + " nanoseconds: " + (bil / Stopwatch.Frequency) * (times[i] / totalSimulations));
                }
            }
        }

        public void PopulateTable2_Implementace()
        {
            GameReversi game32 = new GameReversi(r, 2, 0, 32);
            GameReversi game64 = new GameReversi(r, 2, 0, 64);
            GameReversi gameP = new GameReversi(r, 3, 0);

            GameReversi[] games = new GameReversi[] { game32, game64, gameP };
            long[] times = new long[] { 0, 0, 0 };
            int iter = 5000;

            MCTS AI = new MCTS(game32, new UCTSelectionPolicy(game32, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Table2.txt"))
            {
                sw.WriteLine("Game 1: Randomly split to 32 groups heuristic");
                sw.WriteLine("Game 2: Randomly split to 64 groups heuristic");
                sw.WriteLine("Game 3: Search through random permutation");
                sw.WriteLine("Results calculated over the course of 1 game using game board in individual turns as starting point of 5000 random valid state generations.");
                sw.WriteLine("AI used to play the game: MCTS, random simulation, UCT - 15, Stop policy - Time 1000 ms");


                int totalSimulations = 0;
                GameState currentState = gameP.DefaultState(0);
                while (!game32.IsTerminal(currentState))
                {

                    currentState.ExploredMoves = new List<GameState>();
                    currentState.Value = 0;
                    currentState.Visits = 0;
                    currentState.SetValidMoves(null);


                    Stopwatch watch = new Stopwatch();
                    for (int i = 0; i < games.Count(); i++)
                    {
                        GameReversi game = games[i];
                        for (int j = 0; j < iter; j++)
                        {
                            watch.Reset();
                            watch.Start();
                            GameState tmp = game.GetRandomValidMove(currentState);
                            currentState.SetValidMoves(null);
                            watch.Stop();
                            times[i] += watch.ElapsedTicks;
                        }
                    }
                    totalSimulations += iter;

                    currentState.SetValidMoves(game32.CalcValidMoves(currentState));

                    if (currentState.PlayedBy == 0)
                        currentState = AI.BestMove(currentState, 1);
                    else
                        currentState = AI.BestMove(currentState, 0);

                    currentState.Parent.ExploredMoves = null;
                    currentState.Parent = null;
                }


                long bil = 1000L * 1000L * 1000L;

                sw.WriteLine("Stopwatch frequency: " + Stopwatch.Frequency);

                for (int i = 0; i < times.Count(); i++)
                {
                    sw.WriteLine("Game" + (i + 1) + " nanoseconds: " + (bil / Stopwatch.Frequency) * (times[i] / totalSimulations));
                }
            }
        }

        public void PopulateTable1_Core_W3(bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table1_W3M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table1_W3M" + d + ".txt"));
        }

        public void PopulateTable1_Core_W2(bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table1_W2M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table1_W2M" + d + ".txt"));
        }

        public void PopulateTable1_Core_W1(bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table1_W1M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table1_W1M" + d + ".txt"));
        }

        private void PopulateTable1_Core_HelpFunction(double _W, double _mult, string _name, bool fakePrune = true)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}",_W, _mult);

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

            int miss  = 0;
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

        public void PopulateTable3_Core_W3(bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable3_Core_HelpFunction(W, mult[i], "Core_Table3_W075M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable3_Core_HelpFunction(W, d, "Core_Table3_W075M" + d + ".txt"));

        }

        public void PopulateTable3_Core_W2(bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable3_Core_HelpFunction(W, mult[i], "Core_Table3_W05M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable3_Core_HelpFunction(W, d, "Core_Table3_W05M" + d + ".txt"));
        }

        public void PopulateTable3_Core_W1(bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable3_Core_HelpFunction(W, mult[i], "Core_Table3_W25M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable3_Core_HelpFunction(W, d, "Core_Table3_W25M" + d + ".txt"));
        }

        private void PopulateTable3_Core_HelpFunction(double _W, double _mult, string _name)
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

                    

                    while(!game.IsTerminal(currentState))
                    {
                        currentState.ExploredMoves = new List<GameState>();
                        currentState.Value = 0;
                        currentState.Visits = 0;
                        currentState.SetValidMoves(game.CalcValidMoves(currentState));


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

        public void PopulateTable4_Core_W3(bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table4_W3M" + mult[i] + ".txt", false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table4_W3M" + d + ".txt", false));
        }

        public void PopulateTable4_Core_W2(bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table4_W2M" + mult[i] + ".txt", false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table4_W2M" + d + ".txt", false));
        }

        public void PopulateTable4_Core_W1(bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable1_Core_HelpFunction(W, mult[i], "Core_Table4_W1M" + mult[i] + ".txt", false);
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable1_Core_HelpFunction(W, d, "Core_Table4_W1M" + d + ".txt", false));
        }

        public void PopulateTable6_Core_W3(bool parallel = false)
        {
            double W = 0.75;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };
            
            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable6_Core_HelpFunction(W, mult[i], "Core_Table6_W3M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable6_Core_HelpFunction(W, d, "Core_Table6_W3M" + d + ".txt"));

        }

        public void PopulateTable6_Core_W2(bool parallel = false)
        {
            double W = 0.5;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable6_Core_HelpFunction(W, mult[i], "Core_Table6_W2M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable6_Core_HelpFunction(W, d, "Core_Table6_W2M" + d + ".txt"));

        }

        public void PopulateTable6_Core_W1(bool parallel = false)
        {
            double W = 0.25;

            double[] mult = new double[] { 0.25, 0.5, 0.75, 0.9 };

            if (!parallel)
            {
                for (int i = 0; i < mult.Count(); i++)
                    PopulateTable6_Core_HelpFunction(W, mult[i], "Core_Table6_W1M" + mult[i] + ".txt");
            }
            else
                Parallel.ForEach(mult, (double d) => PopulateTable6_Core_HelpFunction(W, d, "Core_Table6_W1M" + d + ".txt"));

        }

        private void PopulateTable6_Core_HelpFunction(double _W, double _mult, string name)
        {
            Console.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

            int iter = 100;

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

            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine("Optimal setup, 100 iterations, time limit 500 ms.");
                sw.WriteLine("Width: {0}, Multiplier: {1}", _W, _mult);

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);

                    GameState currentState = game.DefaultState((byte)(i % 2));

                    while (!game.IsTerminal(currentState))
                    {
                        currentState.ExploredMoves = new List<GameState>();
                        currentState.Value = 0;
                        currentState.Visits = 0;
                        currentState.SetValidMoves(game.CalcValidMoves(currentState));


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


        public void MiscTest_2048EffectivityByTime(Random r, bool parallel = false)
        {
            int[] times = new int[] { 100, 300, 400 };



            int iter = 20;

            string name = "Misc_Table_2048Effectivity";

            if (!parallel)
            {
                foreach (int time in times)
                {
                    MiscTest_2048EffectivityByTime_HelpFunction(time, name, iter, r);
                }
            }
            else
            {
                Parallel.ForEach(times, (int i) => MiscTest_2048EffectivityByTime_HelpFunction(i, name, iter, r));
            }
        }

        private void MiscTest_2048EffectivityByTime_HelpFunction(int time, string name, int iter, Random r)
        {
            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            Console.WriteLine("Time: {0}", time);
            using (StreamWriter sw = new StreamWriter(name + time + ".txt"))
            {
                sw.WriteLine("All opt. with time limit of {0} ms", time);
                MCTS AI = new MCTS(game, selPol, new StopPolicyTime(time));

                int depthTotal = 0;

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);
                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);


                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048(AI, game, currentState, false, r);
                    }

                    depthTotal += currentState.Depth;
                }

                double depthAverage = (double)depthTotal / (double)iter;
                sw.WriteLine("Average depth: {0}", depthAverage);
            }
        }


        #region UCT_Testing - Should be correct

        public void TestUCTParam2048(StopPolicy stp, int iterations, double[] param, IGame tofe, string id, string intro, bool derandomized = false)
        {
            Parallel.ForEach(param, (double d) =>
            {
                MCTS AI = new MCTS(tofe, new UCTSelectionPolicy(tofe, d), stp.Clone());
                if (derandomized)
                {
                    PlayControl.Play2048DAI(AI, (Game2048Derandomized)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
                    {
                        return g.Depth.ToString() + "\r\n";
                    });
                }
                else
                {
                    PlayControl.Play2048AI(AI, (Game2048)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
                        {
                            return g.Depth.ToString() + "\r\n";
                        });
                }

            });


        }

        public void TestUCTReversi(StopPolicy stp, int iterations, double[] param, GameReversi rev, string id, string intro, double compareAgainstUCTParam)
        {
            using (StreamWriter sw = new StreamWriter("UCTTest_Reversi_" + id + ".txt"))
            {
                sw.WriteLine(intro);

                #region List UCT params
                sw.Write("[");
                for (int i = 0; i < param.Length - 1; i++)
                {
                    sw.Write(param[i] + ",");
                }
                sw.Write(param[param.Length - 1] + "]");
                sw.WriteLine(); 
                #endregion

                UCTSelectionPolicy compSelPol = new UCTSelectionPolicy(rev, compareAgainstUCTParam);
                MCTS compAI = new MCTS(rev, compSelPol, stp.Clone());

                UCTSelectionPolicy[] selectionPolicies = new UCTSelectionPolicy[param.Length];
                MCTS[] AIs = new MCTS[param.Length];
                double[][] results = new double[param.Length][];

                for (int i = 0; i < param.Length; i++)
                {
                    selectionPolicies[i] = new UCTSelectionPolicy(rev, param[i]);
                    AIs[i] = new MCTS(rev, selectionPolicies[i], stp.Clone());
                    results[i] = new double[] { 0, 0, 0 };
                }



                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine(i);
                    Parallel.For(0, param.Length, (int a) =>
                    {
                        double res = PlayReversiAIAI(compAI, AIs[a], rev, r, (byte)(a % 2), false);
                        if (res > 0.5)
                            results[a][0]++;
                        else if (res < 0.5)
                            results[a][1]++;
                        else
                            results[a][2]++;
                    });
                }
                for (int i = 0; i < param.Length; i++)
                {
                    sw.WriteLine("Parameter " + param[i] + " had " + results[i][0] + " wins, " + results[i][1] + " losses and " + results[i][2] + " ties against parameter " + compareAgainstUCTParam);
                }
            }
        }

        #endregion








        
        


        

        public void TestAIsReversi(MCTS AI1, MCTS AI2, GameReversi rev, Random r, int iter, string id, string msg = "", bool print = false)
        {
            using (StreamWriter sw = new StreamWriter("ReversiAITest" + id + ".txt"))
            {
                sw.WriteLine(msg);

                int won1 = 0;
                int won2 = 0;
                int tie = 0;


                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: " + (i + 1));
                    double res = PlayReversiAIAI(AI1, AI2, rev, r, (byte)(i % 2), print);
                    if (res == 1)
                    {
                        won1++;
                        sw.WriteLine("Game " + i + " won by AI1");
                    }
                    else if (res == 0)
                    {
                        won2++;
                        sw.WriteLine("Game " + i + " won by AI2");
                    }
                    else
                    {
                        tie++;
                        sw.WriteLine("Game " + i + " was a tie");
                    }
                }

                sw.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
                Console.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
            }
        }

        public void TestEvalFuncReversi(MCTS AI1, MCTS AI2, GameReversi rev, Random r, int iter)
        {
            using (StreamWriter sw = new StreamWriter("EvalTestReversi.txt"))
            {
                sw.WriteLine("AI1 = 0/1");
                sw.WriteLine("AI2 = [0,1]");
                int won1 = 0;
                int won2 = 0;
                int tie = 0;


                for (int i = 0; i < iter; i++)
                {
                    double res = PlayReversiAIAI(AI1, AI2, rev, r, (byte)(i % 2), true);
                    if (res == 1)
                    {
                        won1++;
                        sw.WriteLine("Game " + i + " won by AI1");
                    }
                    else if (res == 0)
                    {
                        won2++;
                        sw.WriteLine("Game " + i + " won by AI2");
                    }
                    else
                    {
                        tie++;
                        sw.WriteLine("Game " + i + " was a tie");
                    }
                }

                sw.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
                Console.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
            }
        }       

        public double PlayReversiAIAI(MCTS AI1, MCTS AI2, GameReversi rev, Random r, byte first, bool print)
        {
            AI1.Reset();
            AI2.Reset();

            GameState initState = rev.DefaultState(first);

            GameState currentState = initState;

            while (!rev.IsTerminal(currentState))
            {
                GameState tmpState = currentState;

                currentState.ExploredMoves = new List<GameState>();
                currentState.Value = 0;
                currentState.Visits = 0;
                currentState.SetValidMoves(rev.CalcValidMoves(currentState));


                if (currentState.PlayedBy == 0)
                    currentState = AI2.BestMove(currentState, 1);
                else
                    currentState = AI1.BestMove(currentState, 0);
                if (print)
                    rev.PrintState(currentState);
                currentState.Parent.ExploredMoves = null;
                currentState.Parent = null;
            }


            double ret = 0;
            double ev = rev.Evaluate(currentState);
            if (ev == 0.5)
                ret = 0.5;
            else if (ev > 0.5)
                ret = 1;
            return ret;
        }

        


    }









}
