using System;
using ZeroFrame.Buffer;
namespace ZeroFrame.Protocol
{

    [ZeroMessage]
    internal class ChatSendReqMessage : IZeroMessage<ChatSendReqMessage>
    {
        public string msg;
        public uint num;
        public uint num2;
        public bool[] testBoolArray;

        public void FromBytes(byte[] src, ref int offset)
        {
            msg = BufferReader.ReadUTF8String(src, ref offset);
            num = BufferReader.ReadUInt32(src, ref offset);
            num2 = BufferReader.ReadUInt32(src, ref offset);
            offset += src.Length;
        }

        public byte[] ToBytes()
        {
            int offset = 0;
            byte[] result = new byte[1000];
            BufferWriter.WriteUTF8String(result, msg, ref offset);
            BufferWriter.WriteUInt32(result, num, ref offset);
            BufferWriter.WriteUInt32(result, num2, ref offset);
            return result;
        }

    }

}

