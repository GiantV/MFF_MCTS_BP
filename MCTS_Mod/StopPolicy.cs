using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Represents the time allocated to tree building.
    /// </summary>
    abstract class StopPolicy
    {
        /// <summary>
        /// Returns false if we ran out of time. 
        /// </summary>
        /// <param name="s">Entry state.</param>
        /// <returns></returns>
        public abstract bool StopCondition(GameState s);

        /// <summary>
        /// Resets the stop policy to be used again. Some implementations might not need this.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns clone of this policy.
        /// </summary>
        /// <returns>New object with same parameters.</returns>
        public abstract StopPolicy Clone();

        /// <summary>
        /// Returns progress of StopPolicy in percents. 0 = Not started, 1 = finished.
        /// </summary>
        /// <returns></returns>
        public abstract double Progress();
    }
}
