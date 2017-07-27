using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class MCTS
    {
        protected IGame game;
        protected SelectionPolicy selectionPolicy;
        public StopPolicy stopPolicy;
        public int statesExpanded = 0;

        public Action<GameState> iterAction;
        public Action<GameState> begAction;
        public Action<GameState> endAction;

        public string debug = "";

        public MCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null)
        {
            game = _game;
            selectionPolicy = selPolicy;
            stopPolicy = stpPolicy;
            iterAction = f;
            begAction = g;
            endAction = h;
        }


        public virtual GameState BestMove(GameState root, int player)
        {
            if (begAction != null)
                begAction(root);
            stopPolicy.Reset();
            while (stopPolicy.StopCondition(root))
            {
                GameState selectedState = SelectState(root);
                if (selectedState == null) break;
                double value = Simulate(selectedState);
                Update(selectedState, value);



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

        protected virtual GameState SelectState(GameState root)
        {
            return ExpandState(selectionPolicy.Select(root));
        }

        protected virtual GameState ExpandState(GameState root)
        {
            if (root == null)
                return null;
            statesExpanded++;
            List<GameState> validStates = game.GetValidMoves(root);

            if (validStates.Count == 0)
                return null;

            GameState toExpand = validStates[0];
            root.ExploredMoves.Add(toExpand);
            root.ValidMoves().Remove(toExpand);

            return toExpand;
        }

        protected virtual double Simulate(GameState root)
        {
            GameState currentState = root;

            while (!game.IsTerminal(currentState))
            {
                GameState nextState = game.GetRandomValidMove(currentState);
                currentState = nextState;          
            }


            return game.Evaluate(currentState);
        }

        protected virtual void Update(GameState leaf, double value)
        {
            GameState currentState = leaf;
            do
            {
                currentState.Visits++;
                currentState.AddValue(value);

                if (currentState.MaxDepth < leaf.Depth)
                    currentState.MaxDepth = leaf.Depth;

                currentState = currentState.Parent;

            } while (currentState != null);
        }

        protected virtual GameState BestChild(GameState root)
        {
            GameState returnState = null;

            double best = Double.NegativeInfinity;

            for (int i = 0; i < root.ExploredMoves.Count; i++)
            {
                GameState g = root.ExploredMoves[i];

                if (g.Winrate > best)
                {
                    best = g.Winrate;
                    returnState = g;
                }

                /*if (g.Visits > best)
                {
                    best = g.Visits;
                    returnState = g;
                }*/
            }
            return returnState;
        }

        protected virtual GameState WorstChild(GameState root)
        {
            GameState returnState = null;

            double worst = Double.PositiveInfinity;

            for (int i = 0; i < root.ExploredMoves.Count; i++)
            {
                GameState g = root.ExploredMoves[i];
                if (g.Winrate < worst)
                {
                    worst = g.Winrate;
                    returnState = g;
                }
            }

            return returnState;
        }

        public virtual void Reset()
        {

        }
    }
}
