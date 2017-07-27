using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class BMCTS2 : MCTS
    {

        List<List<GameState>> levels = new List<List<GameState>>();

        const int MOVELISTSCONST = 100;

        int offset = 0;

        List<int> visits = new List<int>();

        int simLimit = 0;

        int maxWidth = 0;

        int levelToPruneNotOffset = -1;

        List<int> prunedAt = new List<int>();

        double unpruneMult1 = 0.0;
        double unpruneMult2 = 0.0;

        public BMCTS2(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, 
            int L, int W,
            Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            simLimit = L;
            maxWidth = W;
        }

       public BMCTS2(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy,
            int L, int W,
            double A, double B,
            Action<GameState> f = null, Action<GameState> g = null, Action<GameState> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            simLimit = L;
            maxWidth = W;
            unpruneMult1 = A;
            unpruneMult2 = B;
            Action<GameState> a = (GameState state) =>
            {
                if (prunedAt.Count > state.Depth - offset)
                {
                    if (unpruneMult1 * Math.Pow(unpruneMult2,(levels[state.Depth - offset + 1].Count + 1 - maxWidth)) + prunedAt[state.Depth - offset]
                    <
                    visits[state.Depth - offset])
                    {
                        List<GameState> validMoves = game.CalcValidMoves(state);
                        for(int i = 0; i < validMoves.Count; i++)
                        {
                            if (!state.ExploredMoves.Any((GameState same) => same.ID == validMoves[i].ID))
                            {
                                List<GameState> newValidMoves = new List<GameState>();
                                newValidMoves.Add(validMoves[i]);
                                newValidMoves[0].SetValidMoves(new List<GameState>());
                                state.SetValidMoves(newValidMoves);
                                break;
                            }
                        }
                    }
                }
            };

            selPolicy.onVisitAction = a;
        }

        public override GameState BestMove(GameState root, int player)
        {
            statesExpanded = 0;

            Console.WriteLine("Root depth: " + root.Depth);

            if (root.Depth == 270)
                Console.WriteLine("a");

            InitializeBMCTS(root);

            if (begAction != null)
                begAction(root);

            stopPolicy.Reset();
            while (stopPolicy.StopCondition(root))
            {
                #region MCTS base
                GameState selectedState = SelectState(root);
                double value = 0.0;
                if (selectedState != null)
                    value = Simulate(selectedState);
                else
                    selectedState = selectionPolicy.previousState;
                Update(selectedState, value); 
                #endregion

                if (levelToPruneNotOffset != -1)
                {
                    Prune(levelToPruneNotOffset, root);

                    levelToPruneNotOffset = -1;
                    if (levels.Count > root.Depth - offset + 1)
                        if (levels[root.Depth - offset + 1].Count == 1)
                            return levels[root.Depth - offset + 1][0];
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

        protected override void Update(GameState leaf, double value)
        {
            GameState currentState = leaf;
            do
            {
                #region BMCTS visitsStuff
                while (visits.Count <= currentState.Depth - offset)
                {
                    visits.Add(0);
                }
                visits[currentState.Depth - offset]++;

                if (visits[currentState.Depth - offset] == simLimit)
                    levelToPruneNotOffset = currentState.Depth; 
                #endregion

                currentState.Visits++;

                currentState.AddValue(value);

                if (currentState.MaxDepth < leaf.Depth)
                    currentState.MaxDepth = leaf.Depth;

                currentState = currentState.Parent;

            } while (currentState != null);
        }

        protected override GameState ExpandState(GameState state)
        {
            if (state == null)
                return null;
            statesExpanded++;
            List<GameState> validStates = game.GetValidMoves(state);

            if (validStates.Count == 0)
                return null;

            GameState toExpand = validStates[0];
            state.ExploredMoves.Add(toExpand);
            state.ValidMoves().Remove(toExpand);

            #region BMCTS add to levels
            if (toExpand == null)
                Console.WriteLine("hi");
            if (levels.Count <= toExpand.Depth - offset)
            {
                levels.Add(new List<GameState>());
                levels[toExpand.Depth - offset].Add(toExpand);
            }
            else
            {
                levels[toExpand.Depth - offset].Add(toExpand);
            }
            /*if (toExpand == null)
                huh++;*/
            #endregion

            return toExpand;
        }

        /// <summary>
        /// Called at start of BestMove. 
        /// Currently:
        ///     Pushes back lists if too large (based on MOVELISTSCONST).
        ///     Initializes lists with enough values to fit current root.
        /// </summary>
        /// <param name="root">Root</param>
        private void InitializeBMCTS(GameState root)
        {
            while (visits.Count <= root.Depth - offset)
            {
                visits.Add(0);
                levels.Add(new List<GameState>());
            }

            if (root.Depth % MOVELISTSCONST == 0 && root.Depth != 0)
            {
                List<List<GameState>> newLevels = new List<List<GameState>>();
                List<int> newVisits = new List<int>();
                List<int> newPrundeAt = new List<int>();
                for (int i = root.Depth - offset; i < levels.Count; i++)
                {
                    newLevels.Add(levels[i]);
                    newVisits.Add(visits[i]);
                    if (i < prunedAt.Count)
                    newPrundeAt.Add(prunedAt[i]);
                }

                offset = root.Depth;

                levels   =   newLevels;
                visits   =   newVisits;
                prunedAt = newPrundeAt;
            }

            

            if (levels.Count > 0 && offset != root.Depth)
            {
                for (int i = root.Depth - offset - 1; i >= 0; i--)
                {
                    levels[i] = null;
                    visits[i] = -1;
                }
            }

            for (int i = root.Depth - offset; i < levels.Count; i++)
            {
                levels[i].Clear();
            }

            DFS(root, (GameState a) => {
                if (a==null)
                    Console.WriteLine("hi");
                levels[a.Depth - offset].Add(a);
            });
        }

        /// <summary>
        /// Prunes tree defined by "root" at depth "depth".
        /// Uses auxiliary list levels
        /// </summary>
        /// <param name="depth">Where to prune</param>
        /// <param name="root">Defines tree</param>
        private void Prune(int depth, GameState root)
        {
            if (depth == root.Depth)
                return;
      
            List<GameState> remainder = levels[depth - offset];

            List<GameState> parents   = levels[depth - offset - 1];

            #region Return conditions
            if (remainder.Count <= maxWidth) //Not enough currently to prune => don't prune for now
            {
                visits[depth - offset] = 0;
                return;
            }
            #endregion


            #region Pruning at depth
            remainder.Sort(new PruneComparer());

            remainder.RemoveRange(maxWidth, remainder.Count - maxWidth);

            foreach (GameState g in remainder)
            {
                g.ExploredMoves.Clear();
                g.SetValidMoves(game.CalcValidMoves(g));
            }
            #endregion

            Console.WriteLine("Prune depth: " + depth);

            #warning Error below?

            #region Fixing tree after pruning
            levels[depth - offset] = remainder;


            int parentDepth = depth - offset - 1;

            while (parentDepth >= root.Depth - offset)
            {
                List<GameState> newParents = remainder.Select(son => son.Parent).Distinct().ToList();

                foreach (GameState g in newParents)
                {
                    if (g == null)
                        Console.WriteLine("hi");
                }

                levels[parentDepth] = newParents;

                foreach (GameState parent in newParents)
                {
                    List<GameState> toRemove = new List<GameState>();

                    foreach (GameState son in parent.ExploredMoves)
                    {
                        if (!remainder.Any((GameState same) => same == son))
                            toRemove.Add(son);
                    }

                    foreach (GameState sonToRemove in toRemove)
                    {
                        levels[sonToRemove.Depth - offset].RemoveAll(item => item == sonToRemove);
                        sonToRemove.Parent.ExploredMoves.Remove(sonToRemove);
                        sonToRemove.Parent.SetValidMoves(new List<GameState>());
                        sonToRemove.Parent = null;    
                    }

                    foreach(GameState test in levels[parent.Depth + 1 - offset])
                    {
                        if (test.Parent == null)
                            Console.WriteLine("hi");
                    }
                }

                remainder = newParents;

                parentDepth--;
            }
            #endregion

            #region Reset visits for pruned depths
            for (int i = depth - offset; i < visits.Count; i++)
            {
                visits[i] = 0;
            }
            #endregion

            #region Parents visits when pruned
            while (prunedAt.Count <= depth - offset)
            {
                prunedAt.Add(0);
            }
            prunedAt[depth - offset] = levels[depth - offset - 1].Sum((GameState state) => state.Visits); 
            #endregion
        }

        /// <summary>
        /// Goes through nodes in DFS order and applies action "a".
        /// </summary>
        /// <param name="root">Root of tree to search through.</param>
        /// <param name="a">Action applied to every node along the path.</param>
        private void DFS(GameState root, Action<GameState> a)
        {
            Stack<GameState> stack = new Stack<GameState>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                GameState current = stack.Pop();
                a(current);
                foreach (GameState g in current.ExploredMoves)
                {
                    stack.Push(g);
                }
            }
        }

        public override void Reset()
        {
            visits = new List<int>();

            levels = new List<List<GameState>>();

            levelToPruneNotOffset = -1;

            base.Reset();
        }
    }

    class PruneComparer : IComparer<GameState>
    {
        public int Compare(GameState x, GameState y)
        {
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            return y.Visits.CompareTo(x.Visits);
        }
    }
}
