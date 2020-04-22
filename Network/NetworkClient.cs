using System;
using System.Threading;
using UnityEngine;
using HybridWebSocket;
using Iterum.Buffers;

namespace Iterum.Network
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
                Debug.Log($"WebSocket {sockets.GetState()}");
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
                Disconnected?.Invoke();

                Stop();
                
                Debug.LogError($"WebSocket error: {errMsg}");
            };

            // Add OnClose event listener
            sockets.OnClose += code =>
            {
                Disconnected?.Invoke();

                Stop();
                
                Debug.Log($"WebSocket closed with code: {code}");
            };

            // Connect to the server
            sockets.Connect();

            isRunning = true;

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
