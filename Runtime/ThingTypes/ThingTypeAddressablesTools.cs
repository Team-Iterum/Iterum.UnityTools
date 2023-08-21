#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Iterum.Logs;
using Iterum.ThingTypes;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Iterum.BaseSystems
{
    public static class ThingTypeAddressablesTools
    {
        [MenuItem("Tools/Refresh ThingType Addressables")]
        public static void RefreshAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.FindGroup("ThingTypes") ?? settings.DefaultGroup;
            var dirToFind = "Assets/ThingTypeRefs";

            var dirs = AssetDatabase.GetSubFolders(dirToFind).ToList();
            dirs.Insert(0, dirToFind);
            var guids = AssetDatabase.FindAssets("t:Prefab", dirs.ToArray());

            var entriesAdded = new List<AddressableAssetEntry>();
            foreach (string guid in guids)
            {
                var ttRef = AssetDatabase.LoadAssetAtPath<ThingTypeRef>(AssetDatabase.GUIDToAssetPath(guid));
                if (ttRef == null) return;

                var assetEntry = settings.FindAssetEntry(guid);
                if (assetEntry != null)
                {
                    group = assetEntry.parentGroup;
                }

                var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                entry.address = $"ThingType_{ttRef.ID}";

                entriesAdded.Add(entry);
            }

            Log.Success("ThingTypeAddressables", $"Entries updated: {entriesAdded.Count}");
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
        }
    }
}
#endif
