using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyButtons;
using Iterum.Logs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Iterum.ThingTypes
{
    [ExecuteInEditMode]
    public class MapDataSettings : MonoBehaviour
    {
        public Dictionary<string, string> MapDataAttrs; 
        
        public string SavePath = "Things/Maps";

        [Multiline]
        public string ExcludeCategory = string.Empty;

        public string MapName;

        private void Awake()
        {
            if (Application.isPlaying) Destroy(this);
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(MapName))
            {
                MapName = SceneManager.GetActiveScene().name.Replace(".scene", "");
            }
        }

#if UNITY_EDITOR
        [EasyButtons.Button("Create / Update refs", Mode = ButtonMode.DisabledInPlayMode)]
        [ContextMenu("Create / Update refs")]
        public void CreateUpdate()
        {
            MapData md;
            
            if (!File.Exists(GetPath(MapName)))
            {
                md = new MapData
                {
                    Name = MapName,
                    // Test attrs
                    Attrs = new Dictionary<string, string>()
                };
            }
            else
            {
                md = MapDataSerializer.Deserialize(GetPath(MapName));
            }
            
            var settings = ThingTypeSettings.instance;
            var refs = FindObjectsOfType<ThingTypeRef>();
            
            var exclude = ExcludeCategory
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Split(' ');;


            var thingTypes = ThingTypeSerializer.DeserializeAll(settings.SavePath);
            
            md.Refs = refs
                // exclude categories
                .Where(ttr => !exclude.Contains(ThingTypeSerializer.Find(thingTypes, ttr.ID).Category))
                .Select(ttRef => new MapRef
            {
                ID = ttRef.ID,
                name = ttRef.gameObject.name,
                tag = ttRef.GetComponent<ThingTypeMapRef>()  ?  ttRef.GetComponent<ThingTypeMapRef>().Tag : null,
                position = (Math.Vector3)ttRef.transform.position,
                rotation = (Math.Vector3)ttRef.transform.eulerAngles,

            }).ToArray();
            
            if (MapDataAttrs != null)
            {
                foreach (var attr in MapDataAttrs)
                {
                    if (!md.Attrs.ContainsKey(attr.Key))
                        md.Attrs.Add(attr.Key, attr.Value);
                    
                    md.Attrs[attr.Key] = attr.Value;
                }
            }
            
            MapDataSerializer.Serialize(GetPath(md.Name), md);
            
            
            Log.Success("MapDataSettings", $"Updated MapData {md.Name} RefsCount: {md.Refs.Length}");
        }

        private string GetPath(string mName)
        {
            return Path.Combine(SavePath, $"{mName}.yml");
        }
        

#endif
    }

}