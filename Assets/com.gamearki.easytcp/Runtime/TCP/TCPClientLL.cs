using System;
using Telepathy;

namespace ZeroFrame.Network.TCP
{

    public class TCPClientLL
    {
        Client client;


        string host;
        public string Host => this.host;

        int port;
        public int Port => this.port;

        ConnType type;
        public ConnType Type => this.type;

        public int MaxMessageSize => client.MaxMessageSize;
        public bool Connected => client.Connected;

        public event Action OnConnectedHandle;
        public event Action OnDisconnectedHandle;
        public event Action<ArraySegment<byte>> OnDataHandle;

        public TCPClientLL(int maxMessageSize)
        {
            client = new Client(maxMessageSize);
            client.OnConnected += OnConnected;
            client.OnData += OnData;
            client.OnDisconnected += OnDisconnected;
            type = ConnType.Disconnected;
        }

        public void SendMsg(byte[] bytes)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
            client.Send(segment);
        }


        public void Connect(string host, int port)
        {
            client.Connect(host, port);
            this.host = host;
            this.port = port;
        }

        public void Reconnect(string host, int port)
        {
            client.Connect(host, port);
            this.host = host;
            this.port = port;
        }

        public void OnConnected()
        {
            OnConnectedHandle?.Invoke();
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public void OnDisconnected()
        {
            OnDisconnectedHandle?.Invoke();
        }

        public void OnData(ArraySegment<byte> bytes)
        {
            OnDataHandle?.Invoke(bytes);
        }

        public void Tick(int count = 100)
        {
            client.Tick(count);
        }

    }

}
