﻿using System;
using System.IO;

namespace Sir.Store
{
    public static class StoreSerializer
    {
        public static (long offset, int len, byte dataType) Serialize(IComparable value, Stream stream)
        {
            byte[] buffer;
            byte dataType = 0;

            if (value is bool)
            {
                buffer = BitConverter.GetBytes((bool)value);
                dataType = DataType.BOOL;
            }
            else if (value is char)
            {
                buffer = BitConverter.GetBytes((char)value);
                dataType = DataType.CHAR;
            }
            else if (value is float)
            {
                buffer = BitConverter.GetBytes((float)value);
                dataType = DataType.FLOAT;
            }
            else if (value is int)
            {
                buffer = BitConverter.GetBytes((int)value);
                dataType = DataType.INT;
            }
            else if (value is double)
            {
                buffer = BitConverter.GetBytes((double)value);
                dataType = DataType.DOUBLE;
            }
            else if (value is long)
            {
                buffer = BitConverter.GetBytes((long)value);
                dataType = DataType.LONG;
            }
            else if (value is DateTime)
            {
                buffer = BitConverter.GetBytes(((DateTime)value).ToBinary());
                dataType = DataType.DATETIME;
            }
            else
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(value.ToString());
                dataType = DataType.STRING;
            }

            var offset = stream.Position;
            stream.Write(buffer, 0, buffer.Length);
            return (offset, buffer.Length, dataType);
        }
    }
}
