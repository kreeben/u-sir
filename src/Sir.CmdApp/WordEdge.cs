using System.Text;

namespace Sir.CmdApp
{
    public class WordEdge
    {
        public double Angle { get; set; }
        public WordNode Node { get; private set; }
        public WordNode Parent { get; private set; }

        public WordEdge(WordNode node, WordNode parent, double angle)
        {
            Angle = angle;
            Node = node;
            Parent = parent;

            node.EdgeToParent = this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Node);
            var x = this;
            while (x != null)
            {
                sb.AppendFormat(" {0} {1}", x.Angle, x.Parent);
                x = x.Parent.EdgeToParent;
            }
            return sb.ToString();
        }
    }
}
