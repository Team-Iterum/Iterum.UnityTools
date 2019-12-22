

using UnityEngine;

namespace Magistr.Things
{
    public struct Thing : IThing
    {
        private GameObject unityObject;
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }

        public Thing(int id, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            ThingTypeId = id;
            unityObject = default;

        }

        public void SyncTransform()
        {
            unityObject.transform.position = Position;
            unityObject.transform.rotation = Rotation;
        }

        public void Create()
        {
            var thingType = ThingTypeManager.GetTningType(ThingTypeId);

            var go = new GameObject(ThingTypeId.ToString());
        }

        public void Destroy()
        {
            Object.Destroy(unityObject);
        }
    }
}
