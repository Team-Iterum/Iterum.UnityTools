using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Iterum.BaseSystems;
#if UNITY_EDITOR
using System.IO;
using Iterum.Logs;
using UnityEditor;
#endif
using static Iterum.BaseSystems.TTManagerAlias;
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
        public void Create() => Create(this);

        public static void Create(Component self)
        {
            var settings = ThingTypeSettings.instance;

            var nameCategory = ThingTypeSettings.ParseName(self.name);

            List<string> flags = new();
            Dictionary<string, string> attrs = new();

            if (self.gameObject.GetComponent<Terrain>())
            {
                flags.Add("TerrainShape");
                attrs.Add("ThingClass", "Terrain");
            }
            if (self.gameObject.GetComponent<MeshFilter>()) flags.Add("ShapeMesh");
            if (self.gameObject.GetComponent<CapsuleCollider>()) flags.Add("ShapeCapsule");
            if (self.gameObject.GetComponent<SphereCollider>()) flags.Add("ShapeSphere");
            if (self.gameObject.GetComponent<BoxCollider>()) flags.Add("ShapeBox");

            if (self.gameObject.name.Contains("Tree")) attrs.Add("ThingClass", "Tree");


            var tt = new ThingType
            {
                ID = settings.ID,
                Name = nameCategory.Name,
                Description = string.Empty,
                Category = nameCategory.Category,
                Flags = flags.ToArray(),
                Attrs = attrs
            };

            ThingTypeLoader.Load();

            TTSerializer.Serialize(settings.GetPath(tt), tt);

            settings.ID += 1;

            DestroyImmediate(self.GetComponent<ThingTypeRef>());
            DestroyImmediate(self.GetComponent<ThingTypeUpdate>());

            var ttRef = self.gameObject.AddComponent<ThingTypeRef>();
            ttRef.ID = tt.ID;

            DestroyImmediate(self, true);

            settings.Save();

            ThingTypeLoader.RemoveAll();

            Log.Success("ThingTypeCreate", $"Created ThingType {tt.Category}/{tt.Name}");
        }
#endif
    }
}
