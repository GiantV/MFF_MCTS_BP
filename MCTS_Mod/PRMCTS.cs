using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class PRMCTS : MCTS
    {
        double width = 0.0;
        double timeLimit = 0;

        int initialCount = 0;

        /// <summary>
        /// Should quality of moves be based on winrate. False means based on visits.
        /// </summary>
        bool evaluateByWinrate = false;

        public bool fakePrune = false;

        private int pruned = -1;

        private bool hasPruned = false;

        public PRMCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, double _width, double _timeLimit, Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            width = _width;
            timeLimit = _timeLimit;
        }

        public override GameState BestMove(GameState root, int player)
        {
            hasPruned = false;
            initialCount = root.Visits;
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
            List<GameState> candidates = root.ExploredMoves;

            if (evaluateByWinrate)
                candidates.OrderBy((GameState g) => g.Winrate);
            else
                candidates.OrderBy((GameState g) => g.Visits);

            int limit = (int)Math.Floor(width * candidates.Count);

            candidates.RemoveRange(limit, candidates.Count - limit);
            root.ExploredMoves = candidates;
        }

        private void FakePrune(GameState root)
        {
            if (root.Depth == 0)
                pruned = -1;

            if (pruned >= root.Depth)
                return;

            List<GameState> candidates = root.ExploredMoves;

            
            candidates.OrderBy((GameState g) => g.Visits);

            int limit = (int)Math.Floor(width * candidates.Count);

            for (int i = limit; i < candidates.Count; i++)
            {
                candidates[i].MiscValue += 100;
            }

            candidates.OrderBy((GameState g) => g.Winrate);

            for (int i = limit; i < candidates.Count; i++)
            {
                candidates[i].MiscValue += 101;
            }
            pruned = root.Depth;
        }
    }
}
