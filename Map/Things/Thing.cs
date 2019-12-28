using UnityEngine;

namespace Magistr.Things
{
    public struct Thing
    {

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }

    }
}
