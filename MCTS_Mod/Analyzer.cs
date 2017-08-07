using System;
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

        #region Hry Tables 1+2, 2048 Heurstic 1 parameter testing, final?
        public void PopulateTable1_Hry()
        {
            double[] TileVal = new double[] { 1, 2, 3, 5 };

            Game2048 game = new Game2048(r, 1);

            PopulateTable1_Hry_HelpFunction("Hry_Table1.txt", TileVal, game, 1000);
        }

        public void PopulateTable1_Hry_Fast()
        {
            double[] TileVal = new double[] { 1, 2, 3, 5 };

            Game2048 game = new Game2048(r, 1);

            PopulateTable1_Hry_HelpFunction("Hry_Table1.txt", TileVal, game, 1);
        }

        public void PopulateTable2_Hry()
        {
            double[] TileVal = new double[] { 1.5, 1.9, 2, 2.1, 2.5 };

            Game2048 game = new Game2048(r, 1);

            PopulateTable1_Hry_HelpFunction("Hry_Table2.txt", TileVal, game, 1000);
        }

        public void PopulateTable2_Hry_Fast()
        {
            double[] TileVal = new double[] { 1.5, 1.9, 2, 2.1, 2.5 };

            Game2048 game = new Game2048(r, 1);

            PopulateTable1_Hry_HelpFunction("Hry_Table2.txt", TileVal, game, 1);
        }
        private void PopulateTable1_Hry_HelpFunction(string name, double[] TileVal, Game2048 game, int iterations)
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

        #region Hry Table 3+4, 2048 UCT parameter testing, final?
        public void PopulateTable3_0_Hry()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_0.txt", new Game2048(r, 0), new double[] { 0.1, 1, 10, 100, 1000 },20);
        }

        public void PopulateTable3_1_Hry()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_1.txt", new Game2048(r, 1), new double[] { 0.1, 1, 10, 100, 1000 },20);
        }

        public void PopulateTable3_1_Hry_Fast()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_1.txt", new Game2048(r, 1), new double[] { 0.1, 1, 10, 100, 1000 }, 1);
        }

        public void PopulateTable3_2_Hry()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_2.txt", new Game2048(r, 2), new double[] { 0.1, 1, 10, 100, 1000 },20);
        }

        public void PopulateTable3_2_Hry_Fast()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table3_2.txt", new Game2048(r, 2), new double[] { 0.1, 1, 10, 100, 1000 }, 1);
        }

        private void PopulateTable3_Hry_HelpFunction(string name, Game2048 game, double[] param, int iterations)
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

                        GameState result = Play2048AI(AI, game, false);
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
        
        public void PopulateTable4_Hry()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table4.txt", new Game2048(r, 2), new double[] { 0.3, 0.7, 1, 3, 7 }, 40);
        }

        public void PopulateTable4_Hry_Fast()
        {
            PopulateTable3_Hry_HelpFunction("Hry_Table4.txt", new Game2048(r, 2), new double[] { 0.3, 0.7, 1, 3, 7 }, 1);
        }
        #endregion

        #region Hry Table 5+6, 2048D UCT parameter testing, final?
        public void PopulateTable5_1_Hry()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_1.txt", new Game2048Derandomized(r, 0), new double[] { 0.1, 1, 10, 100, 1000 }, 20);
        }

        public void PopulateTable5_1_Hry_Fast()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_1.txt", new Game2048Derandomized(r, 0), new double[] { 0.1, 1, 10, 100, 1000 }, 1);
        }

        public void PopulateTable5_2_Hry()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_2.txt", new Game2048Derandomized(r, 1), new double[] { 0.1, 1, 10, 100, 1000 }, 20);
        }

        public void PopulateTable5_2_Hry_Fast()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_2.txt", new Game2048Derandomized(r, 1), new double[] { 0.1, 1, 10, 100, 1000 }, 1);
        }

        public void PopulateTable5_3_Hry()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_3.txt", new Game2048Derandomized(r, 2), new double[] { 0.1, 1, 10, 100, 1000 }, 20);
        }

        public void PopulateTable5_3_Hry_Fast()
        {
            PopulateTable5_Hry_HelpFunction("Hry_Table5_3.txt", new Game2048Derandomized(r, 2), new double[] { 0.1, 1, 10, 100, 1000 }, 1);
        }

        public void PopulateTable5_Hry()
        {
            PopulateTable5_1_Hry();
            PopulateTable5_2_Hry();
            PopulateTable5_3_Hry();
        }

        public void PopulateTable5_Hry_Fast()
        {
            PopulateTable5_1_Hry_Fast();
            PopulateTable5_2_Hry_Fast();
            PopulateTable5_3_Hry_Fast();
        } 
        

        public void PopulateTable6_Hry()
        {
            Parallel.Invoke(
                () => PopulateTable5_Hry_HelpFunction("Hry_Table6_1.txt", new Game2048Derandomized(r, 1), new double[] { 0.001, 0.01 }, 40, true),
                () => PopulateTable5_Hry_HelpFunction("Hry_Table6_2.txt", new Game2048Derandomized(r, 1), new double[] { 1500, 2000, 10000 }, 40, true));
        }

        private void PopulateTable5_Hry_HelpFunction(string name, Game2048 game, double[] param, int iterations, bool resetTree = false)
        {
            double[] UCTParams = param;

            StopPolicyTime stp = new StopPolicyTime(200);

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

                        GameState result = Play2048DAI(AI, game, false, resetTree);  //Play2048AI(AI, game, false);
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

        public void PopulateTable7_Hry()
        {
            GameReversi linEval = new GameReversi(r, 2, 0);
            GameReversi disEval = new GameReversi(r, 2, 1);

            int UCTParam = 15;
            int iter = 50;
            int time = 1000;

            MCTS AI1 = new MCTS(linEval, new UCTSelectionPolicy(linEval, UCTParam), new StopPolicyTime(time));
            MCTS AI2 = new MCTS(disEval, new UCTSelectionPolicy(disEval, UCTParam), new StopPolicyTime(time));

            PopulateTable7_Hry_HelpFunction("Hry_Table7.txt",
                AI1, AI2, linEval,
                "linear evaluation function",
                "discrete evaluation function",
                "iterations: " + iter + ", UCT parameter: " + UCTParam + ", time per move: " + time + " ms",
                iter
                );
        }

        public void PopulateTable8_Hry()
        {

        }

        public void PopulateImage1_Hry()
        {
            
        }

        public void PopulateImage1_Hry_Round1()
        {
            GameReversi game = new GameReversi(r, 2, 1);

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

        public void PopulateImage1_Hry_Round2()
        {
            GameReversi game = new GameReversi(r, 2, 1);

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

        public void PopulateImage1_Hry_Round3()
        {
            GameReversi game = new GameReversi(r, 2, 1);

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

        private void PopulateTable7_Hry_HelpFunction(string name, MCTS AI1, MCTS AI2, GameReversi game, string ai1desc, string ai2desc, string intro, int iter)
        {
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.WriteLine(intro);
                sw.WriteLine("AI 1: " + ai1desc);
                sw.WriteLine("AI 2: " + ai2desc);
                int ai1Wins = 0;
                int ai2Wins = 0;
                int draws   = 0;

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

                sw.WriteLine("AI1 won: {0} times" , ai1Wins);
                sw.WriteLine("AI2 won: {0} times" , ai2Wins);
                sw.WriteLine("AIs tied: {0} times",   draws);
            }
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



        #region UCT_Testing - Should be correct

        public void TestUCTParam2048(StopPolicy stp, int iterations, double[] param, IGame tofe, string id, string intro, bool derandomized = false)
        {
            Parallel.ForEach(param, (double d) =>
            {
                MCTS AI = new MCTS(tofe, new UCTSelectionPolicy(tofe, d), stp.Clone());
                if (derandomized)
                {
                    Play2048DAI(AI, (Game2048Derandomized)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
                    {
                        return g.Depth.ToString() + "\r\n";
                    });
                }
                else
                {
                    Play2048AI(AI, (Game2048)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
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



        private GameState Play2048AI(MCTS AI, IGame game, bool printStates, bool resetTree = false)
        {
            AI.Reset();
            GameState initState = game.DefaultState(0);

            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            AllRounds(AI, game, ref initState, printStates, false, resetTree);

            return initState;
        }

        private GameState Play2048DAI(MCTS AI, IGame game, bool printStates, bool resetTree = false)
        {
            AI.Reset();
            GameState initState = game.DefaultState(0);

            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            AllRounds(AI, game, ref initState, printStates, true, resetTree);

            return initState;
        }




        public void Play2048AI(MCTS AI, Game2048 tofe, Random r, int iter, string id, Func<MCTS,string> finalMessage = null, string msg = "", bool printStates = false,  params Func<GameState,string>[] messages)
        {
            using (StreamWriter sw = new StreamWriter("2048AITest" + id + ".txt"))
            {
                sw.WriteLine(msg);
                sw.Flush();
                for (int i = 0; i < iter; i++)
                {
                    AI.Reset();
                    GameState initState = tofe.DefaultState(0);
                    if (printStates)
                        tofe.PrintState(initState);
                    initState = GetBestState2048(AI, initState);

                    AllRounds(AI, tofe, ref initState, printStates);

                    foreach (Func<GameState,string> a in messages)
                    {
                        sw.Write(a(initState));
                    }
                    if (finalMessage != null)
                        sw.WriteLine(finalMessage(AI));
                    else
                        sw.WriteLine();
                    
                }
                
            }
        }

        public void Play2048(MCTS AI, Game2048 tofe, Random r, bool printStates)
        {
            GameState initState = tofe.DefaultState(0);
            if (printStates)
                tofe.PrintState(initState);
            initState = GetBestState2048(AI, initState);

            AllRounds(AI, tofe, ref initState, printStates, false);
            tofe.PrintState(initState);

            StopPolicyDepthTime q = (StopPolicyDepthTime)AI.stopPolicy;

            Console.WriteLine(q.totalCountDepth + "/" + q.totalCountTime);

            q.totalCountTime = 0;
            q.totalCountDepth = 0;

            Console.WriteLine(initState.Depth);
        }

        public void Play2048DAI(MCTS AI, Game2048Derandomized tofe, Random r, int iter, string id, Func<MCTS, string> finalMessage = null, string msg = "", bool printStates = false, params Func<GameState, string>[] messages)
        {
            using (StreamWriter sw = new StreamWriter("2048DAITest" + id + ".txt"))
            {
                sw.WriteLine(msg);
                sw.Flush();
                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: " + i);

                    AI.Reset();
                    GameState initState = tofe.DefaultState(0);
                    if (printStates)
                        tofe.PrintState(initState);
                    initState = GetBestState2048(AI, initState);

                    AllRounds(AI, tofe, ref initState, printStates, true);

                    foreach (Func<GameState, string> a in messages)
                    {
                        sw.Write(a(initState));
                    }
                    if (finalMessage != null)
                        sw.WriteLine(finalMessage(AI));
                    //else
                        //sw.WriteLine();

                }

            }
        }

        #region Playing MCTS stuff
        private GameState AllRounds(MCTS AI, IGame tofe, ref GameState initState, bool printStates, bool derandomized = false, bool resetTree = false)
        {
            while (!tofe.IsTerminal(initState))
            {
                initState = (derandomized) ? OneRound2048D(AI, tofe, initState, printStates) : OneRound2048(AI, tofe, initState, printStates);
                if (resetTree)
                {
                    initState.ExploredMoves.Clear();
                    initState.SetValidMoves(null);
                }
            }
            if (printStates)
                Console.WriteLine("Game over!");

            return initState;
        }

        private GameState OneRound2048(MCTS AI, IGame tofe, GameState initState, bool printStates)
        {
            if (printStates)
                tofe.PrintState(initState);

            initState = GetRandomState2048((Game2048)tofe, initState);
            if (printStates)
                tofe.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        private GameState OneRound2048D(MCTS AI, IGame tofe, GameState initState, bool printStates)
        {
            if (printStates)
                tofe.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        private GameState GetBestState2048(MCTS AI, GameState s)
        {
            GameState bestState = AI.BestMove(s, 0);
            if (bestState.Parent != null)
            {
                bestState.Parent.ExploredMoves = null;
                bestState.RemoveParent();
            }
            return bestState;
        }

        private GameState GetRandomState2048(Game2048 tofe, GameState s)
        {
            int tmp = s.ExploredMoves.Count;
            GameState randomState = null;
            randomState = s.ExploredMoves[r.Next(0, tmp)];
            randomState.Parent.ExploredMoves = null;
            randomState.RemoveParent();

            return randomState;
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
