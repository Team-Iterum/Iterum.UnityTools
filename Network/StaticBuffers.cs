﻿using NetStack.Buffers;
using NetStack.Serialization;
using NetStack.Threading;


namespace Magistr.Buffers
{
    public static class StaticBuffers
    {
        public static ArrayPool<byte> Buffers = ArrayPool<byte>.Create(1024, 50);

        public static ConcurrentPool<BitBuffer> BitBuffers = new ConcurrentPool<BitBuffer>(8, () => new BitBuffer(8));

        public static BitBuffer PacketToBitBuffer(byte[] packet)
        {
            var data = BitBuffers.Acquire();
            data.FromArray(packet, packet.Length);

            //Buffers.Return(packet);

            return data;
        }

        public static void Release(BitBuffer data)
        {
            data.Clear();
            BitBuffers.Release(data);
        }

        public static void Release(byte[] data)
        {
           Buffers.Return(data, true);
        }

        public static byte[] BitBufferToPacket(BitBuffer data)
        {
            var buffer = Buffers.Rent(data.Length);
            data.ToArray(buffer);
            Release(data);

            return buffer;
        }
    }
}
