using NetStack.Serialization;
using NetStack.Threading;
using NetStack.Buffers;

namespace Iterum.Buffers2
{
    public static class BufferManager
    {
        public static readonly ArrayPool<byte> DataPool = ArrayPool<byte>.Shared;
        public static readonly ConcurrentPool<BitBuffer> BitBufferPool = new ConcurrentPool<BitBuffer>(8, () => new BitBuffer());

        public static BitBuffer ToBitBuffer(byte[] packet)
        {
            var bitBuffer = BitBufferPool.Acquire();
            bitBuffer.FromArray(packet, packet.Length);
            DataPool.Return(packet);
            return bitBuffer;
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

        public static byte[] ToData(BitBuffer bitBuffer)
        {
            byte[] data = DataPool.Rent(bitBuffer.Length);
            bitBuffer.ToArray(data);
            return data;
        }
    }
}