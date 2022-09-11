using System.Collections.Generic;
using System.Linq;
using EasyButtons;
#if UNITY_EDITOR
using System.IO;
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
        [Button("Create", Mode = ButtonMode.DisabledInPlayMode)]
        public void Create()
        {
            var settings = ThingTypeSettings.instance;

            var nameCategory = ThingTypeSettings.ParseName(name);

            List<string> flags = new();
            Dictionary<string, string> attrs = new();

            if (gameObject.GetComponent<Terrain>())
            {
                flags.Add("TerrainShape");
                attrs.Add("ThingClass", "Terrain");
            }
            if (gameObject.GetComponent<MeshFilter>()) flags.Add("ShapeMesh");
            if (gameObject.GetComponent<CapsuleCollider>()) flags.Add("ShapeCapsule");
            if (gameObject.GetComponent<SphereCollider>()) flags.Add("ShapeSphere");
            if (gameObject.GetComponent<BoxCollider>()) flags.Add("ShapeBox");
            
            if (gameObject.name.Contains("Tree")) attrs.Add("ThingClass", "Tree");

            
            var tt = new ThingType
            {
                ID = settings.ID,
                Name = nameCategory.Name, 
                Description = string.Empty, 
                Category = nameCategory.Category,
                Flags = flags.ToArray(),
                Attrs = attrs
            };
            
            ThingTypeSerializer.Serialize(settings.GetPath2(tt), tt);

            settings.ID += 1;
            
            DestroyImmediate(GetComponent<ThingTypeRef>());
            DestroyImmediate(GetComponent<ThingTypeUpdate>());
            
            var ttRef = gameObject.AddComponent<ThingTypeRef>();
            ttRef.ID = tt.ID;
            
            gameObject.AddComponent<ThingTypeUpdate>();
            DestroyImmediate(this);

            settings.Save();
            
            Log.Success("ThingTypeCreate", $"Created ThingType {tt.Category}/{tt.Name}");
        }
#endif
    }
}