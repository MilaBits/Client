﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Lidgren.Network;
using Network.Config;
using UnityEditor;
using UnityEngine;

namespace Network
{
    public class Server : MonoBehaviour
    {
        [SerializeField] private ServerConfig config = null;

        private readonly Peer<NetServer> server = new Peer<NetServer>();
        private readonly List<NetConnection> clients = new List<NetConnection>();


        private void Awake()
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            
            server.AssetManager.LoadFromFile($"{Application.streamingAssetsPath}/assets");
            
            server.Connected += ServerOnConnected;
            server.Disconnected += ServerOnDisconnected;
            server.Data += ServerOnData;
            var success = server.Start(ref config, true);

            if (!success) return;
            Debug.Log($"Server running on port {config.Port}");
        }

        private void OnDestroy()
        {
            server.Connected -= ServerOnConnected;
            server.Disconnected -= ServerOnDisconnected;
            server.Data -= ServerOnData;
            Debug.Log("Server shutting down");
            server.Stop("Server shutting down");
        }

        private void FixedUpdate()
        {
            server.Receive();
        }

        private void ServerOnConnected(NetConnection connection)
        {
            clients.Add(connection);

            var msg = server.NetPeer.CreateMessage();
            msg.Write((byte) NetOp.SystemInfo);
            connection.SendMessage(msg, NetDeliveryMethod.ReliableUnordered, 0);
            Debug.Log(connection.RemoteEndPoint + " connected");
        }

        private void ServerOnDisconnected(NetConnection connection)
        {
            clients.Remove(connection);
            Debug.Log(connection.RemoteEndPoint + " disconnected");
        }

        private void ServerOnData(ref NetMessage msg)
        {
            RuntimePlatform platform;
            switch (msg.op)
            {
                case NetOp.SystemInfo:
                    platform = (RuntimePlatform) msg.msg.ReadByte();
                    if (server.AssetManager.Available(platform))
                    {
                        msg.res.Write((byte)NetOp.AssetsStart);
                        msg.res.Write(server.AssetManager.Size(platform));
                    }
                    break;
                case NetOp.AssetsStart:
                    platform = (RuntimePlatform) msg.msg.ReadByte();
                    if (server.AssetManager.Available(platform))
                    {
                        var connection = msg.msg.SenderConnection;
                        StartCoroutine(server.AssetManager.SendAssetBundle(
                            platform, 
                            msg.msg.SenderConnection.CurrentMTU - 100, 
                            msg.msg.SenderConnection.AverageRoundtripTime, (start, length, data) =>
                            {
                                var m = server.NetPeer.CreateMessage();
                                m.Write((byte) NetOp.AssetsData);
                                m.Write(start);
                                m.Write(length);
                                m.Write(data);

                                connection.SendMessage(m, NetDeliveryMethod.ReliableUnordered, 0);
                            }));
                    }

                    break;
                case NetOp.Ready:
                    Debug.Log(msg.msg.SenderEndPoint + " is ready");
                    break;
            }
        }
    }
}