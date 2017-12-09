using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MCTS_Mod
{
    class Analyzer
    {
        Random r;

        public Analyzer(Random _r)
        {
            r = _r;
        }

        public void MiscTest_2048EffectivityByTime(Random r, bool parallel = false)
        {
            int[] times = new int[] { 100, 300, 400 };



            int iter = 20;

            string name = "Misc_Table_2048Effectivity";

            if (!parallel)
            {
                foreach (int time in times)
                {
                    MiscTest_2048EffectivityByTime_HelpFunction(time, name, iter, r);
                }
            }
            else
            {
                Parallel.ForEach(times, (int i) => MiscTest_2048EffectivityByTime_HelpFunction(i, name, iter, r));
            }
        }

        private void MiscTest_2048EffectivityByTime_HelpFunction(int time, string name, int iter, Random r)
        {
            Game2048 game = Game2048.OptimalGame(r);

            UCTSelectionPolicy selPol = UCTSelectionPolicy.OptimalSelectionPolicy(game);

            Console.WriteLine("Time: {0}", time);
            using (StreamWriter sw = new StreamWriter(name + time + ".txt"))
            {
                sw.WriteLine("All opt. with time limit of {0} ms", time);
                MCTS AI = new MCTS(game, selPol, new StopPolicyTime(time));

                int depthTotal = 0;

                for (int i = 0; i < iter; i++)
                {
                    Console.WriteLine("Iteration: {0}", i);
                    GameState currentState = game.DefaultState(0);

                    currentState = PlayControl.GetBestState2048(AI, currentState);


                    while (!game.IsTerminal(currentState))
                    {
                        currentState = PlayControl.OneRound2048(AI, game, currentState, false, r);
                    }

                    depthTotal += currentState.Depth;
                }

                double depthAverage = (double)depthTotal / (double)iter;
                sw.WriteLine("Average depth: {0}", depthAverage);
            }
        }


        #region UCT_Testing - Should be correct

        public void TestUCTParam2048(StopPolicy stp, int iterations, double[] param, IGame tofe, string id, string intro, bool derandomized = false)
        {
            Parallel.ForEach(param, (double d) =>
            {
                MCTS AI = new MCTS(tofe, new UCTSelectionPolicy(tofe, d), stp.Clone());
                if (derandomized)
                {
                    PlayControl.Play2048DAI(AI, (Game2048Derandomized)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
                    {
                        return g.Depth.ToString() + "\r\n";
                    });
                }
                else
                {
                    PlayControl.Play2048AI(AI, (Game2048)tofe, r, iterations, "UCTTest_2048_" + id + "_" + d, null, intro, false, delegate (GameState g)
                        {
                            return g.Depth.ToString() + "\r\n";
                        });
                }

            });


        }

        public void TestUCTReversi(StopPolicy stp, int iterations, double[] param, GameReversi rev, string id, string intro, double compareAgainstUCTParam)
        {
            using (StreamWriter sw = new StreamWriter("UCTTest_Reversi_" + id + ".txt"))
            {
                sw.WriteLine(intro);

                #region List UCT params
                sw.Write("[");
                for (int i = 0; i < param.Length - 1; i++)
                {
                    sw.Write(param[i] + ",");
                }
                sw.Write(param[param.Length - 1] + "]");
                sw.WriteLine(); 
                #endregion

                UCTSelectionPolicy compSelPol = new UCTSelectionPolicy(rev, compareAgainstUCTParam);
                MCTS compAI = new MCTS(rev, compSelPol, stp.Clone());

                UCTSelectionPolicy[] selectionPolicies = new UCTSelectionPolicy[param.Length];
                MCTS[] AIs = new MCTS[param.Length];
                double[][] results = new double[param.Length][];

                for (int i = 0; i < param.Length; i++)
                {
                    selectionPolicies[i] = new UCTSelectionPolicy(rev, param[i]);
                    AIs[i] = new MCTS(rev, selectionPolicies[i], stp.Clone());
                    results[i] = new double[] { 0, 0, 0 };
                }



                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine(i);
                    Parallel.For(0, param.Length, (int a) =>
                    {
                        double res = PlayControl.PlayReversiAIAI(compAI, AIs[a], rev, r, (byte)(a % 2), false);
                        if (res > 0.5)
                            results[a][0]++;
                        else if (res < 0.5)
                            results[a][1]++;
                        else
                            results[a][2]++;
                    });
                }
                for (int i = 0; i < param.Length; i++)
                {
                    sw.WriteLine("Parameter " + param[i] + " had " + results[i][0] + " wins, " + results[i][1] + " losses and " + results[i][2] + " ties against parameter " + compareAgainstUCTParam);
                }
            }
        }

        #endregion








        
        


        



        public void TestEvalFuncReversi(MCTS AI1, MCTS AI2, GameReversi rev, Random r, int iter)
        {
            using (StreamWriter sw = new StreamWriter("EvalTestReversi.txt"))
            {
                sw.WriteLine("AI1 = 0/1");
                sw.WriteLine("AI2 = [0,1]");
                int won1 = 0;
                int won2 = 0;
                int tie = 0;


                for (int i = 0; i < iter; i++)
                {
                    double res = PlayControl.PlayReversiAIAI(AI1, AI2, rev, r, (byte)(i % 2), true);
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

        

        


    }









}
