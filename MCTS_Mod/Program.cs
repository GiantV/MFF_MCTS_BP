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

            a.PopulateTable6_Hry();

            Console.ReadLine();
        }
    }
}
