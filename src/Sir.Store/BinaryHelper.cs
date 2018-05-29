using System;
using System.Linq;

namespace Sir.Store
{
    public static class BinaryHelper
    {
        public static ulong To64BitTerm(IComparable key, IComparable value)
        {
            if (value is decimal)
            {
                throw new NotSupportedException("128-bit words not allowed");
            }

            return Dot(ConvertToString(key).ToArray(), ConvertToString(value).ToArray());
        }

        private static string ConvertToString(IComparable comparable)
        {
            if (comparable is string)
            {
                return (string)comparable;
            }
            else if (comparable is DateTime)
            {
                return ((DateTime)comparable).ToBinary().ToString();
            }
            else
            {
                return comparable.ToString();
            }
        }

        private static ulong Dot(char[] sparse1, char[] sparse2)
        {
            char[] longest, shortest;
            ulong result = 0;

            if (sparse1.Length > sparse2.Length)
            {
                longest = sparse1;
                shortest = sparse2;
            }
            else
            {
                longest = sparse2;
                shortest = sparse1;
            }
            
            for (int i = 0; i < longest.Length; i++)
            {
                var x = longest[i];
                var y = shortest.Length <= i ? char.MinValue : shortest[i];
                result += (uint)x * y;
            }

            return result;
        }
    }
}

