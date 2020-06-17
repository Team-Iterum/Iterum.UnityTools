using System;
using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("ShapeBox", nameof(Create))]
    public class ShapeBoxData : IDataBlock
    {
        public float[] HalfSize;
        
        public static IDataBlock Create(GameObject go)
        {
            if (!go.GetComponent<MeshRenderer>()) 
                throw new Exception("Factory ShapeBoxData: MeshRenderer not found");

            var bounds = go.GetComponent<MeshRenderer>().bounds;
            ShapeBoxData data = new ShapeBoxData
            {
                HalfSize = new Math.Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z)
            };
            return data;
        }
        
    }
}
