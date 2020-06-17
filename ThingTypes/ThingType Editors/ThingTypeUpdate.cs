using EasyButtons;
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
            settings = FindObjectOfType<ThingTypeSettings>();
            var thingTypeRef = self.gameObject.GetComponent<ThingTypeRef>();

            tt = default;
            
            if (settings == null)
            {
                Debug.LogError("ThingTypeSettings not found");
                return false;
            }
            if (thingTypeRef == null)
            {
                Debug.LogError("ThingTypeRef not found");
                return false;
            }
                        
            tt = ThingTypeSerializer.Find(settings.SavePath, thingTypeRef.ID);
            if (tt.Name == null)
            {
                Debug.LogError($"ThingType '{thingTypeRef.ID}' not found");
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
            
            Debug.Log($"Updated (Name, Category) ThingType {tt.Category}/{tt.Name}");
            
        }
      
        
        [Button("Update Data Blocks", ButtonMode.DisabledInPlayMode)]
        public void UpdateDataBlocks()
        {
            if (!Check(this, out ThingTypeSettings settings, out ThingType tt)) return;
            
            var list = DataBlockFactory.GetDataBlocks(gameObject, tt.Flags);
            tt.DataBlocks = list.ToArray();

            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);
            
            Debug.Log($"Updated (DataBlocks) ThingType {tt.Category}/{tt.Name}");
            
        }

#endif
    }
}