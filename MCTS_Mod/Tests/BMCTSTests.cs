using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class BMCTSTests
    {
        public static void Init2048Tests(Random r)
        {
            int timeLimit = 200;

            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            StopPolicyTime stpPol = new StopPolicyTime(timeLimit);

            int iter = 5;

            int L = 0;

            int[] Ws = new int[] { 1, 2, 3, 4, 6, 8, 12, 16, 20, 24, 28, 32 };
            int[] Ts = new int[] { 10, 20, 40, 80, 160, 320 };

            using (StreamWriter sw = new StreamWriter("AAAInit2048Test.txt"))
            {
                foreach (int W in Ws)
                {
                    foreach (int T in Ts)
                    {
                        int prunes = 0;
                        int lastPrunes = 0;

                        BMCTS ai = new BMCTS(game, selPol, stpPol.Clone(), T, W,
                            (GameState g, MCTS b) => lastPrunes = ((BMCTS)b).prunedAt.Count,
                            (GameState g, MCTS b) => prunes = (((BMCTS)b).prunedAt.Count != lastPrunes) ? prunes + 1 : prunes);

                        int depths = 0;

                        for (int i = 0; i < iter; i++)
                        {
                            GameState s = PlayControl.Play2048AI(ai, game, false, r, true);
                            depths += s.Depth;
                        }

                        sw.WriteLine($"{W}/{T} ->  {(double)depths/(double)iter}/{(double)prunes/(double)iter}");

                    }
                }
            }
        }
    }
}
