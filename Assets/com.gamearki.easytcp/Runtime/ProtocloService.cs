using System;
using ZeroFrame.Protocol;

namespace ZeroFrame.Network
{
    public class ProtocloService : IProtocolService
    {

        public Func<T> GetGenerateHandle<T>() where T : IZeroMessage<T>
        {
            throw new NotImplementedException();
        }

        public (byte serviceID, byte messageID) GetMessageID<T>() where T : IZeroMessage<T>
        {
            throw new NotImplementedException();
        }

    }

}
