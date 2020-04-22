using UnityEngine;
using UnityEngine.Serialization;

namespace Iterum.ThingTypes
{
    public class ThingTypeRef : MonoBehaviour, IPrefabRef
    {
        [SerializeField]
        [FormerlySerializedAs("ID")]
        private int Id;

        public int ID
        {
            get => Id;
            internal set => Id = value;
        }
        
        
    }
}