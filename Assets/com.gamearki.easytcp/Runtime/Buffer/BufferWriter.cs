using System;
using System.Text;

namespace ZeroFrame.Buffer
{

    public static class BufferWriter
    {

        public static void WriteInt64Array(byte[] src, Int64[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteInt64(src, v, ref offset);
            }
        }

        public static void WriteInt64(byte[] src, Int64 value, ref int offset)
        {
            for (int i = 0; i < 8; i++)
            {
                src[offset++] = (byte)(value >> ((7 - i) * 8));
            }
        }

        public static void WriteUInt64Array(byte[] src, UInt64[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteUInt64(src, v, ref offset);
            }
        }

        public static void WriteUInt64(byte[] src, UInt64 value, ref int offset)
        {
            for (int i = 0; i < 8; i++)
            {
                src[offset++] = (byte)(value >> ((7 - i) * 8));
            }
        }

        public static void WriteInt32Array(byte[] src, Int32[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteInt32(src, v, ref offset);
            }
        }

        public static void WriteInt32(byte[] src, Int32 value, ref int offset)
        {
            for (int i = 0; i < 4; i++)
            {
                src[offset++] = (byte)(value >> ((3 - i) * 8));
            }
        }

        public static void WriteUInt32Array(byte[] src, UInt32[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteUInt32(src, v, ref offset);
            }
        }

        public static void WriteUInt32(byte[] src, UInt32 value, ref int offset)
        {
            for (int i = 0; i < 4; i++)
            {
                src[offset++] = (byte)(value >> ((3 - i) * 8));
            }
        }

        public static void WriteInt16Array(byte[] src, Int16[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteInt16(src, v, ref offset);
            }
        }

        public static void WriteInt16(byte[] src, Int16 value, ref int offset)
        {
            for (int i = 0; i < 2; i++)
            {
                src[offset++] = (byte)(value >> ((1 - i) * 8));
            }
        }

        public static void WriteUTF8StringArray(byte[] src, string[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteUTF8String(src, v, ref offset);
            }
        }

        public static void WriteUTF8String(byte[] src, string value, ref int offset)
        {
            //1.写入字符串字节长度（ushort）
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            ushort len = (ushort)strBytes.Length;
            WriteUInt16(src, len, ref offset);
            //2.存入真正的字符串字节
            for (int i = 0; i < strBytes.Length; i++)
            {
                src[offset] = strBytes[i];
                offset++;
            }
        }

        public static void WriteCharArray(byte[] src, char[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteChar(src, v, ref offset);
            }
        }

        public static void WriteChar(byte[] src, char value, ref int offset)
        {
            WriteUInt16(src, value, ref offset);
        }

        public static void WriteUInt16Array(byte[] src, UInt16[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                var v = value[i];
                WriteUInt16(src, v, ref offset);
            }
        }

        public static void WriteUInt16(byte[] src, UInt16 value, ref int offset)
        {
            for (int i = 0; i < 2; i++)
            {
                src[offset++] = (byte)(value >> ((1 - i) * 8));
            }
        }

        public static void WriteBoolArray(byte[] src, bool[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < len; i++)
            {
                bool v = value[i];
                WriteBool(src, v, ref offset);
            }
        }

        public static void WriteBool(byte[] src, bool value, ref int offset)
        {
            src[offset++] = (byte)(value == true ? 1 : 0);
        }

        public static void WriteSByteArray(byte[] src, sbyte[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < value.Length; i++) WriteSByte(src, value[i], ref offset);
        }

        public static void WriteSByte(byte[] src, sbyte value, ref int offset)
        {
            src[offset++] = (byte)value;
        }

        public static void WriteByteArray(byte[] src, byte[] value, ref int offset)
        {
            ushort len = (ushort)value.Length;
            src[offset++] = (byte)(len >> 8);
            src[offset++] = (byte)len;
            for (int i = 0; i < value.Length; i++) WriteByte(src, value[i], ref offset);
        }

        public static void WriteByte(byte[] src, byte value, ref int offset)
        {
            src[offset++] = value;
        }

    }

}
