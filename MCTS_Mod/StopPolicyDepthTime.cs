using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Stop policy based on depth reached and time elapsed.
    /// </summary>
    class StopPolicyDepthTime : StopPolicy
    {
        /// <summary>
        /// The depth part of the stop policy
        /// </summary>
        StopPolicyDepth stpDepth;
        /// <summary>
        /// The time part of the stop policy
        /// </summary>
        StopPolicyTime stpTime;

        //For debugging purposes
        public int totalCountDepth = 0;
        public int totalCountTime = 0;

        private int depth = 0;
        private int time = 0;


        /// <summary>
        /// Stop policy based on depth reached and time elapsed. Stops whene either of the limits is reached.
        /// </summary>
        /// <param name="_depth">Maximum depth.</param>
        /// <param name="_time">Time allowed</param>
        public StopPolicyDepthTime(int _depth, int _time)
        {
            time = _time;
            depth = _depth;

            stpDepth = new StopPolicyDepth(_depth);
            stpTime = new StopPolicyTime(_time);
        }

        /// <summary>
        /// Resets the stop policy. Should be called before usage.
        /// </summary>
        public override void Reset()
        {
            stpDepth.Reset();
            stpTime.Reset();
        }

        /// <summary>
        /// Returns false when depth is reached or time is exceeded.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <returns>True when none of the limits have been exceeded, false otherwise.</returns>
        public override bool StopCondition(GameState root)
        {
            bool d = stpDepth.StopCondition(root);
            bool t = stpTime.StopCondition(root);
#warning check this out
            if (!d) totalCountDepth++;
            if (!t) totalCountTime++;

            return d && t; // d || t
        }

        /// <summary>
        /// Returns clone of this policy.
        /// </summary>
        /// <returns>New object with same parameters.</returns>
        public override StopPolicy Clone()
        {
            return new StopPolicyDepthTime(depth, time);
        }

        /// <summary>
        /// Returns rough progress as percentage. Note, very rough.
        /// </summary>
        /// <returns>Progress made.</returns>
        public override double Progress()
        {
            return Math.Max(stpDepth.Progress(), stpTime.Progress());
        }
    }
}
