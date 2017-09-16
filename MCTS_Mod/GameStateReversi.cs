using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class GameStateReversi : GameState
    {
        /// <summary>
        /// Tag of game state, used for miscellaneous purposes (such as determining terminal state)
        /// </summary>
        public int tag = -1;

        /// <summary>
        /// List of valid moves.
        /// </summary>
        private List<GameState> validMoves;

        /// <summary>
        ///  Represent the game state for the game Reversi.
        /// </summary>
        /// <param name="_parent">Parent of state. Null parent implies root.</param>
        /// <param name="_playedBy">Who played this move.</param>
        /// <param name="state">Board of this move.</param>
        /// <param name="_LMID">Represents misc. information about previous move, such as whether it was a pass.</param>
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

        /// <summary>
        /// Returns list of valid moves.
        /// </summary>
        /// <returns>List of valid moves.</returns>
        public override List<GameState> ValidMoves()
        {
            return validMoves;
        }

        /// <summary>
        /// Sets valid moves to "_validMoves".
        /// </summary>
        /// <param name="_validMoves">List of new valid moves.</param>
        public override void SetValidMoves(List<GameState> _validMoves)
        {
            validMoves = _validMoves;
        }

        /// <summary>
        /// Adds "val" to value of this node.
        /// </summary>
        /// <param name="val"></param>
        public override void AddValue(double val)
        {
            this.Value += val;
        }

        /// <summary>
        /// Sets parent node to null.
        /// </summary>
        public override void RemoveParent()
        {
            this.Parent = null;
        }
    }
}
