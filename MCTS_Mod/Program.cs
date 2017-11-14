﻿using System;
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

            InputReader read = new InputReader(a, r);
            read.Read();

            //read.Init();


            /*PRMCTSTests.PopulateGraph8_W1(r, true);
            PRMCTSTests.PopulateGraph8_W2(r, true);
            PRMCTSTests.PopulateGraph8_W3(r, true);*/

            BMCTSTests.Init2048Tests(r);

#warning check count limit -> time limit in Core tests

            Console.ReadLine();
        }
    }
}
