#if UNITY_EDITOR
using Magistr.Things;
using Magistr.Things.Editor;

namespace Magistr.WorldMap.Editor
{
    public static class MapEditorExt
    {
        public static void LoadObjectsFromCurrentScene(this Map map)
        {
            var thingTypes = UnityEngine.Object.FindObjectsOfType<ThingTypeGroup>();

            foreach (var item in thingTypes)
            {
                if(item.ThingTypeId > -1)
                {
                    var tt = ThingTypeManager.GetThingType(item.ThingTypeId);
                    if (tt.Category == ThingCategory.Station || tt.Category == ThingCategory.Spawn)
                    {
                        var pos = item.transform.position;
                        item.MapPosition = pos;
                        item.MapRotation = item.transform.rotation;

                        var thing = new Thing()
                        {
                            Position = pos, Rotation = item.transform.rotation, Scale = item.transform.lossyScale,
                            ThingTypeId = item.ThingTypeId
                        };

                        map.Add(thing);
                    }
                }
            }
        }
    }
}
#endif