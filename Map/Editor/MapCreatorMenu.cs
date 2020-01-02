#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Magistr.Things;
using UnityEditor.SceneManagement;
using UnityEngine;
using Magistr.Things.Editor;
using UnityEditor;

namespace Magistr.WorldMap.Editor
{
    public static class PacketGenerateMenu
    {
        private const string ThingsDir = @"";

        private const string StreamingThings = "Assets/StreamingAssets/Things/";
        public const string AssetContent = "Assets/Content/";
        public const string AssetThings = AssetContent + "Things/";

        [MenuItem("Things/Clear things")]
        public static void ClearThings()
        {
            foreach (var item in Object.FindObjectsOfType<ThingTypeGroup>())
            {
                item.ClearGenerated();
            }

            DirectoryExt.DeleteIfExit(AssetThings, true);
            AssetDatabase.DeleteAsset(AssetContent + "ThingTypes.asset");

            ThingTypeManager.ThingTypes.Clear();

            AssetDatabase.Refresh();
            EditorSceneManager.MarkAllScenesDirty();
            Debug.Log("Things cleared & removed");
        }


        [MenuItem("Things/Save map")]
        public static void SaveMap()
        {
            var filename = EditorUtility.SaveFilePanel("Save map", ThingsDir, "Default", "map");
            if (string.IsNullOrEmpty(filename)) return;
            
            var mapName = Path.GetFileNameWithoutExtension(filename);
            
            var map = new Map();
            map.LoadObjectsFromCurrentScene();
            
            map.Save(File.OpenWrite(filename), mapName);
            
            AssetDatabase.Refresh();
            Debug.Log($"Map saved {mapName}");
        }


        [MenuItem("Things/Save things")]
        public static void SaveThings()
        {
            var filename = EditorUtility.SaveFilePanel("Save things", ThingsDir, "Things", "dat");
            if (string.IsNullOrEmpty(filename)) return;

            // create if not exist all paths
            DirectoryExt.CreateIfNotExist(AssetContent);
            DirectoryExt.CreateIfNotExist(AssetThings);
            DirectoryExt.CreateIfNotExist(StreamingThings);

            // package prefab-ref
            var package = ScriptableObject.CreateInstance<ThingTypePackage>();
            package.ThingTypes = new List<GameObject>();

            //
            var added = new Dictionary<ThingTypeGroup, ThingTypeGroup>(ThingTypeGroup.ThingTypeGroupComparer);

            var groups = Object.FindObjectsOfType<ThingTypeGroup>();

            int i = 0;
            foreach (var item in groups)
            {

                if (!added.ContainsKey(item))
                {
                    item.ThingTypeId = i;
                    item.Created = true;

                    ThingTypeManager.ThingTypes.Add(i, item.Create());

                    package.ThingTypes.Add(item.UpdatePrefab());
                    added.Add(item, item);
                    
                    i++;
                }
                else
                {
                    item.ThingTypeId = added[item].ThingTypeId;
                    item.Created = true;
                }
            }
            
            // save unity-package with references to prefabs
            AssetDatabase.CreateAsset(package, AssetContent + "ThingTypes.asset");

            // save to server
            ThingTypeManager.Save(File.OpenWrite(filename), Application.productName + "_Things", 1);

            // save streaming
            var streamingFilename = Path.Combine(StreamingThings, Path.GetFileName(filename));
            ThingTypeManager.Save(File.OpenWrite(streamingFilename), Application.productName + "_Things", 1);


            AssetDatabase.Refresh();
            EditorSceneManager.MarkAllScenesDirty();

            Debug.Log("Things saved");
        }

        
    }

    internal class DirectoryExt
    {
        public static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void DeleteIfExit(string path, bool recursive)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }
    }
}

#endif
