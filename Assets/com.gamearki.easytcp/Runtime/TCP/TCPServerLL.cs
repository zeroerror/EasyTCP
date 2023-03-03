using System;
using Telepathy;

namespace ZeroFrame.Network.TCP
{

    public class TCPServerLL
    {

        Server server;

        public int port;
        public int maxMessageSize;

        public bool Active => server.Active;

        public event Action<int> OnConnectedHandle;
        public event Action<int, ArraySegment<byte>> OnDataHandle;
        public event Action<int> OnDisconnectedHandle;

        public TCPServerLL(int maxMessageSize)
        {
            this.maxMessageSize = maxMessageSize;
            server = new Server(maxMessageSize);
            server.OnConnected += OnConnected;
            server.OnDisconnected += OnDisconnected;
            server.OnData += OnData;
        }

        public void Tick(int processLimit = 100)
        {
            server.Tick(processLimit);
        }

        public void StartListen(int port)
        {
            this.port = port;
            server.Start(port);
        }

        public void RestartListen(int port)
        {
            if (server.Active) server.Stop();
            server.Start(port);
        }

        void OnConnected(int connID)
        {
            OnConnectedHandle?.Invoke(connID);
        }

        void OnDisconnected(int connID)
        {
            OnDisconnectedHandle?.Invoke(connID);
        }

        void OnData(int connID, ArraySegment<byte> bytes)
        {
            OnDataHandle?.Invoke(connID, bytes);
        }

        public void Send(int connID, byte[] byets)
        {
            server.Send(connID, new ArraySegment<byte>(byets));
        }
    }

}
