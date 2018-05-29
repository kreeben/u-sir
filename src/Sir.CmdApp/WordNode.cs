using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sir.CmdApp
{
    public class WordNode
    {
        private WordEdge right;
        private WordEdge left;

        private SortedList<char, int> wordVector;

        public WordNode(string s)
        {
            wordVector = ToSparseCharacterVector(s);
        }

        public WordNode(SortedList<char, int> wordVector)
        {
            this.wordVector = wordVector;
        }

        public WordNode FindClosestTangent(WordNode node)
        {
            var angle = GetAngle(wordVector, node.wordVector);

            if (angle == 0)
            {
                if (left != null)
                {
                    return left.Node.FindClosestTangent(node);
                }
                else if (right != null)
                {
                    return right.Node.FindClosestTangent(node);
                }
            }
            return this;
        }

        public void Add(WordNode node)
        {
            var tangent = FindClosestTangent(node);
            var angle = GetAngle(node.wordVector, tangent.wordVector);

            if (angle == 0)
            {
                if (tangent.right == null)
                {
                    // insert
                    tangent.right = new WordEdge(node, this, angle);
                }
                else
                {
                    tangent.right.Node.Add(node);
                }
            }
            else
            {
                if (tangent.left == null)
                {
                    // insert
                    tangent.left = new WordEdge(node, tangent, angle);
                }
                else
                {
                    // v1+v2
                    var resultant = Add(node.wordVector, tangent.left.Node.wordVector);
                    var a = GetAngle(tangent.wordVector, resultant);
                    tangent.left = new WordEdge(new WordNode(resultant), tangent, a);
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
                }
                else if (p1 > p2)
                {
                    cursor2++;
                }
                else
                {
                    product += vec1[p1] * vec2[p2];
                    cursor1++;
                    cursor2++;
                }
            }
            return product;
        }

        public static SortedList<char, int> Subtract(SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var result = new SortedList<char, int>();
            var cursor1 = 0;
            var cursor2 = 0;

            while (cursor1 < vec1.Count || cursor2 < vec2.Count)
            {
                char? p1 = null;
                char? p2 = null;

                if (cursor1 < vec1.Count)
                {
                    p1 = vec1.Keys[cursor1];
                }
                if (cursor2 < vec2.Count)
                {
                    p2 = vec2.Keys[cursor2];
                }

                if ((p2.HasValue && p1.HasValue && p2.Value > p1.Value) || (p2.HasValue == false && p1.HasValue))
                {
                    result[p1.Value] = 0 - vec1[p1.Value];
                    cursor1++;
                }
                else if ((p2.HasValue && p1.HasValue && p2.Value < p1.Value) || (p1.HasValue == false && p2.HasValue))
                {
                    result[p2.Value] = 0 - vec2[p2.Value];
                    cursor2++;
                }
                else
                {
                    result[p1.Value] = vec1[p1.Value] - vec2[p2.Value];
                    cursor1++;
                    cursor2++;
                }
            }
            return result;
        }

        public static SortedList<char, int> Add(SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var result = new SortedList<char, int>();
            var cursor1 = 0;
            var cursor2 = 0;

            while (cursor1 < vec1.Count || cursor2 < vec2.Count)
            {
                char? p1 = null;
                char? p2 = null;

                if (cursor1 < vec1.Count)
                {
                    p1 = vec1.Keys[cursor1];
                }
                if (cursor2 < vec2.Count)
                {
                    p2 = vec2.Keys[cursor2];
                }

                if ((p2.HasValue && p1.HasValue && p2.Value > p1.Value) || (p2.HasValue == false && p1.HasValue))
                {
                    result[p1.Value] = vec1[p1.Value];
                    cursor1++;
                }
                else if ((p2.HasValue && p1.HasValue && p2.Value < p1.Value) || (p1.HasValue == false && p2.HasValue))
                {
                    result[p2.Value] = vec2[p2.Value];
                    cursor2++;
                }
                else
                {
                    result[p1.Value] = vec1[p1.Value] + vec2[p2.Value];
                    cursor1++;
                    cursor2++;
                }
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
    }
}
