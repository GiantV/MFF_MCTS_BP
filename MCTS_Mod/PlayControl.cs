using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class PlayControl
    {
        public static GameState Play2048AI(MCTS AI, IGame game, bool printStates, Random r, bool resetTree = false)
        {
            AI.Reset();
            GameState initState = game.DefaultState(0);

            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            AllRounds(AI, game, ref initState, printStates, r, false, resetTree);

            return initState;
        }

        public static GameState Play2048DAI(MCTS AI, IGame game, bool printStates, Random r, bool resetTree = false)
        {
            AI.Reset();
            GameState initState = game.DefaultState(0);

            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            AllRounds(AI, game, ref initState, printStates, r,  true, resetTree);

            return initState;
        }

        public static void Play2048AI(MCTS AI, Game2048 tofe, Random r, int iter, string id, Func<MCTS, string> finalMessage = null, string msg = "", bool printStates = false, params Func<GameState, string>[] messages)
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

                    AllRounds(AI, tofe, ref initState, printStates, r);

                    foreach (Func<GameState, string> a in messages)
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

        public static void Play2048(MCTS AI, Game2048 tofe, Random r, bool printStates)
        {
            GameState initState = tofe.DefaultState(0);
            if (printStates)
                tofe.PrintState(initState);
            initState = GetBestState2048(AI, initState);

            AllRounds(AI, tofe, ref initState, printStates, r, false);
            tofe.PrintState(initState);

            Console.WriteLine(initState.Depth);
        }

        public static void Play2048DAI(MCTS AI, Game2048Derandomized tofe, Random r, int iter, string id, Func<MCTS, string> finalMessage = null, string msg = "", bool printStates = false, params Func<GameState, string>[] messages)
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

                    AllRounds(AI, tofe, ref initState, printStates, r, true);

                    foreach (Func<GameState, string> a in messages)
                    {
                        sw.Write(a(initState));
                    }
                    if (finalMessage != null)
                        sw.WriteLine(finalMessage(AI));
                }

            }
        }

        #region Playing MCTS stuff
        public static GameState AllRounds(MCTS AI, IGame tofe, ref GameState initState, bool printStates, Random r, bool derandomized = false, bool resetTree = false)
        {
            while (!tofe.IsTerminal(initState))
            {
                initState = (derandomized) ? OneRound2048D(AI, tofe, initState, printStates) : OneRound2048(AI, tofe, initState, printStates, r);
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

        public static GameState OneRound2048(MCTS AI, IGame tofe, GameState initState, bool printStates, Random r)
        {
            if (printStates)
                tofe.PrintState(initState);

            initState = GetRandomState2048((Game2048)tofe, initState, r);
            if (printStates)
                tofe.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        public static GameState OneRound2048D(MCTS AI, IGame tofe, GameState initState, bool printStates)
        {
            if (printStates)
                tofe.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        public static GameState GetBestState2048(MCTS AI, GameState s)
        {
            GameState bestState = AI.BestMove(s, 0);
            if (bestState.Parent != null)
            {
                bestState.Parent.ExploredMoves = null;
                bestState.RemoveParent();
            }
            return bestState;
        }

        public static GameState GetRandomState2048(Game2048 tofe, GameState s, Random r)
        {
            int tmp = s.ExploredMoves.Count;
            GameState randomState = null;
            randomState = s.ExploredMoves[r.Next(0, tmp)];
            randomState.Parent.ExploredMoves = null;
            randomState.RemoveParent();

            return randomState;
        }

        #endregion

        public static void TestAIsReversi(MCTS AI1, MCTS AI2, GameReversi rev, Random r, int iter, string id, string msg = "", bool print = false)
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

        public static double PlayReversiAIAI(MCTS AI1, MCTS AI2, GameReversi rev, Random r, byte first, bool print)
        {
            AI1.Reset();
            AI2.Reset();

            GameState initState = rev.DefaultState(first);

            GameState currentState = initState;

            while (!rev.IsTerminal(currentState))
            {
                GameState tmpState = currentState;

                /*currentState.ExploredMoves = new List<GameState>();
                currentState.Value = 0;
                currentState.Visits = 0;*/

                currentState.ResetBelow();

                currentState.SetValidMoves(rev.GetValidMoves(currentState));

                GameState prevState = currentState;

                if (currentState.PlayedBy == 0)
                    currentState = AI2.BestMove(currentState, 1);
                else
                    currentState = AI1.BestMove(currentState, 0);

                if (currentState == null)
                {
                    if (prevState.PlayedBy == 0)
                        currentState = AI2.BestMove(prevState, 1);
                    else
                        currentState = AI1.BestMove(prevState, 0);
                }

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
