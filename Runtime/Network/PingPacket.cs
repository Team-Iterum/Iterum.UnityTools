using System;
using Iterum.Network;

namespace Iterum.BaseSystems
{

    public struct PingPacket : ISerializablePacketSegment
    {
        public long ticks;

        public ArraySegment<byte> Serialize()
        {
            var bytes = BitConverter.GetBytes(ticks);
            var targetBuffer = new byte[2 + bytes.Length];
            targetBuffer[0] = 0;
            targetBuffer[1] = 255;
            Buffer.BlockCopy(bytes, 0, targetBuffer, 2, bytes.Length);

            return new ArraySegment<byte>(targetBuffer);
        }

        public void Deserialize(ArraySegment<byte> packet)
        {
        }

    }
}
