using System;

namespace Sir.Store
{
    public static class BinaryHelper
    {
        public static uint ToBinary(this IComparable value)
        {
            if (value is string)
            {
                return ((string)value).ToLowerInvariant().ToDotProductOfSelf();
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToDotProductOfSelf();
            }
            else
            {
                return Convert.ToUInt32(value);
            }
        }

        public static uint ToDotProductOfSelf(this string text)
        {
            uint result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                result += (uint)text[i] * text[i];
            }
            return result;
        }

        public static uint ToDotProductOfSelf(this DateTime when)
        {
            return ((uint)when.Kind*(uint)when.Ticks);
        }
    }
}
