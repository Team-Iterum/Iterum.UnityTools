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
            var meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = go.GetComponentInChildren<MeshRenderer>();

            if (!meshRenderer)
                throw new Exception("Factory ShapeBoxData: MeshRenderer not found");

            var bounds = meshRenderer.bounds;
            ShapeBoxData data = new ShapeBoxData
            {
                HalfSize = new Math.Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z)
            };
            return data;
        }

    }
}
