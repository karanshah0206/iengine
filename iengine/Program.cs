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

            // Initialise KB
            KB kB = new();

            // Get Data From File
            string[] data = File.ReadAllLines(args[1]);

            // Add Sentences To KB
            foreach (string sentence in data[1].Split(";"))
                kB.AddSentence(sentence.Trim().Replace(" ", string.Empty));
        }
    }
}
