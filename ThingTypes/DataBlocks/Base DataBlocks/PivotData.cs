using System.Linq;
using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("HasPivots", nameof(Create))]
    public class PivotData : IDataBlock
    {
        public struct Pivot
        {
            public string Name;
            public float[] Position;
        }

        public Pivot[] Pivots;
        
        public static IDataBlock Create(GameObject go)
        {
            
            var pivotData = new PivotData();

            var pivots = go.GetComponentsInChildren<Transform>(true)
                .Where(e => e.name.Contains("Pivot"))
                .Select(e=> 
                    new PivotData.Pivot
                    {
                        Position = (Math.Vector3)e.transform.localPosition,
                        Name = e.name
                    }
                ).ToArray();
            
            pivotData.Pivots = pivots;

            return pivotData;
        }
        
    }
}