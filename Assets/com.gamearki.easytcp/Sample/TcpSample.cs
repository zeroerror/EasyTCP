using System.Threading;
using UnityEngine;

namespace GameArki.EasyTcp.Sample {

    public class TcpSample : MonoBehaviour {

        TcpClientLL client;
        TcpServerLL server;

        string ip = "127.0.0.1";
        int port = 4000;

        void Awake() {
            client = new TcpClientLL(ip, port);
            server = new TcpServerLL(ip, port);
        }

        void OnGUI() {
            if (GUILayout.Button("开启服务端")) {
                Thread thread = new Thread(() => {
                    server.Connect();
                });
                thread.Start();
            }
            if (GUILayout.Button("关闭客户端")) {
                Thread thread = new Thread(() => {
                    server.Connect();
                });
                thread.Start();
            }
            if (GUILayout.Button("开启客户端")) {
                Thread thread = new Thread(() => {
                    client.Connect();
                });
                thread.Start();
            }
            if (GUILayout.Button("关闭客户端")) {
                Thread thread = new Thread(() => {
                    client.Close();
                });
                thread.Start();
            }
        }

    }

}