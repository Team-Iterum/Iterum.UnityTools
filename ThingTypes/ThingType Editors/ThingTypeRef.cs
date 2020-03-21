using UnityEngine;
using UnityEngine.Serialization;

namespace Magistr.New.ThingTypes
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