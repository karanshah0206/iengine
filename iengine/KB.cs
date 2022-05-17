using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    class KB
    {
        private Dictionary<string, int> _operatorPrecedence;
        private Dictionary<string, bool> _symbols;
        private List<string> _sentences;
        private List<Queue<string>> _postfixSentences;

        // Constructor
        public KB()
        {
            _operatorPrecedence = new();
            _symbols = new();
            _sentences = new();
            _postfixSentences = new();

            // Setting Up Logicl Operator Precedences
            _operatorPrecedence.Add("~", 4);
            _operatorPrecedence.Add("&", 3);
            _operatorPrecedence.Add("||", 2);
            _operatorPrecedence.Add("=>", 1);
            _operatorPrecedence.Add("<=>", 0);
        }

        // Read-Only Property Symbols
        public Dictionary<string, bool> Symbols
        {
            get { return _symbols; }
        }

        // Read-Only Property Sentences (Infix)
        public List<string> Sentences
        {
            get { return _sentences; }
        }

        // Read-Only Property Postfix Sentences
        public List<Queue<string>> PostfixSentences
        {
            get { return _postfixSentences; }
        }

        // Add Sentence To KB (Sentence Cannot Contain Spaces)
        public void AddSentence(string sentence)
        {
            if (!_sentences.Contains(sentence) && sentence.Length > 0)
            {
                // Check If Sentence Is A Fact
                Regex factPattern = new(@"^[a-zA-Z0-9]+$");

                // If Sentence Is Fact, Append Symbol Value To True
                if (factPattern.IsMatch(sentence)) _symbols[sentence] = true;
                else // Else Add New Sentence
                {
                    _sentences.Add(sentence);
                    _postfixSentences.Add(ShuntingYard(SentenceToArray(sentence)));

                    // Look For Unknown Symbols In Sentence
                    foreach (string symbol in GetSymbolsFromSentence(SentenceToArray(sentence)))
                        if (!_symbols.ContainsKey(symbol))
                            _symbols[symbol] = false;
                }
            }
        }

        // Get All Propositional Symbols From A Sentence String
        public static List<string> GetSymbolsFromSentence(string[] sentence)
        {
            List<string> symbols = new();
            foreach (string token in sentence)
                if (Regex.IsMatch(token, "^[a-zA-Z0-9]+$"))
                    symbols.Add(token);
            //foreach (string symbol in Regex.Split(sentence, "[()&|<=>~]+"))
            //    if (symbol.Trim().Length <= 0) continue;
            //    else symbols.Add(symbol.Trim());
            return symbols;
        }

        // Convert Infix Sentences To Postfix Using Shunting Yard Algorithm
        public Queue<string> ShuntingYard(string[] infixSentence)
        {
            Queue<string> queue = new();
            Stack<string> stack = new();

            for (int index = 0; index < infixSentence.Length; index++)
            {
                string token = infixSentence[index];

                // If Symbol, Add To Queue
                if (Regex.IsMatch(token, "^[a-zA-Z0-9]+$"))
                    queue.Enqueue(token);

                // If Opening Bracket, Add To Stack
                else if (token == "(") stack.Push(token);

                // If Closing Bracket, Shift Enclosed Operators From Stack To Queue
                else if (token == ")")
                {
                    string stackItem = "";
                    while (stackItem != "(")
                    {
                        // Cannot Find A Corresponding Opening Bracket
                        if (stack.Count == 0)
                            throw new FormatException("Unbalanced Sentence Found In Data File.");

                        // Add Stack Item To Queue If Not An Opening Bracket
                        stackItem = stack.Pop();
                        if (stackItem != "(") queue.Enqueue(stackItem);
                    };
                }

                // If Token Is An Operator
                else
                {
                    while (true)
                    {
                        // If Stack Empty, Push Operator To Stack
                        if (stack.Count == 0)
                        {
                            stack.Push(token);
                            break;
                        }

                        string stackItem = stack.Peek();

                        // If First Item On Stack Is Opening Bracket, Push Operator
                        if (stackItem == "(")
                        {
                            stack.Push(token);
                            break;
                        }

                        // If Stack Item Takes Precedence
                        // Or Equal Precedence And Token Is Left-Associative, Enqueue Stack Item
                        if (_operatorPrecedence[stackItem] > _operatorPrecedence[token]
                            || (_operatorPrecedence[stackItem] == _operatorPrecedence[token] && token != "~"))
                            queue.Enqueue(stack.Pop());
                        // If Token Takes Precedence Or Is Right-Associative, Push Into Stack
                        else
                        {
                            stack.Push(token);
                            break;
                        }
                    }
                }
            }

            // Enqueue Remaining Operators In Stack
            while (stack.Count > 0) queue.Enqueue(stack.Pop());
            return queue;
        }

        // Split Elements In Sentence To An Array
        public static string[] SentenceToArray(string sentence)
        {
            List<string> tokens = new();
            string token = "";

            // Get Complete Token From Sentence
            for (int i = 0; i < sentence.Length; i++)
            {
                // Update Token
                token += sentence.Substring(i, 1);

                // If Token Is Incomplete, Continue
                if (i < sentence.Length - 1 && !IsFullToken(token, sentence.Substring(i + 1, 1)))
                    continue;
                // Else Check If Last Character In Sentence Is Valid
                else if (i == sentence.Length - 1) IsFullToken(token);

                // Add Complete Token To Array
                tokens.Add(token);
                token = "";
            }

            return tokens.ToArray();
        }

        // Check If Token Is Complete
        private static bool IsFullToken(string token, string next = "#")
        {
            // Token Is A Symbol
            if (Regex.IsMatch(token, "^[a-zA-Z0-9]+$"))
                return (next == "#" || !Regex.IsMatch(next, "^[a-zA-Z0-9]$"));
            // Token Is A Valid Operator
            else if (Regex.IsMatch(token, "^[|]{2}$|^<=>$|^=>$|^&$|^~$|^[(]{1}$|^[)]{1}$"))
                return true;
            // Invalid Token (Throw Format Exception)
            else if (token != "|" && token != "<" && token != "<=" && token != "=")
                throw new FormatException("Unknown Token '" + token + "' Found In Data File.");
            // Default False Return
            return false;
        }
    }
}
