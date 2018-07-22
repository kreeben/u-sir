using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class WriteSession : Session
    {
        public WriteSession(string directory, ulong collectionId) : base(directory, collectionId)
        {
        }

        public void Write(IEnumerable<IModel> data, ITokenizer tokenizer)
        {
            var vals = new ValueWriter(ValueStream);
            var keys = new ValueWriter(KeyStream);
            var docs = new DocWriter(DocStream);
            var valIx = new ValueIndexWriter(ValueIndexStream);
            var keyIx = new ValueIndexWriter(KeyIndexStream);
            var docIx = new DocIndexWriter(DocIndexStream);

            foreach (var model in data)
            {
                var docId = docIx.GetNextDocId();

                var docMapping = new List<(uint keyId, uint valId)>();

                for (int i = 0; i < model.Keys.Length; i++)
                {
                    var key = model.Keys[i];
                    var keyHash = key.ToHash();
                    var fieldIndex = GetKeyIndex(keyHash);
                    var val = model.Values[i];
                    var str = val as string;
                    var terms = new List<Term>();
                    uint keyId, valId;

                    if (str == null)
                    {
                        throw new NotImplementedException("Data type is not supported yet.");
                    }
                    else
                    {
                        foreach (var term in tokenizer.Tokenize(str))
                        {
                            terms.Add(new Term(key, term, CollectionId));
                        }
                    }

                    if (fieldIndex == null)
                    {
                        // We have a new key!

                        // store key
                        var keyInfo = keys.Append(key);
                        keyId = keyIx.Append(keyInfo.offset, keyInfo.len, keyInfo.dataType);
                        _keys.Add(keyHash, keyId);

                        // add new index to global in-memory tree
                        fieldIndex = new VectorNode();
                        Index.Add(keyId, fieldIndex);
                    }
                    else
                    {
                        keyId = _keys[keyHash];
                    }

                    foreach (var term in terms)
                    {
                        var match = fieldIndex.ClosestMatch((string)term.Value);

                        if (match.Highscore < VectorNode.IdenticalAngle)
                        {
                            // We have a new unique value!

                            // store value
                            var valInfo = vals.Append(term.Value);
                            valId = valIx.Append(valInfo.offset, valInfo.len, valInfo.dataType);
                        }
                        else
                        {
                            valId = match.ValueId;
                        }

                        // add term and posting to index
                        fieldIndex.Add((string)term.Value, docId, valId);
                    }
                }

                var docMeta = docs.Append(docMapping);
                var confirmedDocId = docIx.Append(docMeta.offset, docMeta.length);

                if (confirmedDocId != docId)
                {
                    throw new InvalidDataException();
                }
            }
        }

    }
}
