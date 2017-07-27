using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Stop policy based on depth reached.
    /// </summary>
    class StopPolicyDepth : StopPolicy
    {
        /// <summary>
        /// Upon reaching what depth should stop exploring.
        /// </summary>
        int stopDepth = 0;

        /// <summary>
        /// Stop policy based on depth reached.
        /// </summary>
        /// <param name="depth">Upon reaching what depth should stop exploring.</param>
        public StopPolicyDepth(int depth)
        {
            stopDepth = depth;
        }

        /// <summary>
        /// Returns true if subtree of GameState "root" has no states in specified depth.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public override bool StopCondition(GameState root)
        {
            return (root.MaxDepth < stopDepth + root.Depth);
        }

        /// <summary>
        /// Resets stop policy. Does nothing in this implementation.
        /// </summary>
        public override void Reset()
        {
            //EMPTY
        }

        /// <summary>
        /// Returns clone of this policy.
        /// </summary>
        /// <returns>New object with same parameters.</returns>
        public override StopPolicy Clone()
        {
            return new StopPolicyDepth(this.stopDepth);
        }

    }
}
