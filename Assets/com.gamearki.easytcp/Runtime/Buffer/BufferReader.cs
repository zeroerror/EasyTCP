
using System;
using System.Text;

namespace ZeroFrame.Buffer
{

    public static class BufferReader
    {

        public static Int64[] ReadInt64Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            Int64[] result = new Int64[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt64(src, ref offset);
            }
            return result;
        }

        public static Int64 ReadInt64(byte[] src, ref int offset)
        {
            Int64 result = 0;
            for (int i = 0; i < 2; i++)
            {
                int moveBit = (1 - i) * 32;
                result |= (Int64)ReadUInt32(src, ref offset) << moveBit;
            }
            return result;
        }

        public static UInt64[] ReadUInt64Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            UInt64[] result = new UInt64[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt64(src, ref offset);
            }
            return result;
        }

        public static UInt64 ReadUInt64(byte[] src, ref int offset)
        {
            UInt64 result = 0;
            for (int i = 0; i < 2; i++)
            {
                int moveBit = (1 - i) * 32;
                result |= (UInt64)ReadUInt32(src, ref offset) << moveBit;
            }
            return result;
        }

        public static Int32[] ReadInt32Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            Int32[] result = new Int32[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt32(src, ref offset);
            }
            return result;
        }

        public static Int32 ReadInt32(byte[] src, ref int offset)
        {
            Int32 result = 0;
            for (int i = 0; i < 2; i++)
            {
                int moveBit = (1 - i) * 16;
                result |= (Int32)ReadUInt16(src, ref offset) << moveBit;
            }
            return result;
        }

        public static UInt32[] ReadUInt32Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            UInt32[] result = new UInt32[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt32(src, ref offset);
            }
            return result;
        }

        public static UInt32 ReadUInt32(byte[] src, ref int offset)
        {
            UInt32 result = 0;
            for (int i = 0; i < 2; i++)
            {
                int moveBit = (1 - i) * 16;
                result |= (UInt32)ReadUInt16(src, ref offset) << moveBit;
            }
            return result;
        }

        public static Int16[] ReadInt16Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            Int16[] result = new Int16[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadInt16(src, ref offset);
            }
            return result;
        }

        public static Int16 ReadInt16(byte[] src, ref int offset)
        {
            Int16 result = 0;
            for (int i = 0; i < 2; i++)
            {
                result |= (Int16)(src[offset++] << (1 - i) * 8);
            }
            return result;
        }

        public static UInt16[] ReadUInt16Array(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            UInt16[] result = new UInt16[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUInt16(src, ref offset);
            }
            return result;
        }

        public static UInt16 ReadUInt16(byte[] src, ref int offset)
        {
            UInt16 result = 0;
            for (int i = 0; i < 2; i++)
            {
                result |= (UInt16)(src[offset++] << (1 - i) * 8);
            }
            return result;
        }

        public static String[] ReadUTF8StringArray(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            String[] result = new String[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadUTF8String(src, ref offset);
            }
            return result;
        }

        public static String ReadUTF8String(byte[] src, ref int offset)
        {
            ushort count = ReadUInt16(src, ref offset);
            string data = Encoding.UTF8.GetString(src, offset, count);
            offset += count;
            return data;
        }

        public static char[] ReadCharArray(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            char[] result = new char[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadChar(src, ref offset);
            }
            return result;
        }

        public static char ReadChar(byte[] src, ref int offset)
        {
            char result = (char)ReadUInt16(src, ref offset);
            return result;
        }

        public static bool[] ReadBoolArray(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            bool[] result = new bool[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadBool(src, ref offset);
            }
            return result;
        }

        public static bool ReadBool(byte[] src, ref int offset)
        {
            bool result = src[offset] != 0;
            offset++;
            return result;
        }

        public static byte[] ReadByteArray(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadByte(src, ref offset);
            }
            return result;
        }

        public static byte ReadByte(byte[] src, ref int offset)
        {
            return src[offset++];
        }

        public static sbyte[] ReadSByteArray(byte[] src, ref int offset)
        {
            ushort len = (ushort)(src[offset++] << 8);
            len |= (ushort)src[offset++];
            sbyte[] result = new sbyte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = (sbyte)src[offset++];
            }
            return result;
        }

        public static sbyte ReadSByte(byte[] src, ref int offset)
        {
            sbyte result = (sbyte)src[offset++];
            return result;
        }


    }

}
