using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class GameStateReversi : GameState
    {
        public int tag = -1;

        private List<GameState> validMoves;

        public GameStateReversi(GameState _parent, byte _playedBy, object state, int _LMID)
        {
            Parent = _parent;
            PlayedBy = _playedBy;
            this.ExploredMoves = new List<GameState>();
            Board = state;
            tag = _LMID;

            if (Parent != null)
                this.Depth = Parent.Depth + 1;
        }

        public override List<GameState> ValidMoves()
        {
            return validMoves;
        }


        public override void SetValidMoves(List<GameState> _validMoves)
        {
            validMoves = _validMoves;
        }


        public override void AddValue(double val)
        {
            this.Value += val;
        }


        public override void RemoveParent()
        {
            this.Parent = null;
        }
    }
}
