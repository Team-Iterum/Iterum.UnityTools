using EasyButtons;
using UnityEngine;

namespace Magistr.New.ThingTypes
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
            var settings = FindObjectOfType<ThingTypeSettings>();
            if (settings == null)
            {
                Debug.LogError("ThingTypeSettings not found");
                return;
            }

            var nameCategory = ThingTypeSettings.ParseName(name);

            
            
            var tt = new ThingType
            {
                ID = settings.ID,
                Name = nameCategory.Name, 
                Description = string.Empty, 
                Category = nameCategory.Category,
            };

            settings.ID += 1;
            
            ThingTypeSerializer.Serialize(settings.GetPath(tt), tt);

            DestroyImmediate(GetComponent<ThingTypeRef>());
            DestroyImmediate(GetComponent<ThingTypeUpdate>());
            
            var ttRef = gameObject.AddComponent<ThingTypeRef>();
            ttRef.ID = tt.ID;
            
            gameObject.AddComponent<ThingTypeUpdate>();
            DestroyImmediate(this);
            
            Debug.Log($"Created ThingType {tt.Category}/{tt.Name}");
        }
#endif
    }
}