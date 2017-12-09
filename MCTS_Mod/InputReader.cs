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

        private enum ReversiTestStartingPlayer { AlwaysFirst, AlwaysSecond, SwitchEveryGame, Random };

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
                games.Add(v.Attribute("id").Value, new GameReversi(r, Int32.Parse(v.Descendants("HeuristicReversi").First().Value), 
                    Int32.Parse(v.Descendants("EvaluationFunction").First().Value), 
                    Int32.Parse(v.Descendants("Heuristic2GroupCount").First().Value)));

            }

            var getGame2 = from c in doc.Root.Descendants("G2048")
                           select c;
            foreach (var v in getGame2)
            {
                games.Add(v.Attribute("id").Value, new Game2048(r, Int32.Parse(v.Descendants("Heuristic2048").First().Value), 
                    Double.Parse(v.Descendants("NextVal").First().Value), 
                    Double.Parse(v.Descendants("TileVal").First().Value), 
                    Int32.Parse(v.Descendants("EvalRoot").First().Value)));
            }

            var getGame2D = from c in doc.Root.Descendants("G2048D")
                            select c;
            foreach (var v in getGame2D)
            {
                games.Add(v.Attribute("id").Value, new Game2048Derandomized(r, Int32.Parse(v.Descendants("Heuristic2048").First().Value), 
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
                    stopPolicies[v.Descendants("UseStopPolicy").First().Attribute("refid").Value].Clone()));
            }

            var getAIPRMCTS = from c in doc.Root.Descendants("PRMCTS")
                              select c;

            foreach (var v in getAIPRMCTS)
            {
                AIs.Add(v.Attribute("id").Value, new PRMCTS(games[v.Descendants("PlayGame").First().Attribute("refid").Value],
                    selectionPolicies[v.Descendants("UseSelectionPolicy").First().Attribute("refid").Value],
                    stopPolicies[v.Descendants("UseStopPolicy").First().Attribute("refid").Value].Clone(),
                    Double.Parse(v.Descendants("Width").First().Value), Double.Parse(v.Descendants("TimeLimit").First().Value)));
            }

            var getAIBoMCTS = from c in doc.Root.Descendants("BoMCTS")
                              select c;

            foreach (var v in getAIBoMCTS)
            {
                AIs.Add(v.Attribute("id").Value, new BoMCTS(games[v.Descendants("PlayGame").First().Attribute("refid").Value],
                    selectionPolicies[v.Descendants("UseSelectionPolicy").First().Attribute("refid").Value],
                    stopPolicies[v.Descendants("UseStopPolicy").First().Attribute("refid").Value].Clone(),
                    Int32.Parse(v.Descendants("ParallelSimulations").First().Value), Boolean.Parse(v.Descendants("UseRAVE").First().Value),
                    Double.Parse(v.Descendants("Beta").First().Value)));
            }

            #endregion


