namespace Sir.CmdApp
{
    public class WordEdge
    {
        public double Angle { get; private set; }
        public WordNode Node { get; private set; }
        public WordNode Parent { get; private set; }

        public WordEdge(WordNode node, WordNode parent, double angle)
        {
            Angle = angle;
            Node = node;
            Parent = parent;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} angle to {2}", Node, Angle, Parent);
        }
    }
}
