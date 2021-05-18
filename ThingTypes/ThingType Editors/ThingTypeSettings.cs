using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Iterum.ThingTypes
{
    
    public class ThingTypeSettings : ScriptableObject
    {
        private const string DefaultCategoryName = "Default";
        
        public string SavePath = "../Shared/ThingTypes";
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


        public string GetPath(ThingType tt)
        {
            return Path.Combine(SavePath, $"{tt.Name} [{tt.ID}].yml");
        }
        
        public string GetPath(ThingType tt, string ttName)
        {
            return Path.Combine(SavePath, $"{ttName}");
        }
        
    }

    public struct NameCategory
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }
}