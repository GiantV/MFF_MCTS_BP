using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MCTS_Mod
{
    /// <summary>
    /// Stop policy based on time elapsed.
    /// </summary>
    class StopPolicyTime : StopPolicy
    {
        /// <summary>
        /// Time period that can be spent building the tree.
        /// </summary>
        int timeTotal = 0;

        /// <summary>
        /// Bool used to make certain at least on state is expanded.
        /// </summary>
        bool oneExpanded = false;

        /// <summary>
        /// Time elapsed is calculated by a stopwatch.
        /// </summary>
        Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Stop policy based on time elapsed.
        /// </summary>
        /// <param name="time">How much time needs to pass before StopCondition returns false.</param>
        public StopPolicyTime(int time)
        {
            timeTotal = time;
        }

        /// <summary>
        /// Resets the stopwatch. Should be called before usage.
        /// </summary>
        public override void Reset()
        {
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>
        /// Returns true if time limit has not been exceeded. False otherwise.
        /// </summary>
        /// <param name="s">Not actually used.</param>
        /// <returns>True if time limit has not been exceeded. False otherwise.</returns>
        public override bool StopCondition(GameState s)
        {
            if (!oneExpanded)
            {
                oneExpanded = true;
                return true;
            }
            return this.stopwatch.ElapsedMilliseconds < timeTotal;
        }

        /// <summary>
        /// Returns clone of this policy.
        /// </summary>
        /// <returns>New object with same parameters.</returns>
        public override StopPolicy Clone()
        {
            return new StopPolicyTime(timeTotal);
        }

        /// <summary>
        /// Returns rough progress as percentage. Note, very rough.
        /// </summary>
        /// <returns>Progress made.</returns>
        public override double Progress()
        {
            return (double)this.stopwatch.ElapsedMilliseconds / (double)timeTotal;
        }
    }
}
