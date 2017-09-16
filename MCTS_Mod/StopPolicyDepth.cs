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
        /// Depth of the last tested root. -1 default.
        /// </summary>
        int rootAt = -1;
        /// <summary>
        /// Maximum last reached depth.
        /// </summary>
        int currentMaxReachedDepth = 0;

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
            if (root.Depth > rootAt)
                rootAt = root.Depth;

            currentMaxReachedDepth = root.MaxDepth;


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

        /// <summary>
        /// Returns rough progress as percentage. Note, very rough.
        /// </summary>
        /// <returns>Progress made.</returns>
        public override double Progress()
        {
            return currentMaxReachedDepth / (rootAt + stopDepth);
        }
    }
}
