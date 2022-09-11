using System;
using System.Collections.Generic;
using System.IO;
using Iterum.Logs;
using UnityEditor;
using UnityEngine;

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
                
                if(privateInstance == null)
                    privateInstance = Create();
                
                return privateInstance;
#else
return null;
#endif
            }
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
            var nameCategory = ttName.Split(new [] { " // ", "//", "/", " / "  }, StringSplitOptions.RemoveEmptyEntries);

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


        public string GetPath2(ThingType tt)
        {
            var path = ThingTypeSerializer.FindPath(SavePath, tt.ID);
            if (string.IsNullOrEmpty(path)) path = GetPath(tt);
            return path;
        }
        public string GetPath(ThingType tt)
        {
            return Path.Combine(SavePath, GetFilename(tt));
        }
        
        public string GetFilename(ThingType tt)
        {
            return $"{tt.Name} [{tt.ID}].yml";
        }
        
        public string GetPath(string ttName)
        {
            return Path.Combine(SavePath, $"{ttName}");
        }
        
        public string GetPathOrExist(ThingType tt)
        {
            string path = GetPath(tt);

            var files = Directory.GetFiles(SavePath, GetFilename(tt), SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                path = files[0];
                Log.Debug($"Found exist {path}");
            }

            return path;
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