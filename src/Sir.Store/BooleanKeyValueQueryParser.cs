namespace Sir.Store
{
    public class BooleanKeyValueQueryParser : IQueryParser
    {
        public string ContentType => "text/plain";
        public int Ordinal => 0;

        public void Dispose()
        {
        }

        public Query Parse(string query)
        {
            var tokens = query.Split(new[] { ' ', ':' });
            if (tokens.Length > 1)
            {
                var key = tokens[0];
                var val = tokens[1];
                var q = new Query { Term = new Term { Key = key, Value = val } };
                Parse(q, tokens);
                return q;
            }
            return null;
        }

        private void Parse(Query query, string[] tokens)
        {
            for (int i = 2; i < tokens.Length; i++)
            {
                var key = tokens[i];
                var val = tokens[++i];
                var next = new Query
                {
                    Term = new Term { Key = key, Value = val }
                };

                if (next.Term.Value.StartsWith('-'))
                {
                    next.Term.Value = next.Term.Value.Substring(1, next.Term.Value.Length - 1);
                    query.Not = next;
                }
                else if (next.Term.Value.StartsWith('+'))
                {
                    next.Term.Value = next.Term.Value.Substring(1, next.Term.Value.Length - 1);
                    query.And = next;
                }
                else
                {
                    query.Or = next;
                }

                query = next;
            }
        }
    }
}
