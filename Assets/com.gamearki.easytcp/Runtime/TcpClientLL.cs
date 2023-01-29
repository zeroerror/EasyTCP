using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameArki.EasyTcp.Logger;

namespace GameArki.EasyTcp {

    public class TcpClientLL {

        IPAddress ipAddress;
        int port;

        // 客户端 Socket 
        Socket clientSocket;
        readonly int maxMsgSize;

        Task recTask;
        CancellationTokenSource cts;

        public Action OnClose;

        bool isConnected;

        public TcpClientLL(string ip, int port, int maxMsgSize = 1024) {
            this.ipAddress = IPAddress.Parse(ip);
            this.port = port;
            this.maxMsgSize = maxMsgSize;
        }

        public void Connect() {
            if (clientSocket != null) return;

            // 创建客户端 Socket 并连接到服务器
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(new IPEndPoint(ipAddress, port));
            isConnected = true;
            EasyLogger.Log("Client: 已连接到服务器");

            cts = new CancellationTokenSource();
            recTask = new Task(RecieveMsg);
            recTask.Start();
        }

        public void Reconnect() {
            if (clientSocket == null) return;

            // 连接到服务器
            clientSocket.Connect(new IPEndPoint(ipAddress, port));
            isConnected = true;
            EasyLogger.Log("Client: 已重新连接到服务器");

            recTask = new Task(RecieveMsg);
            recTask.Start();
        }

        void RecieveMsg() {
            while (isConnected && !cts.IsCancellationRequested) {
                byte[] data = new byte[maxMsgSize];
                int receivedDataLength = 0;
                try {
                    receivedDataLength = clientSocket.Receive(data);
                } catch (SocketException e) {
                    EasyLogger.Log(e.ToString());
                }
                if (receivedDataLength == 0) {
                    OnClose?.Invoke();
                    return;
                }

                string response = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                EasyLogger.Log($"收到服务器数据：{response} ");
            }
        }

        public void SendMsg(string msg) {
            if (!isConnected) {
                return;
            }
            // 向服务器发送数据
            byte[] data = Encoding.ASCII.GetBytes(msg);
            clientSocket.Send(data);
        }

        public void Disconnect() {
            isConnected = false;
            clientSocket.Disconnect(true);
        }

        public void Close() {
            isConnected = false;
            cts.Cancel();
            // 关闭客户端 Socket
            clientSocket.Close();
        }

    }
}
