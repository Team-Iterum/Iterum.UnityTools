using System;

namespace Iterum.Packets
{

    [AttributeUsage(AttributeTargets.Field)]
    public class UseIfAttribute : Attribute
    {

        public UseIfAttribute(string name)
        {

        }
    }
}