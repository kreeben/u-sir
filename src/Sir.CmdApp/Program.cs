using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sir.CmdApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new WordNode('\0'.ToString());
            var timer = new Stopwatch();
            string[] input;

            input = args.Length == 0 ? null : args;

            while (true)
            {
                if (input == null)
                {
                    input = Console.ReadLine().Split(
                        " ", StringSplitOptions.RemoveEmptyEntries);
                }

                if (input.Length == 0 || input[0] == "q" || input[0] == "quit")
                {
                    break;
                }

                if (input[0] == "add")
                {
                    timer.Restart();

                    Add(input.Skip(1).ToArray(), tree);

                    timer.Stop();

                    var edges = tree.ToList();
                    foreach (var edge in edges)
                    {
                        Console.WriteLine(edge);
                    }
                }
                else if (input[0] == "find" && input.Length > 1)
                {
                    timer.Restart();

                    var result = Find(input[1], tree);

                    timer.Stop();

                    if (result != null)
                    {
                        foreach (var edge in result)
                            Console.WriteLine(edge);
                    }
                }

                Console.WriteLine("{0} ticks", timer.Elapsed.Ticks);
                Console.WriteLine();

                input = null;
            }
        }

        private static void Add(string[] input, WordNode tree)
        {
            foreach (var word in input)
            {
                tree.Add(new WordNode(word));
            }
        }

        private static IList<WordEdge> Find(string input, WordNode tree)
        {
            var result = new List<string>();
            var word = new WordNode(input);
            var closest = tree.FindClosestTangent(word);
            return closest == null ? null : closest.ToList();
        }
    }
}
