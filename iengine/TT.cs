using System.Collections.Generic;

namespace iengine
{
    class TT : IE
    {
        public TT() : base()
        { _output = "0"; }

        public override void Infer(KB kB, string query)
        {
            // Get All Symbols From KB
            Dictionary<string, bool> symbols = new(kB.Symbols);

            // Get All Symbols From Query
            foreach (string symbol in KB.GetSymbolsFromSentence(KB.SentenceToArray(query)))
                if (!symbols.ContainsKey(symbol)) symbols[symbol] = false;

            // Check All Models Using Truth Table Model Checking
            TTCheckAll(kB, kB.ShuntingYard(KB.SentenceToArray(query)), new(symbols.Keys), new());

            if (_output == "0") _output = "NO";
            else if (_output != "NO") _output = "YES: " + _output;
        }

        // Use Truth Table Checking Method To Enumerate Models
        private void TTCheckAll(KB kB, Queue<string> query, Queue<string> symbols, Dictionary<string, bool> model)
        {
            if (_output == "NO") return; // Query Not True For All Models Of KB

            if (symbols.Count == 0)
            {
                if (PLTrue(kB, model)) // Model Satisfies KB
                {
                    if (PostfixEvaluator(query, model)) // Model Satisfies Query
                        _output = (int.Parse(_output) + 1).ToString();
                    else { _output = "NO"; return; }
                }
            }
            else
            {
                // Generate Models Using Recursion
                Dictionary<string, bool> mod = new(model);
                string symbol = symbols.Dequeue();
                mod[symbol] = true; TTCheckAll(kB, new(query), new(symbols), mod);
                mod[symbol] = false; TTCheckAll(kB, new(query), new(symbols), mod);
            }
        }
    }
}
