using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// FULLY COMMENTED
namespace MCTS_Mod
{
    /// <summary>
    /// Represents AI using the MCTS.
    /// </summary>
    class MCTS
    {
        /// <summary>
        /// Game that is played.
        /// </summary>
        protected IGame game;

        /// <summary>
        /// Selection policy used.
        /// </summary>
        protected SelectionPolicy selectionPolicy;

        /// <summary>
        /// Stop policy used.
        /// </summary>
        public StopPolicy stopPolicy;

        /// <summary>
        /// How many states were expanded so far.
        /// </summary>
        public int statesExpanded = 0;

        /// <summary>
        /// Action called with root as argument after every iteration of MCTS method inside the BestMove Function.
        /// </summary>
        public Action<GameState> iterAction;

        /// <summary>
        /// Action called with root as argument at the beginning of BestMove function.
        /// </summary>
        public Action<GameState> begAction;

        /// <summary>
        /// Action called with root as argument at the end of BestMove function.
        /// </summary>
        public Action<GameState> endAction;

        /// <summary>
        /// String for debugging purposes.
        /// </summary>
        public string debug = "";

        /// <summary>
        /// Initializes the AI.
        /// </summary>
        /// <param name="_game">Game that is played.</param>
        /// <param name="selPolicy">Selection policy used.</param>
        /// <param name="stpPolicy">Stop policy used.</param>
        /// <param name="f">Action called with root as argument after every iteration of MCTS method inside the BestMove Function.</param>
        /// <param name="g">Action called with root as argument at the beginning of BestMove function.</param>
        /// <param name="h">Action called with root as argument at the end of BestMove function.</param>
        public MCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null)
        {
            game = _game;
            selectionPolicy = selPolicy;
            stopPolicy = stpPolicy;
            iterAction = f;
            begAction = g;
            endAction = h;
        }

        /// <summary>
        /// Creates a game tree and returns best possible move from state "root" for player "player".
        /// </summary>
        /// <param name="root">Input game state.</param>
        /// <param name="player">Player for whom we chose best move.</param>
        /// <returns>Best move from "root" for "player".</returns>
        public virtual GameState BestMove(GameState root, int player)
        {
            if (begAction != null)
                begAction(root);

            stopPolicy.Reset();

            statesExpanded = 0;

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

        /// <summary>
        /// Finds and expands state in a game tree defined by "root", using selection policy.
        /// </summary>
        /// <param name="root">Root of a tree.</param>
        /// <returns>Newly expanded state.</returns>
        protected virtual GameState SelectState(GameState root)
        {
            return ExpandState(selectionPolicy.Select(root));
        }

        /// <summary>
        /// Expands son of state "root", adding it to the game tree, and returns it.
        /// </summary>
        /// <param name="root">State with unexplored son.</param>
        /// <returns>Expanded son of "root".</returns>
        protected virtual GameState ExpandState(GameState root)
        {
            if (root == null) // dealing with border case scenario
                return null;

            statesExpanded++; // increment total number of states expanded

            List<GameState> validStates = game.GetValidMoves(root); 

            if (validStates.Count == 0) // dealing with border case scenario
                return null;

            GameState toExpand = validStates[0]; // select first unexplored son as the state to expand

            root.ExploredMoves.Add(toExpand);
            root.ValidMoves().Remove(toExpand); // expand

            return toExpand; // and return
        }

        /// <summary>
        /// Runs a simulation from "root" until a terminal state is reached. Type of simualiton depends on game. Returns value representing the desirability of terminal state reached.
        /// </summary>
        /// <param name="root">Root of simulation tree.</param>
        /// <returns>Value representing the desirability of terminal state reached.</returns>
        protected virtual double Simulate(GameState root)
        {
            GameState currentState = root;

            while (!game.IsTerminal(currentState)) // while terminal state is not reached
            {
                GameState nextState = game.GetRandomValidMove(currentState); // select next state
                currentState = nextState;          
            }


            return game.Evaluate(currentState); // return evaluation of final state
        }

        /// <summary>
        /// Updates relevant states with value "value".
        /// </summary>
        /// <param name="leaf">Starting place for updating.</param>
        /// <param name="value">Value to update with.</param>
        protected virtual void Update(GameState leaf, double value)
        {
            GameState currentState = leaf;
            do // go up the tree
            {
                currentState.Visits++;
                currentState.AddValue(value); // update value

                if (currentState.MaxDepth < leaf.Depth)
                    currentState.MaxDepth = leaf.Depth; // update maximum depth reachable from state if need be

                currentState = currentState.Parent;

            } while (currentState != null);
        }

        /// <summary>
        /// Returns best child of "root" based on winrate.
        /// </summary>
        /// <param name="root">Game state.</param>
        /// <returns>Best child.</returns>
        protected virtual GameState BestChild(GameState root)
        {
            GameState returnState = null;

            double best = Double.NegativeInfinity;

            for (int i = 0; i < root.ExploredMoves.Count; i++) // Go through all children and select one with highest winrate
            {
                GameState g = root.ExploredMoves[i];

                if (g.Winrate > best)
                {
                    best = g.Winrate;
                    returnState = g;
                }
            }
            return returnState;
        }

        /// <summary>
        /// Returns worst child of "root" based on winrate.
        /// </summary>
        /// <param name="root">Game state.</param>
        /// <returns>Worst child.</returns>
        protected virtual GameState WorstChild(GameState root)
        {
            GameState returnState = null;

            double worst = Double.PositiveInfinity;

            for (int i = 0; i < root.ExploredMoves.Count; i++)  // Go through all children and select one with highest winrate
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

        /// <summary>
        /// Resets this AI. Empty in this case.
        /// </summary>
        public virtual void Reset()
        {

        }
    }
}
