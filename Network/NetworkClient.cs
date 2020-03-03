using System;
using System.Threading;
using UnityEngine;
using HybridWebSocket;
using Magistr.Buffers;

namespace Magistr.Network
{
    public delegate void ReceiveNetworkMessage(ref NetworkMessage msg);
    public class NetworkClient
    {
        private WebSocket sockets;
        private bool isRunning;

        public event ReceiveNetworkMessage Received;
        public event Action Connected;
        public event Action Disconnected;
        public bool IsConnected => sockets != null && sockets.GetState() == WebSocketState.Open;
        
        public void Stop()
        {
            if (!isRunning) return;
            
            sockets?.Close();
            isRunning = false;
        }

        public void Start(string host, int port)
        {
            if (isRunning) return;

            // Create WebSocket instance
            sockets = WebSocketFactory.CreateInstance("ws://" + host + ":" + port);
            // Add OnOpen event listener
            sockets.OnOpen += () =>
            {
                Connected?.Invoke();
                Debug.Log("WebSocket connected");
                Debug.Log("WebSocket State: " + sockets.GetState());
            };

            // Add OnMessage event listener
            sockets.OnMessage += data =>
            {
                var msg = new NetworkMessage
                {
                    Data = data,
                };
                Received?.Invoke(ref msg);
            };

            // Add OnError event listener
            sockets.OnError += errMsg =>
            {
                Debug.LogError("WS error: " + errMsg);
            };

            // Add OnClose event listener
            sockets.OnClose += code =>
            {
                Disconnected?.Invoke();

                if(code == WebSocketCloseCode.Normal)
                    Debug.Log("WS closed with code: " + code);
                else
                    Debug.LogError("WS closed with code: " + code);
            };

            // Connect to the server
            sockets.Connect();

        }
        public void Send(ISerializablePacket packet)
        {
            if (!IsConnected) return;
            
            var buffer = packet.Serialize();
            sockets.Send(buffer);
            StaticBuffers.Release(buffer);
            
        }
        
    }
}
