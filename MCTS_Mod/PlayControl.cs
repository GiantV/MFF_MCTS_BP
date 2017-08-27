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

            StopPolicyDepthTime q = (StopPolicyDepthTime)AI.stopPolicy;

            Console.WriteLine(q.totalCountDepth + "/" + q.totalCountTime);

            q.totalCountTime = 0;
            q.totalCountDepth = 0;

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


    }
}
