using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Represents the derandomized game 2048.
    /// </summary>
    class Game2048Derandomized : Game2048
    {

        int MAXESTDEPTHD = 0;

        /// <summary>
        /// Represents the derandomized game 2048.
        /// </summary>
        /// <param name="globalRandom">Random</param>
        /// <param name="useHeuristicSimulation">Whether heuristic simulation should be used. Same as in Game2048.</param>
        public Game2048Derandomized(Random globalRandom, int useHeuristicSimulation) : base(globalRandom, useHeuristicSimulation)
        {
            MAXESTDEPTHD = MAXESTDEPTH / 2;
        }


        /// <summary>
        /// Returns all possible valid moves for game state "state". Note, does not take into account already explored moves.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>List of all possible valid moves,</returns>
        public override List<GameState> CalcValidMoves(GameState state)
        {
            if (((GameState2048)state).tag == CANNOTPLACE)
                return new List<GameState>();

            List<GameState> validMoves = new List<GameState>();
            int[,] board = (int[,])state.Board;

            validMoves.Add(RandomMove(PlayerMove(state, board, Direction.Up   ), Direction.Up   ));
            validMoves.Add(RandomMove(PlayerMove(state, board, Direction.Down ), Direction.Down ));
            validMoves.Add(RandomMove(PlayerMove(state, board, Direction.Left ), Direction.Left ));
            validMoves.Add(RandomMove(PlayerMove(state, board, Direction.Right), Direction.Right));
            
            return validMoves;
        }

        /// <summary>
        /// Returns the next player. In this case, always returns input as this is a single-player game.
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>Next player. Or just player.</returns>
        public override byte NextPlayer(byte player)
        {
            return player;
        }

        /// <summary>
        /// Returns true if game state "state" is terminal. False otherwise.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Returns true if game state "state" is terminal. False otherwise.</returns>
        public override bool IsTerminal(GameState state)
        {
            GameState2048 g = (GameState2048)state;

            if (g.tag == CANNOTPLACE) // We check terminality by looking at the tag. Tag may be set in RandomMove function.
                return true;

            return false;
        }

        /// <summary>
        /// Deterministically determines the random move in state "g".
        /// </summary>
        /// <param name="g">Entry state.</param>
        /// <param name="dir">Direction we moved to get to "g".</param>
        /// <returns>A random move, except not really.</returns>
        private GameState RandomMove(GameState g, Direction dir)
        {
            int[,] returnBoard = new int[BOARDSIZE, BOARDSIZE];

            int[,] b = (int[,])g.Board;

            int emptySpaces = 0;

            int temp = 0;

            g.PlayedBy = PLAYER;

            #region Count empty spaces and temp value
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    returnBoard[i, j] = b[i, j];

                    if (b[i, j] == 0)
                        emptySpaces++;
                    else
                        temp += b[i, j] + i * j + j + 3; // Temp value is combination of tile value, it's coordinates and a constant.
                }
            }
            #endregion

            #region If terminal state
            if (emptySpaces == 0)
            {
                GameState2048 gg = (GameState2048)g;
                gg.tag = CANNOTPLACE;
                return gg;
            } 
            #endregion

            temp = temp % emptySpaces;
            int par = temp % 2;
            bool doBreak = false;

            #region Place value and calculate ID
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (returnBoard[i, j] == 0)
                    {
                        if (temp == 0)
                        {
                            if (par == 0)
                                returnBoard[i, j] = 2;
                            else returnBoard[i, j] = 4;
                            doBreak = true;

                            int dirID = 0; // Id is a four digit number, where first represents direction, second value, and last two the coordinates
                            if (dir == Direction.Up) dirID = 0; else if (dir == Direction.Right) dirID = 1; else if (dir == Direction.Down) dirID = 2; else dirID = 3;
                            dirID *= 1000;
                            int valID = (par == 0) ? 200 : 400;
                            dirID += valID + 10 * j + i;
                            g.ID = dirID;
                            break;
                        }
                        temp--;
                    }
                }
                if (doBreak) break;
            } 
            #endregion

            g.Board = returnBoard;
            ((GameState2048)g).tiles++; //We added a tile, so we increment the total count.

            return g;
        }

        /// <summary>
        /// Returns the default state. Pretty much the same as Game2048 variation, except sets PlayedBy to 0, instead of 1.
        /// </summary>
        /// <param name="firstPlayer">Unneeded here.</param>
        /// <returns>Default state.</returns>
        public override GameState DefaultState(byte firstPlayer)
        {
            GameState a = base.DefaultState(firstPlayer);
            a.PlayedBy = PLAYER;
            return a;
        }

        /// <summary>
        /// Returns the valid moves of game state "state". Note this does not take explored moves into account.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>List of valid moves.</returns>
        public override List<GameState> GetValidMoves(GameState state)
        {
            List<GameState> validMoves = state.ValidMoves();
            List<GameState> exploredMoves = state.ExploredMoves;

            if (validMoves != null && validMoves.Count() > 0)
                return validMoves;
            else if (validMoves == null)
            {
                state.SetValidMoves(CalcValidMoves(state));
            }

            return state.ValidMoves();
        }

        /// <summary>
        /// Works as a switch for selected heuristic. Heuristics are the same as in Game2048.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Random valid move.</returns>
        public override GameState GetRandomValidMove(GameState state)
        {
            return (HEURSIM == 0) ? GetRandomValidMoveNonHeur(state) : ((HEURSIM == 1) ? GetRandomValidMoveHeur1(state) : GetRandomValidMoveHeur2(state));
        }

        /// <summary>
        /// Returns a random valid move without using any heuristic.
        /// </summary>
        /// <param name="state">Entry state.</param>
        /// <returns>Random valid move.</returns>
        protected override GameState GetRandomValidMoveNonHeur(GameState state)
        {
            int i = base.r.Next(0, 4);
            int[,] board = (int[,])state.Board;
            switch (i)
            {
                case  0: return RandomMove(PlayerMove(state, board, Direction.Up   ), Direction.Up   );
                case  1: return RandomMove(PlayerMove(state, board, Direction.Down ), Direction.Down );
                case  2: return RandomMove(PlayerMove(state, board, Direction.Left ), Direction.Left );
                default: return RandomMove(PlayerMove(state, board, Direction.Right), Direction.Right);
            }
        }

        /// <summary>
        /// Evaluates a terminal game state assigning it a value from the interval [0,1].
        /// </summary>
        /// <param name="state">Terminal game state to be evaluated.</param>
        /// <returns>Value from the interval [0,1].</returns>
        public override double Evaluate(GameState state)
        {
            return Math.Pow((double)state.Depth / (double)MAXESTDEPTHD, 1.0 / EVALROOT);
        }

        public string Name()
        {
            return "2048D";
        }
    }
}
