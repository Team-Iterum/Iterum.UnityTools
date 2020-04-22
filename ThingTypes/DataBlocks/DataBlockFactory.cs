using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magistr.New.ThingTypes;
using UnityEngine;

namespace Iterum.ThingTypes
{
    public static class DataBlockFactory
    {
        private static readonly Dictionary<string, Func<GameObject, IDataBlock>> factory = new Dictionary<string, Func<GameObject, IDataBlock>>();
        
        public static void Register()
        {
            RegisterDynamicDataBlocks();
        }

        private static void RegisterDynamicDataBlocks()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute(typeof(AutoRegisterDataBlock), true);
                    if (attr == null || !type.GetInterfaces().Contains(typeof(IDataBlock))) continue;
                    
                    if (attr is AutoRegisterDataBlock regAttr)
                    {
                        Add(regAttr.FlagKeyword, o =>
                        {
                            var method = type.GetMethod(regAttr.FactoryMethod);
                            var instance = method?.Invoke(null, new object[] {o});
                            return instance as IDataBlock;
                        });
                    }
                }
            }
        }

        private static void Add(string attrName, Func<GameObject, IDataBlock> func)
        {
            if (factory.ContainsKey(attrName)) return;

            factory.Add(attrName, func);
        }

        public static List<IDataBlock> GetDataBlocks(GameObject go, IEnumerable<string> ttAttrs)
        {
            var dataBlocks = new List<IDataBlock>();
            if (ttAttrs == null) return new List<IDataBlock>();
            foreach (string attr in ttAttrs)
            {
                if (!factory.ContainsKey(attr)) continue;
                
                try
                {
                    var block = factory[attr].Invoke(go);
                    dataBlocks.Add(block);
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            return dataBlocks;
        }
    }
}