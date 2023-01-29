using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.Debug;

namespace GameArki.EasyTcp.Sample {

    public class TcpSample : MonoBehaviour {

        List<TcpClientLL> clientList;
        TcpServerLL server;

        string ip = "127.0.0.1";
        int port = 4000;

        void Awake() {
            clientList = new List<TcpClientLL>();
            server = new TcpServerLL(ip, port);
        }

        string msg_client;
        string msg_server;
        void OnGUI() {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("开启服务端")) {
                server.Connect();
            }
            if (GUILayout.Button("开启客户端")) {
                var client = new TcpClientLL(ip, port);
                client.Connect();
                clientList.Add(client);
                client.OnClose += () => {
                    Log("客户端断开连接");
                };
            }
            if (GUILayout.Button("断开和服务端连接")) {
                clientList.ForEach((client) => {
                    client.Disconnect();
                });
            }
            if (GUILayout.Button("重新连接至服务端")) {
                clientList.ForEach((client) => {
                    client.Reconnect();
                });
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("关闭服务端")) {
                server.Close();
            }
            if (GUILayout.Button("关闭客户端")) {
                clientList.ForEach((client) => {
                    client.Close();
                });
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("(服务端)发送消息:")) {
                server.SendMsg(msg_server, 0);
            }
            msg_server = GUILayout.TextField(msg_server, GUILayout.Width(200));
            if (GUILayout.Button("(客户端)发送消息:")) {
                clientList.ForEach((client) => {
                    client.SendMsg(msg_client);
                });
            }
            msg_client = GUILayout.TextField(msg_client, GUILayout.Width(200));
            GUILayout.EndHorizontal();

        }

        void OnApplicationQuit() {
            if (server != null) {
                server.Close();
            }
            clientList.ForEach((client) => {
                client.Close();
            });
        }

    }

}