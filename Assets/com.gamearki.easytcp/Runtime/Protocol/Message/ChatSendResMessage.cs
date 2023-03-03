using System;
using ZeroFrame.Buffer;

namespace ZeroFrame.Protocol
{

    [ZeroMessage]
    internal class ChatSendResMessage : IZeroMessage<ChatSendResMessage>
    {
        public string msg;
        public uint num;

        public void FromBytes(byte[] src, ref int offset)
        {
            ArraySegment<byte> bytes = new ArraySegment<byte>(src);
            msg = BufferReader.ReadUTF8String(src, ref offset);
            num = BufferReader.ReadUInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1024];
            BufferWriter.WriteUTF8String(result, msg, ref offset);
            BufferWriter.WriteUInt32(result, num, ref offset);
            return result;
        }

    }
}

