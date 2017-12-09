using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class ImplementationTests : ITests
    {
        private static List<List<Action<Random>>> Tests = new List<List<Action<Random>>>()
        {
            new List<Action<Random>>()
            {
                r => PopulateTable1_Implementace(r),
                r => PopulateTable1_Implementace_Fast(r),
                r => PopulateGraph1_Implementace(r),
                r => PopulateGraph1_Implementace_Fast(r),
                r => PopulateTable2_Implementace(r),
                r => PopulateTable2_Implementace_Fast(r)
            }
        };



        public static void PopulateTable1_Implementace(Random r, int iter = 5000)
        {
            GameReversi gameNaive = new GameReversi(r, (int)GameReversi.HeuristicReversi.None, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi gameFast = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 2);

            GameReversi[] games = new GameReversi[] { gameNaive, gameFast };
            long[] times = new long[] { 0, 0 };


            MCTS AI = new MCTS(gameNaive, new UCTSelectionPolicy(gameNaive, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Table1" + ((iter == 1) ? "Fast" : "") + ".txt"))
            {
                sw.WriteLine("Game 1: Standard approach");
                sw.WriteLine("Game 2: Randomly split to 2 groups heuristic");
                sw.WriteLine($"Results calculated over the course of 1 game using game board in individual turns as starting point of {iter} random valid state generations.");
                sw.WriteLine("AI used to play the game: MCTS, random simulation, UCT - 15, Stop policy - Time 1000 ms");

                GameState currentState = gameFast.DefaultState(0);

                int totalSimulations = 0;

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

                    currentState.SetValidMoves(gameNaive.GetValidMoves(currentState));

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

        public static void PopulateGraph1_Implementace(Random r, int iter = 5000)
        {
            GameReversi gameNaive = new GameReversi(r, (int)GameReversi.HeuristicReversi.None, (int)GameReversi.EvaluationTypeReversi.Linear);
            GameReversi gameFast2 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 2);
            GameReversi gameFast4 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 4);
            GameReversi gameFast8 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 8);
            GameReversi gameFast16 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 16);
            GameReversi gameFast32 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 32);
            GameReversi gameFast64 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 64);

            GameReversi[] games = new GameReversi[] { gameNaive, gameFast2, gameFast4, gameFast8, gameFast16, gameFast32, gameFast64 };
            long[] times = new long[] { 0, 0, 0, 0, 0, 0, 0 };

            MCTS AI = new MCTS(gameNaive, new UCTSelectionPolicy(gameNaive, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Graph1" + ((iter == 1) ? "Fast" : "") + ".txt"))
            {
                sw.WriteLine("Game 1: Standard simulation");
                sw.WriteLine("Game 2: Randomly split to 2 groups heuristic");
                sw.WriteLine("Game 3: Randomly split to 4 groups heuristic");
                sw.WriteLine("Game 4: Randomly split to 8 groups heuristic");
                sw.WriteLine("Game 5: Randomly split to 16 groups heuristic");
                sw.WriteLine("Game 6: Randomly split to 32 groups heuristic");
                sw.WriteLine("Game 7: Randomly split to 64 groups heuristic");
                sw.WriteLine($"Results calculated over the course of 1 game using game board in individual turns as starting point of {iter} random valid state generations.");
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

                    currentState.SetValidMoves(gameNaive.GetValidMoves(currentState));

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

        public static void PopulateTable2_Implementace(Random r, int iter = 5000)
        {
            GameReversi game32 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 32);
            GameReversi game64 = new GameReversi(r, (int)GameReversi.HeuristicReversi.NGroups, (int)GameReversi.EvaluationTypeReversi.Linear, 64);
            GameReversi gameP = new GameReversi(r, (int)GameReversi.HeuristicReversi.RandomPermutation, (int)GameReversi.EvaluationTypeReversi.Linear);

            GameReversi[] games = new GameReversi[] { game32, game64, gameP };
            long[] times = new long[] { 0, 0, 0 };

            MCTS AI = new MCTS(game32, new UCTSelectionPolicy(game32, 15), new StopPolicyTime(1000));

            using (StreamWriter sw = new StreamWriter("Implementace_Table2" + ((iter == 1) ? "Fast" : "") + ".txt"))
            {
                sw.WriteLine("Game 1: Randomly split to 32 groups heuristic");
                sw.WriteLine("Game 2: Randomly split to 64 groups heuristic");
                sw.WriteLine("Game 3: Search through random permutation");
                sw.WriteLine($"Results calculated over the course of 1 game using game board in individual turns as starting point of {iter} random valid state generations.");
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

                    currentState.SetValidMoves(game32.GetValidMoves(currentState));

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

        public static void PopulateTable1_Implementace_Fast(Random r)
        {
            PopulateTable1_Implementace(r, 1);
        }

        public static void PopulateTable2_Implementace_Fast(Random r)
        {
            PopulateTable2_Implementace(r, 1);
        }

        public static void PopulateGraph1_Implementace_Fast(Random r)
        {
            PopulateGraph1_Implementace(r, 1);
        }

        public List<string> GenerateMenu()
        {
            List<string> menu = new List<string>();

            menu.Add("1) Reversi -  random simulation imporvement tests");

            return menu;
        }

        public List<string> GenerateSubmenu(string option)
        {
            List<string> menu = new List<string>();

            menu.Add("1) Standard random simulation vs splitting tiles into 2 groups (5000 iterations)");
            menu.Add("2) Standard random simulation vs splitting tiles into 2 groups (1 iteration)");
            menu.Add("3) Testing different number of groups, tiles are split into (5000 iterations)");
            menu.Add("4) Testing different number of groups, tiles are split into (1 iteration)");
            menu.Add("5) Splitting into N groups vs random permutation (5000 iterations)");
            menu.Add("6) Splitting into N groups vs random permutation (1 iteration)");

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
