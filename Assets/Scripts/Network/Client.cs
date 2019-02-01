﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Network.Config;
using Network.Entities;
using UnityEngine;

namespace Network
{
    public class Client : MonoBehaviour
    {
        [SerializeField] private ServerConfig config = null;
        [SerializeField] private string host = "localhost";
        [SerializeField] private float connectTimeout = 10f;
        [SerializeField] private float sendRate = 10;

        private readonly Peer<NetClient> client = new Peer<NetClient>();
        private readonly List<NetEntity> entities = new List<NetEntity>();

        private bool isConnecting;
        private bool connected;

        private AssetBundle assets;
        private byte[] assetsData;
        private int assetsProgress;
        
        private void Awake()
        {
            client.Connected += ClientConnected;
            client.Disconnected += ClientDisconnected;
            client.Data += ClientData;
            
            if (string.IsNullOrWhiteSpace(host))
            {
                Debug.LogError("No host server specified");
                return;
            }
            
            var success = client.Start(ref config, false);

            if (!success) return;

            var resolved = client.Connect(host, config.Port);
            if (!resolved)
                Debug.Log($"Could not resolve host: {host}:{config.Port}");
            else
                StartCoroutine(CheckNoConnection());
        }

        private IEnumerator CheckNoConnection()
        {
            isConnecting = true;
            yield return new WaitForSeconds(connectTimeout);

            if (isConnecting)
                Debug.Log("Could not connect to server");
        }

        private void ClientDisconnected(NetConnection connection)
        {
            connected = false;
            Debug.Log("Disconnecting");
        }

        private void ClientConnected(NetConnection connection)
        {
            isConnecting = false;
            connected = true;
            Debug.Log("Connected to server: " + connection.RemoteEndPoint);
        }

        private void OnDestroy()
        {
            client.Data -= ClientData;
            client.Stop("Disconnecting");
        }

        private void Update()
        {
            client.Receive();
        }

        private void ClientData(ref NetMessage msg)
        {
            switch (msg.op)
            {
                case NetOp.SystemInfo:
                    msg.res.Write((byte)NetOp.SystemInfo);
                    msg.res.Write((byte)Application.platform);
                    break;
                case NetOp.AssetsStart:
                    msg.res.Write((byte)NetOp.AssetsStart);
                    msg.res.Write((byte)Application.platform);
                    assetsData = new byte[msg.msg.ReadInt32()];
                    break;
                case NetOp.AssetsData:
                    var p = msg.msg.ReadInt32();
                    var n = msg.msg.ReadInt32();
                    var bytes = msg.msg.ReadBytes(n);
                    for (var i = 0; i < n; i++)
                        assetsData[i + p] = bytes[i];
                    assetsProgress += n;

                    if (assetsProgress == assetsData.Length)
                    {
                        assets = AssetBundle.LoadFromMemory(assetsData);
                        
                        msg.res.Write((byte)NetOp.Ready);
                    }
                    break;
            }
        }

        public void Register(NetEntity entity)
        {
            entities.Add(entity);
        }

        public void Deregister(NetEntity entity)
        {
            entities.Remove(entity);
        }
    }
}
