using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iterum.Logs;
using Iterum.Utils;
using Telepathy;
using EventType = Telepathy.EventType;

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

        private Queue<NetworkMessage> queue = new Queue<NetworkMessage>();
        
        public double RTT { get; private set; }

        public TelepathyClient()
        {
            pingSw = new Stopwatch();
            pingSw.Start();
            
            pingTimer = new StopwatchTimer(1f);
            
            // create and connect the client
            client = new Client();

            // use Debug.Log functions for Telepathy so we can see it in the console
            Logger.Log = s => Log.Info(LogGroup, s);
            Logger.LogWarning = s => Log.Warn(LogGroup, s);
            Logger.LogError =s => Log.Error(LogGroup, s);
        }

        public void Stop()
        {
            // disconnect from the server when we are done
            client.Disconnect();
        }

        private void SendPing()
        {
            if (!client.Connected) return;
            if (pingTimer.Check())
            {
                Send(new byte[] {0,255});
                pingSw.Restart();

            }
        }

        public void Update()
        {
            if (Immediate)
            {
                while (queue.Count > 0)
                {
                    var networkMessage = queue.Dequeue();
                    Received?.Invoke(ref networkMessage);
                }
            }

            SendPing();
            
            // grab all new messages. do this in your Update loop.
            while (client.GetNextMessage(out Message msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        Log.Info(LogGroup, $"Connected - {Address}");
                        
                        Connected?.Invoke();
                        break;
                    case EventType.Data:
                        // ping answer
                        CheckPing(ref msg);
                        var networkMessage = new NetworkMessage
                        {
                            Data = msg.data,
                        };

                        
                        
                        if(!Immediate)
                            queue.Enqueue(networkMessage);
                        else
                        {
                            Received?.Invoke(ref networkMessage);
                        }

                        break;
                    case EventType.Disconnected:
                        Log.Info(LogGroup, "Disconnected");
                        
                        Disconnected?.Invoke();
                        break;
                }
            }
        }

        private void CheckPing(ref Message msg)
        {
            if (msg.data[0] == 0 && msg.data[1] == 254)
            {
                RTT = TimeConvert.TicksToMs(pingSw.ElapsedTicks);
            }
        }

        public void Send(byte[] bytes)
        {
            client.Send(bytes);
        }

        public void Start(string host, int port)
        {
            Address = $"{host}:{port}";
            Log.Info(LogGroup, $"Connecting... - {Address}");
            client.Connect(host, port);
        }

        private const string LogGroup = "TelepathyClient";
        private string Address { get; set; }

        public bool Immediate { get; set; } = true;
    }

    public delegate void ReceiveNetworkMessage(ref NetworkMessage msg);
}