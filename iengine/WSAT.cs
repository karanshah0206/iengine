using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class WSAT : IE
    {
        public WSAT()
        { _output = ""; }

        public override void Infer(KB kB, string query)
        {
            _output = "NO"; // Initialise Output

            // Get Maximum Iteration Count
            Console.Write("Specify Maximum Iterations: ");
            int maxIterations = int.Parse(Console.ReadLine());
            if (maxIterations <= 0) throw new Exception("Maximum Iterations Must Be Greater Than 0.");

            // Get All Symbols From KB
            Dictionary<string, bool> model = new(kB.Symbols);
            // Get Relevance Of Symbols To Clauses In KB
            Dictionary<string, int> symbolPriority = FindMostRelevantSymbol(new(kB.PostfixSentences));

            float randomFlipChance = 0.5f; // Change Of A Symbol Being Flipped Randomly
            ShuffleModel(model); // Random Assignment Of True/False To Symbols In Model

            for (int i = 1; i <= maxIterations; i++)
            {
                if (PLTrue(kB, model)) // Check If Model Satisfies Knowledge Base
                {
                    // Check If Model Satisfies Query
                    if (PostfixEvaluator(kB.ShuntingYard(KB.SentenceToArray(query)), model))
                    {
                        _output = "YES: " + i + " ";
                        foreach (string symbol in model.Keys)
                            if (model[symbol])
                                _output += symbol + " ";
                        return;
                    }
                }

                // Get A Random Clause From KB Which Evaluates To False
                Queue<string> clause = GetRandomFalseClause(new(kB.PostfixSentences), model);
                if (clause.Count > 0)
                {
                    Random random = new();
                    // Flip Random Symbol From The Random Clause
                    if (random.NextDouble() > randomFlipChance) FlipRandomSymbol(clause, model);
                    // Flip Symbol That Maximises Number Of Satisfied Clauses
                    else FlipSymbolByRelevance(new(clause), symbolPriority, model);
                }
                else ShuffleModel(model);
            }
        }

        // Assign Random Values To Symbols Within A Given  Model
        private void ShuffleModel(Dictionary<string, bool> model)
        {
            Random random = new();
            foreach(string symbol in model.Keys)
                if (random.NextDouble() > 0.5) model[symbol] = true;
                else model[symbol] = false;
        }

        // Identifies How Relevant A Symbol Is To Clauses In KB
        private Dictionary<string, int> FindMostRelevantSymbol(List<Queue<string>> clauses)
        {
            Dictionary<string, int> relevance = new();

            foreach (Queue<string> clause in clauses)
                foreach (string s in clause)
                    if (Regex.IsMatch(s, "^[a-zA-z0-9]+$"))
                    {
                        if (relevance.ContainsKey(s)) relevance[s]++;
                        else relevance[s] = 1;
                    }

            return relevance;
        }

        // Select A Random Clause From A List That Evaluates To False
        private Queue<string> GetRandomFalseClause(List<Queue<string>> clauses, Dictionary<string, bool> model)
        {
            // Check If Any False Clause Exists
            bool falseClauseExists = false;
            foreach(Queue<string> clause in clauses)
                if (!PostfixEvaluator(new(clause), new(model)))
                {
                    falseClauseExists = true;
                    break;
                }
            if (!falseClauseExists) return new();

            Random random = new();
            while (true)
            {
                // Get A Random Clause
                int randomIndex = random.Next(clauses.Count);

                // If Clause Evaluates To False, Return
                if (!PostfixEvaluator(new(clauses[randomIndex]), new(model)))
                    return clauses[randomIndex];
            }
        }

        // Select A Random Symbol From A Clause & Flip Its Value
        private void FlipRandomSymbol(Queue<string> clause, Dictionary<string, bool> model)
        {
            Random random = new();
            string[] clauseArray = clause.ToArray();

            while (true)
            {
                // Get A Random Token From Clause
                string symbol = clauseArray[random.Next(clause.Count)];

                // If Token Is Symbol, Flip &  Return
                if (Regex.IsMatch(symbol, "^[a-zA-Z0-9]+$"))
                {
                    model[symbol] = !model[symbol];
                    return;
                }
            }
        }

        // Flip Symbol From A Clause Which Has The Highest Relevance To Clauses In KB
        private void FlipSymbolByRelevance(Queue<string> clause, Dictionary<string, int> relevance, Dictionary<string, bool> model)
        {
            string[] clauseArray = clause.ToArray();
            string symbolToFlip = null;

            foreach (string s in clauseArray)
                if (Regex.IsMatch(s, "^[a-zA-Z0-9]+$"))
                    if (symbolToFlip == null || relevance[s] > relevance[symbolToFlip])
                        symbolToFlip = s;

            if (symbolToFlip != null) model[symbolToFlip] = !model[symbolToFlip];
        }
    }
}
