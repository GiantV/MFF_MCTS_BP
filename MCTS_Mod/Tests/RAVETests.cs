using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class RAVETests
    {

        private static double PerformTestsReversi(GameReversi gameReversi, MCTS AI1, MCTS AI2, int iterations, Random r)
        {
            double winrate = 0.0;

            for (int i = 0; i < iterations; i++)
            {
                double result = PlayControl.PlayReversiAIAI(AI1, AI2, gameReversi, r, (byte)(i % 2), false);
                winrate += result;
            }

            return winrate;
        }

        public static void PopulateGraph1_Part1(Random r)
        {
            PopulateGraph13_HelpFunc(r, 1000, new double[] { 0.25, 0.5, 0.75 }, "RAVE_Graph1_Part1");
        }

        public static void PopulateGraph1_Part2(Random r)
        {
            PopulateGraph13_HelpFunc(r, 1000, new double[] { 0.35, 0.4, 0.45 }, "RAVE_Graph1_Part2");
        }

        public static void PopulateGraph1_Fast(Random r)
        {
            PopulateGraph13_HelpFunc(r, 1000, new double[] { 0.25, 0.35, 0.4, 0.45, 0.5, 0.75 }, "RAVE_Graph1_Fast", 1);
        }

        private static void PopulateGraph13_HelpFunc(Random r, int limit, double[] betas, string name, int iter = 100)
        {
            GameReversi game = GameReversi.OptimalGame(r);
            int countLimit = limit;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            MCTS AI2 = new MCTS(game, selPolicy, stpPolicy);

            BoMCTS AI1 = new BoMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);

            double[] winrates = new double[betas.Count()];

            for (int i = 0; i < betas.Count(); i++)
            {
                RAVEInfo.RAVEBeta = betas[i];
                winrates[i] = PerformTestsReversi(game, AI1, AI2, iter, r) / iter;
            }

            using (StreamWriter sw = new StreamWriter(name + ".txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                for (int i = 0; i < betas.Count(); i++)
                {
                    sw.WriteLine("Winrate for β = {0}: {1}%", betas[i], winrates[i]);
                }
            }
        }

        public static void PopulateGraph2_Part1(Random r)
        {
            PopulateGraph24_HelpFunc(r, 1000, 0.25, "RAVE_Graph2_Part1");
        }

        public static void PopulateGraph2_Part2(Random r)
        {
            PopulateGraph24_HelpFunc(r, 1000, 0.5, "RAVE_Graph2_Part2");
        }

        public static void PopulateGraph2_Part3(Random r)
        {
            PopulateGraph24_HelpFunc(r, 1000, 0.75, "RAVE_Graph2_Part3");
        }

        public static void PopulateGraph2_Part4(Random r)
        {
            PopulateGraph24_HelpFunc(r, 1000, 0, "RAVE_Graph2_Part4");
        }

        public static void PopulateGraph2_Fast(Random r)
        {
            PopulateGraph24_HelpFunc(r, 1000, new double[] { 0.25, 0.5, 0.75, 0 }, "RAVE_Graph2_Fast");
        }

        private static void PopulateGraph24_HelpFunc(Random r, int limit, double beta, string name, int iter = 20)
        {
            PopulateGraph24_HelpFunc(r, limit, new double[] { beta }, name, iter);
        }

        private static void PopulateGraph24_HelpFunc(Random r, int limit, double[] betas, string name, int iter = 20)
        {
            Game2048 game = Game2048.OptimalGame(r);
            Game2048Derandomized gameD = Game2048Derandomized.OptimalGame(r);
            int countLimit = limit;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);
            UCTSelectionPolicy selPolicyD= UCTSelectionPolicy.OptimalSelectionPolicy(gameD);

            BoMCTS AI = new BoMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);
            BoMCTS AID = new BoMCTS(gameD, selPolicyD, stpPolicy.Clone(), 1, true);

            double[] raveWinrate = new double[betas.Length];

            double[] raveWinrateD = new double[betas.Length];

            for (int i = 0; i < betas.Length; i++)
            {
                double beta = betas[i];

                RAVEInfo.RAVEBeta = beta;


                for (int j = 0; j < iter; j++)
                {
                    Console.WriteLine("Iteration: {0}", j);
                    raveWinrateD[i] += PlayControl.Play2048DAI(AID, gameD, false, r).Depth;
                    raveWinrate[i] += PlayControl.Play2048AI(AI, game, false, r).Depth;
                }
            }

            using (StreamWriter sw = new StreamWriter(name + ".txt"))
            {
                for (int i = 0; i < betas.Length; i++)
                {

                    sw.WriteLine("Count limit: {0}", countLimit);
                    sw.WriteLine("Beta: {0}", betas[i]);
                    sw.WriteLine("2048");
                    sw.WriteLine("Average depth reached for Follow2Highest identification: {0}", (double)(raveWinrate[i] / iter));
                    sw.WriteLine();
                    sw.WriteLine("2048D");
                    sw.WriteLine("Average depth reached for Follow2Highest identification: {0}", (double)(raveWinrateD[i] / iter));
                }
            }
        }

        public static void PopulateGraph3_Part1(Random r)
        {
            PopulateGraph13_HelpFunc(r, 100, new double[] { 0.25, 0.5, 0.75 }, "RAVE_Graph3_Part1", 200);
        }

        public static void PopulateGraph3_Part2(Random r)
        {
            PopulateGraph13_HelpFunc(r, 100, new double[] { 0.3, 0.35, 0.4 }, "RAVE_Graph3_Part2", 200);
        }

        public static void PopulateGraph3_Fast(Random r)
        {
            PopulateGraph13_HelpFunc(r, 100, new double[] { 0.25, 0.3, 0.35, 0.4, 0.5, 0.75 }, "RAVE_Graph3_Fast", 200);
        }

        public static void PopulateGraph4_Part1(Random r)
        {
            PopulateGraph24_HelpFunc(r, 100, 0.25, "RAVE_Graph4_Part1", 40);
        }

        public static void PopulateGraph4_Part2(Random r)
        {
            PopulateGraph24_HelpFunc(r, 100, 0.5, "RAVE_Graph4_Part2", 40);
        }

        public static void PopulateGraph4_Part3(Random r)
        {
            PopulateGraph24_HelpFunc(r, 100, 0.75, "RAVE_Graph4_Part3", 40);
        }

        public static void PopulateGraph4_Part4(Random r)
        {
            PopulateGraph24_HelpFunc(r, 100, 0, "RAVE_Graph4_Part4", 40);
        }

        public static void PopulateGraph4_Fast(Random r)
        {
            PopulateGraph24_HelpFunc(r, 100, new double[] { 0.25, 0.5, 0.75, 0}, "RAVE_Graph4_Fast", 40);
        }
    }
}
