using System;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace ZeroFrame.Network.TCP
{

    public class TCPClient
    {
        TCPClientLL clientLL;

        public bool Connected => clientLL.Connected;
        public string Host => clientLL.Host;
        public int Port => clientLL.Port;
        public ConnType Type => clientLL.Type;
        public int MaxMessageSize => clientLL.MaxMessageSize;


        Dictionary<ushort, Action<ArraySegment<byte>>> m_registers;

        public event Action OnConnectedHandle;
        public event Action OnDisconnectedHandle;


        public TCPClient(int maxMessageSize)
        {
            clientLL = new TCPClientLL(maxMessageSize);
            m_registers = new Dictionary<ushort, Action<ArraySegment<byte>>>();

            clientLL.OnConnectedHandle += OnConnected;
            clientLL.OnDataHandle += OnData;
            clientLL.OnDisconnectedHandle += OnDisconnected;
        }

        public void Tick(int count = 100)
        {
            clientLL.Tick(count);
        }

        public void Connect(string host, int port)
        {
            clientLL.Connect(host, port);
        }

        void OnConnected()
        {
            OnConnectedHandle?.Invoke();
        }

        public void Disconnect()
        {
            clientLL.Disconnect();
        }

        void OnDisconnected()
        {
            OnDisconnectedHandle?.Invoke();
        }

        public void Reconnect(string host, int port)
        {

        }

        protected void SendMessage<T>(byte serviceId, byte messageId, T msg) where T : IZeroMessage<T>
        {
            byte[] msgBytes = msg.ToBytes();
            int len = msgBytes.Length + 2;
            if (len > MaxMessageSize)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError("TCPServer:发送数据超过MaxMessageSize！");
#else
                Console.WriteLine("TCPServer:发送数据超过MaxMessageSize！");
#endif        
                return;
            }

            byte[] bytes = new byte[len];
            bytes[0] = serviceId;
            bytes[1] = messageId;
            for (int i = 2; i < len; i++)
            {
                bytes[i] = msgBytes[i - 2];
            }

            clientLL.SendMsg(bytes);
        }

        void OnData(ArraySegment<byte> data)
        {
            var arr = data.Array;

            if (arr == null)
            {
                Console.WriteLine($"消息为空");
                return;
            }

            if (arr.Length < 2)
            {
                Console.WriteLine($"消息长度过短: {arr.Length}");
                return;
            }

            byte serviceId = arr[0];
            byte messageId = arr[1];
            ushort key = (ushort)serviceId;
            key |= (ushort)(messageId << 8);

            //Invoke注册事件
            if (m_registers.TryGetValue(key, out var handle))
            {
                handle.Invoke(data);
            }
            else
            {
                Console.WriteLine($"未注册 serviceId:{serviceId}, messageId:{messageId}");
            }

        }

        protected void AddRegister<T>(byte serviceId, byte messageId, Func<T> generateHandle, Action<T> handle) where T : IZeroMessage<T>
        {
            if (generateHandle == null)
            {
                Console.WriteLine($"未注册：{nameof(generateHandle)}");
                return;
            }

            if (handle == null)
            {
                Console.WriteLine($"未注册：{nameof(handle)}");
                return;
            }

            ushort key = Utils.Combine(serviceId, messageId);
            if (m_registers.ContainsKey(key))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning($"TCPServer：serviceId={serviceId} messageId={messageId}  register已注册");
#else
                Console.WriteLine($"TCPServer：serviceId={serviceId} messageId={messageId}  register已注册");
#endif
                return;
            }
            else
            {
                m_registers.Add(key, (bytes) =>
                {
                    T msg = generateHandle.Invoke();
                    int offset = 2;
                    msg.FromBytes(bytes.Array, ref offset);
                    handle.Invoke(msg);
                });
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"TCPServer：serviceId={serviceId} messageId={messageId} key={key}  register注册成功");
#else
                Console.WriteLine($"TCPServer：serviceId={serviceId} messageId={messageId} key={key}  register注册成功");
#endif
            }

        }

    }

}
