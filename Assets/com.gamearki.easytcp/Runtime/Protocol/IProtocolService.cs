using System;

namespace ZeroFrame.Protocol
{

    internal interface IProtocolService
    {

        (byte serviceID, byte messageID) GetMessageID<T>() where T : IZeroMessage<T>;

        Func<T> GetGenerateHandle<T>() where T : IZeroMessage<T>;

    }

}
