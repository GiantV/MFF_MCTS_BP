using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    class BMCTS : MCTS
    {
        /// <summary>
        /// Tree nodes in lists per their depth.
        /// </summary>
        List<List<GameState>> levels = new List<List<GameState>>();

        const int MOVELISTSCONST = 100;

        int offset = 0;

        List<int> visits = new List<int>();

        int simLimit = 0;

        int maxWidth = 0;

        int levelToPrune_NotOffset = -1;

        public List<int> prunedAt = new List<int>();

        double unpruneMult1 = 0.0;
        double unpruneMult2 = 0.0;

        /// <summary>
        /// Are we unpruning. 0 - no, 1 - yes
        /// </summary>
        byte unpruning = 0;

        public BMCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, 
            int L, int W,
            Action<GameState, MCTS> f = null, Action<GameState, MCTS> g = null, Action<GameState, MCTS> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            simLimit = L;
            maxWidth = W;
        }

       public BMCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy,
            int L, int W,
            double A, double B,
            Action<GameState, MCTS> f = null, Action<GameState, MCTS> g = null, Action<GameState, MCTS> h = null) : base(_game, selPolicy, stpPolicy, f, g, h)
        {
            simLimit = L;
            maxWidth = W;
            unpruneMult1 = A;
            unpruneMult2 = B;
            unpruning = 1;
            Action<GameState> a = (GameState state) =>
            {
                if (prunedAt.Count > state.Depth - offset)
                {
                    if (unpruneMult1 * Math.Pow(unpruneMult2,(levels[state.Depth - offset + 1].Count + 1 - maxWidth)) + prunedAt[state.Depth - offset]
                    <
                    visits[state.Depth - offset])
                    {
                        List<GameState> validMoves = Game.GetValidMoves(state);
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
            if (begAction != null)
                begAction(root, this);

            InitializeBMCTS(root);

            stopPolicy.Reset();
            statesExpanded = 0;

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

                if (levelToPrune_NotOffset != -1)
                {
                    Prune(levelToPrune_NotOffset, root);

                    levelToPrune_NotOffset = -1;
                    if (unpruning == 0)
                        if (levels.Count > root.Depth - offset + 1) 
                            if (levels[root.Depth - offset + 1].Count == 1) // If root has only one child
                                if (root.ValidMoves().Count == 0) // And no other valid move
                                    return levels[root.Depth - offset + 1][0]; // Return only child
                }

                if (iterAction != null)
                    iterAction(root, this);
            }

            if (endAction != null)
                endAction(root, this);

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
                    levelToPrune_NotOffset = currentState.Depth; 
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
            List<GameState> validStates = Game.GetValidMoves(state);

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

            GameState.DFS(root, (GameState a) => {
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
            if (depth == root.Depth) // Don't prune at root
                return;
      
            List<GameState> remainder = levels[depth - offset];

            int parentDepth = depth - offset - 1;

            List<GameState> parents   = levels[parentDepth];

            #region Return conditions
            if (remainder.Count <= maxWidth) //Not enough currently to prune => don't prune for now
            {
                visits[depth - offset] = 0;
                return;
            }
            #endregion


            #region Pruning at depth
            remainder.Sort(new PruneComparer());

            remainder.RemoveRange(maxWidth, remainder.Count - maxWidth); // Select maxWidth best nodes

            foreach (GameState g in remainder) // Prune their children and reset valid moves
            {
                g.ExploredMoves.Clear();
                g.SetValidMoves(Game.GetValidMoves(g));
            }
            #endregion

            //Console.WriteLine("Prune depth: " + depth);

            #warning Error below?

            #region Fixing tree after pruning
            levels[depth - offset] = remainder; // Set pruned level to non-pruned nodes only

            while (parentDepth >= root.Depth - offset) // Climbing levels up the tree to the root
            {
                List<GameState> newParents = remainder.Select(son => son.Parent).Distinct().ToList(); // Select surviving parents

                foreach (GameState g in newParents)
                {
                    if (g == null)
                        Console.WriteLine("hi");
                }

                levels[parentDepth] = newParents; // Set parent level to surviving parents

                foreach (GameState parent in newParents)
                {
                    List<GameState> toRemove = new List<GameState>();

                    foreach (GameState son in parent.ExploredMoves) // Select all children nodes, that were not in remainder
                    {
                        if (!remainder.Any((GameState same) => same == son))
                            toRemove.Add(son);
                    }

                    foreach (GameState sonToRemove in toRemove) // Remove all references to them, so that GC can clean them up
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

                remainder = newParents; // Set remainder as new parents

                parentDepth--; // Go up the tree
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
        /// Resets pruning related metadata.
        /// </summary>
        public override void Reset()
        {
            visits = new List<int>();

            levels = new List<List<GameState>>();

            levelToPrune_NotOffset = -1;

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
