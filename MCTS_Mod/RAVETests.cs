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

            using (StreamWriter sw = new StreamWriter("RAVE_Table1"))
            {
                sw.WriteLine("Count limit: {0}", countLimit);
                sw.WriteLine("Winrate for β = 0.25: {0}%", winrate025);
                sw.WriteLine("Winrate for β = 0.5: {0}%",  winrate05 );
                sw.WriteLine("Winrate for β = 0.75: {0}%", winrate075);
            }  
        }


        
    }
}
