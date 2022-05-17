using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iengine
{
    abstract class IE
    {
        protected string _output;

        // Constructor
        public IE()
        {}

        // Read-Only Property Output
        public string Output
        {
            get { return _output; }
        }

        // Abstract Inference Function
        public abstract void Infer(KB kB, string query);

        // Check If Model Satisfies Knowledge Base
        protected static bool PLTrue(KB kB, Dictionary<string, bool> model)
        {
            // Check Model Satisfies All True Symbols In KB
            foreach(var symbol in kB.Symbols)
                if (symbol.Value && model[symbol.Key] == false)
                    return false;

            // Check Model Satisfies All Sentences In KB
            List<Queue<string>> sentences = new(kB.PostfixSentences);
            foreach (Queue<string> sentence in sentences)
                if (!PostfixEvaluator(new(sentence), model))
                    return false;

            return true;
        }

        // Evaluate Propositional Postfix Sentence Using Model
        protected static bool PostfixEvaluator(Queue<string> sentence, Dictionary<string, bool> model)
        {
            Stack<bool> stack = new Stack<bool>();
            bool LHS, RHS;

            while (sentence.Count > 0)
                // Token Is Symbol
                if (Regex.IsMatch(sentence.Peek(), "^[a-zA-Z0-9]+$"))
                    stack.Push(model[sentence.Dequeue()]);
                // Token Is Operation
                else
                    switch (sentence.Dequeue())
                    {
                        case "~": // Negation
                            stack.Push(!stack.Pop()); break;
                        case "&": // Conjunction
                            RHS = stack.Pop(); LHS = stack.Pop();
                            stack.Push(RHS && LHS); break;
                        case "||": // Disjunction
                            RHS = stack.Pop(); LHS = stack.Pop();
                            stack.Push(LHS || RHS); break;
                        case "=>": // Implication
                            RHS = stack.Pop(); LHS = stack.Pop();
                            stack.Push(!LHS || RHS); break;
                        case "<=>": // Biconditional
                            RHS = stack.Pop(); LHS = stack.Pop();
                            stack.Push(LHS == RHS); break;
                        default: // Invalid Token
                            throw new FormatException("Postfix Sentence Is Invalid.");
                    }

            return stack.Pop();
        }

        // Creator Pattern
        public static IE CreateIE(string mode)
        {
            switch (mode)
            {
                case "TT": return new TT();
                case "FC": return new FC();
                case "BC": return new BC();
                default: throw new ArgumentException("Invalid Method '" + mode + "'");
            }
        }
    }
}
