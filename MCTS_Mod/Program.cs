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

            a.PopulateTable1_Core_W2(true);
            a.PopulateTable1_Core_W1(true);

            Console.ReadLine();
        }
    }
}
