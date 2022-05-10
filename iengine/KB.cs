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

        // Add New Sentence To KB
        // Sentence Contains Only Alphanumeric Symbols & Operations (No Spaces)
        public void AddSentence(string sentence)
        {
            sentence = sentence.Replace(" ", string.Empty);
            if (!_sentences.Contains(sentence))
            {
                // Check If Sentence Is Atomic (i.e. Is It A Symbol?)
                // Sentence Is Atomic If Has No Operations (Alphanumeric)
                Regex alphaNumeric = new(@"^[a-zA-Z0-9]+$");

                // Add Symbol With Value True
                if (alphaNumeric.IsMatch(sentence))
                {
                    _symbols[sentence] = true;
                }
                // Add Sentence And Unknown Symbols
                else
                {
                    // Add Sentence
                    _sentences.Add(sentence);

                    // Add Symbols
                    string[] symbols = Regex.Split(sentence, "[()&|<=>~]+");
                    foreach (string symbol in symbols)
                    {
                        string value = symbol.Trim();

                        // Continue If String Is Blank
                        if (value.Length <= 0) continue;

                        // Add Symbol If Doesn't Exist
                        if (!_symbols.ContainsKey(value))
                            _symbols[value] = false;
                    }
                }
            }
        }
    }
}
