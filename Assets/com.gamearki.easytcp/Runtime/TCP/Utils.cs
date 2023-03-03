using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroFrame.Network.TCP
{

    public static class Utils
    {
        public static ushort Combine(byte b1, byte b2)
        {
            ushort key = (ushort)b1;
            key |= (ushort)(b2 << 8);
            return key;
        }

        public static int Combine(byte b1, byte b2, byte b3)
        {
            int key = (int)b1;
            key |= ((int)b2) << 8;
            key |= ((int)b3) << 16;
            return key;
        }

        public static int Combine(byte b1, byte b2, byte b3, byte b4)
        {
            int key = (int)b1;
            key |= ((int)b2) << 8;
            key |= ((int)b3) << 16;
            key |= ((int)b4) << 24;
            return key;
        }
    }

}
