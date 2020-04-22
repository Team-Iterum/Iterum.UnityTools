using System;
using Iterum.ThingTypes;
using Magistr.New.ThingTypes;
using UnityEngine;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("ShapeSphere", nameof(Create))]
    public class ShapeSphereData : IDataBlock
    {
        public float Radius;
        
        public static IDataBlock Create(GameObject go)
        {
            if (!go.GetComponent<SphereCollider>()) 
                throw new Exception("Factory ShapeSphereData: Sphere collider not found");

            ShapeSphereData data = new ShapeSphereData {Radius = go.GetComponent<SphereCollider>().radius};
            return data;
        }

    }
}