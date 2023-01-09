using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static UnityEngine.Debug;

namespace GameArki.EasyTcp {

    public class TcpServerLL {

        // 服务器 IP 地址和端口号
        IPAddress ipAddress;
        int port;

        // 服务器 Socket
        Socket serverSocket;
        Socket[] clientSocketList;
        int count = 0;

        public TcpServerLL(string ip, int port) {
            this.ipAddress = IPAddress.Parse(ip);
            this.port = port;
            clientSocketList = new Socket[100];
        }

        bool received = false;
        public void Connect() {
            // 创建服务器Socket并绑定到指定的 IP 地址和端口号
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ipAddress, port));


            // 启动服务器并监听客户端连接
            serverSocket.Listen(5);

            Log("服务器已启动，正在等待客户端连接...");

            // 接受客户端连接
            Socket clientSocket = serverSocket.Accept();
            Log($"客户端已连接 {count}");

            clientSocketList[count++] = clientSocket;

            // 接收客户端发送的数据
            byte[] data = new byte[1024];
            clientSocket.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback((result) => {
                Socket socket = result.AsyncState as Socket;
                int endIndex = socket.EndReceive(result);
                Array.Copy((byte[])result.AsyncState, data, endIndex);
                string message = Encoding.ASCII.GetString(data);
                Log("收到客户端的数据：" + message);
                received = true;
            }), clientSocket);

            // while (!received) {
            //     Log("等待接受客户端数据----------");
            //     Thread.Sleep(100);
            // }

            // 发送响应数据到客户端
            string response = "Hello, client!";
            byte[] responseData = Encoding.ASCII.GetBytes(response);
            clientSocket.Send(responseData);

        }

        public void Close() {
            // 关闭客户端 Socket 和服务器 Socket
            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
        }

    }


}
