using System;

namespace Magistr.Network
{
    public struct NetworkMessage
    {
        public byte[] data;
        public int length;
        public UInt32 connection;
        public long userData;
        public Int64 timeReceived; // microsecunds
        public long messageNumber;
        public int channel;
    }
}
