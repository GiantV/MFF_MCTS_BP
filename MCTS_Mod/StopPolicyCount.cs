using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Stop policy returning false when tree reaches specific size
    /// </summary>
    class StopPolicyCount : StopPolicy
    {

        int currentCount = 0;
        int maxCount = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">Size of tree in nodes after which to stop.</param>
        public StopPolicyCount(int count)
        {
            maxCount = count;
        }

        /// <summary>
        /// Resets stop policy. Should be called before starting tree construction.
        /// </summary>
        public override void Reset()
        {
            currentCount = 0;
        }

        /// <summary>
        /// Retruns false when tree reaches the size specified in constructor.
        /// </summary>
        /// <param name="s">Root of game tree.</param>
        /// <returns></returns>
        public override bool StopCondition(GameState s)
        {
            if (currentCount == 0)
                currentCount = s.Visits;
            return (s.Visits - currentCount < maxCount);
        }

        /// <summary>
        /// Returns clone of this policy.
        /// </summary>
        /// <returns>New object with same parameters.</returns>
        public override StopPolicy Clone()
        {
            return new StopPolicyCount(this.maxCount);
        }
    }

}
