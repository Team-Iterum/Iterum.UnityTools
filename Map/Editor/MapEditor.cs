#if UNITY_EDITOR
using Magistr.Things;
using Magistr.Things.Editor;
using UnityEngine;

namespace Magistr.WorldMap.Editor
{
    public static class MapEditorExt
    {
        public static void LoadObjectsFromCurrentScene(this Map map)
        {
            var thingTypes = Object.FindObjectsOfType<ThingTypeGroup>();

            foreach (var item in thingTypes)
            {
                if(item.ThingTypeId > -1)
                {
                    var tt = ThingTypeManager.GetTningType(item.ThingTypeId);

                    if(tt.Category == ThingCategory.Item)
                    {
                        var pos = item.transform.position;
                        item.MapPosition = pos;
                        item.MapRotation = item.transform.rotation;

                        var thing = new Thing(item.ThingTypeId, pos, item.transform.rotation, item.transform.lossyScale);
                        map.Add(thing);
                    }
                    
                }
            }
        }
    }
}
#endif