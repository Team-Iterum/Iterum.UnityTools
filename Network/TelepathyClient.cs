using System;
using System.Diagnostics;
using Iterum.Utils;
using Telepathy;
using Debug = UnityEngine.Debug;
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

        private Stopwatch pingSw;
        private StopwatchTimer pingTimer;
        
        public double RTT { get; private set; }

        public TelepathyClient()
        {
            pingSw = new Stopwatch();
            pingSw.Start();
            
            pingTimer = new StopwatchTimer(1f);
            
            // create and connect the client
            client = new Client();
            
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

        private void SendPing()
        {
            if (pingTimer.Check())
            {
                Send(new byte[] {0,255});
                pingSw.Restart();

            }
        }

        public void Update()
        {
            SendPing();
            
            // grab all new messages. do this in your Update loop.
            while (client.GetNextMessage(out Message msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        Debug.Log($"Client connected - {Address}");
                        
                        Connected?.Invoke();
                        break;
                    case EventType.Data:
                        // ping answer
                        if (msg.data[0] == 0 && msg.data[1] == 254)
                        {
                            RTT = TimeConvert.TicksToMs(pingSw.ElapsedTicks);
                        }
                        
                        var networkMessage = new NetworkMessage
                        {
                            Data = msg.data,
                        };
                        Received?.Invoke(ref networkMessage);
                        break;
                    case EventType.Disconnected:
                        Debug.Log($"Client disconnected");
                        
                        Disconnected?.Invoke();
                        break;
                }
            }
        }

        public void Send(byte[] bytes)
        {
            client.Send(bytes);
        }

        public void Start(string host, int port)
        {
            Address = $"{host}:{port}";
            Debug.Log($"Client connecting... - {Address}");
            client.Connect(host, port);
        }

        private string Address { get; set; }
    }

    public delegate void ReceiveNetworkMessage(ref NetworkMessage msg);
}