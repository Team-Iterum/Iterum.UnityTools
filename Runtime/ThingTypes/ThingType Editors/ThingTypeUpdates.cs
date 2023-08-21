using EasyButtons;
using Iterum.BaseSystems;
using Iterum.DataBlocks;
using Iterum.Logs;
using UnityEngine;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.ThingTypes
{
    public static class ThingTypeUpdates
    {
#if UNITY_EDITOR

        private static bool Check(Component self, out ThingTypeSettings settings, out ThingType tt)
        {
            settings = ThingTypeSettings.instance;
            var thingTypeRef = self.gameObject.GetComponent<ThingTypeRef>();

            ThingTypeLoader.Load();

            tt = default;

            if (settings == null)
            {
                Log.Error("ThingTypeUpdate", "ThingTypeSettings not found");
                return false;
            }
            if (thingTypeRef == null)
            {
                Log.Error("ThingTypeUpdate", "ThingTypeRef not found");
                return false;
            }

            tt = TTStore.Find(thingTypeRef.ID);
            if (tt.Name == null)
            {
                Log.Error("ThingTypeUpdate", $"ThingType '{thingTypeRef.ID}' not found");
                return false;
            }

            return true;
        }

        public static void UpdateNameCategory(Component self)
        {
            if (!Check(self, out ThingTypeSettings settings, out ThingType tt)) return;

            tt.Name = ThingTypeSettings.ParseName(self.name).Name;
            tt.Category = ThingTypeSettings.ParseName(self.name).Category;

            TTSerializer.Serialize(settings.GetPath(tt), tt);

            Log.Success("ThingTypeUpdate", $"Updated (Name, Category) ThingType {tt.Category}/{tt.Name}");
        }


        public static void UpdateDataBlocks(Component self)
        {
            DataBlockFactory.Register();

            if (!Check(self, out ThingTypeSettings settings, out ThingType tt)) return;

            ShapeMeshData.Skip = false;
            ShapeMeshDataArray.Skip = false;
            TerrainShapeData.Skip = false;

            var list = DataBlockFactory.GetDataBlocks(self.gameObject, tt.Flags, null);
            tt.DataBlocks = list.ToArray();

            TTSerializer.Serialize(settings.GetPath(tt), tt);

            DataBlockFactory.ClearRegister();

            Log.Success("ThingTypeUpdate", $"Updated (DataBlocks) ThingType {tt.Category}/{tt.Name}");
        }

        public static void UpdateDataBlocksNoMesh(Component self)
        {
            DataBlockFactory.Register();

            if (!Check(self, out ThingTypeSettings settings, out ThingType tt)) return;

            ShapeMeshData.Skip = true;
            ShapeMeshDataArray.Skip = true;
            TerrainShapeData.Skip = true;

            var list = DataBlockFactory.GetDataBlocks(self.gameObject, tt.Flags, null);
            tt.DataBlocks = list.ToArray();

            TTSerializer.Serialize(settings.GetPath(tt), tt);

            ShapeMeshData.Skip = false;
            ShapeMeshDataArray.Skip = false;
            TerrainShapeData.Skip = false;

            DataBlockFactory.ClearRegister();

            Log.Success("ThingTypeUpdate", $"Updated (DataBlocks) (no mesh) ThingType {tt.Category}/{tt.Name}");
        }


#endif
    }
}
