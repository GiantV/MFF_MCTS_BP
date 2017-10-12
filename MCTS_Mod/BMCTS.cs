using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Mod
{
    /// <summary>
    /// Old class, unused
    /// </summary>

    class BMCTS : MCTS
    {
        int wLimit = 0;
        int simLimit = 0;

        int multi  =   0;
        double exp = 0.0;

        int lastCleared = -1;

        List<int> visits = new List<int>();

        List<int> prunedAt = new List<int>();

        List<List<GameState>> depths = new List<List<GameState>>();

        List<int> toPrune = new List<int>();

        public BMCTS(IGame _game, SelectionPolicy selPolicy, StopPolicy stpPolicy, int L, int W) : base(_game,selPolicy,stpPolicy)
        {
            wLimit = W;
            simLimit = L;
            visits.Add(1);
            game = _game;
        }

        public BMCTS(IGame _game, UCTSelectionPolicy selPolicy, StopPolicy stpPolicy, int L, int W, int A, double B) : base(_game, selPolicy, stpPolicy)
        {
            wLimit = W;
            simLimit = L;
            visits.Add(1);
            game = _game;
            multi = A;
            exp = B;

            SelectionPolicy newSelPolicy = new BMCTSUCTSelectionPolicy(_game, selPolicy.UCT, true, prunedAt, A, B, depths, visits, W);
            this.selectionPolicy = newSelPolicy;
            base.selectionPolicy = newSelPolicy;
        }

        public override GameState BestMove(GameState root, int player)
        {
            ClearDepths(root.Depth,root);
            stopPolicy.Reset();
            if (depths.Count <= root.Depth)
            {
                while (depths.Count <= root.Depth)
                    depths.Add(new List<GameState>());
                depths[root.Depth].Add(root);
            }
            else
            {
                depths[root.Depth].Add(root);
            }

            while (stopPolicy.StopCondition(root))
            {
                GameState selectedState = SelectState(root);

                if (selectedState.Depth > visits.Count)
                    visits.Add(0);

                double value = Simulate(selectedState);
                this.Update(selectedState, value);

                foreach(int depth in toPrune)
                {
                    Prune(depth, root);                   
                }
                if (toPrune.Count > 0)
                    toPrune.Clear();
            }
            if (player == 0)
                return BestChild(root);
            else
                return WorstChild(root);
        }

        protected override GameState ExpandState(GameState root)
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

            if (depths.Count <= toExpand.Depth)
            {
                depths.Add(new List<GameState>());
                depths[toExpand.Depth].Add(toExpand);
            }
            else
            {
                depths[toExpand.Depth].Add(toExpand);
            }

            return toExpand;
        }

        protected override void Update(GameState leaf, double value)
        {
            GameState currentState = leaf;
            do
            {
                while (visits.Count <= currentState.Depth)
                {
                    visits.Add(0);
                }
                visits[currentState.Depth]++;

                if (visits[currentState.Depth] == simLimit)
                    toPrune.Add(currentState.Depth);

                currentState.Visits++;

                currentState.AddValue(value);

                if (currentState.MaxDepth < leaf.Depth)
                    currentState.MaxDepth = leaf.Depth;

                currentState = currentState.Parent;

            } while (currentState != null);
        }

        private void Prune(int depth, GameState root)
        {
            if (depth == root.Depth)
                return;

            List<GameState> candidates = AllGameStatesAtDepth(depth);

            while (prunedAt.Count < depth)
            {
                prunedAt.Add(0);
            }

            List<GameState> listForVisits = AllGameStatesAtDepth(depth - 1);

            int totalVisits = listForVisits.Sum(item => item.Visits);

            prunedAt[depth - 1] = totalVisits;

            if (candidates.Count <= this.wLimit)
            {
                List<GameState> parents = AllGameStatesAtDepth(depth - 1);
                int maxWidth = 0;
                foreach (GameState parent in parents)
                {
                    if (parent == null)
                        continue;
                    maxWidth += parent.ExploredMoves.Count;
                    if (parent.ValidMoves() != null)
                        maxWidth += parent.ValidMoves().Count;
                }
                if (maxWidth <= wLimit)
                    return;
                else
                {
                    visits[depth] -= simLimit;
                    return;
                }
            }
            candidates.RemoveAll(item => item == null);
            candidates.Sort(new PruneComparer());

            List<GameState> remainder = new List<GameState>();

            for (int i = 0; i < candidates.Count; i++)
            {
                if (i >= wLimit)
                {
                    candidates[i].Parent.ExploredMoves.Remove(candidates[i]);
                    candidates[i] = null;
                }
                else
                {
                    candidates[i].ExploredMoves = new List<GameState>();
                    candidates[i].SetValidMoves(game.GetValidMoves(candidates[i]));
                    
                        
                    remainder.Add(candidates[i]);
                }
            }
            int tmpDepth = depth - 1;

            depths[depth] = remainder;

            while (tmpDepth >= root.Depth)
            {
                List<GameState> newRemainder = new List<GameState>();

                foreach (GameState g in remainder)
                {
                    if (!newRemainder.Contains(g.Parent))
                    {
                        newRemainder.Add(g.Parent);
                        List<GameState> toRemove = new List<GameState>();

                        foreach (GameState son in g.Parent.ExploredMoves)
                        {
                            if (!remainder.Any(item => item.ID == son.ID))
                                toRemove.Add(son);
                        }
                        foreach (GameState son in toRemove)
                        {
                            g.Parent.ExploredMoves.Remove(son);
                        }
                    }
                }

                

                remainder = newRemainder;
                tmpDepth--;
            }
            PostPruneProcedure(depth);
        }

        private List<GameState> AllGameStatesAtDepth(int depth)
        {
            if (depths.Count <= depth)
                return null;

            return depths[depth];
        }

        private void ClearDepths(int rootDepth, GameState root)
        {
            if (depths == null || depths.Count == 0)
                return;

            for (int i = rootDepth; i < depths.Count; i++)
            {
                depths[i].Clear();
            }

            DFS(root, (GameState g) => { depths[g.Depth].Add(g);});

            rootDepth--;
            while (rootDepth >= 0 && rootDepth > lastCleared)
            {
                depths[rootDepth] = null;

                rootDepth--;
            }

            lastCleared = root.Depth - 1;
        }

        private void PostPruneProcedure(int prunedAt)
        {
            for (int i = prunedAt + 1; i < visits.Count; i++)
            {
                visits[i] = 0;
                depths[i].Clear();
            }
        }

        public override void Reset()
        {
            visits = new List<int>();

            depths = new List<List<GameState>>();

            toPrune = new List<int>();

            base.Reset();
        }

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

    }

    class BMCTSUCTSelectionPolicy : UCTSelectionPolicy
    {
        bool unpruning = false;

        List<int> prunedAt;
        List<List<GameState>> depths;
        List<int> visits;

        int A = 0;
        double B = 0;

        int W = 0;

        public BMCTSUCTSelectionPolicy(IGame _game, double _con, bool _unpruning, List<int> _prunedAt, int _A, double _B, List<List<GameState>> _depths, List<int> _visits, int _W) : base(_game, _con)
        {
            unpruning = _unpruning;
            game = _game;
            UCT = _con;
            prunedAt = _prunedAt;
            A = _A;
            B = _B;
            depths = _depths;
            visits = _visits;
            W = _W;
        }

        public override GameState Select(GameState root)
        {
            if (!unpruning)
                return base.Select(root);
            else
            {
                GameState currentState = root;

                while (!this.game.IsTerminal(currentState))
                {
                    if (game.GetAmountOfValidMoves(currentState) > 0)
                        return currentState;
                    else if (CanUnprune(currentState))
                    {
                        bool first = true;
                        List<GameState> validMoves = game.CalcValidMoves(currentState);

                        for(int i = 0; i < validMoves.Count; i++)
                        {
                            if (!first)
                                validMoves[i] = null;
                            if (currentState.ExploredMoves.Any(item => item.ID == validMoves[i].ID))
                            {
                                validMoves[i] = null;
                            }
                            else
                                first = false;
                        }

                        validMoves.RemoveAll(item => item == null);

                        currentState.SetValidMoves(validMoves);

                        return currentState;
                    }
                    else if (root.PlayedBy == 1)
                        currentState = BestUCBChild(currentState);
                    else
                        currentState = WorstUCBChild(currentState);

                    if (currentState == null)
                        return null;
                }

                return currentState;
            }
        }

        private bool CanUnprune(GameState g)
        {
            if (depths.Count <= g.Depth + 1)
                return false;

            int k = W;

            int currentK = depths[g.Depth + 1].Count;

            int sv = prunedAt[g.Depth];

            if ((A * Math.Pow(B,(currentK + 1 - k))) + prunedAt[g.Depth] < visits[g.Depth])
            {
                return true;
            }

            return false;
        }
    }

}
