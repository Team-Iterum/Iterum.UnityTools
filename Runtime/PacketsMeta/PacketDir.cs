namespace Iterum.Packets
{

    public enum PacketDir
    {
        /// <summary>
        /// Server -> client and client -> server
        /// </summary>
        Both,

        /// <summary>
        /// Server -> client
        /// </summary>
        SC,

        /// <summary>
        /// Client -> server
        /// </summary>
        CS
    }
}
