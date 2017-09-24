using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    abstract class GameState
    {
        /// <summary>
        /// Parent of node.
        /// </summary>
        GameState _parent;

        /// <summary>
        /// Value of node
        /// </summary>
        double _value = 0.0;

        /// <summary>
        /// Misc value of node. Used to store various info by various methods.
        /// </summary>
        double _misc = 0.0;
        
        /// <summary>
        /// Depth of node in tree. Not relative to current root. Also can be thought of as the turn the move was played.
        /// </summary>
        int _depth = 0;

        /// <summary>
        /// Maximum depth of an ancestor of current node. Used in the StopPolicyDepth for time saving purposes.
        /// </summary>
        int _maxDepth = 0;

        /// <summary>
        /// ID of player that played this move.
        /// </summary>
        byte _playedBy = 0;

        /// <summary>
        /// Board representation
        /// </summary>
        object _board = null;

        /// <summary>
        /// Amount of times the state was visited.
        /// </summary>
        int _visits = 0;

        /// <summary>
        /// RAVE visits.
        /// </summary>
        public int RAVEVisits = 0;

        /// <summary>
        /// List of explored sons.
        /// </summary>
        List<GameState> _exp = null;

        /// <summary>
        /// ID of the move the state represents.
        /// </summary>
        public int ID = 0;

        /// <summary>
        /// Should a RAVE winrate be returned. Note that AI must implement RAVE for this to work.
        /// </summary>
        public bool returnRAVEWinrate = false;

        /// <summary>
        /// MC-RAVE evaluation function.
        /// </summary>
        public Func<GameState, GameState, double> MCRAVEEval = (GameState state, GameState parent) =>
        {
            double MCTSEval = state.Value / (double)state.Visits;
            double RAVEEval = state.MiscValue / (double)state.RAVEVisits;

            double beta = 0.5;

            return (1 - beta) * MCTSEval + beta * RAVEEval;
        };

        /// <summary>
        /// List of valid sons.
        /// </summary>
        /// <returns>List of valid sons.</returns>
        public abstract List<GameState> ValidMoves();

        /// <summary>
        /// Sets valid sons of the state to "validMoves".
        /// </summary>
        /// <param name="validMoves">List of new valid moves.</param>
        public abstract void SetValidMoves(List<GameState> validMoves);

        /// <summary>
        /// List of explored sons.
        /// </summary>
        public List<GameState> ExploredMoves
        {
            get { return _exp; }
            set { _exp = value; }
        }

        /// <summary>
        /// ID of player that played this mvoe.
        /// </summary>
        public byte PlayedBy
        {
            get { return _playedBy; }
            set { _playedBy = value; }
        }

        /// <summary>
        /// Board representation.
        /// </summary>
        public object Board
        {
            get { return _board; }
            set { _board = value; }
        }

        /// <summary>
        /// Parent of state. Aka previous move.
        /// </summary>
        public GameState Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Value of this state.
        /// </summary>
        public virtual double Value
        {
            get { return  _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Returns winrate of this node. Note that the value returned may differ if RAVE evaluation is turned on.
        /// </summary>
        public virtual double Winrate
        {
            get
            {
                if (returnRAVEWinrate)
                {
                    return MCRAVEEval(this, this.Parent);
                }
                else
                    return _value / (double)_visits;
            }
        }

        /// <summary>
        /// Depth of this state.
        /// </summary>
        public int Depth
        {
            get { return _depth; }
            set { _depth = value; MaxDepth = value; }  
        }

        /// <summary>
        /// Maximum depth amongst the descendants of this state. Used by StopPolicyDepth for time saving purposes.
        /// </summary>
        public int MaxDepth
        {
            get { return _maxDepth; }
            set { _maxDepth = value; }
        }

        /// <summary>
        /// Number of times this state was visited.
        /// </summary>
        public int Visits
        {
            get { return _visits; }
            set { _visits = value; }
        }

        /// <summary>
        /// Increments value of this state by "val".
        /// </summary>
        /// <param name="val">Value to be added.</param>
        public abstract void AddValue(double val);

        /// <summary>
        /// Removes current parent.
        /// </summary>
        public abstract void RemoveParent();

        /// <summary>
        /// Returns the misc value of this node. Misc value means various things to various methods.
        /// </summary>
        public double MiscValue { get
            {
                return _misc;
            }
            set
            {
                _misc = value;
            }
        }


        /// <summary>
        /// Goes through nodes in DFS order and applies action "a".
        /// </summary>
        /// <param name="root">Root of tree to search through.</param>
        /// <param name="a">Action applied to every node along the path.</param>
        public static void DFS(GameState root, Action<GameState> a)
        {
            Stack<GameState> stack = new Stack<GameState>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                GameState current = stack.Pop();
                a(current);
                foreach (GameState g in current.ExploredMoves)
                {
                    stack.Push(g);
                }
            }
        }
    }
}
