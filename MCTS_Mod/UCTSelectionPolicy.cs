using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// The UCT selection policy.
    /// </summary>
    class UCTSelectionPolicy : SelectionPolicy
    {
        /// <summary>
        /// The actual game we work with. Used to determine things like terminality of states.
        /// </summary>
        public IGame game;

        /// <summary>
        /// The exploration vs exploitation constant.
        /// </summary>
        public double con = 0.0;

        /// <summary>
        /// Depth reacher during the last selection process. Used for debugging only.
        /// </summary>
        public int depthReached = 0;

        /// <summary>
        /// The ID of the last move visited in the selection process. Used for debugging only.
        /// </summary>
        public int lastID = 0;


        /// <summary>
        /// Selection policy based on the UCT method.
        /// </summary>
        /// <param name="_game">Game played.</param>
        /// <param name="_con">The exploration vs exploitation constant.</param>
        /// <param name="onVisit">ACtion to be called on gamestates visited in the selection process. Null by default.</param>
        public UCTSelectionPolicy(IGame _game, double  _con, Action<GameState> onVisit = null)
        {
            game = _game;
            con = _con;

            onVisitAction = onVisit;
        }

        /// <summary>
        /// Returns state to be expanded in a subtree defined by "root",
        /// </summary>
        /// <param name="root">Root.</param>
        /// <returns>State to be expanded.</returns>
        public override GameState Select(GameState root)
        {
            GameState currentState = root;

            depthReached = root.Depth;

           
            while (!game.IsTerminal(currentState))  //While not reached terminal state
            {
                if (onVisitAction != null) // Apply action if it exists
                {
                    onVisitAction(currentState);
                }

                if (game.GetAmountOfValidMoves(currentState) > 0) //If can expand, return current state
                    return currentState;
                else if (game.NextPlayer(root.PlayedBy) == 0) //Else if next player is player 0, set current state as best UCB child
                {
                    previousState = currentState;
                    currentState = BestUCBChild(currentState);
                }
                else //Else, meaning next player is player 1 (for 2-player games) set current state as worst UCB child
                {
                    previousState = currentState;
                    currentState = WorstUCBChild(currentState);
                }

                depthReached++;
                if (currentState != null)
                    lastID = currentState.ID;

                if (currentState == null)
                    return null;
            }

            return currentState;
        }

        /// <summary>
        /// Returns child with highest UCB value.
        /// </summary>
        /// <param name="entryState">Parent state.</param>
        /// <returns>Best child.</returns>
        public GameState BestUCBChild(GameState entryState)
        {
            GameState currentState = entryState;
            GameState bestState = null;
            double best = Double.NegativeInfinity;
            for (int i = 0; i < currentState.ExploredMoves.Count(); i++)
            {
                GameState g = currentState.ExploredMoves[i];
                double UCB1 = g.Winrate + con * Math.Sqrt((2.0 * Math.Log((double)currentState.Visits) / (double)g.Visits));
                if (UCB1 > best)
                {
                    bestState = g;
                    best = UCB1;
                }
            }
            return bestState;
        }

        /// <summary>
        /// Returns child with lowest UCB value.
        /// </summary>
        /// <param name="entryState">Parent state.</param>
        /// <returns>Worst child.</returns>
        public GameState WorstUCBChild(GameState entryState)
        {
            GameState currentState = entryState;
            GameState bestState = null;
            double best = Double.NegativeInfinity;
            for (int i = 0; i < currentState.ExploredMoves.Count(); i++)
            {
                GameState g = currentState.ExploredMoves[i];
                double UCB1 = 1 - g.Winrate + con * Math.Sqrt((2.0 * Math.Log((double)currentState.Visits) / (double)g.Visits));
                if (UCB1 > best)
                {
                    bestState = g;
                    best = UCB1;
                }
            }

            return bestState;
        }


        /// <summary>
        /// Returns UCT seletion policy with optimal value for stanmdard MCTS.
        /// </summary>
        /// <param name="game">Game we will be using selection policy on.</param>
        /// <param name="onVisit">Any Action we want to apply on every visit.</param>
        /// <returns>Optimal UCT selection policy.</returns>
        public static UCTSelectionPolicy OptimalSelectionPolicy(IGame game, Action<GameState> onVisit = null)
        {
            switch (game.Name())
            {
                case "2048":
                    return new UCTSelectionPolicy(game, 0.7, onVisit);
                case "2048D":
                    return new UCTSelectionPolicy(game, 250, onVisit);
                case "Reversi":
                    return new UCTSelectionPolicy(game, 0.7, onVisit);                    
                default:
                    return null;
            }
        }



    }
}
