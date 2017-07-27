using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    abstract class SelectionPolicy
    {
        /// <summary>
        /// Points on the previous state in the selection process. Used for BMCTS where non-terminal states may have no valid states.
        /// </summary>
        public GameState previousState = null;

        /// <summary>
        /// Called on current state in each step of the selection process.
        /// </summary>
        public Action<GameState> onVisitAction = null;

        /// <summary>
        /// Returns state to be expanded in a subtree defined by "root",
        /// </summary>
        /// <param name="root">Root.</param>
        /// <returns>State to be expanded.</returns>
        public abstract GameState Select(GameState root);
    }
}
