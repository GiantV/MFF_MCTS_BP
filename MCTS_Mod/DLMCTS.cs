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

        //List<int> metMovesIDs = new List<int>();

        Dictionary<int, bool> metMovesIDs = new Dictionary<int, bool>();

        public DLMCTS(IGame _game, SelectionPolicy selPolicy, int depth, int parallelSimulations, bool _useRAVE,
            Action<GameState> f = null, 
            Action<GameState> g = null, 
            Action<GameState> h = null) : base(_game, selPolicy, new StopPolicyDepth(depth), f, g, h)
        {
            pSimulations = parallelSimulations;
            if (useRAVE = _useRAVE) SetupRAVE();
        }

        private void SetupRAVE()
        {
            this.selectionPolicy.onVisitAction = (GameState g) => metMovesIDs.Add(g.ID,true);
        }

        public override GameState BestMove(GameState root, int player)
        {
            if (begAction != null)
                begAction(root);

            stopPolicy.Reset();

            statesExpanded = 0;

            while (stopPolicy.StopCondition(root))
            {
                GameState selectedState = SelectState(root);
                if (selectedState == null) break;

                if (useRAVE)
                {


                    Dictionary<int, bool>[] waitList = new Dictionary<int, bool>[pSimulations];

                    Parallel.For(0, pSimulations, (int i) =>
                    {
                        waitList[i] = metMovesIDs.ToDictionary(entry => entry.Key, entry => entry.Value);
                    });

                    Parallel.ForEach(waitList, (Dictionary<int, bool> personalDictionary) =>
                    {
                        double value = RAVESimulation(selectedState, ref personalDictionary);
                        RAVEUpdate(selectedState, value, personalDictionary);
                    });
                }
                else
                {
                    double value = Simulate(selectedState);
                    Update(selectedState, value);
                }


                if (iterAction != null)
                    iterAction(root);

            }
            if (endAction != null)
                endAction(root);
            if (player == 0)
                return BestChild(root);
            else
                return WorstChild(root);
        }

        protected override double Simulate(GameState root)
        {
            double[] res = new double[pSimulations];

            Parallel.For(0, pSimulations, (int i) => { res[i] = base.Simulate(root); });

            double result = res.Sum() / (double)pSimulations;

            return result;
        }

        private double RAVESimulation (GameState root, ref Dictionary<int, bool> metIDs)
        {
            GameState currentState = root;

            while (!game.IsTerminal(currentState))
            {
                GameState nextState = game.GetRandomValidMove(currentState);
                currentState = nextState;
                if (!metIDs.ContainsKey(currentState.ID)) metIDs.Add(currentState.ID, true);
            }


            return game.Evaluate(currentState);
        }

        private void RAVEUpdate(GameState leaf, double value, Dictionary<int, bool> metIDs)
        {
            GameState currentState = leaf;
            do
            {
                lock (currentState)
                {
                    currentState.Visits++;
                    currentState.AddValue(value);

                    foreach (GameState g in currentState.ExploredMoves)
                    {
                        if (metIDs.ContainsKey(g.ID))
                        {
                            g.MiscValue += value;
                            g.RAVEVisits++;
                        }
                    }

                    if (currentState.MaxDepth < leaf.Depth)
                        currentState.MaxDepth = leaf.Depth;
                }
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
