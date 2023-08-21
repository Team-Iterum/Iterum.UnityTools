using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iterum.BaseSystems;
using Iterum.Logs;
using Iterum.Utils;
using Telepathy;
using EventType = Telepathy.EventType;
using Log = Iterum.Logs.Log;

namespace Iterum.Network
{
    public class TelepathyClient
    {
        private Client client;

        public event ReceiveNetworkMessage Received;
        public event Action Connected;
        public event Action Disconnected;
        public bool IsConnected => client.Connected;

        private Stopwatch pingTicks;

        public long LastSentTick { get; private set; }
        public long ElapsedTicks => pingTicks.ElapsedTicks;

        private StopwatchTimer pingTimer;

        private Queue<NetworkMessage> queue = new Queue<NetworkMessage>();

        public double RTT { get; private set; }

        public TelepathyClient()
        {
            pingTicks = new Stopwatch();
            pingTicks.Start();

            pingTimer = new StopwatchTimer(1f);

            // create and connect the client
            var maxMessageSize = 64 * 1024;
            client = new Client(maxMessageSize);
            client.OnConnected = Client_Connected;
            client.OnDisconnected = Client_Disconnected;
            client.OnData = Client_Received;

            // use Debug.Log functions for Telepathy so we can see it in the console
            Telepathy.Log.Info = s => Logs.Log.Info(LogGroup, s);
            Telepathy.Log.Warning = s => Logs.Log.Warn(LogGroup, s);
            Telepathy.Log.Error = s => Logs.Log.Error(LogGroup, s);
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
                var ticks = ElapsedTicks;
                Send(new PingPacket { ticks = ticks });
                LastSentTick = ticks;
            }
        }

        public void Update()
        {
            SendPing();

            client.Tick(10000);
        }

        private void Client_Disconnected()
        {
            Log.Info(LogGroup, "Disconnected");

            Disconnected?.Invoke();
        }

        private void Client_Received(ArraySegment<byte> msg)
        {
            // ping answer
            if (CheckPing(msg)) return;

            var networkMessage = new NetworkMessage
            {
                dataSegment = msg
            };

            Received?.Invoke(ref networkMessage);
        }

        private void Client_Connected()
        {
            Log.Info(LogGroup, $"Connected - {Address}");

            Connected?.Invoke();
        }

        private bool CheckPing(ArraySegment<byte> msg)
        {
            if (msg[0] == 0 && msg[1] == 254)
            {
                var tick = BitConverter.ToInt64(msg.Array, 2);
                RTT = TimeConvert.TicksToMs(pingTicks.ElapsedTicks - tick);
                return true;
            }

            return false;
        }

        public void Send(ArraySegment<byte> bytes)
        {
            client.Send(bytes);
        }

        public void Send<T>(T packet) where T : struct, ISerializablePacketSegment
        {
            client.Send(packet.Serialize());
        }

        public void Start(string host, int port)
        {
            Address = $"{host}:{port}";
            Log.Debug(LogGroup, $"Connecting... - {Address}");
            client.Connect(host, port);
        }

        private const string LogGroup = "TelepathyClient";
        private string Address { get; set; }

    }

    public delegate void ReceiveNetworkMessage(ref NetworkMessage msg);
}
