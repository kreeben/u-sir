using System;
using System.Collections.Generic;

namespace Sir.Store
{
    public static class VectorHelper
    {
        private static SortedList<char, int> Root = new SortedList<char, int> { { '\0', 1 } };

        public static double CosAngle(this SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var dotboth = Dot(vec1, vec2);
            var dot1 = Dot(vec1, vec1);
            var len1 = Math.Sqrt(dot1);
            var dot2 = Dot(vec2, vec2);
            var len2 = Math.Sqrt(dot2);
            return dotboth / (len1 * len2);
        }

        public static long Dot(this SortedList<char, int> vec1, SortedList<char, int> vec2)
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

        public static SortedList<char, int> Add(this SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var result = new SortedList<char, int>();
            foreach (var x in vec1)
            {
                int val;
                if (vec2.TryGetValue(x.Key, out val))
                {
                    result[x.Key] = val + x.Value;
                }
                else
                {
                    result[x.Key] = x.Value;
                }
            }
            foreach (var x in vec2)
            {
                if (!vec1.ContainsKey(x.Key))
                {
                    result[x.Key] = x.Value;
                }
            }
            return result;
        }

        public static SortedList<char, int> Subtract(this SortedList<char, int> vec1, SortedList<char, int> vec2)
        {
            var result = new SortedList<char, int>();
            foreach (var x in vec1)
            {
                if (vec2.ContainsKey(x.Key))
                {
                    result[x.Key] = x.Value - vec2[x.Key];
                }
                else
                {
                    result[x.Key] = x.Value;
                }
            }
            foreach (var x in vec2)
            {
                if (!vec1.ContainsKey(x.Key))
                {
                    result[x.Key] = 0 - x.Value;
                }
            }
            return result;
        }

        public static SortedList<char, int> ToVector(this string word)
        {
            if (word.Length == 0) throw new ArgumentException();
            //if (word.Length > 0 && word.Length < 4) return word.ToCharVector();

            return word.ToCharVector();

            //var tgs = word.ToTriGrams();
            //var first = tgs[0];
            //var result = first.ToCharVector();
            //for(int i = 1; i < tgs.Length; i++)
            //{
            //    var second = tgs[i];
            //    var v = second.ToCharVector();
            //    result = result.Add(v);
            //}
            //return result;
        }

        public static string[] ToTriGrams(this string word)
        {
            if (word.Length == 0) throw new ArgumentException();
            if (word.Length > 0 && word.Length < 4) return new string[] { word };

            var result = new string[word.Length-2];
            for(int i = 0; i < word.Length - 2; i++)
            {
                result[i] = word.Substring(i, 3);
            }
            return result;
        }

        public static SortedList<char, int> ToCharVector(this string word)
        {
            var vec = new SortedList<char, int>();

            for (int i = 0; i < word.Length; i++)
            {
                var c = word[i];
                if (vec.ContainsKey(c))
                {
                    vec[c] += 1;
                }
                else
                {
                    vec[c] = 1;
                }
            }
            return vec;
        }

        public static double Length(this SortedList<char, int> vector)
        {
            return Math.Sqrt(Dot(vector, vector));
        }
    }
}
