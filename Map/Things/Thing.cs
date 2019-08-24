

using UnityEngine;

namespace Magistr.Things
{
    public struct Thing : IThing
    {
        private GameObject UnityObject;
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
            UnityObject = default;

        }

        public void SyncTransform()
        {
            UnityObject.transform.position = Position;
            UnityObject.transform.rotation = Rotation;
        }

        public void Create()
        {
            var thingType = ThingTypeManager.GetTningType(ThingTypeId);

            GameObject go = new GameObject(ThingTypeId.ToString());
        }

        public void Destroy()
        {
            GameObject.Destroy(UnityObject);
        }
    }
}
