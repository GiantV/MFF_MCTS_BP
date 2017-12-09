using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MCTS_Mod
{
    class BMCTSTests
    {
        public static void Init2048Tests(Random r)
        { //                OLD
            /*int timeLimit = 500;

            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol = new UCTSelectionPolicy(game, 10);

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 30;

            int[] Ws = new int[] { 24, 32, 64, 128 };
            int[] Ts = new int[] { 700, 800 };

            Parallel.ForEach(new double[] { 0.1, 1, 10, 100, 1000}, uct =>
            {
                selPol = new UCTSelectionPolicy(game, uct);
                BMCTS2048Test(r, game, selPol, stpPol, $"BMCTS_Graph1", Ws, Ts, iter);
            });*/

            /*Parallel.ForEach(Ws, W =>
                 {
                     BMCTS2048Test(r, game, selPol, stpPol, "BMCTS_Graph1", new int[] { W }, Ts, iter);
                 });*/
        }

        public static void Init2048DTests(Random r)
        {
            int timeLimit = 300;

            Game2048 game = Game2048Derandomized.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 30;

            int[] Ws = new int[] { 3, 8, 16 };
            int[] Ts = new int[] { 35, 70, 80};

            Parallel.ForEach(new double[] { 0.1, 1, 10, 100 }, uct =>
            {
                selPol = new UCTSelectionPolicy(game, uct);
                BMCTS2048Test(r, game, selPol, stpPol, $"BMCTS_Graph2_{uct}", Ws, Ts, iter);
            });

        }

        public static void Init2048Tests2(Random r)
        {
            int timeLimit = 500;

            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol;

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 30;

            int[] Ws = new int[] {  24, 48, 96 };
            int[] Ts = new int[] {  150, 200, 250 };

            Parallel.ForEach(new double[] { 0.1, 1, 10, 100 }, uct =>
            {
                selPol = new UCTSelectionPolicy(game, uct);
                BMCTS2048Test(r, game, selPol, stpPol, $"BMCTS_Graph1_{uct}", Ws, Ts, iter);
            });
        }

        public static void InitCompTest(Random r)
        {
            int timeLimit = 500;

            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 5;

            using (StreamWriter sw = new StreamWriter("AAAComp2048Test.txt"))
            {
                int depths = 0;

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine($"{i}");
                    MCTS ai = new MCTS(game, selPol, stpPol.Clone());

                    GameState s = PlayControl.Play2048AI(ai, game, false, r, true);
                    depths += s.Depth;
                }
                lock (sw)
                {
                    sw.WriteLine($" ->  {(double)depths / (double)iter}");
                }
            }
        }

        public static void InitCompTestD(Random r)
        {
            int timeLimit = 300;

            Game2048 game = Game2048Derandomized.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 5;

            using (StreamWriter sw = new StreamWriter("AAAComp2048DTest.txt"))
            {
                int depths = 0;

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine($"{i}");
                    MCTS ai = new MCTS(game, selPol, stpPol.Clone());

                    GameState s = PlayControl.Play2048DAI(ai, game, false, r, true);
                    depths += s.Depth;
                }
                lock (sw)
                {
                    sw.WriteLine($" ->  {(double)depths / (double)iter}");
                }
            }
        }

        public static void InitReversiTest(Random r)
        {
            int[] Ws = new int[] { 25, 50, 100 };
            int[] Ts = new int[] { 100, 150, 200 };
            double[] ucts = new double[] {0.01, 0.1, 1, 10 };

            StopPolicyTime stpPol = new StopPolicyTime(1000);
            GameReversi game = GameReversi.OptimalGame(r);

            int iter = 50;

            Parallel.ForEach(ucts, (uct) =>
            {
                SelectionPolicy selPol = new UCTSelectionPolicy(game, uct);

                MCTS mcts = new MCTS(game, selPol, stpPol.Clone());
                int i = 0;

                using (StreamWriter sw = new StreamWriter($"BMCTS_Graph3_{uct}.txt"))
                {
                    sw.WriteLine($"W/T = tot");

                    foreach (var W in Ws)
                    {
                        foreach (var T in Ts)
                        {
                            BMCTS bmcts = new BMCTS(game, selPol, stpPol.Clone(), T, W);

                            double tot = 0;

                            for (int j = 0; j < iter; j++)
                            {
                                Console.WriteLine($"{W}/{T} - {j}");
                                tot += PlayControl.PlayReversiAIAI(bmcts, mcts, game, r, (byte)((++i) % 2), false);
                            }
                            sw.WriteLine($"{W}/{T} = {tot * 2}");
                        }
                    }
                }
            });
        }


        private static void BMCTS2048Test(Random r, Game2048 game, SelectionPolicy selPol, StopPolicy stpPol,
            string path, int[] Ws, int[] Ts, int iter)
        {
            using (StreamWriter sw = new StreamWriter($"{path}.txt"))
            {
                foreach (int W in Ws)
                {
                    foreach (var T in Ts)
                    {
                        int prunes = 0;
                        int lastPrunes = 0;

                        int depths = 0;


                        for (int i = 0; i < iter; i++)
                        {
                            Console.WriteLine($"{W}/{T} - {i}");
                            BMCTS ai = new BMCTS(game, selPol, stpPol.Clone(), T, W,
                                (GameState g, MCTS b) => lastPrunes = ((BMCTS)b).prunedAt.Count,
                                (GameState g, MCTS b) => prunes = (((BMCTS)b).prunedAt.Count != lastPrunes) ? prunes + 1 : prunes);

                            GameState s = PlayControl.Play2048AI(ai, game, false, r, true);
                            depths += s.Depth;
                        }
                        lock (sw)
                        {
                            sw.WriteLine($"{W}/{T} ->  {(double)depths / (double)iter}/{(double)prunes / (double)iter}");
                        }
                    }

                }
            }
        }
    }
}
