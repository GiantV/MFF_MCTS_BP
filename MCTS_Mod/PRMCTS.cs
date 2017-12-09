using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class PRMCTS : MCTS
    {
        /// <summary>
        /// Percentage of roots children remaining after prune. 0 = 0%, 1 = 100%.
        /// </summary>
        double width = 0.0;

        /// <summary>
        /// At what percemtage of elapsed time should the tree be pruned. 0 = 0%, 1 = 100%.
        /// </summary>
        double timeLimit = 0.0;


        /// <summary>
        /// Should quality of moves be based on winrate. False means based on visits.
        /// </summary>
        bool evaluateByWinrate = false;

        /// <summary>
        /// If set to true, nodes that would be pruned are just marked as pruned. Used for testing purposes.
        /// </summary>
        public bool fakePrune = false;

        /// <summary>
        /// 
        /// </summary>
        private int pruned = -1;

        public bool hasPruned = false;

        /// <summary>
        /// Total nodes pruned. If no pruning happened is 0.
        /// </summary>
        public int totalPruned = 0;

        /// <summary>
        /// Initializes the AI.
        /// </summary>
        /// <param name="_game">Game that is played.</param>
        /// <param name="selPolicy">Selection policy used.</param>
        /// <param name="stpPolicy">Stop policy used.</param>
        /// <param name="_width">Percentage of roots children remaining after prune. 0 = 0%, 1 = 100%.</param>
        /// <param name="_timeLimit">At what percemtage of elapsed time should the tree be pruned. 0 = 0%, 1 = 100%.</param>
        /// <param name="f">Action called with root as argument after every iteration of MCTS method inside the BestMove Function.</param>
        /// <param name="g">Action called with root as argument at the beginning of BestMove function.</param>
        /// <param name="h">Action called with root as argument at the end of BestMove function.</param>
        public PRMCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, double _width, double _timeLimit,
            Action<GameState, MCTS> f = null, Action<GameState, MCTS> g = null, Action<GameState, MCTS> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            width = _width;
            timeLimit = _timeLimit;
        }

        public override GameState BestMove(GameState root, int player)
        {
            hasPruned = false;
            totalPruned = 0;

            return base.BestMove(root, player);
        }

        protected override GameState SelectState(GameState root)
        {
            
            if (!hasPruned && stopPolicy.Progress() >= timeLimit)
            {
                hasPruned = true;

                if (!fakePrune)
                    Prune(root);
                else
                    FakePrune(root);
            }
            
            return base.SelectState(root);
        }

        private void Prune(GameState root)
        {
            List<GameState> candidates = root.ExploredMoves; // Candidates for pruing

            /*if (candidates.Count == 0)
                return;*/

            if (evaluateByWinrate)
                candidates.OrderBy((GameState g) => g.Winrate);
            else    // Order either by Winrate or Visits
                candidates.OrderBy((GameState g) => g.Visits);

            int limit = (int)Math.Floor(width * candidates.Count); // Calculate actual width we are pruning to

            /*if (limit >= candidates.Count)
                limit = candidates.Count - 1;*/

           for (int i = limit; i < candidates.Count; i++) // Count number of nodes removed
                totalPruned += candidates[i].Visits;

            candidates.RemoveRange(limit, candidates.Count - limit); // Remove bad candidates such that only "limit" candidates are left.
            root.ExploredMoves = candidates;
        }

        private void FakePrune(GameState root)
        {
            if (root.Depth == 0)
                pruned = -1;

            if (pruned >= root.Depth) // If we pruned once already, reuturn
                return;

            List<GameState> candidates = root.ExploredMoves; // Candidates for pruning


            // Candidates are ordered first by Vists and marked
            candidates.OrderBy((GameState g) => g.Visits);

            int limit = (int)Math.Floor(width * candidates.Count);

            for (int i = limit; i < candidates.Count; i++)
            {
                candidates[i].MiscValue += 100;
            }
            // Then by Winrate and marked differently
            candidates.OrderBy((GameState g) => g.Winrate);

            for (int i = limit; i < candidates.Count; i++)
            {
                candidates[i].MiscValue += 101;
            }

            // In the end we differentiate between 3 kinds of marks. "Would be pruned if we used Visits based ordering",
            // "would be pruned if we used Winrate based ordering" and "would be pruned regardless of ordering used".
            pruned = root.Depth;
        }
    }
}
