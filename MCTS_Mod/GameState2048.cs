using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Represent the game state for the game 2048
    /// </summary>
    class GameState2048 : GameState
    {
        /// <summary>
        /// Tag of game state, used for miscellaneous purposes (such as determining terminal state)
        /// </summary>
        public int tag = -1;

        /// <summary>
        /// Number of non-empty tiles.
        /// </summary>
        public int tiles = 0;

        /// <summary>
        /// List of valid moves.
        /// </summary>
        private List<GameState> validMoves;

        /// <summary>
        ///  Represent the game state for the game 2048
        /// </summary>
        /// <param name="_parent">Parent of state. Null parent implies root.</param>
        /// <param name="_playedBy">Who played this move.</param>
        /// <param name="state">Board of this move.</param>
        public GameState2048(GameState _parent, byte _playedBy, object state)
        {
            Parent = _parent;
            PlayedBy = _playedBy;
            this.ExploredMoves = new List<GameState>();
            Board = state;
            tag = 0;

            if (Parent != null)
                this.Depth = Parent.Depth + 1;
        }

        /// <summary>
        ///  Represent the game state for the game 2048
        /// </summary>
        /// <param name="_parent">Parent of state. Null parent implies root.</param>
        /// <param name="_playedBy">Who played this move.</param>
        /// <param name="state">Board of this move.</param>
        /// <param name="_tag">Tag of this move.</param>
        public GameState2048(GameState _parent, byte _playedBy, object state, int _tag)
        {
            Parent = _parent;
            PlayedBy = _playedBy;
            this.ExploredMoves = new List<GameState>();
            Board = state;
            tag = _tag;

            if (Parent != null)
                this.Depth = Parent.Depth + 1;
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

        /// <summary>
        /// Sets valid moves to "_validMoves".
        /// </summary>
        /// <param name="_validMoves">List of new valid moves.</param>
        public override void SetValidMoves(List<GameState> _validMoves)
        {
            this.validMoves = _validMoves;
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
        /// Returns value of this state. States representing random moves return 0.
        /// </summary>
        public override double Value
        {
            get
            {
                return (this.PlayedBy == 0) ? base.Value : 0;
            }

            set
            {
                base.Value = value;
            }
        }
    }
}
