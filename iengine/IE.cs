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
        protected bool PLTrue(KB kB, Dictionary<string, bool> model)
        {
            foreach (Queue<string> sentence in kB.PostfixSentences)
                if (!PostfixEvaluator(sentence, model))
                    return false;
            return true;
        }

        // Evaluate Propositional Postfix Sentence Using Model
        protected bool PostfixEvaluator(Queue<string> sentence, Dictionary<string, bool> model)
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
