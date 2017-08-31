using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCTS_Mod
{
    class InputReader
    {
        Analyzer de;
        Random r;

        static string path = "input.txt";

        string outputFile = "";

        public InputReader(Analyzer extractor, Random random)
        {
            de = extractor;
            r = random;
        }

        public void Read()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string gameID = "";
                string outputPath = "";
                string iterations = "";
                string print = "";
                string aiID = "";
                string aiUCTParam = "";
                string aiStpPolicyID = "";
                string aiStpPolicyParam1 = "";
                string aiStpPolicyParam2 = "";
                string aiParams = "";
                string aiGameHeur = "";
                string aiGameEval = "";


                string ai2ID = "";
                string ai2UCTParam = "";
                string ai2StpPolicyID = "";
                string ai2StpPolicyParam1 = "";
                string ai2StpPolicyParam2 = "";
                string ai2Params = "";
                string ai2GameHeur = "";
                string ai2GameEval = "";


                string line;
                #region Read file
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "" || line[0] == '%')
                        continue;

                    string[] split = line.Split(':');
                    switch (split[0].ToLower().Replace(" ",""))
                    {
                        case "gameid":
                            gameID = split[1].ToLower().Replace(" ","");
                            break;
                        case "outputfile":
                            outputPath = split[1];
                            break;
                        case "print":
                            print = split[1].ToLower().Replace(" ","");
                            break;
                        case "aiid":
                            aiID = split[1].ToLower().Replace(" ","");
                            break;
                        case "aiuctparam":
                            aiUCTParam = split[1].ToLower().Replace(" ","");
                            break;
                        case "aistoppolicyid":
                            aiStpPolicyID = split[1].ToLower().Replace(" ","");
                            break;
                        case "aistoppolicyparam1":
                            aiStpPolicyParam1 = split[1].ToLower().Replace(" ","");
                            break;
                        case "aistoppolicyparam2":
                            aiStpPolicyParam2 = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2id":
                            ai2ID = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2uctparam":
                            ai2UCTParam = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2stoppolicyid":
                            ai2StpPolicyID = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2stoppolicyparam1":
                            ai2StpPolicyParam1 = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2stoppolicyparam2":
                            ai2StpPolicyParam2 = split[1].ToLower().Replace(" ","");
                            break;
                        case "iterations":
                            iterations = split[1].ToLower().Replace(" ","");
                            break;
                        case "aiparams":
                            aiParams = split[1].ToLower().Replace(" ","");
                            break;
                        case "ai2params":
                            ai2Params = split[1].ToLower().Replace(" ","");
                            break;
                        case "gameheurai1":
                            aiGameHeur = split[1].ToLower().Replace(" ","");
                            break;
                        case "gameevalai1":
                            aiGameEval = split[1].ToLower().Replace(" ","");
                            break;
                        case "gameheurai2":
                            ai2GameHeur = split[1].ToLower().Replace(" ","");
                            break;
                        case "gameevalai2":
                            ai2GameEval = split[1].ToLower().Replace(" ","");
                            break;
                        default: continue;

                    }
                }
                #endregion

                int aGameParam1 = Int32.Parse(aiGameHeur);
                int aGameParam2 = Int32.Parse(aiGameEval);

                IGame game1;

                #region Setup game
                switch (gameID)
                {
                    case "2048":
                        game1 = new Game2048(r, aGameParam1);
                        break;
                    case "2048d":
                        game1 = new Game2048Derandomized(r, aGameParam1);
                        break;
                    case "reversi":
                        if (aGameParam1 > 1)
                            aGameParam1 = 0;
                        game1 = new GameReversi(r, aGameParam1, aGameParam2);
                        break;
                    default:
                        game1 = DefaultGame();
                        break;
                }
                #endregion

                int a2GameParam1 = Int32.Parse(aiGameHeur);
                int a2GameParam2 = Int32.Parse(aiGameEval);

                IGame game2;

                #region Setup game
                switch (gameID)
                {
                    case "2048":
                        game2 = new Game2048(r, a2GameParam1);
                        break;
                    case "2048d":
                        game2 = new Game2048Derandomized(r, a2GameParam1);
                        break;
                    case "reversi":
                        if (a2GameParam1 > 1)
                            a2GameParam1 = 0;
                        game2 = new GameReversi(r, a2GameParam1, a2GameParam2);
                        break;
                    default:
                        game2 = DefaultGame();
                        break;
                }
                #endregion

                StopPolicy stp;

                #region Setup stop policy
                switch (aiStpPolicyID)
                {
                    case "time":
                        stp = new StopPolicyTime(Int32.Parse(aiStpPolicyParam1));
                        break;
                    case "count":
                        stp = new StopPolicyCount(Int32.Parse(aiStpPolicyParam1));
                        break;
                    case "depth":
                        stp = new StopPolicyDepth(Int32.Parse(aiStpPolicyParam1));
                        break;
                    case "depthtime":
                        stp = new StopPolicyDepthTime(Int32.Parse(aiStpPolicyParam1), Int32.Parse(aiStpPolicyParam2));
                        break;
                    default:
                        stp = DefaultStopPolicy();
                        break;
                }
                #endregion

                StopPolicy stp2;

                #region Setup stop policy 2
                switch (aiStpPolicyID)
                {
                    case "time":
                        stp2 = new StopPolicyTime(Int32.Parse(ai2StpPolicyParam1));
                        break;
                    case "count":
                        stp2 = new StopPolicyCount(Int32.Parse(ai2StpPolicyParam1));
                        break;
                    case "depth":
                        stp2 = new StopPolicyDepth(Int32.Parse(ai2StpPolicyParam1));
                        break;
                    case "depthtime":
                        stp2 = new StopPolicyDepthTime(Int32.Parse(ai2StpPolicyParam1), Int32.Parse(ai2StpPolicyParam2));
                        break;
                    default:
                        stp2 = DefaultStopPolicy();
                        break;
                }
                #endregion

                MCTS AI1;

                #region Setup AI 1
                switch (aiID)
                {
                    case "mcts":
                        AI1 = new MCTS(game1, new UCTSelectionPolicy(game1, Double.Parse(aiUCTParam)), stp);
                        break;
                    case "bmcts":
                        string[] parametersB = aiParams.Split(',');
                        if (parametersB.Count() == 2)
                            AI1 = new BMCTS2(game1, new UCTSelectionPolicy(game1, Double.Parse(aiUCTParam)), stp, Int32.Parse(parametersB[0]), Int32.Parse(parametersB[1]));
                        else
                            AI1 = new BMCTS2(game1, new UCTSelectionPolicy(game1, Double.Parse(aiUCTParam)), stp,
                                Int32.Parse(parametersB[0]), Int32.Parse(parametersB[1]),
                                Double.Parse(parametersB[2]), Double.Parse(parametersB[3])
                                );
                        break;
                    case "dlmcts":
                        string[] parametersD = aiParams.Split(',');
                        if (parametersD.Count() == 2)
                            AI1 = new DLMCTS(game1, new UCTSelectionPolicy(game1, Double.Parse(aiUCTParam)),
                                Int32.Parse(parametersD[0]), Int32.Parse(parametersD[1]));
                        else
                            AI1 = new DLMCTS(game1, new UCTSelectionPolicy(game1, Double.Parse(aiUCTParam)),
                                Int32.Parse(parametersD[0]), Int32.Parse(parametersD[1]), Int32.Parse(parametersD[2]));
                        break;
                    default:
                        AI1 = DefaultAI();
                        break;
                }
                #endregion

                MCTS AI2;

                #region Setup AI 2
                switch (ai2ID)
                {
                    case "mcts":
                        AI2 = new MCTS(game2, new UCTSelectionPolicy(game2, Double.Parse(ai2UCTParam)), stp2);
                        break;
                    case "bmcts":
                        string[] parametersB = ai2Params.Split(',');
                        if (parametersB.Count() == 2)
                            AI2 = new BMCTS2(game2, new UCTSelectionPolicy(game2, Double.Parse(ai2UCTParam)), stp2, Int32.Parse(parametersB[0]), Int32.Parse(parametersB[1]));
                        else
                            AI2 = new BMCTS2(game2, new UCTSelectionPolicy(game2, Double.Parse(ai2UCTParam)), stp2,
                                Int32.Parse(parametersB[0]), Int32.Parse(parametersB[1]),
                                Double.Parse(parametersB[2]), Double.Parse(parametersB[3])
                                );
                        break;
                    case "dlmcts":
                        string[] parametersD = ai2Params.Split(',');
                        if (parametersD.Count() == 2)
                            AI2 = new DLMCTS(game2, new UCTSelectionPolicy(game2, Double.Parse(ai2UCTParam)),
                                Int32.Parse(parametersD[0]), Int32.Parse(parametersD[1]));
                        else
                            AI2 = new DLMCTS(game2, new UCTSelectionPolicy(game2, Double.Parse(ai2UCTParam)),
                                Int32.Parse(parametersD[0]), Int32.Parse(parametersD[1]), Int32.Parse(parametersD[2]));
                        break;
                    default:
                        AI2 = DefaultAI();
                        break;
                }
                #endregion

                outputFile = outputPath;
                bool aPrint = (print == "true" || print == "t") ? true : false;

                int iter = Int32.Parse(iterations);

                switch (gameID)
                {
                    case "2048d":
                        PlayControl.Play2048DAI(AI1, (Game2048Derandomized)game1, r, iter, outputFile, null, "", aPrint);
                        break;
                    case "reversi":
                        de.TestAIsReversi(AI1, AI2, (GameReversi)game1, r, iter, outputFile, "", aPrint);
                        break;
                    default:
                        PlayControl.Play2048AI(AI1, (Game2048)game1, r, iter, outputFile, null, "", aPrint);
                        break;
                }


            }
        }

        private IGame DefaultGame()
        {
            return new Game2048(r, 0);
        }

        private MCTS DefaultAI()
        {
            return new MCTS(DefaultGame(), new UCTSelectionPolicy(DefaultGame(), 0.1), DefaultStopPolicy());
        }

        private StopPolicy DefaultStopPolicy()
        {
            return new StopPolicyTime(500);
        }

    }
}
