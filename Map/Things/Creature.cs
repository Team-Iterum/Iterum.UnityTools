

using UnityEngine;

namespace Magistr.Things
{
    public struct Creature : ICreature
    {
        #region IThing
        private GameObject UnityObject;
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }
        #endregion

        
        public Creature(int id, Vector3 position)
        {
            var thingType = ThingTypeManager.GetTningType(id);
            Position = position;
            ThingTypeId = id;
            UnityObject = null;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }

        public void Create()
        {
            var thingType = ThingTypeManager.GetTningType(ThingTypeId);
            UnityObject = new GameObject(ThingTypeId.ToString());
            //PhysicsObject = world.CreateCapsuleCharacter(Position, Vector3.up, Height, Radius);
        }

        public void Destroy()
        {
            GameObject.Destroy(UnityObject);
        }

        public void Move(MoveDirection directions)
        {
            //PhysicsObject.Move(directions, false);
        }

    }
}
