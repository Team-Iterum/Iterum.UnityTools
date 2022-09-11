using System;

namespace Iterum.Packets
{

    [AttributeUsage(AttributeTargets.Field)]
    public class BitsAttribute : Attribute
    {

        public BitsAttribute(int bits)
        {

        }
    }
}