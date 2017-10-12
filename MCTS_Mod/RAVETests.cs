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

        public static void PopulateTable_RAVE_1(Random r)
        {
            GameReversi game = GameReversi.OptimalGame(r);
            int countLimit = 1000;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            MCTS AI2 = new MCTS(game, selPolicy, stpPolicy);

            DLMCTS AI1 = new DLMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);

            int iterations = 100;

            Console.WriteLine("Starting Beta = 0.5");
                
            double winrate05 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            Console.WriteLine("Starting Beta = 0.25");

            RAVEInfo.RAVEBeta = 0.25;

            double winrate025 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            Console.WriteLine("Starting Beta = 0.75");

            RAVEInfo.RAVEBeta = 0.75;

            double winrate075 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            using (StreamWriter sw = new StreamWriter("RAVE_Table1.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Winrate for β = 0.25: {0}%", winrate025);
                sw.WriteLine("Winrate for β = 0.5: {0}%",  winrate05 );
                sw.WriteLine("Winrate for β = 0.75: {0}%", winrate075);
            }  
        }

        public static void PopulateTable_RAVE_2(Random r)
        {
            GameReversi game = GameReversi.OptimalGame(r);
            int countLimit = 1000;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            MCTS AI2 = new MCTS(game, selPolicy, stpPolicy);

            DLMCTS AI1 = new DLMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);

            int iterations = 100;

            Console.WriteLine("Starting Beta = 0.45");

            RAVEInfo.RAVEBeta = 0.45;

            double winrate045 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            Console.WriteLine("Starting Beta = 0.4");

            RAVEInfo.RAVEBeta = 0.4;

            double winrate04 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            Console.WriteLine("Starting Beta = 0.35");

            RAVEInfo.RAVEBeta = 0.35;

            double winrate035 = PerformTestsReversi(game, AI1, AI2, iterations, r) / iterations;

            using (StreamWriter sw = new StreamWriter("RAVE_Table2.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Winrate for β = 0.35: {0}%", winrate035);
                sw.WriteLine("Winrate for β = 0.4: {0}%", winrate04);
                sw.WriteLine("Winrate for β = 0.45: {0}%", winrate045);
            }
        }

        public static void PopulateTable_RAVE_3(Random r)
        {
            Game2048 game = Game2048.OptimalGame(r);
            int countLimit = 1000;
            int iter = 20;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);
            DLMCTS AI1 = new DLMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);
            MCTS AI2 = new MCTS(game, selPolicy, stpPolicy);

            double raveWinrate = 0.0;

            double standardWinrate = 0.0;


            for (int i = 0; i < iter; i++)
            {
                raveWinrate += PlayControl.Play2048AI(AI1, game, false, r).Depth;
                standardWinrate += PlayControl.Play2048AI(AI2, game, false, r).Depth;
            }

            using (StreamWriter sw = new StreamWriter("RAVE_Table3.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Average depth reached for Follow2Highest identification: {0}", (double)(raveWinrate / iter));
            }

            using (StreamWriter sw = new StreamWriter("RAVE_Table3_ForComparison.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Average depth reached for standard MCTS: {0}", (double)(standardWinrate / iter));
            }
        }

        public static void PopulateTable_RAVE_4(Random r)
        {
            Game2048Derandomized game = Game2048Derandomized.OptimalGame(r);
            int countLimit = 1000;
            int iter = 20;
            StopPolicyCount stpPolicy = new StopPolicyCount(countLimit);
            UCTSelectionPolicy selPolicy = UCTSelectionPolicy.OptimalSelectionPolicy(game);
            DLMCTS AI1 = new DLMCTS(game, selPolicy, stpPolicy.Clone(), 1, true);
            MCTS AI2 = new MCTS(game, selPolicy, stpPolicy);

            double raveWinrate = 0.0;

            double standardWinrate = 0.0;


            for (int i = 0; i < iter; i++)
            {
                raveWinrate += PlayControl.Play2048DAI(AI1, game, false, r).Depth;
                standardWinrate += PlayControl.Play2048DAI(AI2, game, false, r).Depth;
            }

            using (StreamWriter sw = new StreamWriter("RAVE_Table4.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Average depth reached for Follow2Highest identification: {0}", (double)(raveWinrate / iter));
            }

            using (StreamWriter sw = new StreamWriter("RAVE_Table4_ForComparison.txt"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Average depth reached for standard MCTS: {0}", (double)(standardWinrate / iter));
            }
        }
    }
}
