using EasyButtons;
using Iterum.DataBlocks;
using Iterum.Logs;
using UnityEngine;

namespace Iterum.ThingTypes
{
    public class ThingTypeUpdate : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isPlaying) Destroy(this);
        }
        
#if UNITY_EDITOR

        private static bool Check(Component self, out ThingTypeSettings settings, out ThingType tt)
        {
            settings = ThingTypeSettings.instance;
            var thingTypeRef = self.gameObject.GetComponent<ThingTypeRef>();

            tt = default;
            
            if (settings == null)
            {
                Log.Error("ThingTypeUpdate", "ThingTypeSettings not found");
                return false;
            }
            if (thingTypeRef == null)
            {
                Log.Error("ThingTypeUpdate", "ThingTypeRef not found");
                return false;
            }
                        
            tt = ThingTypeSerializer.Find(settings.SavePath, thingTypeRef.ID);
            if (tt.Name == null)
            {
                Log.Error("ThingTypeUpdate", $"ThingType '{thingTypeRef.ID}' not found");
                return false;
            }
            
            return true;
        }
       
        
        [Button("Update Name & Category", ButtonMode.DisabledInPlayMode)]
        public void UpdateNameCategory()
        {
            if (!Check(this, out ThingTypeSettings settings, out ThingType tt)) return;
            
            tt.Name = ThingTypeSettings.ParseName(name).Name;
            tt.Category = ThingTypeSettings.ParseName(name).Category;

            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);
            
            Log.Success("ThingTypeUpdate", $"Updated (Name, Category) ThingType {tt.Category}/{tt.Name}");
            
        }
      
        
        [Button("Update Data Blocks", ButtonMode.DisabledInPlayMode)]
        public void UpdateDataBlocks()
        {
            if (!Check(this, out ThingTypeSettings settings, out ThingType tt)) return;
            
            DataBlockFactory.Register();
            
            ShapeMeshData.Skip = false;
            ShapeMeshDataArray.Skip = false;
            
            var list = DataBlockFactory.GetDataBlocks(gameObject, tt.Flags, null);
            tt.DataBlocks = list.ToArray();

            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);
            
            DataBlockFactory.ClearRegister();
            
            Log.Success("ThingTypeUpdate", $"Updated (DataBlocks) ThingType {tt.Category}/{tt.Name}");
            
        }
        
        [Button("Update DataBlocks (no mesh)", ButtonMode.DisabledInPlayMode)]
        public void UpdateDataBlocksNoMesh()
        {
            if (!Check(this, out ThingTypeSettings settings, out ThingType tt)) return;

            DataBlockFactory.Register();
            
            ShapeMeshData.Skip = true;
            ShapeMeshDataArray.Skip = true;
            
            var list = DataBlockFactory.GetDataBlocks(gameObject, tt.Flags, null);
            tt.DataBlocks = list.ToArray();

            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);
            
            ShapeMeshData.Skip = false;
            ShapeMeshDataArray.Skip = false;
            
            DataBlockFactory.ClearRegister();
            
            Log.Success("ThingTypeUpdate", $"Updated (DataBlocks) (no mesh) ThingType {tt.Category}/{tt.Name}");
            
        }


#endif
    }
}