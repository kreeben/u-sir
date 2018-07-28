﻿using System;
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
            var docIx = new DocIndexReader(DocIndexStream);
            var docs = new DocReader(DocStream);
            var keyIx = new ValueIndexReader(KeyIndexStream);
            var valIx = new ValueIndexReader(KeyIndexStream);
            var keyReader = new ValueReader(KeyStream);
            var valReader = new ValueReader(ValueStream);
            var postingsReader = new PostingsReader(PostingsStream);

            var keyHash = query.Term.Key.ToString().ToHash();
            var ix = GetIndex(keyHash);
            var match = ix.ClosestMatch(query.Term.Value.ToString());
            var docIds = postingsReader.Read(match.PostingsOffset, match.PostingsSize);

            foreach (var docId in docIds)
            {
                var docInfo = docIx.Read(docId);
                var docMap = docs.Read(docInfo.offset, docInfo.length);
                var keys = new IComparable[docMap.Count];
                var vals = new IComparable[docMap.Count];

                for (int i = 0; i < docMap.Count; i++)
                {
                    var kvp = docMap[i];
                    var kInfo = keyIx.Read(kvp.keyId);
                    var vInfo = valIx.Read(kvp.valId);
                    var key = keyReader.Read(kInfo.offset, kInfo.len, kInfo.dataType);
                    var val = valReader.Read(vInfo.offset, vInfo.len, vInfo.dataType);

                    keys[i] = key;
                    vals[i] = val;
                }

                yield return new Model(keys, vals);
            }
        }
    }
}
