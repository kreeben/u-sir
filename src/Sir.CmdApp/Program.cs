using Sir.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Sir.CmdApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new WordTree();
            var timer = new Stopwatch();
            string[] input = args.Length == 0 ? null : args;

            while (true)
            {
                if (input == null)
                {
                    var line = Console.ReadLine();
                    input = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                }

                if (input.Length == 0 || input[0] == "q" || input[0] == "quit")
                {
                    break;
                }
                else if (input[0] == "add_page" && input.Length == 2)
                {
                    timer.Restart();
                    AddWebPage(input[1], tree);
                    timer.Stop();
                    Console.WriteLine(tree.Visualize());
                    Console.WriteLine("count: {0}", tree.Count);
                    Console.WriteLine("merges: {0}", tree.MergeCount);
                }
                else if (input[0] == "add_file" && input.Length == 3)
                {
                    Console.Write("indexing ");
                    timer.Restart();

                    foreach (var file in Directory.GetFiles(input[1], input[2]))
                    {
                        var n = AddFile(file, tree);
                        Console.Write("{0} {1} ", file, n);
                    }

                    Console.WriteLine();
                    timer.Stop();

                    Console.WriteLine(tree.Visualize());
                    Console.WriteLine("count: {0}", tree.Count);
                    Console.WriteLine("merges: {0}", tree.MergeCount);
                }
                else if (input[0] == "add")
                {
                    timer.Restart();
                    Add(input.Skip(1).ToArray(), tree);
                    timer.Stop();
                    Console.WriteLine(tree.Visualize());
                }
                else if (input[0] == "find" && input.Length > 1)
                {
                    timer.Restart();
                    foreach(var term in input.Skip(1))
                    {
                        var result = Find(term, tree);

                        if (result == null)
                        {
                            Console.WriteLine("{0} not found", term);
                        }
                        else
                        {
                            Console.WriteLine("{0} {1} ", result, result.Highscore);
                            var sb = new StringBuilder();
                            var cursor = result.Ancestor;
                            while (cursor != null)
                            {
                                Console.Write("{0} {1} ", cursor, cursor.Angle);
                                cursor = cursor.Ancestor;
                            }
                            Console.WriteLine();
                        }
                    }
                    timer.Stop();
                }
                else if (input[0] == "analyze" && input.Length == 2)
                {
                    timer.Restart();
                    var node = Find(input[1], tree);
                    if (node != null)
                    {
                        foreach(var x in node.WordVector)
                        {
                            Console.WriteLine("{0}:{1}", x.Key, x.Value);
                        }
                    }
                    timer.Stop();
                }
                else if (input[0] == "compare" && input.Length > 2)
                {
                    timer.Restart();
                    var result = Compare(input[1], input[2]);
                    timer.Stop();
                    Console.WriteLine("angle: {0} len1: {1}, len2: {2}", result.angle, result.len1, result.len2);
                }
                else
                {
                    break;
                }

                Console.WriteLine("{0} ticks", timer.Elapsed.Ticks);
                Console.WriteLine();

                input = null;
            }

            tree.Dispose();
        }

        private static WordNode Find(string input, WordTree tree)
        {
            return tree.Find(input);
        }

        private static int AddFile(string path, WordTree tree)
        {
            var text = GetLocalString(path);
            return Add(Tokenize(text), tree);
        }

        private static void AddWebPage(string url, WordTree tree)
        {
            var text = GetWebString(url);
            Add(Tokenize(text), tree);
        }

        private static int Add(string[] input, WordTree tree)
        {
            var dic = new SortedList<double, string>();
            var count = tree.Count;

            foreach (var word in input)
            {
                tree.Add(word);

                if (new WordNode(word).WordVector.CosAngle(tree.Find(word).WordVector) < WordNode.TRUE_ANGLE)
                {
                    throw new Exception("error");
                }

            }

            return tree.Count - count;
        }

        static string[] Tokenize(string text)
        {
            var decoded = WebUtility.HtmlDecode(text);
            var parser = new YesNoParser.YesNoParser('>', '<');
            var parsed = parser.Parse(decoded).ToLower();
            return parsed.Split(
                new char[] { ' ', '(', ')', '[', ']', '{', '}', '.', ',', ';', ':', '/', '\\', '*', '"', '\'',
                    '?', '-', '+', '_', '^', '<', '>', '|', '=', '&', '\r', '\n', '\t'},
                StringSplitOptions.RemoveEmptyEntries);
        }

        static string GetWebString(string url)
        {
            var req = WebRequest.CreateHttp(url);
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0";
            using (var response = req.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                return reader.ReadToEnd();
            }
        }

        static string GetLocalString(string fileName)
        {
            var fn = fileName.Replace("file://", "");
            return File.ReadAllText(fn);
        }

        private static (double angle, double len1, double len2) Compare(string v1, string v2)
        {
            var a = v1.ToVector();
            var b = v2.ToVector();

            return (a.CosAngle(b), Math.Sqrt(a.Dot(a)), Math.Sqrt(b.Dot(b)));
        }
    }
}
