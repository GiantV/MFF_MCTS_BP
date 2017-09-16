using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Represents the game 2048. Implements IGame interface.
    /// </summary>
    class Game2048 : IGame
    {
        /// <summary>
        /// Size of the game board. Board is a square so this is both height and width.
        /// </summary>
        protected const int BOARDSIZE = 4;

        // IDs of player/random
        protected const int PLAYER = 0;
        protected const int RANDOM = 1;

        /// <summary>
        /// Marks a board after a player turn without an empty spot
        /// </summary>
        protected const int CANNOTPLACE = 1;

        /// <summary>
        /// Maximum estimated reachable depth
        /// </summary>
        protected const int MAXESTDEPTH = 131038;

        /// <summary>
        /// What root we take of the result of evaluation function
        /// </summary>
        public int EVALROOT = 3;

        // Parameters for heuristic simulation
        public double HEURNEXTVAL = 1;
        public double HEURTILEVAL = 1.9;

        protected Random r;

        /// <summary>
        /// Are we using heuristic simulation and if so, which one
        /// </summary>
        public int HEURSIM = 0;

        /// <summary>
        /// Represents directions of player moves
        /// </summary>
        protected enum Direction { Up, Down, Left, Right };

        /// <summary>
        /// Represent the game 2048.
        /// </summary>
        /// <param name="globalRandom">Random to be used.</param>
        /// <param name="useHeuristicSimulation">Whether we should use a heuristic simulation. 0-No, 1-Use heuristic 1, 2-Use heuristic 2.</param>
        public Game2048(Random globalRandom, int useHeuristicSimulation)
        {
            r = globalRandom;
            HEURSIM = useHeuristicSimulation;
        }

        /// <summary>
        /// Represent the game 2048. More customizable.
        /// </summary>
        /// <param name="globalRandom">Random to be used.</param>
        /// <param name="useHeuristicSimulation">Whether we should use a heuristic simulation. 0-No, 1-Use heuristic 1, 2-Use heuristic 2.</param>
        /// <param name="_HEURNEXTVAL">Heuristic simulation parameter 1.</param>
        /// <param name="_HEURTILEVAL">Heuristic simulation parameter 2.</param>
        /// <param name="_EVALROOT">Root of final fraction of evaluation.</param>
        public Game2048(Random globalRandom, int useHeuristicSimulation, double _HEURNEXTVAL, double _HEURTILEVAL, int _EVALROOT)
        {
            r = globalRandom;
            HEURSIM = useHeuristicSimulation;
            HEURNEXTVAL = _HEURNEXTVAL;
            HEURTILEVAL = _HEURTILEVAL;
            EVALROOT = _EVALROOT;
        }

        /// <summary>
        /// Returns all possible valid moves for game state "state". Note, does not take into account already explored moves.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>List of all possible valid moves,</returns>
        public virtual List<GameState> CalcValidMoves(GameState state)
        {
            List<GameState> validMoves = new List<GameState>();
            int[,] board = (int[,])state.Board;
            if (state.PlayedBy == RANDOM) //If previous state was random move, there always 4 option: Up, Down, Left and Right
            {
                validMoves.Add(PlayerMove(state, board, Direction.Up   ));
                validMoves.Add(PlayerMove(state, board, Direction.Down ));
                validMoves.Add(PlayerMove(state, board, Direction.Left ));
                validMoves.Add(PlayerMove(state, board, Direction.Right));
            }
            else // If previous state was a player move, there are 2 options for each empty tile
            {
                for (int i = 0; i < BOARDSIZE; i++)
                {
                    for (int j = 0; j < BOARDSIZE; j++)
                    {
                        if (board[i, j] == 0)
                        {
                            GameState2048 twoMove  = new GameState2048(state, RANDOM, SetMove(board, i, j, 2), 0);
                            GameState2048 fourMove = new GameState2048(state, RANDOM, SetMove(board, i, j, 4), 0);

                            twoMove.tiles  = ((GameState2048)state).tiles + 1;
                            fourMove.tiles = ((GameState2048)state).tiles + 1;

                            // Set ID of the random move
                            twoMove.ID  = 200 + j * 10 + i; 
                            fourMove.ID = 400 + j * 10 + i;

                            validMoves.Add(twoMove );
                            validMoves.Add(fourMove);
                        }
                    }
                }
            }


            return validMoves;
        }

        /// <summary>
        /// Returns the default (or initial) board of the game 2048. Meaning an empty board with a 2 or 4 in a random tile.
        /// </summary>
        /// <param name="firstPlayer">Irrelevant in a single player game.</param>
        /// <returns>Game state representing the default position.</returns>
        public virtual GameState DefaultState(byte firstPlayer)
        {
            int[,] board = new int[BOARDSIZE, BOARDSIZE];
            int first = r.Next(0,16); // Select random tile and value
            int par   = r.Next(1, 3);
            par *= 2;
            for (int i = 0; i < BOARDSIZE; i++) // Place
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i, j] = 0;
                    if (first == 0)
                        board[i, j] = par;
                    first--;
                }
            }

            GameState2048 returnState = new GameState2048(null, RANDOM, board, 0);
            returnState.tiles = 1;
            return returnState;
        }

        /// <summary>
        /// Evaluates a terminal game state assigning it a value from the interval [0,1].
        /// </summary>
        /// <param name="state">Terminal game state to be evaluated.</param>
        /// <returns>Value from the interval [0,1].</returns>
        public virtual double Evaluate(GameState state)
        {
            return Math.Pow((double)state.Depth / (double)MAXESTDEPTH,1.0/EVALROOT);
        }
        
        /// <summary>
        /// Returns 1 if game state "state" contains the value 2048, 0 otherwise.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Result of the game in game state "state".</returns>
        public double GameResult(GameState state)
        {
            int[,] board = new int[BOARDSIZE, BOARDSIZE];
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j] == 2048) return 1.0;
                }
            }
            return 0.0;
        }

        /// <summary>
        /// Returns the amount of valid moves availible in game state "state".
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Amount of valid moves.</returns>
        public virtual int GetAmountOfValidMoves(GameState state)
        {
            return GetValidMoves(state).Count();
        }

        /// <summary>
        /// Returns a random valid move from game state "state". Tries to calculate only the one random move so as to speed up simulation. 
        /// May apply heuristics depending on the parameter passed in constructor.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>One random valid move.</returns>
        public virtual GameState GetRandomValidMove(GameState state)
        {
            // Works as a switch for the value HEURSIM. Called function varies based on used heuristic.
            return (HEURSIM==0) ? GetRandomValidMoveNonHeur(state) : ((HEURSIM==1) ?GetRandomValidMoveHeur1(state) : GetRandomValidMoveHeur2(state));
        }

        /// <summary>
        /// Returns a random valid move without using any heuristic.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Random valid move.</returns>
        protected virtual GameState GetRandomValidMoveNonHeur(GameState state)
        {
            // If "state" is played by player, we return a random random move. Unfortunately we need to go throught the entire board for that and return one
            // of the empty tiles with random value (2 or 4)
            if (state.PlayedBy == PLAYER)
            {
                int tot = 0;
                int ran = 2 * r.Next(1, 3);

                int[,] b = (int[,])state.Board;
                    
                for (int i = 0; i < BOARDSIZE; i++)
                {
                    for (int j = 0; j < BOARDSIZE; j++)
                    {
                        if (b[i, j] == 0)
                            tot++;
                    }
                }

                int pos = r.Next(0, tot);

                for (int i = 0; i < BOARDSIZE; i++)
                {
                    for (int j = 0; j < BOARDSIZE; j++)
                    {
                        if (b[i, j] == 0)
                            tot--;
                        if (tot == 0)
                        {
                            GameState2048 returnState = new GameState2048(state, RANDOM, SetMove(b, i, j, ran), 0);
                            returnState.tiles = ((GameState2048)state).tiles + 1;
                            return returnState;
                        }
                    }
                }
                return new GameState2048(state, RANDOM, b);
            }
            else // But if "state" was a random move, we can calculate just one of the player moves.
            {
                int randomMove = r.Next(0, 4);
                switch(randomMove)
                {
                    case 0:
                        return PlayerMove(state, (int[,])state.Board, Direction.Up);
                    case 1:
                        return PlayerMove(state, (int[,])state.Board, Direction.Down);
                    case 2:
                        return PlayerMove(state, (int[,])state.Board, Direction.Left);
                    case 3:
                        return PlayerMove(state, (int[,])state.Board, Direction.Right);
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Returns a random valid move using the heuristic 1.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Random valid move.</returns>
        protected virtual GameState GetRandomValidMoveHeur1(GameState state)
        {
            if (NextPlayer(state.PlayedBy) == RANDOM) // Random moves are still random
                return GetRandomValidMoveNonHeur(state);

            int rTiles = ((GameState2048)state).tiles;

            GameState bestState = null;
            double bestValue = double.NegativeInfinity;

            foreach(GameState2048 g in GetValidMoves(state)) // But for player moves, we calculate all of them and return the heuristically best one
            {
                int tiles = g.tiles;
                int next = 0;

                int[,] gBoard = (int[,])g.Board;

                for (int i = 0; i < BOARDSIZE - 1; i++)
                    for (int j = 0; j < BOARDSIZE - 1; j++)
                    {
                        if (gBoard[i, j] == gBoard[i + 1, j] && gBoard[i, j] != 0) next++;
                        if (gBoard[i, j] == gBoard[i, j + 1] && gBoard[i, j] != 0) next++;
                    }


                double val = HEURTILEVAL * (rTiles - tiles) + next * HEURNEXTVAL; // Quality of move is based on empty tiles and neighboring same tiles
                if (val > bestValue)
                {
                    bestValue = val;
                    bestState = g;
                }
            }
            return bestState;
        }

        /// <summary>
        /// Returns a random valid move using the heuristic 2.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Random valid move.</returns>
        protected virtual GameState GetRandomValidMoveHeur2(GameState state)
        {
            if (NextPlayer(state.PlayedBy) == RANDOM) // Random moves are still random
                return GetRandomValidMoveNonHeur(state);

            GameState bestState = null;
            int bestValue = Int32.MaxValue;

            foreach (GameState2048 g in GetValidMoves(state)) // But for player moves we calculate all of them and return the first one with leat tiles.
            {
                if (g.tiles < bestValue) // Since all GameState2048s keep track opf their number of tiles we don't need to do any more searches through their 
                {                        // boards, saving up some computational time
                    bestValue = g.tiles;
                    bestState = g;
                }
            }

            return bestState;
        }

        /// <summary>
        /// Get all valid moves of game state "state". If they haven't been calculated yet, it happens now.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>All valid moves.</returns>
        public virtual List<GameState> GetValidMoves(GameState state)
        {
            List<GameState> validMoves = state.ValidMoves();
            List<GameState> exploredMoves = state.ExploredMoves;

            if (validMoves != null && validMoves.Count() > 0) // If they have been calculated before, reutnr them
                return validMoves;
            else if (validMoves == null) // Else calculate them first
            {
                state.SetValidMoves(CalcValidMoves(state));
            }

            return state.ValidMoves(); // Return them
        }

        /// <summary>
        /// Return true if game state "state" is terminal. Terminal states are those played by Player with no empty tiles.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>True if "state" is terminal.</returns>
        public virtual bool IsTerminal(GameState state)
        {
            bool end = false;

            int[,] board = (int[,])state.Board;
            GameState2048 state2 = (GameState2048)state;

            int empty = 0;  

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    /*if (board[i, j] == 2048)
                    {
                        end = true;
                        break;
                    }
                    else */if (board[i, j] == 0) empty++;
                }
            }

            if (state.PlayedBy == PLAYER && empty == 0)
            {
                end = true;
            }

            return end;
        }

        /// <summary>
        /// Return the player to play after "player". For Player it's random move and vice versa.
        /// </summary>
        /// <param name="player">Previous player.</param>
        /// <returns>Next player.</returns>
        public virtual byte NextPlayer(byte player)
        {
            return (player == 0) ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Prints the current board to console. For visualization purposes only.
        /// </summary>
        /// <param name="state">State to be printed.</param>
        public void PrintState(GameState state)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine("Depth: " + state.Depth);

            if (state.PlayedBy == PLAYER)
                Console.WriteLine("Players turn:");
            else Console.WriteLine("Random turn:");

            int[,] board = (int[,])state.Board;
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    Console.Write(((String.Format("{0,4}", (board[i, j] == 0) ? " " : board[i, j].ToString()) + " ")));
                }
                Console.WriteLine();
            }

            Console.WriteLine("Tiles: " + ((GameState2048)state).tiles);
        }

        /// <summary>
        /// Creates a game state representing the player move from game state "parent", on the board "b" in the direction "dir".
        /// </summary>
        /// <param name="parent">Parent state</param>
        /// <param name="b">Board</param>
        /// <param name="dir">Direction of player move.</param>
        /// <returns>Resulting game state</returns>
        protected GameState PlayerMove(GameState parent, int[,] b, Direction dir)
        {
            int[] block = new int[] { 0, 0, 0, 0 };

            int[,] board = (int[,])b.Clone();

            int tiles = ((GameState2048)parent).tiles;
            switch (dir)
            {
                case (Direction.Left):
                    {
                        for (int ii = 0; ii < 4; ii++)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                for (int j = 0; j <= i - 1; j++)
                                {

                                    if (board[ii, i - 1 - j] == 0)
                                    {
                                        board[ii, i - 1 - j] = board[ii, i - j];
                                        board[ii, i - j] = 0;
                                    }
                                    else if (board[ii, i - 1 - j] == board[ii, i - j] && i - j > block[ii])
                                    {
                                        board[ii, i - 1 - j] *= 2;
                                        board[ii, i - j] = 0;
                                        block[ii] = i - j;
                                        tiles--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (Direction.Right):
                    {
                        for (int ii = 0; ii < 4; ii++)
                        {
                            for (int i = 2; i >= 0; i--)
                            {
                                for (int j = 0; j <= 2 - i; j++)
                                {

                                    if (board[ii, i + 1 + j] == 0)
                                    {
                                        board[ii, i + 1 + j] = board[ii, i + j];
                                        board[ii, i + j] = 0;
                                    }
                                    else if (board[ii, i + 1 + j] == board[ii, i + j] && i + j < 4 - block[ii])
                                    {
                                        board[ii, i + 1 + j] *= 2;
                                        board[ii, i + j] = 0;
                                        block[ii] = i + j;
                                        tiles--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (Direction.Up):
                    {
                        for (int ii = 0; ii < 4; ii++)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                for (int j = 0; j <= i - 1; j++)
                                {

                                    if (board[i - 1 - j, ii] == 0)
                                    {
                                        board[i - 1 - j, ii] = board[i - j, ii];
                                        board[i - j, ii] = 0;
                                    }
                                    else if (board[i - 1 - j, ii] == board[i - j, ii] && i - j > block[ii])
                                    {
                                        board[i - 1 - j, ii] *= 2;
                                        board[i - j, ii] = 0;
                                        block[ii] = i - j;
                                        tiles--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (Direction.Down):
                    {
                        for (int ii = 0; ii < 4; ii++)
                        {
                            for (int i = 2; i >= 0; i--)
                            {
                                for (int j = 0; j <= 2 - i; j++)
                                {

                                    if (board[i + 1 + j, ii] == 0)
                                    {
                                        board[i + 1 + j, ii] = board[i + j, ii];
                                        board[i + j, ii] = 0;
                                    }
                                    else if (board[i + 1 + j, ii] == board[i + j, ii] && i + j < 4 - block[ii])
                                    {
                                        board[i + 1 + j, ii] *= 2;
                                        board[i + j, ii] = 0;
                                        block[ii] = i + j;
                                        tiles--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            GameState2048 returnState = new GameState2048(parent, PLAYER, board);
            returnState.tiles = tiles;
            switch(dir)
            {
                case Direction.Up:
                    returnState.ID = 0;
                    break;
                case Direction.Right:
                    returnState.ID = 1;
                    break;
                case Direction.Down:
                    returnState.ID = 2;
                    break;
                default:
                    returnState.ID = 3;
                    break;


            }
            return returnState;
        }

        /// <summary>
        /// Places value "val" on coordinate "x,y" on the board "b" and return. Returned board is a new object.
        /// </summary>
        /// <param name="b">Board</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="val">Value</param>
        /// <returns>New board</returns>
        protected int[,] SetMove(int[,] b, int x, int y, int val)
        {
            int[,] board = (int[,])b.Clone();
            board[x, y] = val;
            return board;
        }

        /// <summary>
        /// Returns name of this game.
        /// </summary>
        /// <returns>Name</returns>
        public string Name()
        {
            return "2048";
        }

        /// <summary>
        /// Returns optimal setting of game for standard MCTS using UCT.
        /// </summary>
        /// <param name="r">Random.</param>
        /// <returns>Optimally set game.</returns>
        public static Game2048 OptimalGame(Random r)
        {
            return new Game2048(r, 2);
        }


    }
}
