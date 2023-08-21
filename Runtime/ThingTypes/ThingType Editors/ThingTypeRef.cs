using EasyButtons;
using UnityEngine;

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

#if UNITY_EDITOR

        [Button("Update Name & Category", Mode = ButtonMode.DisabledInPlayMode)]
        public void UpdateNameCategory() => ThingTypeUpdates.UpdateNameCategory(this);

        [Button("Update Data Blocks", Mode = ButtonMode.DisabledInPlayMode)]
        public void UpdateDataBlocks() => ThingTypeUpdates.UpdateDataBlocks(this);

        [Button("Update DataBlocks (no mesh)", Mode = ButtonMode.DisabledInPlayMode)]
        public void UpdateDataBlocksNoMesh() => ThingTypeUpdates.UpdateDataBlocksNoMesh(this);
#endif


    }
}
