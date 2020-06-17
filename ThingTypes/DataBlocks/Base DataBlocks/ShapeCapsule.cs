using System;
using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("ShapeCapsule", nameof(Create))]
    public class ShapeCapsuleData : IDataBlock
    {
        public float Radius;
        public float Height;


        public static IDataBlock Create(GameObject go)
        {
            if (!go.GetComponent<CapsuleCollider>()) 
                throw new Exception("Factory ShapeCapsuleData: CapsuleCollider not found");

            var capsuleCollider = go.GetComponent<CapsuleCollider>();

            ShapeCapsuleData data = new ShapeCapsuleData 
            {
                Height = capsuleCollider.height, 
                Radius = capsuleCollider.radius
            };

            return data;
        }
    }
}