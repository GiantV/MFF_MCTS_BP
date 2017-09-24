using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class Program
    {
        static void Main(string[] args)
        {
            int seed = 5;
            Random r = new Random(seed);

            Analyzer a = new Analyzer(r);

            //a.PopulateImage1_Hry_Round1();

            //a.PopulateImage1_Hry_Round2();

            //a.PopulateImage1_Hry_Round3();

            //a.PopulateImage2_Hry_Round2();

            //a.PopulateTable6_Hry();

            //a.PopulateTable7_Hry();
            //a.PopulateTable8_Hry();

            //a.PopulateTable1_Core_W3();

            //a.PopulateTable1_Core_W2(true);
            //a.PopulateTable1_Core_W1(true);

            //a.PopulateTable3_Core_W1();

            //a.MiscTest_2048EffectivityByTime();

            //a.PopulateTable4_Core_W2(true);
            //a.PopulateTable4_Core_W1(false);

            //a.PopulateTable6_Core_W2(true);
            //a.PopulateTable6_Core_W1(true);

            //a.PopulateTable1_Core_W3(true);

            //a.PopulateTable1_Core_W2(true);
            //a.PopulateTable1_Core_W1(true);

            //a.PopulateTable4_Core_W1(true);
            //a.PopulateTable4_Core_W2(true);
            //a.PopulateTable4_Core_W3(true);

            /*a.PopulateTable2_Core_W3(true);
            a.PopulateTable2_Core_W2(true);
            a.PopulateTable2_Core_W1(true);

            a.PopulateTable5_Core_W3(true);
            a.PopulateTable5_Core_W2(true);
            a.PopulateTable5_Core_W1(true);*/

            //GameTests.PopulateTable5_Hry(r);
            //GameTests.PopulateTable5_3_Hry(r);
            //GameTests.PopulateTable6_Hry(r, true);

            InputReader read = new InputReader(a, r);
            //read.Read();

            /*a.PopulateTable7_Core_W2(true);
            a.PopulateTable7_Core_W1(true);

            a.PopulateTable8_Core_W3(true);
            a.PopulateTable8_Core_W2(true);
            a.PopulateTable8_Core_W1(true);

            a.PopulateTable9_Core_W3(true);
            a.PopulateTable9_Core_W2(true);
            a.PopulateTable9_Core_W1(true);*/

            Game2048 game = new Game2048(r, 0);
            SelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            BMCTS2 test = new BMCTS2(game, selPol, new StopPolicyTime(500), 600, 16);

            MCTS test2 = new MCTS(game, selPol, new StopPolicyTime(500));

            PlayControl.Play2048(test, game, r, false);


#warning check count limit -> time limit in Core tests

            Console.ReadLine();
        }
    }
}
