using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magistr.New.ThingTypes
{
    public static class DataBlockFactory
    {
        private static readonly Dictionary<string, Func<GameObject, IDataBlock>> factory = new Dictionary<string, Func<GameObject, IDataBlock>>();
        
        public static  void Add(string attrName, Func<GameObject, IDataBlock> func)
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