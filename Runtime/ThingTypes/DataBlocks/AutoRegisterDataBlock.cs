using System;

namespace Iterum.ThingTypes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterDataBlock : Attribute
    {
        public readonly string FlagKeyword;

        public readonly string FactoryMethod;

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public AutoRegisterDataBlock(string flagKeyword, string factoryMethod)
        {
            FlagKeyword = flagKeyword;
            FactoryMethod = factoryMethod;
        }
    }
}