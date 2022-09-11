namespace Iterum.Network
{
    public struct PingPacket : ISerializablePacket
    {
        private static byte[] PingBuffer = {0, 255};
        
        public byte[] Serialize()
        {
            return PingBuffer;
        }

        public void Deserialize(byte[] packet)
        {
        }
        
        public static PingPacket Static = new PingPacket();
    }
}