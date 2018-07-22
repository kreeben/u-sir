using System;
using System.Collections.Generic;

namespace Sir.Store
{
    public class ReadSession : Session
    {
        public ReadSession(string directory, ulong collectionId, SessionFactory sessionFactory) 
            : base(directory, collectionId, sessionFactory)
        {
        }

        public IEnumerable<IModel> Read(Query query)
        {
            ulong keyHash = query.Term.Key.ToString().ToHash();
            var ix = GetIndex(keyHash);
            var match = ix.ClosestMatch(query.Term.Value.ToString());
            var docIndexReader = new DocIndexReader(DocIndexStream);
            var docReader = new DocReader(DocStream);
        }
    }
}
