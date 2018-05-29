using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

                    var nodes = tree.ToList();
                    foreach (var n in nodes)
                    {
                        Console.WriteLine(n);
                    }
                }
                else if (input[0] == "find" && input.Length > 1)
                {
                    timer.Restart();

                    var result = Find(input[1], tree);

                    timer.Stop();

                    foreach(var r in result)
                        Console.WriteLine(r);
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

        private static IList<string> Find(string input, WordNode tree)
        {
            var result = new List<string>();
            var word = new WordNode(input);
            var closest = tree.FindClosestTangent(word);
            return closest == null ? null : closest.ToList().Select(x=>x.ToString()).ToList();
        }




    }

    public class WordEdge
    {
        public double Angle { get; private set; }
        public WordNode Node { get; private set; }
        public WordNode Parent { get; private set; }

        public WordEdge(WordNode node, WordNode parent, double angle)
        {
            Angle = angle;
            Node = node;
            Parent = parent;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} angle to {2}", Node, Angle, Parent);
        }
    }

    public class WordNode
    {
        private WordEdge right;
        private WordEdge left;

        private SortedList<char, int> wordVector;

        public WordNode(string s)
        {
            wordVector = ToSparseCharacterVector(s);
        }

        public override string ToString()
        {
            var w = new StringBuilder();
            foreach (var c in wordVector)
            {
                w.Append(c.Key);
            }
            return w.ToString();
        }

        public IList<WordEdge> ToList()
        {
            return All().ToList();
        }

        public IEnumerable<WordEdge> All()
        {
            if (left != null)
            {
                yield return left;

                foreach (var x in left.Node.All())
                {
                    yield return x;
                }
            }
            if (right != null)
            {
                yield return right;

                foreach (var x in right.Node.All())
                {
                    yield return x;
                }
            }
        }

        public WordNode FindClosestTangent(WordNode node)
        {
            var angle = GetAngle(wordVector, node.wordVector);

            if (angle == 0)
            {
                if (right == null && left == null)
                {
                    return null;
                }
                if (right != null)
                {
                    return right.Node.FindClosestTangent(node);
                }
                else
                {
                    return left.Node.FindClosestTangent(node);
                }
            }
            else
            {
                return this;
            }
        }

        public void Add(WordNode node)
        {
            var angle = GetAngle(wordVector, node.wordVector);

            if (angle == 0)
            {
                // word is distal

                if (right == null)
                {
                    // add
                    right = new WordEdge(node, this, angle);
                }
                else
                {
                    right.Node.Add(node);
                }
            }
            else
            {
                // word is proximal

                if (left == null)
                {
                    // add
                    left = new WordEdge(node, this, angle);
                }
                else if (left.Angle < angle)
                {
                    // insert
                    node.left = left;
                    var edge = new WordEdge(node, this, angle);
                    left = edge;
                }
                else
                {
                    left.Node.Add(node);
                }
            }
        }

        public static double GetAngle(SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            return Dot(vec1, vec2) / (Math.Sqrt(Dot(vec1, vec1)) * Math.Sqrt(Dot(vec2, vec2)));
        }

        public static long Dot(SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            long product = 0;
            var cursor1 = 0;
            var cursor2 = 0;

            while (cursor1 < vec1.Count && cursor2 < vec2.Count)
            {
                var p1 = vec1.Keys[cursor1];
                var p2 = vec2.Keys[cursor2];

                if (p2 > p1)
                {
                    cursor1++;
                    continue;
                }
                else if (p1 > p2)
                {
                    cursor2++;
                    continue;
                }

                product += vec1[p1] * vec2[p2];
                cursor1++;
                cursor2++;
            }
            return product;
        }

        public static SortedList<char, int> Subtract(SortedList<char, int> first, SortedList<char, int> other)
        {
            var result = new SortedList<char, int>();
            var cursor1 = 0;
            var cursor2 = 0;

            while (cursor1 < first.Count && cursor2 < other.Count)
            {
                var p1 = first.Keys[cursor1];
                var p2 = other.Keys[cursor2];

                if (p2 > p1)
                {
                    result[p1] = 0 - first[p1];
                    cursor1++;
                }
                else if (p1 > p2)
                {
                    result[p2] = 0 - other[p2];
                    cursor2++;
                }

                result[p1] = first[p1] - other[p2];
                cursor1++;
                cursor2++;
            }
            return result;
        }

        public static SortedList<char, int> Add(SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var result = new SortedList<char, int>();
            var cursor1 = 0;
            var cursor2 = 0;

            while (cursor1 < vec1.Count && cursor2 < vec2.Count)
            {
                var p1 = vec1.Keys[cursor1];
                var p2 = vec2.Keys[cursor2];

                if (p2 > p1)
                {
                    result[p1] = vec1[p1];
                    cursor1++;
                }
                else if (p1 > p2)
                {
                    result[p2] = vec2[p2];
                    cursor2++;
                }

                result[p1] = vec1[p1] + vec2[p2];
                cursor1++;
                cursor2++;
            }
            return result;
        }

        public static SortedList<char, int> ToSparseCharacterVector(string word)
        {
            var frequencies = new SortedList<char, int>();

            for (int i = 0; i < word.Length; i++)
            {
                var c = word[i];
                int f;
                if (frequencies.TryGetValue(c, out f))
                {
                    f++;
                }
                else
                {
                    f = 1;
                }
                frequencies[c] = f;
            }
            return frequencies;
        }

    }
}
