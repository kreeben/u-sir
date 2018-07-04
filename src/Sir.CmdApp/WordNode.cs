using System.Collections.Generic;
using System.Text;

namespace Sir.Store
{
    public class WordNode
    {
        public const double TRUE_ANGLE = 0.9d;
        public const double FALSE_ANGLE = 0.45d;
        public SortedList<char, double> WordVector { get; private set; }
        public WordNode Ancestor { get; set; }
        public WordNode Right { get; set; }
        public WordNode Left { get; set; }
        public double Angle { get; set; }
        public double Highscore { get; set; }

        public WordNode(string s):this(s.ToVector())
        {
        }

        public WordNode(SortedList<char, double> wordVector)
        {
            this.WordVector = wordVector;
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
        
        public WordNode ClosestMatch(WordNode node)
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

        public bool Add(WordNode node)
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
                    Right.Ancestor = this;
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
                    node.Ancestor = this;
                    return true;
                }
                else
                {
                    return Left.ClosestMatch(node).Add(node);
                }
            }
        }

        public WordNode GetRoot()
        {
            var cursor = this;
            while (cursor != null)
            {
                if (cursor.Ancestor == null) break;
                cursor = cursor.Ancestor;
            }
            return cursor;
        }



        public void Merge(WordNode node)
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

        private void Visualize(WordNode node, StringBuilder output, int depth)
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
