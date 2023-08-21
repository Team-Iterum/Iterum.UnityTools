using System;

namespace Iterum.Packets
{

    [AttributeUsage(AttributeTargets.Struct)]
    public class PacketAttribute : Attribute
    {

        public PacketAttribute(PacketDir type = PacketDir.Both, byte header = 0)
        {

        }
    }
}
