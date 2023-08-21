using System;
using EasyButtons;
using UnityEngine;

namespace Iterum.ThingTypes
{
    [Obsolete]
    public class ThingTypeUpdate : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isPlaying) Destroy(this);
        }

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
