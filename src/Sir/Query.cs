namespace Sir
{
    public class Query
    {
        public Query And { get; set; }
        public Query Or { get; set; }
        public Query Not { get; set; }
        public Term Term { get; set; }
    }
}
