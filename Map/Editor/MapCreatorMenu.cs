#if UNITY_EDITOR
using Magistr.Things;
using Magistr.Things.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Magistr.WorldMap.Editor
{
    public static class PacketGenerateMenu
    {

        const string thingsDir = @"";

        [UnityEditor.MenuItem("Things/Clear things")]
        public static void ClearThings()
        {
            foreach (var item in GameObject.FindObjectsOfType<ThingTypeGroup>())
            {
                item.ThingTypeId = -1;
                item.Created = false;
                item.MapPosition = Vector3.zero;
                item.MapRotation = Quaternion.identity;
            }
            Directory.Delete("Assets/Content/Things", true);
            AssetDatabase.DeleteAsset("Assets/Content/ThingTypes.asset");
            ThingTypeManager.ThingTypes.Clear();
            EditorSceneManager.MarkAllScenesDirty();
        }

        [UnityEditor.MenuItem("Things/Save map")]
        public static void SaveMap()
        {
            Map map = new Map();

            map.LoadObjectsFromCurrentScene();

            Debug.Log(map.Entities.Count);

            var filename = EditorUtility.SaveFilePanel("Save map", thingsDir, "Default", "map");
            if (string.IsNullOrEmpty(filename)) return;

            map.Save(File.OpenWrite(filename), Application.productName + "_Map");
            AssetDatabase.Refresh();
        }

        [UnityEditor.MenuItem("Things/Save things")]
        public static void SaveThings()
        {
            var filename = EditorUtility.SaveFilePanel("Save things", thingsDir, "Things", "dat");
            var localFilename = Path.Combine("Assets/StreamingAssets/Things/" , Path.GetFileName(filename));

            if (string.IsNullOrEmpty(filename)) return;
            int i = 0;

            var package = ScriptableObject.CreateInstance<ThingTypePackage>();
            package.ThingTypes = new System.Collections.Generic.List<GameObject>();

            if (!Directory.Exists("Assets/Content/Things"))
            {
                Directory.CreateDirectory("Assets/Content/Things");
            }

            if (!Directory.Exists("Assets/Content/Things"))
            {
                Directory.CreateDirectory("Assets/Content/Things");
            }
            var comparer = new ThingTypeGroupComparer();
            Dictionary<ThingTypeGroup, ThingTypeGroup> added = new Dictionary<ThingTypeGroup, ThingTypeGroup>(comparer);
            var groups = GameObject.FindObjectsOfType<ThingTypeGroup>();
            
            
            foreach (var item in groups)
            {
                
                if (!added.ContainsKey(item))
                {
                    item.ThingTypeId = i;
                    item.Created = true;
                    ThingTypeManager.ThingTypes.Add(i, item.Create());
                    package.ThingTypes.Add(item.UpdatePrefab());
                    i++;
                    added.Add(item, item);
                } else
                {
                    item.ThingTypeId = added[item].ThingTypeId;
                    item.Created = true;
                }
            }
            AssetDatabase.CreateAsset(package, "Assets/Content/ThingTypes.asset");

            ThingTypeManager.Save(File.OpenWrite(filename), Application.productName + "_Things", 1, false);
            ThingTypeManager.Save(File.OpenWrite(localFilename), Application.productName + "_Things", 1, true);
            AssetDatabase.Refresh();
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}
#endif
