using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameArki.EasyTcp.Logger;

namespace GameArki.EasyTcp {

    public class TcpClientLL {

        IPAddress ipAddress;
        int port;

        // 客户端 Socket 
        Socket clientSocket;
        readonly int maxMsgSize;

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

            EasyLogger.Log("已连接到服务器");

            // 向服务器发送数据
            string message = "Hello, server!";
            byte[] data = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(data);

            // 接收服务器的响应数据
            data = new byte[maxMsgSize];
            int receivedDataLength = clientSocket.Receive(data);
            string response = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            EasyLogger.Log("收到服务器的响应数据：" + response);
        }

        public void Close() {
            // 关闭客户端 Socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clientSocket = null;
        }

    }
}
