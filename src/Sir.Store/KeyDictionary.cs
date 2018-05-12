using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class KeyDictionary
    {
        private readonly Stream _writeStream;
        private readonly IDictionary<string, int> _keys;
        private readonly IList<string> _list;

        public static KeyDictionary Create(Stream readStream, Stream writeStream)
        {
            var keys = new List<string>();
            while (true)
            {
                var keyLenStream = new byte[sizeof(int)];
                var read = readStream.Read(keyLenStream, 0, sizeof(int));

                if (read == 0) break;

                var keyLen = BitConverter.ToInt32(keyLenStream, 0);
                var keyStream = new byte[keyLen];
                readStream.Read(keyStream, 0, keyLen);

                var key = System.Text.Encoding.Unicode.GetString(keyStream);
                keys.Add(key);
            }
            return new KeyDictionary(keys, writeStream);
        }
        
        public KeyDictionary(IList<string> keys, Stream writeStream)
        {
            _writeStream = writeStream;
            _keys = new Dictionary<string, int>();
            _list = keys;

            for (int i = 0; i< keys.Count; i++)
            {
                _keys.Add(keys[i], i);
            }
        }

        public int Add(string key)
        {
            int id;

            if (!_keys.TryGetValue(key, out id))
            {
                id = _keys.Count;
                _keys.Add(key, id);
                _list.Add(key);

                var buffer = System.Text.Encoding.Unicode.GetBytes(key);

                _writeStream.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
                _writeStream.Write(buffer, 0, buffer.Length);
            }

            return id;
        }

        public string Get(int id)
        {
            if (id > _list.Count - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return _list[id];
        }


    }
}
