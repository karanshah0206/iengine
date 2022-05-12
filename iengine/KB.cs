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

        // Constructor
        public KB()
        {
            _operatorPrecedence = new();
            _symbols = new();
            _sentences = new();

            // Setting Up Logicl Operator Precedences
            _operatorPrecedence.Add("~", 4);
            _operatorPrecedence.Add("&", 3);
            _operatorPrecedence.Add("||", 2);
            _operatorPrecedence.Add("=>", 1);
            _operatorPrecedence.Add("<=>", 0);
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

        // Convert Infix Sentences To Postfix Using Shunting Yard Algorithm
        private Queue<string> ShuntingYard(string[] infixSentence)
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

                        // If First Item On Stack Takes Precedence
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
    }
}
