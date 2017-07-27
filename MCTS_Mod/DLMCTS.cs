using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class DLMCTS : MCTS
    {
        int pSimulations = 1;

        bool useRAVE = false;

        List<int> metMovesIDs = new List<int>();

        public DLMCTS(IGame _game, SelectionPolicy selPolicy, int depth, int parallelSimulations, Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null) : base(_game, selPolicy, new StopPolicyDepth(depth), f, g, h)
        {
            pSimulations = parallelSimulations;
        }
        public DLMCTS(IGame _game, SelectionPolicy selPolicy, int depth, int time, int parallelSimulations, Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null) : base(_game, selPolicy, new StopPolicyDepthTime(depth,time), f, g, h)
        {
            pSimulations = parallelSimulations;
        }

        private void SetupRAVE()
        {
            this.selectionPolicy.onVisitAction = (GameState g) => metMovesIDs.Add(g.ID);
        }


        protected override double Simulate(GameState root)
        {
            double[] res = new double[pSimulations];

            Parallel.For(0, pSimulations, (int i) => {  res[i] = (!useRAVE) ? base.Simulate(root) : this.RAVESimulation(root); });

            double result = res.Sum() / (double)pSimulations;

            return result;
        }

        protected override void Update(GameState leaf, double value)
        {
            if (!useRAVE) base.Update(leaf, value); else this.RAVEUpdate(leaf, value);
        }

        private double RAVESimulation (GameState root)
        {
            GameState currentState = root;

            while (!game.IsTerminal(currentState))
            {
                GameState nextState = game.GetRandomValidMove(currentState);
                currentState = nextState;
                if (!metMovesIDs.Contains(currentState.ID)) metMovesIDs.Add(currentState.ID);
            }


            return game.Evaluate(currentState);
        }

        private void RAVEUpdate(GameState leaf, double value)
        {
            GameState currentState = leaf;
            do
            {
                currentState.Visits++;
                currentState.AddValue(value);

                foreach(GameState g in currentState.ExploredMoves)
                {
                    if (metMovesIDs.Contains(g.ID))
                    {
                        g.MiscValue += value;
                        g.RAVEVisits++;
                    }
                }

                if (currentState.MaxDepth < leaf.Depth)
                    currentState.MaxDepth = leaf.Depth;

                currentState = currentState.Parent;

            } while (currentState != null);
        }

        protected override GameState SelectState(GameState root)
        {
            metMovesIDs.Clear();

            return base.SelectState(root);
        }


    }
}
