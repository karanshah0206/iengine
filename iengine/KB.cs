using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class KB
    {
        private Dictionary<string, bool> _symbols;
        private List<string> _sentences;

        // Constructor
        public KB()
        {
            _symbols = new();
            _sentences = new();
        }

        // Add Sentence To KB (Sentence Cannot Contain Spaces)
        public void AddSentence(string sentence)
        {
            if (!_sentences.Contains(sentence) && sentence.Length > 0)
            {
                // Check If Sentence Is A Fact
                Regex factPattern = new(@"^[a-zA-Z0-9]+$");

                // If Sentence Is Fact, Append Symbol Value To True
                if (factPattern.IsMatch(sentence))
                {
                    _symbols[sentence] = true;
                }
                // Else Add New Sentence
                else
                {
                    _sentences.Add(sentence);

                    // Look For Unknown Symbols In Sentence
                    foreach (string symbol in Regex.Split(sentence, "[()&|<=>~]+"))
                    {
                        if (symbol.Trim().Length <= 0) continue;
                        if (!_symbols.ContainsKey(symbol.Trim()))
                            _symbols[symbol.Trim()] = false;
                    }
                }
            }
        }
    }
}
