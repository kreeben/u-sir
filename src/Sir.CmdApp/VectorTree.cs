using System;
using System.IO;

namespace Sir.Store
{
    public class VectorTree
    {
        public VectorNode Root { get; private set; }

        public int Count { get; private set; }
        public int MergeCount { get; private set; }

        public VectorTree() : this(new VectorNode('\0'.ToString())) { }

        public VectorTree(VectorNode root)
        {
            Root = root;
        }

        public (int depth, int width) Size()
        {
            if (Root == null) return (0, 0);

            var width = 0;
            var depth = 0;
            var node = Root.Right;

            while (node != null)
            {
                var d = node.Depth();
                if (d > depth)
                {
                    depth = d;
                }
                width++;
                node = node.Right;
            }

            return (depth, width);
        }

        public VectorNode Find(string word)
        {
            return Root.ClosestMatch(new VectorNode(word));
        }

        public void Add(string word)
        {
            var node = new VectorNode(word);
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
            if (Root != null)
            {
                return Root.Visualize();
            }
            return string.Empty;
        }

        public void Serialize(Stream treeStream, Stream wordStream)
        {
            if (Root != null)
            {
                Root.Serialize(treeStream, wordStream);
            }
        }

        public static VectorTree Load(Stream treeStream, Stream wordStream)
        {
            var root = VectorNode.Deserialize(treeStream, wordStream);
            return new VectorTree(root);
        }

    }
}
