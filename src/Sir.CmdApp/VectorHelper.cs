using System;
using System.Collections.Generic;

namespace Sir.Store
{
    public static class VectorHelper
    {
        private static SortedList<char, int> Root = new SortedList<char, int> { { '\0', 1 } };

        public static double CosAngle(this SortedList<char, double> vec1, SortedList<char, double> vec2)
        {
            var dotProduct = Dot(vec1, vec2);
            var dotSelf1 = Dot(vec1, vec1);
            var dotSelf2 = Dot(vec2, vec2);
            return dotProduct / (Math.Sqrt(dotSelf1) * Math.Sqrt(dotSelf2));
        }

        public static double Dot(this SortedList<char, double> vec1, SortedList<char, double> vec2)
        {
            double product = 0;
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

        public static double Dot(this double[] vec1, double[] vec2)
        {
            double product = 0;

            for (int i = 0; i < vec1.Length; i++)
            {
                product += vec1[i] * vec2[i];
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

        public static SortedList<char, double> ToVector(this string word)
        {
            if (word.Length == 0) throw new ArgumentException();

            var vec = word.ToCharVector();
            //var increment = 1;

            //vec[word[0]] += increment;
            //vec[word[word.Length - 1]] += increment;

            return vec;
            //var grams = string.Concat(word.ToTriGrams());
            //return grams.ToCharVector();
        }

        public static string[] ToTriGrams(this string word)
        {
            if (word.Length == 0) throw new ArgumentException();
            if (word.Length < 3) return new string[] { word };

            var result = new string[word.Length - 1];
            for (int i = 1; i < word.Length; i++)
            {
                result[i - 1] = new string(new[] { word[0], word[i] });
            }
            return result;
        }

        public static SortedList<char, double> ToCharVector(this string word)
        {
            var vec = new SortedList<char, double>();

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

        public static double Length(this SortedList<char, double> vector)
        {
            return Math.Sqrt(Dot(vector, vector));
        }

        public static SortedList<char, double> Components(this string s)
        {
            var word = new WordNode(s);
            var basevectors = new SortedList<char, SortedList<char, double>>();
            var i = 0;
            foreach (var c in word.WordVector.Keys)
            {
                var basevec = new SortedList<char, double>();
                basevec[c] = 1;
                basevectors[c] = basevec;
            }
            var components = new SortedList<char, double>();
            i = 0;
            foreach (var basevec in basevectors)
            {
                var angle = word.WordVector.CosAngle(basevec.Value);
                components[basevec.Key] = angle;
            }
            return components;
        }
    }
}
