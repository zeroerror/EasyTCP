using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static UnityEngine.Debug;

namespace GameArki.EasyTcp {

    public class TcpServerLL {

        // 服务器 IP 地址和端口号
        IPAddress ipAddress;
        int port;

        Socket serverSocket;
        Socket[] clientSocketList;

        int count = 0;
        public int Count => count;

        Task acceptTask;
        Task recTask;
        CancellationTokenSource cts;

        bool isRunning;

        object lockobj = new Object();

        public TcpServerLL(string ip, int port) {
            this.ipAddress = IPAddress.Parse(ip);
            this.port = port;
            clientSocketList = new Socket[100];
            cts = new CancellationTokenSource();
        }

        public void Connect() {
            // 创建服务器Socket并绑定到指定的 IP 地址和端口号
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ipAddress, port));

            // 启动服务器
            Log("Server: 服务器已启动，正在等待客户端连接...");
            serverSocket.Listen(5);
            isRunning = true;

            // 监听客户端连接
            acceptTask = new Task(AcceptClient);
            acceptTask.Start();

            // 接收客户端数据
            recTask = new Task(ReceiveMsg);
            recTask.Start();
        }

        void AcceptClient() {
            while (isRunning && !cts.IsCancellationRequested) {
                // 接受客户端连接
                Socket clientSocket = serverSocket.Accept();
                Log($"Server: 客户端已连接 {count}");
                lock (lockobj) {
                    clientSocketList[count++] = clientSocket;
                }
            }
        }

        void ReceiveMsg() {
            while (isRunning && !cts.IsCancellationRequested) {
                lock (lockobj) {
                    for (int i = 0; i < count; i++) {
                        Span<byte> data = stackalloc byte[1024];
                        var clientSocket = clientSocketList[i];
                        if (!clientSocket.Connected) {
                            continue;
                        }
                        try {
                            clientSocket.Receive(data);
                        } catch (Exception e) {
                            throw e;
                        }
                        string response = Encoding.ASCII.GetString(data);
                        Log("收到客户端的响应数据：" + response);
                    }
                }
            }
        }

        public void SendMsg(string msg, int connID) {
            if (!isRunning) {
                return;
            }

            // 向服务器发送数据
            byte[] data = Encoding.ASCII.GetBytes(msg);
            var clientSocket = clientSocketList[connID];
            if (clientSocket.Connected) {
                clientSocket.Send(data);
            }
        }

        public void Close() {
            isRunning = false;
            cts.Cancel();
            // 关闭服务器
            try {
                serverSocket.Close();
            } catch (SocketException e1) {
                Log(e1);
            } catch (Exception e2) {
                Log(e2);
            }
            // 关闭已连接客户端
            for (int i = 0; i < count; i++) {
                var clientSocket = clientSocketList[i];
                if (!clientSocket.Connected) {
                    continue;
                }
                clientSocket.Close();
            }
        }

    }

}
