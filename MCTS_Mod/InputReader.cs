using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace MCTS_Mod
{
    class InputReader
    {
        Analyzer de;
        Random r;

        static string path = "input.txt";

        string outputFile = "";

        Dictionary<string, IGame> games = new Dictionary<string, IGame>();
        Dictionary<string, SelectionPolicy> selectionPolicies = new Dictionary<string, SelectionPolicy>();
        Dictionary<String, StopPolicy> stopPolicies = new Dictionary<string, StopPolicy>();
        Dictionary<string, MCTS> AIs = new Dictionary<string, MCTS>();

        public InputReader(Analyzer extractor, Random random)
        {
            de = extractor;
            r = random;
        }


        public void Read()
        {
            XDocument doc = XDocument.Load("settings.xml");


            #region Load Games
            var getGamesR = from c in doc.Root.Descendants("Reversi")
                            select c;
            foreach (var v in getGamesR)
            {
                games.Add(v.Attribute("id").Value, new GameReversi(r, Int32.Parse(v.Descendants("Heuristic").First().Value), 
                    Int32.Parse(v.Descendants("EvaluationFunction").First().Value), 
                    Int32.Parse(v.Descendants("Heuristic2GroupCount").First().Value)));

            }

            var getGame2 = from c in doc.Root.Descendants("G2048")
                           select c;
            foreach (var v in getGame2)
            {
                games.Add(v.Attribute("id").Value, new Game2048(r, Int32.Parse(v.Descendants("Heuristic").First().Value), 
                    Double.Parse(v.Descendants("NextVal").First().Value), 
                    Double.Parse(v.Descendants("TileVal").First().Value), 
                    Int32.Parse(v.Descendants("EvalRoot").First().Value)));
            }

            var getGame2D = from c in doc.Root.Descendants("G2048D")
                            select c;
            foreach (var v in getGame2D)
            {
                games.Add(v.Attribute("id").Value, new Game2048(r, Int32.Parse(v.Descendants("Heuristic").First().Value), 
                    Double.Parse(v.Descendants("NextVal").First().Value), 
                    Double.Parse(v.Descendants("TileVal").First().Value), 
                    Int32.Parse(v.Descendants("EvalRoot").First().Value)));
            }
            #endregion

            #region Load Selection Policies
            var getUCTPolicies = from c in doc.Root.Descendants("UCTSelectionPolicy")
                                 select c;

            foreach (var v in getUCTPolicies)
            {
                selectionPolicies.Add(v.Attribute("id").Value, new UCTSelectionPolicy(games[v.Descendants("Game").First().Attribute("refid").Value], Double.Parse(v.Descendants("Coefficient").First().Value)));
            }
            #endregion

            #region Load Stop Policies
            var getStopPoliciesT = from c in doc.Root.Descendants("StopPolicyTime")
                                   select c;

            foreach (var v in getStopPoliciesT)
            {
                stopPolicies.Add(v.Attribute("id").Value, new StopPolicyTime(Int32.Parse(v.Descendants("TimeLimit").First().Value)));
            }

            var getStopPoliciesC = from c in doc.Root.Descendants("StopPolicyCount")
                                   select c;

            foreach (var v in getStopPoliciesC)
            {
                stopPolicies.Add(v.Attribute("id").Value, new StopPolicyCount(Int32.Parse(v.Descendants("CountLimit").First().Value)));
            }

            var getStopPoliciesD = from c in doc.Root.Descendants("StopPolicyDepth")
                                   select c;

            foreach (var v in getStopPoliciesD)
            {
                stopPolicies.Add(v.Attribute("id").Value, new StopPolicyDepth(Int32.Parse(v.Descendants("DepthLimit").First().Value)));
            }

            var getStopPoliciesDT = from c in doc.Root.Descendants("StopPolicyDepthTime")
                                    select c;

            foreach (var v in getStopPoliciesDT)
            {
                stopPolicies.Add(v.Attribute("id").Value, new StopPolicyDepthTime(Int32.Parse(v.Descendants("DepthLimit").First().Value), 
                    Int32.Parse(v.Descendants("TimeLimit").First().Value)));
            }
            #endregion

            #region Load AIs
            var getAIMCTS = from c in doc.Root.Descendants("MCTS")
                            select c;

            foreach (var v in getAIMCTS)
            {
                AIs.Add(v.Attribute("id").Value, new MCTS(games[v.Descendants("PlayGame").First().Attribute("refid").Value],
                    selectionPolicies[v.Descendants("UseSelectionPolicy").First().Attribute("refid").Value],
                    stopPolicies[v.Descendants("UseStopPolicy").First().Attribute("refid").Value]));
            }

            var getAIPRMCTS = from c in doc.Root.Descendants("PRMCTS")
                              select c;

            foreach (var v in getAIPRMCTS)
            {
                AIs.Add(v.Attribute("id").Value, new PRMCTS(games[v.Descendants("PlayGame").First().Attribute("refid").Value],
                    selectionPolicies[v.Descendants("UseSelectionPolicy").First().Attribute("refid").Value],
                    stopPolicies[v.Descendants("UseStopPolicy").First().Attribute("refid").Value],
                    Double.Parse(v.Descendants("Width").First().Value), Double.Parse(v.Descendants("TimeLimit").First().Value)));
            }

            #endregion


#warning Add remeaining AI types


            Console.WriteLine("hi");
        }

    }
}
