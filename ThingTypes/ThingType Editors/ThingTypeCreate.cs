using EasyButtons;
#if UNITY_EDITOR
using Iterum.Logs;
using UnityEditor;
#endif
using UnityEngine;

namespace Iterum.ThingTypes
{
    public class ThingTypeCreate : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isPlaying) Destroy(this);
        }
        
#if UNITY_EDITOR
        [Button("Create", ButtonMode.DisabledInPlayMode)]
        public void Create()
        {
            var settings = ThingTypeSettings.instance;

            var nameCategory = ThingTypeSettings.ParseName(name);
            
            var tt = new ThingType
            {
                ID = settings.ID,
                Name = nameCategory.Name, 
                Description = string.Empty, 
                Category = nameCategory.Category,
            };
            
            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);

            settings.ID += 1;
            
            DestroyImmediate(GetComponent<ThingTypeRef>());
            DestroyImmediate(GetComponent<ThingTypeUpdate>());
            
            var ttRef = gameObject.AddComponent<ThingTypeRef>();
            ttRef.ID = tt.ID;
            
            gameObject.AddComponent<ThingTypeUpdate>();
            DestroyImmediate(this);
            
            Log.Success("ThingTypeCreate", $"Created ThingType {tt.Category}/{tt.Name}");
        }
#endif
    }
}