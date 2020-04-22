using System;
using Iterum.Buffers;
using Telepathy;
using UnityEngine;
using EventType = Telepathy.EventType;
using Logger = Telepathy.Logger;

namespace Iterum.Network
{
    public class TelepathyClient
    {
        private Client client;
        
        public event ReceiveNetworkMessage Received;
        public event Action Connected;
        public event Action Disconnected;
        public bool IsConnected => client.Connected;


        public TelepathyClient()
        {
            // use Debug.Log functions for Telepathy so we can see it in the console
            Logger.Log = Debug.Log;
            Logger.LogWarning = Debug.LogWarning;
            Logger.LogError = Debug.LogError;
        }

        public void Stop()
        {
            // disconnect from the server when we are done
            client.Disconnect();
        }

        public void Update()
        {
            // grab all new messages. do this in your Update loop.
            while (client.GetNextMessage(out Message msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        Debug.Log($"Telepathy connected");
                        
                        Connected?.Invoke();
                        break;
                    case EventType.Data:
                        var networkMessage = new NetworkMessage
                        {
                            Data = msg.data,
                        };
                        Received?.Invoke(ref networkMessage);
                        break;
                    case EventType.Disconnected:
                        Debug.Log($"Telepathy disconnected");
                        
                        Disconnected?.Invoke();
                        break;
                }
            }
        }

        public void Send(ISerializablePacket packet)
        {
            var buffer = packet.Serialize();
            client.Send(buffer);
            StaticBuffers.Release(buffer);
        }

        public void Start(string host, int port)
        {
            // create and connect the client
            client = new Client();
            client.Connect(host, port);
        }
    }
}