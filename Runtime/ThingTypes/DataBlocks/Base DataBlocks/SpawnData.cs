using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("HasSpawn", nameof(Create))]
    public class SpawnData : IDataBlock
    {
        public float[] Position;
        public float RadiusX;
        public float RadiusY;

        public static IDataBlock Create(GameObject go)
        {

            var spawnData = new SpawnData()
            {
                Position = (Math.Vector3)go.transform.position,
                RadiusX = 0,
                RadiusY = 0,
            };

            if (go.GetComponent<SphereCollider>())
            {
                var bounds = go.GetComponent<SphereCollider>().bounds;
                spawnData.RadiusX = spawnData.RadiusY = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            }

            return spawnData;
        }

    }
}