#warning Add remeaining AI types

        }

        public void Init()
        {
            string introMenu = "Please select what you want to do by entering the id of option below:\r\n1) Run a user defined AI\r\n2) Run a test from predefined tests";

            Console.WriteLine(introMenu);

            bool validInput = false;
            string input = "";

            while (!validInput)
            {
                input = Console.ReadLine();
                
                if (input.Equals("1") || input.Equals("2"))
                    validInput = true;
                else
                {
                    Console.Clear();
                    Console.WriteLine(introMenu);
                    Console.WriteLine("Incorrect format of input, please write '1' or '2'");
                }
            }
            if (input.Equals("1"))
                UserDefinedTests();
            else
                PredefinedTests();            
        }

        private void UserDefinedTests()
        {
            Console.Clear();

            if (AIs.Count <= 0)
            {
                Console.WriteLine("There are no user defined AIs. Press enter to return to main menu.");
                Console.ReadLine();
                Init();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Please select an AI you want to test by entering it's ID or enter 'X' to return to main menu:");
            
            for (int i = 0; i < AIs.Count; i++)
            {
                sb.Append($"\r\n{i+1}) {AIs.ToArray()[i].Key}");
            }

            string introMenu = sb.ToString();

            Console.WriteLine(introMenu);

            bool validInput = false;
            string input = "";
            int inputID = -1;

            while (!validInput)
            {
                input = Console.ReadLine();

                CheckInput(input);

                if (Int32.TryParse(input, out inputID))
                {
                    if (!(inputID <= 0 || inputID > AIs.Count))
                        validInput = true;
                }
                
                if (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(introMenu);
                    Console.WriteLine("Incorrect format of input, please write the id of selected AI. For example '1'.");
                }
            }

            TestUserDefinedAI(AIs.ToArray()[inputID - 1].Value);
        }

        private void TestUserDefinedAI(MCTS ai)
        {
            Console.Clear();
            if (ai.Game.Name().Equals("Reversi"))
                TestUserDefinedAIReversi(ai);
            else
                TestUserDefinedAI2048(ai);
        }

        private void TestUserDefinedAIReversi(MCTS ai)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Please select an AI you want to test selected AI against by entering it's ID or enter 'X' to return to main menu:");

            for (int i = 0; i < AIs.Count; i++)
            {
                sb.Append($"\r\n{i + 1}) {AIs.ToArray()[i].Key}");
            }

            string introMenu = sb.ToString();

            Console.WriteLine(introMenu);

            bool validInput = false;
            string input = "";
            int inputID = -1;

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputID))
                {
                    if (!(inputID <= 0 || inputID > AIs.Count) && AIs.ToArray()[inputID - 1].Value.Game.Name().Equals("Reversi"))
                        validInput = true;
                }

                if (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(introMenu);
                    Console.WriteLine("Incorrect format of input or selected AI was not configured to play Reversi, please write the id of selected AI. For example '1'.");
                }
            }

            MCTS AI2 = AIs.ToArray()[inputID - 1].Value;

            int iter = GetIterations();
            bool reset = GetResetTree();
            ReversiTestStartingPlayer startingPlayer = GetStartingPlayer();

            double finalResult = 0.0;

            for (int i = 0; i < iter; i++)
            {
                Console.Clear();
                Console.WriteLine($"Progress: {i}/{iter}");

                byte first = 255;
                switch (startingPlayer)
                {
                    case ReversiTestStartingPlayer.AlwaysFirst:
                        first = 0;
                        break;
                    case ReversiTestStartingPlayer.AlwaysSecond:
                        first = 1;
                        break;
                    case ReversiTestStartingPlayer.SwitchEveryGame:
                        first = (byte)(i % 2);
                        break;
                    default:
                        first = (byte)r.Next(0, 2);
                        break;
                }

                var resultOfGame = PlayControl.PlayReversiAIAI(ai, AI2, (GameReversi)ai.Game, r, first, false);
                finalResult += resultOfGame;
            }

            Console.Clear();
            Console.WriteLine($"Progress: {iter}/{iter}");
            Console.WriteLine("Test finished.");
            Console.WriteLine($"Winrate of AI1: {((double)finalResult / (double)iter)*100.0}%");
        }

        private void TestUserDefinedAI2048(MCTS ai)
        {
            int iter = GetIterations();
            bool reset = GetResetTree();
            int results = 0;

            for (int i = 0; i < iter; i++)
            {
                Console.Clear();
                Console.WriteLine($"Progress: {i}/{iter}");
                var finalState = PlayControl.Play2048AI(ai, ai.Game, false, r, reset);
                results += finalState.Depth;
            }

            Console.Clear();
            Console.WriteLine($"Progress: {iter}/{iter}");
            Console.WriteLine("Test finished.");
            Console.WriteLine($"Average reached depth: {(double)results / (double)iter}");
        }

        private int GetIterations()
        {
            Console.Clear();
            string intro = "How many iterations to run (Enter 'X' to return to main menu):";
            Console.WriteLine(intro);

            string input = "";
            int inputValue = -1;
            bool validInput = false;

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputValue))
                    if (inputValue > 0)
                        validInput = true;

                if (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(intro);
                    Console.WriteLine("Incorrect input. Please enter a natural number.");
                }
            }
            Console.Clear();
            return inputValue;
        }

        private bool GetResetTree()
        {
            Console.Clear();
            string intro = "Should the game tree be reseted after every move (recommended for large trees) or enter 'X' to return to main menu:\r\n1) Yes\r\n2) No";
            Console.WriteLine(intro);

            string input = "";
            int inputValue = -1;
            bool validInput = false;

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputValue))
                    if (inputValue == 1 || inputValue == 2)
                        validInput = true;

                if (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(intro);
                    Console.WriteLine("Incorrect input. Please enter '1' or '2'.");
                }
            }
            Console.Clear();
            return inputValue == 1;
        }

        private ReversiTestStartingPlayer GetStartingPlayer()
        {
            Console.Clear();
            string intro = "Select who the starting AI should be across iterations (enter 'X' to return to main menu):\r\n1) Always AI1\r\n2) Always AI2\r\n3) Swap every game\r\n4) Random every game";
            Console.WriteLine(intro);

            string input = "";
            int inputValue = -1;
            bool validInput = false;

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputValue))
                    if (inputValue == 1 || inputValue == 2 || inputValue == 3 || inputValue == 4)
                        validInput = true;

                if (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine(intro);
                    Console.WriteLine("Incorrect input. Please enter '1','2','3' or '4'.");
                }
            }
            Console.Clear();
            switch (inputValue)
            {
                case 1:
                    return ReversiTestStartingPlayer.AlwaysFirst;
                case 2:
                    return ReversiTestStartingPlayer.AlwaysSecond;
                case 3:
                    return ReversiTestStartingPlayer.SwitchEveryGame;
                default:
                    return ReversiTestStartingPlayer.Random;
            }
        }

        private void PredefinedTests()
        {
            Console.Clear();
            string predefinedTestsMenu = "Tests used in accompanying work. Split by chapter they appear in. Enter 'X' to return to main menu.\r\n" +
                "1) Hry\r\n" +
                "2) PRMCTS\r\n" +
                "3) BMCTS\r\n" +
                "4) BoMCTS\r\n" +
                "5) Implementace";

            Console.WriteLine(predefinedTestsMenu);

            bool validInput = false;
            string input = "";

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (input.Equals("1") || input.Equals("2") || input.Equals("23") || input.Equals("4") || input.Equals("5"))
                    validInput = true;
                else
                {
                    Console.Clear();
                    Console.WriteLine(predefinedTestsMenu);
                    Console.WriteLine("Incorrect format of input, please write '1', '2', etc");
                }
            }

            List<string> menu = null;
            ITests tests = null;

            switch(input)
            {
                case "1":
                    tests = new GameTests();
                    break;
                case "2":
                    tests = new PRMCTSTests();
                    break;
                case "4":
                    tests = new RAVETests();
                    break;
                case "5":
                    tests = new ImplementationTests();
                    break;
                default:
                    Console.WriteLine("Error");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
            }

            menu = tests.GenerateMenu();
            PredefinedTestsMenuSection(menu, tests);
        }

        private void PredefinedTestsMenuSection(List<string> menu, ITests tests)
        {
            Console.Clear();
            bool validInput = false;
            string input = "";
            int inputID;

            menu.Add("X) Return to main menu");

            menu.ForEach(s => Console.WriteLine(s));

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputID) && inputID >= 1 && inputID <= menu.Count)
                    validInput = true;
                else
                {
                    Console.Clear();
                    menu.ForEach(s => Console.WriteLine(s));
                    Console.WriteLine("Incorrect format of input, please write '1', '2', etc");
                }
            }

            List<string> subMenu = tests.GenerateSubmenu(input);
            PredefinedTestsSubMenuSection(subMenu, tests, input);
        }

        private void PredefinedTestsSubMenuSection(List<string> menu, ITests tests, string id1)
        {
            Console.Clear();
            bool validInput = false;
            string input = "";
            int inputID;
            menu.Add("X) Return to maiń menu");
            menu.ForEach(s => Console.WriteLine(s));

            while (!validInput)
            {
                input = Console.ReadLine();
                CheckInput(input);
                if (Int32.TryParse(input, out inputID) && inputID >= 1 && inputID <= menu.Count)
                    validInput = true;
                else
                {
                    Console.Clear();
                    menu.ForEach(s => Console.WriteLine(s));
                    Console.WriteLine("Incorrect format of input, please write '1', '2', etc");
                }
            }
            tests.RunTest(id1 + ";" + input, r);
            Console.WriteLine("Test finished, results written to file. Press enter to return to main menu");
            Console.ReadLine();
            Console.Clear();
            Init();
        }

        private void CheckInput(string s)
        {
            if (s.ToUpper().Equals("X"))
            {
                Console.Clear();
                Init();
            }
        }



    }
}
