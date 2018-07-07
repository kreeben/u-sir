using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sir.Store
{
    public class VectorNode
    {
        public const double TRUE_ANGLE = 0.9d;
        public const double FALSE_ANGLE = 0.5d;
        private VectorNode _right;
        private VectorNode _left;
        private long _vecOffset = -1;

        public SortedList<char, double> WordVector { get; private set; }
        public VectorNode Ancestor { get; set; }
        public VectorNode Right
        {
            get => _right;
            set
            {
                _right = value;
                _right.Ancestor = this;
            }
        }
        public VectorNode Left
        {
            get => _left;
            set
            {
                _left = value;
                _left.Ancestor = this;
            }
        }
        public double Angle { get; set; }
        public double Highscore { get; set; }

        public VectorNode() : this('\0'.ToString()) { }

        public VectorNode(string s) : this(s.ToVector())
        {
        }

        public VectorNode(SortedList<char, double> wordVector)
        {
            WordVector = wordVector;
        }

        private IEnumerable<byte[]> ToStream()
        {
            byte[] terminator = new byte[1];

            if (Left == null && Right == null)
            {
                terminator[0] = 3;
            }
            else if (Left == null)
            {
                terminator[0] = 2;
            }
            else if (Right == null)
            {
                terminator[0] = 1;
            }
            else
            {
                terminator[0] = 0;
            }

            yield return BitConverter.GetBytes(Angle);
            yield return BitConverter.GetBytes(_vecOffset);
            yield return BitConverter.GetBytes(WordVector.Count);
            yield return terminator;
        }

        public void Serialize(Stream treeStreem, Stream wordStream)
        {
            _vecOffset = WordVector.Serialize(wordStream);

            foreach(var buf in ToStream())
            {
                treeStreem.Write(buf, 0, buf.Length);
            }

            if (Left != null)
            {
                Left.Serialize(treeStreem, wordStream);
            }
            if (Right != null)
            {
                Right.Serialize(treeStreem, wordStream);
            }
        }

        public static VectorNode Deserialize(Stream treeStream, Stream vectorStream)
        {
            const int nodeSize = sizeof(double) + sizeof(long) + sizeof(int) + sizeof(byte);
            const int kvpSize = sizeof(char) + sizeof(double);

            var nodeBuf = new byte[nodeSize];
            var read = treeStream.Read(nodeBuf, 0, nodeBuf.Length);

            if (read < nodeSize)
            {
                throw new Exception("data is corrupt");
            }
            
            var angle = BitConverter.ToDouble(nodeBuf, 0);
            var offset = BitConverter.ToInt64(nodeBuf, sizeof(double));
            var listCount = BitConverter.ToInt32(nodeBuf, sizeof(double) + sizeof(long));
            var terminator = nodeBuf[nodeSize - 1];
            var vec = new SortedList<char, double>();
            var listBuf = new byte[listCount * kvpSize];

            vectorStream.Seek(offset, SeekOrigin.Begin);
            vectorStream.Read(listBuf, 0, listBuf.Length);

            var offs = 0;

            for (int i = 0; i < listCount; i++)
            {
                var key = BitConverter.ToChar(listBuf, offs);
                var val = BitConverter.ToDouble(listBuf, offs + sizeof(char));
                vec.Add(key, val);

                offs += kvpSize;
            }

            var node = new VectorNode(vec);
            node.Angle = angle;

            if (terminator == 0)
            {
                node.Left = Deserialize(treeStream, vectorStream);
                node.Right = Deserialize(treeStream, vectorStream);
            }
            else if (terminator == 1)
            {
                node.Left = Deserialize(treeStream, vectorStream);
            }
            else if (terminator == 2)
            {
                node.Right = Deserialize(treeStream, vectorStream);
            }

            return node;
        }

        public int Depth()
        {
            var count = 0;
            var node = Left;
            while (node != null)
            {
                count++;
                node = node.Left;
            }
            return count;
        }

        public VectorNode ClosestMatch(VectorNode node)
        {
            var best = this;
            var cursor = this;
            double highscore = 0;

            while (cursor != null)
            {
                var angle = cursor.WordVector.CosAngle(node.WordVector);
                if (angle >= TRUE_ANGLE)
                {
                    cursor.Highscore = angle;
                    return cursor;
                }
                else if (angle > FALSE_ANGLE)
                {
                    if (angle > highscore)
                    {
                        highscore = angle;
                        best = cursor;
                    }
                    cursor = cursor.Left;
                }
                else
                {
                    cursor = cursor.Right;
                }
            }

            best.Highscore = highscore;
            return best;
        }

        public bool Add(VectorNode node)
        {
            node.Angle = node.WordVector.CosAngle(WordVector);

            if (node.Angle >= TRUE_ANGLE)
            {
                Merge(node);
                return false;
            }
            else if (node.Angle <= FALSE_ANGLE)
            {
                if (Right == null)
                {
                    Right = node;
                    return true;
                }
                else
                {
                    return Right.ClosestMatch(node).Add(node);
                }
            }
            else
            {
                if (Left == null)
                {
                    Left = node;
                    return true;
                }
                else
                {
                    return Left.ClosestMatch(node).Add(node);
                }
            }
        }

        public VectorNode GetRoot()
        {
            var cursor = this;
            while (cursor != null)
            {
                if (cursor.Ancestor == null) break;
                cursor = cursor.Ancestor;
            }
            return cursor;
        }



        public void Merge(VectorNode node)
        {
            //WordVector = WordVector.Add(node.WordVector);
            //Angle = WordVector.CosAngle(Ancestor.WordVector);
        }

        public string Visualize()
        {
            StringBuilder output = new StringBuilder();
            Visualize(this, output, 0);
            return output.ToString();
        }

        private void Visualize(VectorNode node, StringBuilder output, int depth)
        {
            if (node == null) return;

            double angle = 0;

            if (node.Ancestor != null)
            {
                angle = node.Angle;
            }

            output.Append('\t', depth);
            output.AppendFormat(".{0} ({1})", node.ToString(), angle);
            output.AppendLine();

            if (node.Left != null)
                Visualize(node.Left, output, depth + 1);

            if (node.Right != null)
                Visualize(node.Right, output, depth);
        }

        public override string ToString()
        {
            var w = new StringBuilder();
            foreach (var c in WordVector)
            {
                w.Append(c.Key);
            }
            return w.ToString();
        }
    }
}
