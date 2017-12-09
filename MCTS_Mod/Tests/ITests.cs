using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    interface ITests
    {
        List<string> GenerateMenu();

        List<string> GenerateSubmenu(string option);

        void RunTest(string id, Random r);
    }
}
