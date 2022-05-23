using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class FC : IE
    {
        public FC() : base()
        { _output = ""; }

        public override void Infer(KB kB, string query)
        {
            Dictionary<Queue<string>, int> count = new();
            Dictionary<string, bool> inferred = new();
            Queue<string> agenda = new();

            // Initialise Inferred & Agenda
            foreach(KeyValuePair<string, bool> symbol in kB.Symbols)
            {
                inferred[symbol.Key] = false;
                if (symbol.Value) agenda.Enqueue(symbol.Key);
            }

            // Initialise Count
            foreach(Queue<string> clause in kB.PostfixSentences)
            {
                count[clause] = 0;
                Queue<string> temp = new(clause);

                while (temp.Count > 2)
                    if (Regex.IsMatch(temp.Dequeue(), "^[a-zA-Z0-9]+$"))
                        count[clause]++;
            }

            while (agenda.Count > 0)
            {
                string p = agenda.Dequeue();

                if (p == query) // Query Inferred
                {
                    _output = "YES: " + _output + p;
                    return;
                }

                if (!inferred[p])
                {
                    inferred[p] = true; // Symbol p Inferred
                    _output += p + " ";

                    foreach(Queue<string> clause in kB.PostfixSentences)
                    {
                        string[] temp = clause.ToArray();
                        for (int i = 0; i < temp.Length - 2; i++) // Checking If Premise Has p
                            if (temp[i] == p) // p Found
                            {
                                if (--count[clause] <= 0)
                                    agenda.Enqueue(temp[^2]); // Conclusion Inferred
                                break;
                            }
                    }
                }
            }

            _output = "NO"; // Query Not Inferred
        }
    }
}
