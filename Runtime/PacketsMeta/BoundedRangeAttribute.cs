using System;

namespace Iterum.Packets
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class BoundedRangeAttribute : Attribute
    {
        public BoundedRangeAttribute(float minValue, float maxValue, float precision)
        {
        }
    }
}
