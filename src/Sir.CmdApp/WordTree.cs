using System;
using System.IO;

namespace Sir.Store
{
    public class WordTree : IDisposable
    {
        private readonly Stream stream;
        public WordNode Root { get; private set; }

        public int Count { get; private set; }
        public int MergeCount { get; private set; }

        public WordTree(Stream stream)
        {
            this.stream = stream;
            Root = new WordNode('\0'.ToString());
        }

        public WordTree()
        {
            Root = new WordNode('\0'.ToString());
        }

        public WordNode Find(string word)
        {
            return Root.ClosestMatch(new WordNode(word));
        }

        public void Add(string word)
        {
            var node = new WordNode(word);
            if (Root.ClosestMatch(node).Add(node))
            {
                Count++;
            }
            else
            {
                MergeCount++;
            }
        }

        public string Visualize()
        {
            return Root.Visualize();
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
            }
        }
    }
}
