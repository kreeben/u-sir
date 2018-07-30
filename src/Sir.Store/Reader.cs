using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sir.Store
{
    public class Reader : IReader
    {
        public string ContentType => "*";

        private readonly SessionFactory _sessionFactory;
        private readonly ITokenizer _tokenizer;

        public Reader(SessionFactory sessionFactory, ITokenizer analyzer)
        {
            _tokenizer = analyzer;
            _sessionFactory = sessionFactory;
        }

        public void Dispose()
        {
        }

        public IEnumerable<IModel> Read(Query query)
        {
            ulong keyHash = query.Term.Key.ToString().ToHash();
            uint keyId;

            if (_sessionFactory.TryGetKeyId(keyHash, out keyId))
            {
                using (var session = _sessionFactory.CreateReadSession(query.CollectionId))
                {
                    foreach(var model in session.Read(query))
                    {
                        yield return model;
                    }
                }
            }
        }

        public void Render(IModelFormatter modelFormatter, Query query, Stream output)
        {
            ulong keyHash = query.Term.Key.ToString().ToHash();
            uint keyId;

            if (_sessionFactory.TryGetKeyId(keyHash, out keyId))
            {
                query.Term.KeyId = keyId;

                using (var session = _sessionFactory.CreateReadSession(query.CollectionId))
                {
                    var unformatted = session.Read(query);

                    modelFormatter.Format(unformatted, output);
                }
            }
        }
    }
}
