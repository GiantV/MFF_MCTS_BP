using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    interface IGame
    {
        byte NextPlayer(byte player);

        GameState GetRandomValidMove(GameState root);

        List<GameState> GetValidMoves(GameState root);

        List<GameState> CalcValidMoves(GameState state);

        int GetAmountOfValidMoves(GameState root);

        bool IsTerminal(GameState state);

        double Evaluate(GameState state);

        GameState DefaultState(byte firstPlayer);

        void PrintState(GameState state);

        double GameResult(GameState state);
    }
}
