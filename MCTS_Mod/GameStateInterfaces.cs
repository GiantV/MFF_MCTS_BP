using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    interface IBasicGameState
    {
        IBasicGameState Parent();

        List<IBasicGameState> ValidStates();

        List<IBasicGameState> ExploredStates();

        int Visits();

        double Value();

        int Depth();
    }

    interface IRaveGameState
    {
        double RAVEValue();

        int RAVEVisits();
    }
}
