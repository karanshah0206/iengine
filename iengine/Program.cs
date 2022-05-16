using System;
using System.IO;

namespace iengine
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check Arguments
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid arguments.");
                Console.WriteLine("Usage: iengine <method> <filename>");
                return;
            }

            // Check File Exists
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("Cannot find file: " + args[1]);
                return;
            }

            try
            {
                IE iE = IE.CreateIE(args[0]); // Initialise Inference Engine
                KB kB = new(); // Initialise Knowledge Base

                // Get Data From File
                string[] data = File.ReadAllLines(args[1]);

                // Add Sentences To KB (Remove All Spaces)
                foreach (string sentence in data[1].Split(";"))
                    kB.AddSentence(sentence.Trim().Replace(" ", string.Empty));

                // Run Inference Engine & Show Ouptut
                iE.Infer(kB, data[3].Trim().Replace(" ", string.Empty));
                Console.WriteLine(iE.Output);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
