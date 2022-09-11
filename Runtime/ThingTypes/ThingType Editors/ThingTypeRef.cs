using UnityEngine;
using UnityEngine.Serialization;

namespace Iterum.ThingTypes
{
    public class ThingTypeRef : MonoBehaviour, IPrefabRef
    {
        [SerializeField]
        private int Id;

        public int ID
        {
            get => Id;
            internal set => Id = value;
        }

        public void UpdateId(int id) => Id = id;
    }
}