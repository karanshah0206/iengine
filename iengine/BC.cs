using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class BC : IE
    {
        List<string> _outputList;

        public BC() : base()
        { _output = ""; _outputList = new(); }

        public override void Infer(KB kB, string query)
        {
            // Get All Known Symbols From KB
            Dictionary<string, bool> symbols = new(kB.Symbols);

            _output = "NO"; // Initialise Output

            // If Query Symbol Unknown To KB, BC Cannot Infer Query
            if (!symbols.ContainsKey(query)) return;

            // Recursive Backward-Chaining
            List<string> explored = new();
            if (BCRecursive(kB, symbols, query, explored))
            {
                _output = "YES: ";
                foreach (string s in _outputList)
                    if (s != query)
                        _output += s + ", ";
                _output += query;
            }
        }

        private bool BCRecursive(KB kB, Dictionary<string, bool> symbols, string query, List<string> explored)
        {
            // If Query Is A Known Fact
            if (symbols[query] == true)
            {
                if (!_outputList.Contains(query))
                    _outputList.Add(query);
                return true;
            }

            // Get Clauses That Concludes Query
            foreach (Queue<string> c in kB.PostfixSentences)
            {
                string[] clause = c.ToArray();
                if (clause[^2] == query)
                {
                    Queue<string> premiseSymbols = new();

                    // Get All Symbols In The Premise Of The Clause
                    for (int i = 0; i < clause.Length - 2; i++)
                        if (Regex.IsMatch(clause[i], "^[a-zA-Z0-9]+$"))
                            premiseSymbols.Enqueue(clause[i]);

                    // Check If All Symbols In The Premise Are True
                    int trueSymbolCount = 0;
                    foreach (string p in premiseSymbols)
                    {
                        if (p == query) break; // Avoiding Infinite Recursion
                        if (explored.Contains(p)) // If Already Symbol Explored
                        {
                            if (!symbols[p]) break; // Explored Symbol Evaluated False
                        }
                        else explored.Add(p); // Add Symbol To Explored

                        symbols[p] = BCRecursive(kB, new(symbols), p, explored);
                        if (!symbols[p]) break;
                        trueSymbolCount++;
                    }

                    // Check If All Symbols In Premise Are True
                    if (trueSymbolCount == premiseSymbols.Count)
                    {
                        if (!_outputList.Contains(query))
                            _outputList.Add(query);
                        return true;
                    }
                }
            }

            return false; // KB Does Entail Query
        }
    }
}
