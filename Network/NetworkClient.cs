using System;
using System.Threading;
#if VALVE_SOCKETS
using Valve.Sockets;
#endif
using System.Text;
using System.IO;
using System.IO.Compression;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Math;
using Magistr.Log;
#else
using UnityEngine;
#endif
#if WEBSOCKETS
using HybridWebSocket;
using Magistr.Buffers;
#endif

namespace Magistr.Network
{
    public delegate void ReceiveNetworkMessage(ref NetworkMessage msg);
    public class NetworkClient
    {
        public string statusDetailed;
        public int ping;
        public float connectionQualityLocal;
        public float connectionQualityRemote;
        public float outPacketsPerSecond;
        public float outBytesPerSecond;
        public float inPacketsPerSecond;
        public float inBytesPerSecond;
        public int sendRateBytesPerSecond;
        public int pendingUnreliable;
        public int pendingReliable;
        public int sentUnackedReliable;

#if VALVE_SOCKETS
        private NetworkingSockets sockets;
#endif
#if WEBSOCKETS
        private WebSocket sockets;
#endif
        
        private Thread workerThread;
        private uint connection;
        private bool isReceiveRunning;
        private const int ReceiveThreadSleep = 5;

        public event ReceiveNetworkMessage Received;
        public event Action Connected;
        public event Action Disconnected;
        public event Action ProblemDetectedLocally;

        public NetworkClient()
        {
            #if VALVE_SOCKETS
            Library.Initialize();
            #endif
        }

        private StringBuilder status = new StringBuilder(1024);
        public void UpdateStats()
        {
            if (connection != 0)
            {
#if VALVE_SOCKETS
                status.Clear();
                sockets.GetDetailedConnectionStatus(connection, status, 1024);
                statusDetailed = status.ToString();
                //connectionQualityLocal = status.connectionQualityLocal;
                //connectionQualityRemote = status.connectionQualityRemote;
                //outPacketsPerSecond = status.outPacketsPerSecond;
                //outBytesPerSecond = status.outBytesPerSecond;
                //inPacketsPerSecond = status.inPacketsPerSecond;
                //inBytesPerSecond = status.inBytesPerSecond;
                //sendRateBytesPerSecond = status.sendRateBytesPerSecond;
                //pendingUnreliable = status.pendingUnreliable;
                //pendingReliable = status.pendingReliable;
                //sentUnackedReliable = status.sentUnackedReliable;
#endif
            }
        }

        public void Stop()
        {
            if (!isReceiveRunning) return;
#if VALVE_SOCKETS
            sockets?.CloseConnection(connection);

            //Library.Deinitialize();
#endif
#if WEBSOCKETS
            sockets?.Close();
#endif
            isReceiveRunning = false;
        }

        public void StartClient(string host, int port = 0)
        {
            if (isReceiveRunning) return;
#if WEBSOCKETS
            // Create WebSocket instance
            sockets = WebSocketFactory.CreateInstance("ws://" + host + ":" + port);
            // Add OnOpen event listener
            sockets.OnOpen += () =>
            {
                Connected?.Invoke();
                Debug.Log("WS connected!");
                Debug.Log("WS state: " + sockets.GetState());
            };

            // Add OnMessage event listener
            sockets.OnMessage += data =>
            {
                var msg = new NetworkMessage
                {
                    data = data,
                    channel = 0,
                    connection = 0,
                    length = data.Length,
                    messageNumber = 0,
                    timeReceived = 0,
                    userData = 0
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
#endif
#if VALVE_SOCKETS
            sockets = new NetworkingSockets();
            Address address = new Address();
            address.SetAddress(host, (ushort)port);

            connection = sockets.Connect(ref address);


            IsRecieveRunning = true;

            workerThread = new Thread(RecieveLoop);
            workerThread.Start();
#endif

        }
#if VALVE_SOCKETS
        private void Status(StatusInfo info, IntPtr context)
        {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;
                case ConnectionState.Connecting:
                    Debug.Log($"Client connecting to server - ID: {info.connection} ");
                    break;

                case ConnectionState.Connected:
                    Debug.Log($"Client connected to server - ID: {info.connection} ");
                    Connected?.Invoke();
                    break;

                case ConnectionState.ClosedByPeer:
                    sockets.CloseConnection(info.connection);
                    Disconnected?.Invoke();
                    Debug.Log($"Client disconnected from server - ID: {info.connection}");
                    IsRecieveRunning = false;
                    break;
                case ConnectionState.ProblemDetectedLocally:
                    sockets.CloseConnection(connection);
                    ProblemDetectedLocally?.Invoke();
                    Debug.Log("Client unable to connect");
                    IsRecieveRunning = false;
                    break;
            }
        }
#endif
        public static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using (var stream = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                stream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            var input = new MemoryStream(data);
            var output = new MemoryStream();
            using (var stream = new GZipStream(input, CompressionMode.Decompress))
            {
                stream.CopyTo(output);
            }
            return output.ToArray();
        }
        public void Send(ISerializablePacket packet)
        {
#if VALVE_SOCKETS
            var packetBuffer = packet.Serialize();

            var buffer = Compress(packetBuffer);// BrotliSharpLib.Brotli.CompressBuffer(packetBuffer, 0, packetBuffer.Length);
            sockets.SendMessageToConnection(connection, buffer, SendType.Reliable);
#endif
#if WEBSOCKETS
            var buffer = packet.Serialize();

            //Debug.Log("Sent " + buffer.Length + " first: " + buffer[0]);
            sockets.Send(buffer);
            StaticBuffers.Release(buffer);

#endif
        }

        public void SendUnreliable(ISerializablePacket packet)
        {
#if VALVE_SOCKETS
            sockets.SendMessageToConnection(connection, packet.Serialize(), SendType.Unreliable);
#endif
        }
#if VALVE_SOCKETS
        private void RecieveLoop()
        {
            const int maxMessages = 20;

            NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

            while (IsRecieveRunning)
            {
                sockets.DispatchCallback(Status);

                int netMessagesCount = sockets.ReceiveMessagesOnConnection(connection, netMessages, maxMessages);

                if (netMessagesCount > 0)
                {
                    for (int i = 0; i < netMessagesCount; i++)
                    {
                        ref NetworkingMessage net = ref netMessages[i];

                        // copy data to own structure
                        NetworkMessage msg = new NetworkMessage
                        {
                            data = new byte[net.length],
                            channel = net.channel,
                            connection = net.connection,
                            length = net.length,
                            messageNumber = net.messageNumber,
                            timeReceived = net.timeReceived,
                            userData = net.userData
                        };
                        net.CopyTo(msg.data);
                        msg.data = Decompress(msg.data);// BrotliSharpLib.Brotli.DecompressBuffer(msg.data, 0, msg.data.Length);
                        Recieved.Invoke(ref msg);
                        net.Destroy();

                    }
                }

                Thread.Sleep(RecieveThreadSleep);
            }
        }
#endif
    }
}
