using System;

namespace Iterum.Network
{
    public struct NetworkMessage
    {
        public byte[] data;
        public ArraySegment<byte> dataSegment;

    }

}
