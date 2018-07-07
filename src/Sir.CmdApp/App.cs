using Sir.Store;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Sir.CmdApp
{
    public class App
    {
        public void ClearFiles(string[] input, VectorTree tree)
        {
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bin"))
            {
                File.Delete(file);
            }
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.prs"))
            {
                File.Delete(file);
            }
        }

        public void Analyze(string[] input, VectorTree tree)
        {
            var node = tree.Find(string.Join(" ", input.Skip(1).ToArray()));
            if (node != null)
            {
                foreach (var x in node.WordVector)
                {
                    Console.WriteLine("{0}:{1}", x.Key, x.Value);
                }
            }
        }

        public void Find(string[] input, VectorTree tree)
        {
            var q = string.Join("", input.Skip(1).ToArray());
            var result = tree.Find(q);

            if (result == null)
            {
                Console.WriteLine("{0} not found", q);
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

        public void AddFiles(string[] input, VectorTree tree)
        {
            ClearFiles(input, tree);
            foreach (var file in Directory.GetFiles(input[1], input[2]))
            {
                var n = AddFile(file, tree);
                Console.Write("{0} {1} ", file, n);
            }
            Console.WriteLine(tree.Visualize());
            var size = tree.Size();
            Console.WriteLine();
            Console.WriteLine("depth {0} width {1} count: {2}, merges: {3}", size.depth, size.width, tree.Count, tree.MergeCount);

            using (var treeStream = File.Create("tree.bin"))
            using (var wordStream = File.Create("word.bin"))
            {
                tree.Serialize(treeStream, wordStream);
            }

            //using (var treeStream = File.OpenRead("tree.bin"))
            //using (var wordStream = File.OpenRead("word.bin"))
            //{
            //    var deserialized = VectorTree.Load(treeStream, wordStream);

            //    Console.WriteLine(deserialized.Visualize());
            //    var deserializedSize = deserialized.Size();
            //    Console.WriteLine("depth {0} width {1}", deserializedSize.depth, deserializedSize.width);
            //}
        }

        private int AddFile(string path, VectorTree tree)
        {
            var text = GetLocalResource(path);
            var tokens = Tokenize(text);
            return AddInternal(tokens, tree);
        }

        public void AddWebPage(string[] input, VectorTree tree)
        {
            AddWebPage(input[1], tree);
            Console.WriteLine(tree.Visualize());
            Console.WriteLine("count: {0}", tree.Count);
            Console.WriteLine("merges: {0}", tree.MergeCount);
            var size = tree.Size();
            Console.WriteLine("depth {0} width {1}", size.depth, size.width);
        }

        private void AddWebPage(string url, VectorTree tree)
        {
            var text = GetWebResource(url);
            AddInternal(Tokenize(text), tree);
            Console.WriteLine(tree.Visualize());
            var size = tree.Size();
            Console.WriteLine();
            Console.WriteLine("depth {0} width {1}", size.depth, size.width);
        }

        public void Add(string[]input, VectorTree tree)
        {
            Add(input.Skip(1).ToArray(), tree);
            Console.WriteLine(tree.Visualize());
            var size = tree.Size();
            Console.WriteLine();
            Console.WriteLine("depth {0} width {1}", size.depth, size.width);
        }

        private int AddInternal(string[] input, VectorTree tree)
        {
            var dic = new SortedList<double, string>();
            var count = tree.Count;

            foreach (var word in input)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                tree.Add(word);

                var wordvec = new VectorNode(word).WordVector;

                if (wordvec.CosAngle(tree.Find(word).WordVector) < VectorNode.TRUE_ANGLE)
                {
                    throw new Exception("error");
                }

                //Console.Write("{0} [", word);
                //foreach (var c in word.Components())
                //{
                //    Console.Write("{0}:{1}, ", c.Key, c.Value);
                //}
                //Console.WriteLine("]");
            }

            return tree.Count - count;
        }

        public void Tokenize(string[] input, VectorTree tree)
        {
            var tokens = Tokenize(string.Join("", input.Skip(1).ToArray()));
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        private string[] Tokenize(string dirtyText)
        {
            var decoded = WebUtility.HtmlDecode(dirtyText);
            var parser = new YesNoParser.YesNoParser('>', '<', new[] { "script" });
            var parsed = parser.Parse(decoded).ToLower(CultureInfo.CurrentCulture);
            File.WriteAllText(Guid.NewGuid() + ".prs", parsed);
            //return parsed.Split(
            //    new char[] { ' ', '(', ')', '[', ']', '{', '}', '.', ',', ';', ':', '/', '\\', '*', '"', '\'',
            //        '?', '-', '+', '_', '^', '<', '>', '|', '=', '&', '\r', '\n', '\t'},
            //    StringSplitOptions.RemoveEmptyEntries);
            return parsed.Split(new char[] { '.', ',', ';', ':', '?',
                                             '\n', '\r', '\t', '(', ')', '[', ']',
                                             '"', '\'', '`', '´',
                                             '-'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void GetWebResource(string[] input, VectorTree tree)
        {
            GetWebResource(input[1]);
        }

        private string GetWebResource(string url)
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

        static string GetLocalResource(string fileName)
        {
            var fn = fileName.Replace("file://", "");
            return File.ReadAllText(fn);
        }

        public void Compare(string[]input, VectorTree tree)
        {
            var result = Compare(input[1], input[2]);
            Console.WriteLine("angle: {0} len1: {1}, len2: {2}", result.angle, result.len1, result.len2);

        }

        private (double angle, double len1, double len2) Compare(string v1, string v2)
        {
            var a = v1.ToVector();
            var b = v2.ToVector();

            return (a.CosAngle(b), Math.Sqrt(a.Dot(a)), Math.Sqrt(b.Dot(b)));
        }
    }
}
