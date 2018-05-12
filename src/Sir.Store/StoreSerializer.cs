using System;
using System.IO;

namespace Sir.Store
{
    public static class StoreSerializer
    {
        public static (long offset, int len, byte dataType) Serialize(IComparable value, Stream stream)
        {
            byte[] buffer;
            byte dataType = 0;

            if (value is int)
            {
                buffer = BitConverter.GetBytes((int)value);
                dataType = DataType.INT32;
            }
            else if (value is long)
            {
                buffer = BitConverter.GetBytes((long)value);
                dataType = DataType.INT64;
            }
            else if (value is DateTime)
            {
                buffer = BitConverter.GetBytes(((DateTime)value).ToBinary());
                dataType = DataType.DATETIME;
            }
            else if (value is float)
            {
                buffer = BitConverter.GetBytes((float)value);
                dataType = DataType.FLOAT;
            }
            else
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(value.ToString());
                dataType = DataType.UNKNOWN;
            }

            var offset = stream.Position;
            stream.Write(buffer, 0, buffer.Length);
            return (offset, buffer.Length, dataType);
        }
    }
}
