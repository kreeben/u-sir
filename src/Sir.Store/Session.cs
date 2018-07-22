﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Sir.Store
{
    public class Session : IDisposable
    {
        private readonly SortedList<ulong, uint> _keys;

        protected readonly string Dir;

        public ulong CollectionId { get; private set; }
        public SortedList<uint, VectorNode> Index { get; set; }
        public Stream ValueStream { get; set; }
        public Stream KeyStream { get; set; }
        public Stream DocStream { get; set; }
        public Stream ValueIndexStream { get; set; }
        public Stream KeyIndexStream { get; set; }
        public Stream DocIndexStream { get; set; }
        public Stream PostingsStream { get; set; }
        public Stream VectorStream { get; set; }
        public Stream IndexStream { get; set; }

        public Session(string directory, ulong collectionId)
        {
            Dir = directory;
            _keys = LoadKeyMap();
            CollectionId = collectionId;
        }

        public void AddKey(ulong keyHash, uint keyId)
        {
            _keys.Add(keyHash, keyId);
        }

        public uint GetKey(ulong keyHash)
        {
            return _keys[keyHash];
        }

        protected VectorNode GetKeyIndex(ulong key)
        {
            uint keyId;
            if(!TryGetKeyId(key, out keyId))
            {
                return null;
            }
            VectorNode root;
            if(!Index.TryGetValue(keyId, out root))
            {
                return null;
            }
            return root;
        }

        private bool TryGetKeyId(ulong key, out uint keyId)
        {
            if(!_keys.TryGetValue(key, out keyId))
            {
                keyId = 0;
                return false;
            }
            return true;
        }

        private SortedList<ulong, uint> LoadKeyMap()
        {
            var keys = new SortedList<ulong, uint>();

            using (var stream = new FileStream(Path.Combine(Dir, "_.kmap"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                uint i = 0;
                var buf = new byte[sizeof(uint)];
                var read = stream.Read(buf, 0, buf.Length);

                while (read > 0)
                {
                    keys.Add(BitConverter.ToUInt32(buf, 0), i++);

                    read = stream.Read(buf, 0, buf.Length);
                }
            }

            return keys;
        }

        public void Dispose()
        {
        }
    }
}
