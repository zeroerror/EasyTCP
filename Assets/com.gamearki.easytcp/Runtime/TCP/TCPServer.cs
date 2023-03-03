using System;
using System.Collections.Generic;
using ZeroFrame.Protocol;

namespace ZeroFrame.Network.TCP
{

    public class TCPServer
    {
        TCPServerLL serverLL;
        public int MaxMessageSize => serverLL.maxMessageSize;
        public bool Active => serverLL.Active;

        public event Action<int> OnConnectedHandle;
        public event Action<int> OnDisconnectedHandle;

        Dictionary<ushort, Action<int, ArraySegment<byte>>> m_registers;

        public TCPServer(int maxMsgSize = 4096)
        {
            serverLL = new TCPServerLL(maxMsgSize);
            m_registers = new Dictionary<ushort, Action<int, ArraySegment<byte>>>();

            serverLL.OnConnectedHandle += OnConnected;
            serverLL.OnDataHandle += OnData;
        }

        public void Tick(int processLimit = 100)
        {
            serverLL.Tick(processLimit);
        }

        public void StartListen(int port)
        {
            serverLL.StartListen(port);
        }

        public void RestartListen()
        {

        }
        public void StopListen()
        {

        }

        void OnConnected(int connID)
        {
            OnConnectedHandle?.Invoke(connID);
        }

        protected void SendMessage<T>(byte serviceId, byte messageId, T msg, int connId) where T : IZeroMessage<T>
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

            serverLL.Send(connId, bytes);
        }

        public void OnData(int connID, ArraySegment<byte> bytes)
        {
            var arr = bytes.Array;
            if (arr.Length < 2)
            {
                Console.WriteLine($"connID:{connID} 消息长度不足！");
                return;
            }

            byte serviceId = arr[0];
            byte messageId = arr[1];
            ushort key = Utils.Combine(serviceId, messageId);
            if (m_registers.TryGetValue(key, out Action<int, ArraySegment<byte>> action))
            {
                action?.Invoke(connID, bytes);
            }
        }

        protected void AddRegister<T>(byte serviceId, byte messageId, Func<T> generateHandle, Action<int, T> handle) where T : IZeroMessage<T>
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
                Console.WriteLine($"TCPServer：serviceId={serviceId} messageId={messageId}  已注册的OnDataHandler");
                return;
            }
            else
            {
                m_registers.Add(key, (connId, bytes) =>
                {
                    T msg = generateHandle.Invoke();
                    int offset = 2;
                    msg.FromBytes(bytes.Array, ref offset);
                    handle.Invoke(connId, msg);
                });
                Console.WriteLine($"Register注册成功：serviceId:{serviceId}  messageId:{messageId}");
            }
        }

    }

}
