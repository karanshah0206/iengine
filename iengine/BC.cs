using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class BC : IE
    {
        public BC() : base()
        { _output = ""; }

        public override void Infer(KB kB, string query)
        {
            // Get All Known Symbols From KB
            Dictionary<string, bool> symbols = new(kB.Symbols);

            // If Query Symbol Unknown To KB, BC Cannot Infer Query
            if (!symbols.ContainsKey(query))
            {
                _output = "NO";
                return;
            }

            // Recursive Backward-Chaining
            List<string> explored = new();
            if (BCRecursive(kB, symbols, query, explored))
            {
                _output = "YES: ";
                foreach (string s in explored)
                    _output += s + " ";
                _output += query;
            }
            else _output = "NO";
        }

        private bool BCRecursive(KB kB, Dictionary<string, bool> symbols, string query, List<string> explored)
        {
            // If Query Is A Known Fact
            if (symbols[query] == true) return true;

            // Get Clause That Concludes Query
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
                        symbols[p] = BCRecursive(kB, new(symbols), p, explored);
                        if (!symbols[p]) break;
                        if (!explored.Contains(p)) { explored.Add(p); }
                        trueSymbolCount++;
                    }
                    if (trueSymbolCount == premiseSymbols.Count) return true;
                }
            }
            return false;
        }
    }
}
