using System;

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
