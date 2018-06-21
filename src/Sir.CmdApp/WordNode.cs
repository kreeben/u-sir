using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sir.Store
{
    public class WordNode
    {
        public const double TRUE_ANGLE = 0.9d;
        public const double FALSE_ANGLE = 0.5d;
        public SortedList<char, int> WordVector { get; private set; }

        public WordNode Ancestor { get; set; }
        public WordNode Right { get; set; }
        public WordNode Left { get; set; }
        public double Angle { get; set; }
        public double Highscore { get; set; }

        private double highscore;

        public WordNode(string s):this(s.ToVector())
        {
        }

        public WordNode(SortedList<char, int> wordVector)
        {
            this.WordVector = wordVector;
        }
        
        public WordNode ClosestMatch(WordNode node)
        {
            var winner = this;
            var cursor = this;
            double highscore = 0;

            while (cursor != null)
            {
                var angle = node.WordVector.CosAngle(cursor.WordVector);
                if (angle >= TRUE_ANGLE)
                {
                    return cursor;
                }
                else if (angle > FALSE_ANGLE)
                {
                    if (angle > highscore)
                    {
                        highscore = angle;
                        winner = cursor;
                    }
                    cursor = cursor.Left;
                } 
                else
                {
                    cursor = cursor.Right;
                }
            }

            winner.Highscore = highscore;
            return winner;
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
