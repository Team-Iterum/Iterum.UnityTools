using Iterum.ThingTypes;

namespace Iterum.BaseSystems
{
    public static class TTManagerAlias
    {
        public static ThingTypeStore TTStore { get; private set; }
        public static IThingTypeSerializer TTSerializer { get; private set; }

        public static void Set(IThingTypeSerializer serializer)
        {
            TTSerializer = serializer;
        }

        public static void Set(ThingTypeStore store)
        {
            TTStore = store;
        }

        public static ThingTypeStore LoadStore(string editorPath, string runtimePath)
        {
            string path = null;
#if UNITY_EDITOR
            path = editorPath;
#else
            path = runtimePath;
#endif
            var store = TTSerializer.DeserializeAll(path);
            return store;
        }
    }
}
