using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    /// <summary>
    /// Encompasses functions to help with gathering AI data.
    /// </summary>
    class PlayControl
    {
        /// <summary>
        /// Plays a single game of 2048 and returns the final GameState.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of the game to be played</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <param name="r">Global random</param>
        /// <param name="resetTree">Whether should game tree be reseted after every turn. Implicitly set to false.</param>
        /// <returns>Final GameState</returns>
        public static GameState Play2048AI(MCTS AI, IGame game, bool printStates, Random r, bool resetTree = false)
        {
            AI.Reset(); // Reset AI to be safe
            GameState initState = game.DefaultState(0); // Get initial game position

            if (printStates) // Print it (if should be printing)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState); // To get a proper starting place, play one turn first

            AllRounds(AI, game, ref initState, printStates, r, false, resetTree); // Then play the rest

            return initState; // Return result
        }

        /// <summary>
        /// Plays a single game of derandomized 2048 and returns the final GameState.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of the game to be played</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <param name="r">Global random</param>
        /// <param name="resetTree">Whether should game tree be reseted after every turn. Implicitly set to true.</param>
        /// <returns>Final GameState</returns>
        public static GameState Play2048DAI(MCTS AI, IGame game, bool printStates, Random r, bool resetTree = true)
        {
           return  Play2048AI(AI, game, printStates, r, resetTree); // Same thing
        }

        /// <summary>
        /// Plays multiple games of 2048 and exports result to txt file.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="r">Global random</param>
        /// <param name="iter">Total amount of games to play</param>
        /// <param name="id">Identificator of output text file. Final format:"2048AITest(ID).txt"</param>
        /// <param name="finalMessage">Function that takes the AI and returns string to be written at the end of the file.</param>
        /// <param name="intro">String to be written at the start of the file.</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <param name="messages">Array of Functions that take the final GameState and return strings to be written before the finalMessage</param>
        public static void Play2048AI(MCTS AI, IGame game, Random r, int iter, string id, Func<MCTS, string> finalMessage = null, string intro = "", bool printStates = false, params Func<GameState, string>[] messages)
        {
            using (StreamWriter sw = new StreamWriter(game.Name() + "AITest" + id + ".txt"))
            {
                sw.WriteLine(intro); // Print the intro
                sw.Flush();
                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: " + i); // Observe progress
                    AI.Reset(); // To be safe
                    GameState initState = game.DefaultState(0); // Setup initial state
                    if (printStates) // Maybe print
                        game.PrintState(initState);
                    initState = GetBestState2048(AI, initState);
                    // Setup the GameState and play the game
                    if (game.Name() == "2048")
                        AllRounds(AI, game, ref initState, printStates, r);
                    else
                        AllRounds(AI, game, ref initState, printStates, r, true);

                    foreach (Func<GameState, string> a in messages) // Print the messages
                    {
                        sw.Write(a(initState));
                    }
                    if (finalMessage != null)
                        sw.WriteLine(finalMessage(AI)); // Print the final message
                    else
                        sw.WriteLine();

                }

            }
        }

        /// <summary>
        /// Plays multiple games of derandomized 2048 and exports result to txt file.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="r">Global random</param>
        /// <param name="iter">Total amount of games to play</param>
        /// <param name="id">Identificator of output text file. Final format:"2048DAITest(ID).txt"</param>
        /// <param name="finalMessage">Function that takes the AI and returns string to be written at the end of the file.</param>
        /// <param name="intro">String to be written at the start of the file.</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <param name="messages">Array of Functions that take the final GameState and return strings to be written before the finalMessage</param>
        public static void Play2048DAI(MCTS AI, IGame game, Random r, int iter, string id, Func<MCTS, string> finalMessage = null, string intro = "", bool printStates = false, params Func<GameState, string>[] messages)
        {
            Play2048AI(AI, game, r, iter, id, finalMessage, intro, printStates, messages);
        }

        #region Playing MCTS stuff
        /// <summary>
        /// Finishes playing 2048 from state "initState" and returns final GameState.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="initState">Initial state to start playing from</param>
        /// <param name="printStates">>Whether should individual states be printed to Console</param>
        /// <param name="r">Global random</param>
        /// <param name="derandomized">Derandomizer 2048 or not</param>
        /// <param name="resetTree">Whether the game tree should be reset each turn</param>
        /// <returns>Final GameState</returns>
        public static GameState AllRounds(MCTS AI, IGame game, ref GameState initState, bool printStates, Random r, bool derandomized = false, bool resetTree = false)
        {
            while (!game.IsTerminal(initState))
            {
                initState = (derandomized) ? OneRound2048D(AI, game, initState, printStates) : OneRound2048(AI, game, initState, printStates, r);
                if (resetTree)
                {
                    initState.ExploredMoves.Clear();
                    initState.SetValidMoves(null);
                }
            }
            if (printStates)
                Console.WriteLine("Game over!");

            return initState;
        }

        /// <summary>
        /// Plays a one round of 2048. Returns final GameState.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="initState">State to play from</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <param name="r">Global random</param>
        /// <returns>Final GameState</returns>
        public static GameState OneRound2048(MCTS AI, IGame game, GameState initState, bool printStates, Random r)
        {
            if (printStates)
                game.PrintState(initState);

            initState = GetRandomState((Game2048)game, initState, r);
            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        /// <summary>
        /// Plays a one round of derandomized 2048. Returns final GameState.
        /// </summary>
        /// <param name="AI">AI to play the game</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="initState">State to play from</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <returns>Final GameState</returns>
        public static GameState OneRound2048D(MCTS AI, IGame game, GameState initState, bool printStates)
        {
            if (printStates)
                game.PrintState(initState);

            initState = GetBestState2048(AI, initState);

            return initState;
        }

        /// <summary>
        /// Calculates and returns best move from state 's' using AI 'AI'.  
        /// </summary>
        /// <param name="AI"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static GameState GetBestState2048(MCTS AI, GameState s)
        {
            GameState bestState = AI.BestMove(s, 0);
            if (bestState.Parent != null)
            {
                bestState.Parent.ExploredMoves = null; // Clear references to state so GC can collect
                bestState.RemoveParent();
            }
            return bestState;
        }

        /// <summary>
        /// Returns random state from state 's'. Overrides games possible heuristic.
        /// </summary>
        /// <param name="game">Relevant game instance</param>
        /// <param name="s">Relevant GameState</param>
        /// <param name="r">Global random</param>
        /// <returns></returns>
        public static GameState GetRandomState(IGame game, GameState s, Random r)
        {
            int tmp = s.ExploredMoves.Count;
            GameState randomState = null;
            randomState = s.ExploredMoves[r.Next(0, tmp)];
            randomState.Parent.ExploredMoves = null;
            randomState.RemoveParent();

            return randomState;
        }

        #endregion

        /// <summary>
        /// Plays multiple games of Reversi and exports result to txt file.
        /// </summary>
        /// <param name="AI1">AI representing Player 1</param>
        /// <param name="AI2">AI representing Player 2</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="r">Global random</param>
        /// <param name="iter">Total amount of games to play</param>
        /// <param name="id">Identificator of output text file. Final format:"ReversiAITest(ID).txt"</param>
        /// <param name="intro">String to be written at the start of the file.</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        public static void TestAIsReversi(MCTS AI1, MCTS AI2, GameReversi game, Random r, int iter, string id, string intro = "", bool printStates = false)
        {
            using (StreamWriter sw = new StreamWriter("ReversiAITest" + id + ".txt"))
            {
                sw.WriteLine(intro);

                int won1 = 0;
                int won2 = 0;
                int tie = 0;


                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: " + (i + 1));
                    double res = PlayReversiAIAI(AI1, AI2, game, r, (byte)(i % 2), printStates);
                    if (res == 1)
                    {
                        won1++;
                        sw.WriteLine("Game " + i + " won by AI1");
                    }
                    else if (res == 0)
                    {
                        won2++;
                        sw.WriteLine("Game " + i + " won by AI2");
                    }
                    else
                    {
                        tie++;
                        sw.WriteLine("Game " + i + " was a tie");
                    }
                }

                sw.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
                Console.WriteLine("AI1 won " + won1 + " times, AI2 won " + won2 + " times, total ties: " + tie);
            }
        }

        /// <summary>
        /// Plays a single game of Reversi and returns the final GameState.
        /// </summary>
        /// <param name="AI1">AI representing Player 1</param>
        /// <param name="AI2">AI representing Player 2</param>
        /// <param name="game">Instance of played game</param>
        /// <param name="r">Global random</param>
        /// <param name="first">Starting player</param>
        /// <param name="printStates">Whether should individual states be printed to Console</param>
        /// <returns></returns>
        public static double PlayReversiAIAI(MCTS AI1, MCTS AI2, GameReversi game, Random r, byte first, bool printStates)
        {
            AI1.Reset();
            AI2.Reset();

            GameState initState = game.DefaultState(first);

            GameState currentState = initState;

            while (!game.IsTerminal(currentState))
            {
                GameState tmpState = currentState;

                currentState.ResetBelow();

                currentState.SetValidMoves(game.GetValidMoves(currentState));

                GameState prevState = currentState;

                if (currentState.PlayedBy == 0)
                    currentState = AI2.BestMove(currentState, 1);
                else
                    currentState = AI1.BestMove(currentState, 0);

                if (currentState == null)
                {
                    if (prevState.PlayedBy == 0)
                        currentState = AI2.BestMove(prevState, 1);
                    else
                        currentState = AI1.BestMove(prevState, 0);
                }

                if (printStates)
                    game.PrintState(currentState);

                currentState.Parent.ExploredMoves = null;
                currentState.Parent = null;
            }


            double ret = 0;
            double ev = game.Evaluate(currentState);
            if (ev == 0.5)
                ret = 0.5;
            else if (ev > 0.5)
                ret = 1;
            return ret;
        }
    }
}
