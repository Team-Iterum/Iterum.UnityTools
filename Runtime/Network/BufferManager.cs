using System;
using System.Buffers;
using NetStack.Serialization;
using NetStack.Threading;

namespace Iterum.Buffers2
{

    public static class BufferManager
    {
        public static readonly ArrayPool<byte> DataPool = ArrayPool<byte>.Shared;

        public static readonly ConcurrentPool<BitBuffer> BitBufferPool =
            new ConcurrentPool<BitBuffer>(8, () => new BitBuffer());

        [ThreadStatic] private static BitBuffer staticBitBuffer;

        public static BitBuffer GetBitBuffer()
        {
            return staticBitBuffer ??= new BitBuffer(1024);
        }

        public static BitBuffer ToBitBuffer(ArraySegment<byte> packet)
        {
            var bitBuffer = BitBufferPool.Acquire();
            ReadOnlySpan<byte> packetLocal = packet;
            bitBuffer.FromSpan(ref packetLocal, packet.Count);
            DataPool.Return(packet.Array);
            return bitBuffer;
        }

        public static void Release(ArraySegment<byte> data)
        {
            DataPool.Return(data.Array);
        }

        public static BitBuffer Acquire()
        {
            return BitBufferPool.Acquire();
        }

        public static void Release(BitBuffer data)
        {
            data.Clear();
            BitBufferPool.Release(data);
        }

        public static ArraySegment<byte> ToData(BitBuffer bitBuffer)
        {
            byte[] data = DataPool.Rent(bitBuffer.Length);
            bitBuffer.ToArray(data);
            return data;
        }
    }
}