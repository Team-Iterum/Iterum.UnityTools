using System;
using System.IO;
using Iterum.BaseSystems;
using UnityEditor;
using UnityEngine;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.ThingTypes
{

    public class ThingTypeSettings : ScriptableObject
    {
        private const string DefaultCategoryName = "Default";

        public string SavePath = "../Shared/ThingTypes";
        public string SaveDataPath = "../Shared/ThingData";
        public int ID = 1;

        public static string GetFilePath() => "Assets/Settings/ThingTypeSettings.asset";

        private static ThingTypeSettings privateInstance;
        public static ThingTypeSettings instance
        {
            get
            {
#if UNITY_EDITOR
                if (privateInstance == null)
                    privateInstance = AssetDatabase.LoadAssetAtPath<ThingTypeSettings>(GetFilePath());

                if (privateInstance == null)
                    privateInstance = Create();

                return privateInstance;
#else
return null;
#endif
            }
        }

        public static string GetEditorSavePath()
        {
            return instance != null
                ? instance.SavePath
                : null;
        }

#if UNITY_EDITOR
        private static ThingTypeSettings Create()
        {
            var settings = ScriptableObject.CreateInstance<ThingTypeSettings>();
            AssetDatabase.CreateAsset(settings, GetFilePath());
            AssetDatabase.Refresh();
            return settings;
        }
#endif

        public static NameCategory ParseName(string ttName)
        {
            var nameCategory = ttName.Split(new[] { " // ", "//", "/", " / " }, StringSplitOptions.RemoveEmptyEntries);

            NameCategory obj = default;
            if (nameCategory.Length == 1)
            {
                obj.Name = nameCategory[0];
                obj.Category = DefaultCategoryName;
            }
            else
            {
                obj.Name = nameCategory[0];
                obj.Category = nameCategory[1];
            }

            return obj;
        }


        public string GetPath(ThingType tt)
        {
            var path = TTStore.FindPath(tt.ID);
            if (string.IsNullOrEmpty(path)) path = Path.Combine(SavePath, GetFilename(tt));
            return path;
        }

        private string GetFilename(ThingType tt)
        {
            return $"{tt.Name} [{tt.ID}]";
        }

        public void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }

    public struct NameCategory
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
