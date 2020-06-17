using System;
using System.IO;
using UnityEngine;

namespace Iterum.ThingTypes
{
    [ExecuteInEditMode]
    public class ThingTypeSettings : MonoBehaviour
    {
        private const string DefaultCategoryName = "Default";
        
        public string SavePath = "Things/ThingTypes";
        public int ID = 1;

        
        private void OnEnable()
        {
            DataBlockFactory.Register();
        }

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