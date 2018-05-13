using System;

namespace Sir.Store
{
    public static class Hash
    {
        /// <summary>
        /// Knuth hash. http://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
        /// </summary>
        public static ulong ToHash(string text)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            for (int i = 0; i < text.Length; i++)
            {
                hashedValue += text[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/5569545/how-to-generate-a-unique-hash-code-for-an-object-based-on-its-contents
        /// </summary>
        public static ulong ToHash(DateTime when)
        {
            ulong kind = (ulong)(int)when.Kind;
            return (kind << 62) | (ulong)when.Ticks;
        }

        public static long ToHash(long first, long second)
        {
            long hash = 17;
            hash = hash * 23 + first;
            hash = hash * 23 + second;
            return hash;
        }
    }
}
