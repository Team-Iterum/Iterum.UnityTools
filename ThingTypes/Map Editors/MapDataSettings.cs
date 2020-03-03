using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magistr.New.ThingTypes
{
    [ExecuteInEditMode]
    public class MapDataSettings : MonoBehaviour
    {

        
        public string SavePath = "Assets/StreamingAssets/Maps";

        public string MapName;

        private void Awake()
        {
            if (Application.isPlaying) Destroy(this);
        }

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(MapName))
            {
                MapName = SceneManager.GetActiveScene().name.Replace(".scene", "");
            }
        }

#if UNITY_EDITOR
        [Button("Create / Update refs", ButtonMode.DisabledInPlayMode)]
        public void CreateUpdate()
        {
            var md = new MapData();
            
            if (!File.Exists(GetPath(MapName)))
            {
                md = new MapData()
                {
                    Name = MapName,
                    // Test attrs
                    Attrs = new Dictionary<string, string>()
                    {
                        {"AmbientLight", ColorUtility.ToHtmlStringRGBA(UnityEngine.RenderSettings.ambientLight)}
                    }
                };    
            }
            else
            {
                md = MapDataSerializer.Deserialize(GetPath(MapName));
            }
            

            var refs = FindObjectsOfType<ThingTypeRef>();

            md.Refs = refs.Select(ttRef => new MapRef()
            {
                ID = ttRef.ID,
                
                position = (Math.Vector3)ttRef.transform.position,
                rotation = (Math.Vector3)ttRef.transform.eulerAngles,

            }).ToArray();
            
            MapDataSerializer.Serialize(GetPath(md.Name), md);
            
            
            Debug.Log($"Updated MapData {md.Name} RefsCount: {md.Refs.Length}");
        }

        private string GetPath(string mName)
        {
            return Path.Combine(SavePath, $"{mName}.yaml");
        }
        

#endif
    }

}