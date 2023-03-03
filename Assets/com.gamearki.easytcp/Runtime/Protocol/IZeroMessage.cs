namespace ZeroFrame.Protocol
{

    public interface IZeroMessage<T>
    {
        byte[] ToBytes();
        void FromBytes(byte[] src, ref int offset);

    }

}
