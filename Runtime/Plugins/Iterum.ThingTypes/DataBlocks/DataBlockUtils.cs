using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public static class DataBlockUtils
    {
        public static IEnumerable<(string flagKeyword, Type dataBlock)> GetDataBlocksTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.GetInterfaces().Contains(typeof(IDataBlock)))
                    {
                        continue;
                    }
                    var attr = type.GetCustomAttribute(typeof(AutoRegisterDataBlock), true);
                    if (attr == null)
                    {
                        yield return (type.Name, type);
                    };

                    if (attr is AutoRegisterDataBlock regAttr)
                    {
                        yield return (regAttr.FlagKeyword, type);
                    }
                }
            }
        }
    }
}
