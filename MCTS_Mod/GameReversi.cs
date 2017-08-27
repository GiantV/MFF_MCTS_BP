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

        const int BOARDSIZE = 8;

        int nHeurListCount = 64;

        /// <summary>
        /// The 'are we using heuristic simulation' flag. 1 = Yes, 0 = No.
        /// </summary>
        int heur = 0;

        /// <summary>
        /// The 'what evaluation function are we using' flag. 1 = Discrete, 0 = Linear.
        /// </summary>
        int eval = 0;

        /// <summary>
        /// Board tiles represented by "Coord" split into categories based on their desirability. Lower index of array = more desirable. For heuristic simulation.
        /// </summary>
        List<Coord[]> heurCat = new List<Coord[]>();

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


            if (_heur == 1)
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
            return (heur == 0) ? GetRandomValidMoveNonHeur(root) :( (heur == 1) ? GetRandomValidMoveHeur(root) : ((heur == 2) ? GetRandomValidMoveNonHeurFaster(root) : GetRandomValidMoveNonHeurMaybeFastest(root)));
        }

        /// <summary>
        /// Returns random valid move given the board state "state", without using heuristic
        /// </summary>
        /// <param name="state">Board state</param>
        /// <returns>Random valid move</returns>
        private GameState GetRandomValidMoveNonHeur(GameState state)
        {
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
            foreach(Coord[] c in heurCat)
            {
                List<GameState> validMoves = new List<GameState>();
                foreach(Coord cc in c)
                {
                    if (IsValidMove((byte[,])root.Board, cc.x, cc.y, NextPlayer(root.PlayedBy)))
                        validMoves.Add(PlayerMove(root,cc.x,cc.y,NextPlayer(root.PlayedBy))  /*new GameStateReversi(root, NextPlayer(root.PlayedBy), Play((byte[,])root.Board, cc.x, cc.y, NextPlayer(root.PlayedBy)), -1)*/);
                }
                if (validMoves.Count > 0)
                {
                    int pos = r.Next(0, validMoves.Count);
                    return validMoves.ElementAt(pos);
                }
            }

            return new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1);
        }

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
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    int index = r.Next(0, nHeurListCount);
                    lists[index].Add(new Coord(i, j));
                }
            }

            foreach (List<Coord> list in lists)
            {
                List<Coord> validCoords = new List<Coord>();
                foreach (Coord c in list)
                {
                    if (IsValidMove(board, c.x, c.y, nextPlayer))
                    {
                        validCoords.Add(c);
                    }
                }
                if (validCoords.Count > 0)
                {
                    int randomIndex = r.Next(0, validCoords.Count);
                    return PlayerMove(state, validCoords[randomIndex].x, validCoords[randomIndex].y, nextPlayer);
                }
            }
            GameStateReversi newState = new GameStateReversi(state, nextPlayer, board, 1);
            return newState;
        }

        private GameState GetRandomValidMoveNonHeurMaybeFastest(GameState state)
        {
            List<Coord> randomPermutation = allCoords.OrderBy(x => r.Next()).ToList();

            byte nextPlayer = NextPlayer(state.PlayedBy);
            byte[,] board = (byte[,])state.Board;

            foreach (Coord c in randomPermutation)
            {
                if (IsValidMove(board, c.x, c.y, nextPlayer))
                    return PlayerMove(state, c.x, c.y, nextPlayer);
            }

            GameStateReversi newState = new GameStateReversi(state, nextPlayer, board, 1);
            return newState;
        }

        public List<GameState> GetValidMoves(GameState root)
        {
            List<GameState> rootValidMoves = root.ValidMoves();
            List<GameState> rootExploredMoves = root.ExploredMoves;
            if (rootValidMoves != null && rootValidMoves.Count > 0)
                return rootValidMoves;
            else if (rootValidMoves != null && rootValidMoves.Count == 0)
            {
                if (rootExploredMoves.Count > 0)
                    return rootValidMoves;
                else
                {
                    rootValidMoves.Add(new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1));
                    root.SetValidMoves(rootValidMoves);
                    return root.ValidMoves();
                }
            }
            else
            {
                rootValidMoves = CalcValidMoves(root);
                if (rootValidMoves.Count() == 0)
                {
                    rootValidMoves.Add(new GameStateReversi(root, NextPlayer(root.PlayedBy), root.Board, 1));
                }
                root.SetValidMoves(rootValidMoves);
                return root.ValidMoves();
            }
        }

        public int GetAmountOfValidMoves(GameState root)
        {
            return GetValidMoves(root).Count();
        }

        public bool IsTerminal(GameState state)
        {
            GameStateReversi g = (GameStateReversi)state;

            if (g.tag == 1)
            {
                int validMovesAmount = GetAmountOfValidMoves(g);
                if (validMovesAmount == 1)
                {
                    if (CompareBoards(g.ValidMoves().ElementAt(0),g))
                    {
                        g.SetValidMoves(null);

                        return true;
                    }
                }
                else if (validMovesAmount == 0)
                {
                    g.SetValidMoves(null);

                    return true;
                }
            }

            return false;
        }

        public double Evaluate(GameState state)
        {
            return (eval == 0) ? Evaluate1(state) : Evaluate2(state);
        }

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

        public bool IsValidMove(byte[,] iBoard, int x, int y, byte player)
        {
            byte[,] board = MyClone(iBoard);

            if (board[x, y] < 5) return false;

            board[x, y] = player;

            byte nextPlayer = NextPlayer(player);

            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int dist = 1;
                    int xx = (dist * dx) + x;
                    int yy = (dist * dy) + y;

                    while (xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx, yy] == nextPlayer)
                    {
                        dist++;
                        xx = (dist * dx) + x;
                        yy = (dist * dy) + y;
                    }
                    if (dist > 1 && xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == player)
                        return true;
                        
                }
            }
            return false;
        }

        public byte[,] Play(byte[,] iBoard, int x, int y, byte player)
        {
            byte[,] board = MyClone(iBoard);

            board[x, y] = player;

            byte nextPlayer = NextPlayer(player);

            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int dist = 1;
                    int xx = (dist * dx) + x;
                    int yy = (dist * dy) + y;

                    while (xx >= 0 && xx < BOARDSIZE && yy >= 0 && yy < BOARDSIZE && board[xx,yy] == nextPlayer)
                    {
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
                        {
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

        private GameState PlayerMove(GameState parent, int x, int y, byte player)
        {
            GameState newState = new GameStateReversi(parent, player, Play((byte[,])parent.Board, x, y, player), -1);

            newState.ID = (player + 1) * 100 + x * 10 + y;

            return newState;
        }

        private byte[,] MyClone(byte[,] board)
        {
            byte[,] newBoard = new byte[BOARDSIZE,BOARDSIZE];

            for (int i = 0; i < BOARDSIZE; i++)
                for (int j = 0; j < BOARDSIZE; j++)
                    newBoard[i,j] = board[i,j];

            return newBoard;
        }

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

        public string Name()
        {
            return "Reversi";
        }

        public static GameReversi OptimalGame(Random r)
        {
            return new GameReversi(r, 1, 0);
        }
    }
}
