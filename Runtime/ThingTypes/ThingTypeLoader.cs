using System.Data;
using Iterum.BaseSystems.Utility;
using Iterum.Logs;
using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.BaseSystems
{
    public static class ThingTypeLoader
    {
        public static void Load()
        {

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                RemoveAll();
            }
#else
            if (TTManagerAlias.TTStore != null || TTManagerAlias.TTSerializer != null)
            {
                throw new DataException("Global state corrupt. TTStore already exist");
            }
#endif

            TTManagerAlias.Set(new YamlThingTypeSerializer());
            var pw = PerfWatch.StartNew("ThingTypeLoader");
            var store = TTManagerAlias.LoadStore(ThingTypeSettings.GetEditorSavePath(),
                Application.streamingAssetsPath + "/ThingTypes");
            pw.LogTotal();
            TTManagerAlias.Set(store);

            Log.Info("ThingTypeLoader", $"Loaded: {store.ThingTypes.Count}");
        }

        public static void RemoveAll()
        {
            TTManagerAlias.Set((IThingTypeSerializer)null);
            TTManagerAlias.Set((ThingTypeStore)null);
        }
    }
}