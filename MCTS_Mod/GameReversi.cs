using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Represents the rules of the game Reversi, implements IGame interface
    /// </summary>
    class GameReversi : IGame
    {

        Random r;

        /// <summary>
        /// Size of game board (8 by 8 tiles)
        /// </summary>
        const int BOARDSIZE = 8;

        /// <summary>
        /// Number of sets for heuristic.
        /// </summary>
        int nHeurListCount = 64;

        /// <summary>
        /// The 'are we using heuristic simulation' flag. 1 = Yes, 0 = No.
        /// </summary>
        int heur = 0;

        public enum HeuristicReversi : int { None = 0, QualityGroups = 1, NGroups = 2, RandomPermutation = 3 };

        /// <summary>
        /// The 'what evaluation function are we using' flag. 1 = Discrete, 0 = Linear.
        /// </summary>
        int eval = 0;

        public enum EvaluationTypeReversi : int { Linear = 0, Discrete = 1};

        /// <summary>
        /// Board tiles represented by "Coord" split into categories based on their desirability. Lower index of array = more desirable. For heuristic simulation.
        /// </summary>
        List<Coord[]> heurCat = new List<Coord[]>();

        /// <summary>
        /// List containing all possible coordinates. Note we only bother filling it up when we actually use the heuristic.
        /// </summary>
        List<Coord> allCoords = new List<Coord>();

        /// <summary>
        /// Represents 2D xy coordinates
        /// </summary>
        private class Coord
        {
            public int x = 0;
            public int y = 0;
            /// <summary>
            /// Instance of 2D xy coordinates
            /// </summary>
            /// <param name="_x">X coordinate</param>
            /// <param name="_y">Y coordinate</param>
            public Coord(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        /// <summary>
        /// Initialiazes Reversi. If "_heur" is set, initializes list of tile categories.
        /// </summary>
        /// <param name="globalRandom">Random used to return random moves</param>
        /// <param name="_heur">Whether a heuristic simulation should be used. 0 - No, 1 - groups by quality, 2 - N group heuristic, 3 - Random permutation heuristic.</param>
        /// <param name="_eval">What kind of evaluation function should be used. 0 - Linear, 1 - Win/Draw/Loss</param>
        /// <param name="_nHeurListCount">Number of groups for N group heuristic.</param>
        public GameReversi(Random globalRandom, int _heur, int _eval, int _nHeurListCount = 32)
        {
            r = globalRandom;

            eval = _eval;

            heur = _heur;

            nHeurListCount = _nHeurListCount;

            if (heur == 3)
            {
                for (int i = 0; i < BOARDSIZE; i++)
                    for (int j = 0; j < BOARDSIZE; j++)
                        allCoords.Add(new Coord(i, j));
            }


            if (_heur == 1) // Defining coordinate tiers for one of the heuristics
            {
                heurCat.Add(new Coord[] { new Coord(0, 0), new Coord(7, 0), new Coord(0, 7), new Coord(7, 7) });
                heurCat.Add(new Coord[] { new Coord(2, 2), new Coord(2, 3), new Coord(2, 4), new Coord(2, 5),
                new Coord(3, 2), new Coord(3, 3), new Coord(3, 4), new Coord(3, 5),
                new Coord(4, 2), new Coord(4, 3), new Coord(4, 4), new Coord(4, 5),
                new Coord(5, 2), new Coord(5, 3), new Coord(5, 4), new Coord(5, 5),
            });
                heurCat.Add(new Coord[] { new Coord(0, 2), new Coord(0, 3), new Coord(0, 4), new Coord(0, 5),
                new Coord(1, 2), new Coord(1, 3), new Coord(1, 4), new Coord(1, 5),
                new Coord(6, 2), new Coord(6, 3), new Coord(6, 4), new Coord(6, 5),
                new Coord(7, 2), new Coord(7, 3), new Coord(7, 4), new Coord(7, 5),

                new Coord(2, 0), new Coord(3, 0), new Coord(4, 0), new Coord(5, 0),
                new Coord(2, 1), new Coord(3, 1), new Coord(4, 1), new Coord(5, 1),
                new Coord(2, 6), new Coord(3, 6), new Coord(4, 6), new Coord(5, 6),
                new Coord(2, 7), new Coord(3, 7), new Coord(4, 7), new Coord(5, 7),
            });
                heurCat.Add(new Coord[] { new Coord(0, 1), new Coord(1, 0), new Coord(1, 7), new Coord(0, 6),
                new Coord(7, 1), new Coord(6, 0), new Coord(6, 7), new Coord(7, 6)
            });
                heurCat.Add(new Coord[] { new Coord(1, 1), new Coord(6, 1), new Coord(1, 6), new Coord(6, 6) });
            }
        }

        /// <summary>
        /// Returns next player to move after "player"
        /// </summary>
        /// <param name="player">Previous player</param>
        /// <returns>Next player</returns>
        public byte NextPlayer(byte player)
        {
            return (player == 0) ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Returns random valid move given the board state "root", heuristic is used if set in GameReversi constructor
        /// </summary>
        /// <param name="root">Board state</param>
        /// <returns>Radnom valid move</returns>
        public GameState GetRandomValidMove(GameState root)
        {
            // Result depends on heuristic used
            return (heur == 0) ? GetRandomValidMoveNonHeur(root) :( (heur == 1) ? GetRandomValidMoveHeur(root) : ((heur == 2) ? GetRandomValidMoveNonHeurFaster(root) : GetRandomValidMoveNonHeurMaybeFastest(root)));
        }

        /// <summary>
        /// Returns random valid move given the board state "state", without using heuristic
        /// </summary>
        /// <param name="state">Board state</param>
        /// <returns>Random valid move</returns>
        private GameState GetRandomValidMoveNonHeur(GameState state)
        {
            // Randomly selected valid move is returned. As checking for a valid move takes about the same amount of time as generating it,
            // we don't bother with any heuristics for speed here
            List<GameState> validMoves = GetValidMoves(state);
            int pos = r.Next(0, validMoves.Count);
            return validMoves.ElementAt(pos);
        }

        /// <summary>
        /// Returns random valid move given the board state "root", using heuristic
        /// </summary>
        /// <param name="root">Board state</param>
        /// <returns>Random valid move</returns>
        private GameState GetRandomValidMoveHeur(GameState root)
        {
            foreach(Coord[] c in heurCat) // We iterate through lists of coordinates by tier
            {
                List<GameState> validMoves = new List<GameState>();
                foreach(Coord cc in c) // Select all valid moves
                {
                    if (IsValidMove((byte[,])root.Board, cc.x, cc.y, NextPlayer(root.PlayedBy)))
                        validMoves.Add(PlayerMove(root,cc.x,cc.y,NextPlayer(root.PlayedBy)));
                }
                if (validMoves.Count > 0) // And return random one
                {
                    int pos = r.Next(0, validMoves.Count);
                    return validMoves.ElementAt(pos);
                }
            }

            return new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1);
        }

        /// <summary>
        /// Returns random valid move given the board state "root", using the first method for simulation speedup (aka n-sets)
        /// </summary>
        /// <param name="state">Board state</param>
        /// <returns>andom valid move</returns>
        private GameState GetRandomValidMoveNonHeurFaster(GameState state)
        { 
            List<List<Coord>> lists = new List<List<Coord>>();

            byte nextPlayer = NextPlayer(state.PlayedBy);
            byte[,] board = (byte[,])state.Board;

            for (int i = 0; i < nHeurListCount; i++)
            {
                lists.Add(new List<Coord>());
            }

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++) // Randomly split all coordinates into 'nHeurListCount' sets
                {
                    int index = r.Next(0, nHeurListCount);
                    lists[index].Add(new Coord(i, j));
                }
            }

            foreach (List<Coord> list in lists) // We iterate through these sets (order of iteration doesn't matter)
            {
                List<Coord> validCoords = new List<Coord>();
                foreach (Coord c in list)
                {
                    if (IsValidMove(board, c.x, c.y, nextPlayer)) // We remember all valid moves
                    {
                        validCoords.Add(c);
                    }
                }
                if (validCoords.Count > 0) // Select and return a random valid state from the first set that had any valid states
                {
                    int randomIndex = r.Next(0, validCoords.Count);
                    return PlayerMove(state, validCoords[randomIndex].x, validCoords[randomIndex].y, nextPlayer);
                }
            }
            GameStateReversi newState = new GameStateReversi(state, nextPlayer, board, 1);
            return newState;
        }

        /// <summary>
        /// Returns random valid move given the board state "root", using the second method for speedup (aka random permutation)
        /// </summary>
        /// <param name="state">Board state</param>
        /// <returns>Random valid move</returns>
        private GameState GetRandomValidMoveNonHeurMaybeFastest(GameState state)
        {
            List<Coord> randomPermutation = allCoords.OrderBy(x => r.Next()).ToList(); // Get a random permutation by ordering the list by a random attribute

            byte nextPlayer = NextPlayer(state.PlayedBy);
            byte[,] board = (byte[,])state.Board;

            foreach (Coord c in randomPermutation)
            {
                if (IsValidMove(board, c.x, c.y, nextPlayer))
                    return PlayerMove(state, c.x, c.y, nextPlayer); // Return first random valid move
            }

            GameStateReversi newState = new GameStateReversi(state, nextPlayer, board, 1);
            return newState;
        }

        /// <summary>
        /// Get all valid moves of game state "state". If they haven't been calculated yet, it happens now.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>All valid moves.</returns>
        public List<GameState> GetValidMoves(GameState root)
        {
            List<GameState> rootValidMoves = root.ValidMoves();
            List<GameState> rootExploredMoves = root.ExploredMoves;
            if (rootValidMoves != null && rootValidMoves.Count > 0) // If they have been calculated before, reutnr them
                return rootValidMoves;
            else if (rootValidMoves != null && rootValidMoves.Count == 0)  // If there are no valid moves, but we have calculated them before
            {
                if (rootExploredMoves.Count > 0) 
                    return rootValidMoves;
                else
                {
                    rootValidMoves.Add(new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1)); // The "pass the turn" move
                    root.SetValidMoves(rootValidMoves);
                    return root.ValidMoves();
                }
            }
            else // Calculate them
            {
                rootValidMoves = CalcValidMoves(root);
                if (rootValidMoves.Count() == 0) // If there are no valid moves, pass the turn
                {
                    rootValidMoves.Add(new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1));
                }
                root.SetValidMoves(rootValidMoves);
                return root.ValidMoves();
            }
        }

        /// <summary>
        /// Returns the amount of valid moves availible in game state "state".
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Amount of valid moves.</returns>
        public int GetAmountOfValidMoves(GameState root)
        {
            return GetValidMoves(root).Count();
        }

        /// <summary>
        /// Return true if game state "state" is terminal.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>True if "state" is terminal.</returns>
        public bool IsTerminal(GameState state)
        {
            GameStateReversi g = (GameStateReversi)state;

            if (g.tag == 1) // Only moves following the "pass" move can be terminal
            {
                int validMovesAmount = GetAmountOfValidMoves(g);
                if (validMovesAmount == 1)
                {
                    if (CompareBoards(g.ValidMoves().ElementAt(0),g)) // If we get another "pass" move, the state is terminal
                    {
                        g.SetValidMoves(null);

                        return true;
                    }
                }
                else if (validMovesAmount == 0)
                {
                    g.SetValidMoves(null); // Border case scenario

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates a terminal game state assigning it a value from the interval [0,1].
        /// </summary>
        /// <param name="state">Terminal game state to be evaluated.</param>
        /// <returns>Value from the interval [0,1].</returns>
        public double Evaluate(GameState state)
        {
            return (eval == 0) ? Evaluate1(state) : Evaluate2(state);
        }

        /// <summary>
        /// Evaluates a terminal game state assigning it a value from the interval [0,1]. Continuous function.
        /// </summary>
        /// <param name="state">Terminal game state to be evaluated.</param>
        /// <returns>Value from the interval [0,1].</returns>
        private double Evaluate1(GameState state)
        {
            int playerStones = 0;
            int pcStones = 0;

            byte[,] board = (byte[,])state.Board;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[j, i] == 0)
                        playerStones++;
                    else if (board[j, i] == 1)
                        pcStones++;
                }
            }

            double total = ((64.0 + (playerStones - pcStones)) / 128.0);
            return total;
        }

        /// <summary>
        /// Evaluates a terminal game state assigning it a value from the interval [0,1]. Discrete function (Win/Tie/Lose).
        /// </summary>
        /// <param name="state">Terminal game state to be evaluated.</param>
        /// <returns>Value from the interval [0,1].</returns>
        private double Evaluate2(GameState state)
        {
            int playerStones = 0;
            int pcStones = 0;

            byte[,] board = (byte[,])state.Board;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[j, i] == 0)
                        playerStones++;
                    else if (board[j, i] == 1)
                        pcStones++;
                }
            }

            double total = ((64.0 + (playerStones - pcStones)) / 128.0);

            if (total > 0.5)
                total = 1;
            else if (total < 0.5)
                total = 0;
            
            return total;
        }

        /// <summary>
        /// Returns all possible valid moves for game state "state". Note, does not take into account already explored moves.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>List of all possible valid moves,</returns>
        public List<GameState> CalcValidMoves(GameState state)
        {
            List<GameState> validMoves = new List<GameState>();

            byte nextPlayer = NextPlayer(state.PlayedBy);

            byte[,] board = (byte[,])state.Board;

            for (int i = 0; i < BOARDSIZE; i++)
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (IsValidMove(board, i, j, nextPlayer))
                        validMoves.Add(PlayerMove(state,i,j,nextPlayer));
                }

            return validMoves;
            
        }

        /// <summary>
        /// Compares boards. Returns true if identical.
        /// </summary>
        /// <param name="state1">First state</param>
        /// <param name="state2">Second state</param>
        /// <returns>True if identical</returns>
        public bool CompareBoards(GameState state1, GameState state2)
        {
            byte[,] board1 = (byte[,])state1.Board;
            byte[,] board2 = (byte[,])state2.Board;


            for (int i = 0; i < BOARDSIZE; i++)
                for (int j = 0; j < BOARDSIZE; j++)
                    if (board1[i, j] != board2[i, j])
                        if (board1[i, j] < 5 || board1[i, j] < 5)
                            return false;

            return true;
        }

        /// <summary>
        /// Compares boards. Returns true if identical.
        /// </summary>
        /// <param name="board1">First board</param>
        /// <param name="state2">Second state</param>
        /// <returns>True if identical</returns>
        public bool CompareBoards(byte[,] board1, GameState state2)
        {
            byte[,] board2 = (byte[,])state2.Board;
            for (int i = 0; i < BOARDSIZE; i++)
                for (int j = 0; j < BOARDSIZE; j++)
                    if (board1[i, j] != board2[i, j])
                        if (board1[i, j] < 5 || board1[i, j] < 5)
                            return false;

            return true;
        }

        /// <summary>
        /// Returns true if placing stone owned by 'player' on a tile 'x/y' on a board 'iBoard' is a valid move.
        /// </summary>
        /// <param name="iBoard">Board</param>
        /// <param name="x">X coodrinate of placed stone</param>
        /// <param name="y">Y  oodrinate of placed stone</param>
        /// <param name="player">Player placing the stone</param>
        /// <returns>True if move is valid</returns>
        public bool IsValidMove(byte[,] iBoard, int x, int y, byte player)
        {
            byte[,] board = MyClone(iBoard);

            if (board[x, y] < 5) return false;

            board[x, y] = player;

            byte nextPlayer = NextPlayer(player);

            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++) // For each of the 8 directionss
                {
                    if (dx == 0 && dy == 0) // Not a direction
                        continue;

                    int dist = 1;
                    int xx = (dist * dx) + x;
                    int yy = (dist * dy) + y;

                    while (xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx, yy] == nextPlayer)
                    { // Continue in that direction until you hit a stone of your color or is out of bounds
                        dist++;
                        xx = (dist * dx) + x;
                        yy = (dist * dy) + y;
                    }
                    // If you moved more than 1 tile (and you didn't stop by getting out of bounds)
                    if (dist > 1 && xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == player)
                        return true; // Move is valid
                        
                }
            }
            return false; // Otherwise isn't valid
        }

        /// <summary>
        /// Plays stone owned by 'player' on a tile 'x/y' on a board 'iBoard' and returns the resulting board
        /// </summary>
        /// <param name="iBoard">Board</param>
        /// <param name="x">X coodrinate of placed stone</param>
        /// <param name="y">Y  oodrinate of placed stone</param>
        /// <param name="player">Player placing the stone</param>
        /// <returns>Resulting board</returns>
        public byte[,] Play(byte[,] iBoard, int x, int y, byte player)
        {
            byte[,] board = MyClone(iBoard);

            board[x, y] = player;

            byte nextPlayer = NextPlayer(player);

            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++) // For each of the 8 directionss
                {
                    if (dx == 0 && dy == 0) // Not a direction
                        continue;

                    int dist = 1;
                    int xx = (dist * dx) + x;
                    int yy = (dist * dy) + y;

                    while (xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == nextPlayer)
                    { // Continue in that direction until you hit a stone of your color or is out of bounds
                        dist++;
                        xx = (dist * dx) + x;
                        yy = (dist * dy) + y;
                    }
                    if (dist > 1 && xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == player)
                    {
                        dist--;
                        xx = (dist * dx) + x;
                        yy = (dist * dy) + y;
                        while(dist >= 1 && xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == nextPlayer)
                        {  // If you moved more than 1 tile (and you didn't stop by getting out of bounds), move back and flip stones
                            board[xx,yy] = player;
                            dist--;
                            xx = (dist * dx) + x;
                            yy = (dist * dy) + y;
                        }
                    }
                }
            }
            return board;
        }

        /// <summary>
        /// Creates a game state representing the 'player's move from game state "parent", on the tile 'x/y'
        /// </summary>
        /// <param name="parent">Parent state</param>
        /// <param name="x">X coordinate of played stone</param>
        /// <param name="y">Y coordinate of played stone</param>
        /// <param name="player">Player placing the stone.</param>
        /// <returns>Resulting game state</returns>
        private GameState PlayerMove(GameState parent, int x, int y, byte player)
        {
            GameState newState = new GameStateReversi(parent, player, Play((byte[,])parent.Board, x, y, player), -1);

            newState.ID = (player + 1) * 100 + x * 10 + y; // Calculating RAVE ID for RAVE

            return newState;
        }

        /// <summary>
        /// Clones the board.
        /// </summary>
        /// <param name="board">Game board</param>
        /// <returns>Clone</returns>
        private byte[,] MyClone(byte[,] board)
        {
            byte[,] newBoard = new byte[BOARDSIZE,BOARDSIZE];

            for (int i = 0; i < BOARDSIZE; i++)
                for (int j = 0; j < BOARDSIZE; j++)
                    newBoard[i,j] = board[i,j];

            return newBoard;
        }

        /// <summary>
        /// Returns the default (or initial) board of the game Reversi.
        /// </summary>
        /// <param name="firstPlayer">Defines who is next to move</param>
        /// <returns>Game state representing the default position.</returns>
        public GameState DefaultState(byte first)
        {
            byte[,] board = new byte[8, 8] {{5,5,5,5,5,5,5,5},
                                            {5,5,5,5,5,5,5,5},
                                            {5,5,5,5,5,5,5,5},
                                            {5,5,5,1,0,5,5,5},
                                            {5,5,5,0,1,5,5,5},
                                            {5,5,5,5,5,5,5,5},
                                            {5,5,5,5,5,5,5,5},
                                            {5,5,5,5,5,5,5,5}};
            GameStateReversi g = new GameStateReversi(null, first, board, -1);
            return g;
        }

        /// <summary>
        /// Prints the current board to console. For visualization purposes only.
        /// </summary>
        /// <param name="state">State to be printed.</param>
        public void PrintState(GameState state)
        {
            if (state.PlayedBy == 1)
                Console.WriteLine("Players turn");
            else
                Console.WriteLine("Computers turn");

            String[] tmp = new String[] { " ", "a", "b", "c", "d", "e", "f", "g", "h" };

            for (int i = 0; i < 9; i++)
                Console.Write(tmp[i] + " ");

            Console.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                Console.Write((i + 1) + " ");
                for (int j = 0; j < 8; j++)
                {
                    if (((byte[,])state.Board)[i, j] == 0)
                        Console.Write("x ");
                    else if (((byte[,])state.Board)[i,j] == 1)
                        Console.Write("o ");
                    else
                        Console.Write("  ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Returns 1 if game swon by player 1. 0,5 it tie and 0 otherwise.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Result of the game in game state "state".</returns>
        public double GameResult(GameState state)
        {
            int val = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((byte[,])state.Board)[i, j] == 0) val += 1;
                    else if (((byte[,])state.Board)[i,j] == 1) val -= 1;
                }
            }
            if (val > 0)
                return 1.0;
            else if (val < 0)
                return -1.0;
            else return 0.0;           
        }

        /// <summary>
        /// Returns number of empty tiles in a state 'state'
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int CountEmpty(GameState state)
        {
            int returnInt = 0;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (((byte[,])state.Board)[i,j] > 1)
                        returnInt++;
                }


            return returnInt;
        }

        /// <summary>
        /// Translates user input into numeric coodrinates.
        /// </summary>
        /// <param name="s">User input</param>
        /// <returns>int[] {X coord, Y coord}</returns>
        public int[] TranslateCoords(string s)
        {
            if (s.Length != 2)
                return null;
            int firstCoord = 0;
            int secondCoord = 0;

            string first = s.Substring(0, 1);
            string second = s.Substring(1);

            firstCoord = Int32.Parse(second) - 1;
            secondCoord = first[0] - 97;

            if (firstCoord > 7 || firstCoord < 0 || secondCoord < 0 || secondCoord > 7)
                return null;

            return new int[] { firstCoord, secondCoord };
        }

        /// <summary>
        /// Name of the game.
        /// </summary>
        /// <returns>The string "Reversi"</returns>
        public string Name()
        {
            return "Reversi";
        }

        /// <summary>
        /// Returns optimal setting of game for standard MCTS using UCT.
        /// </summary>
        /// <param name="r">Random.</param>
        /// <returns>Optimally set game.</returns>
        public static GameReversi OptimalGame(Random r)
        {
            return new GameReversi(r, (int)GameReversi.HeuristicReversi.QualityGroups, (int)GameReversi.EvaluationTypeReversi.Linear);
        }
    }
}
