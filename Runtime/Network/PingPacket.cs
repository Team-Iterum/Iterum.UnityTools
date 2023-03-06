using System;
using Iterum.Network;

namespace Iterum.BaseSystems
{

    public struct PingPacket : ISerializablePacketSegment
    {
        private static ArraySegment<byte> PingBuffer = new byte[] { 0, 255 };

        public ArraySegment<byte> Serialize()
        {
            return PingBuffer;
        }

        public void Deserialize(ArraySegment<byte> packet)
        {
        }

        public static PingPacket Static = new PingPacket();
    }
}