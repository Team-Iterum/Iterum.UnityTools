using System;
using System.Collections.Generic;
using System.Linq;
using Iterum.Logs;
using Random = UnityEngine.Random;

namespace Iterum.ThingTypes
{
    public class ThingTypeStore
    {
        public Dictionary<int, ThingType> ThingTypes { get; }
        public Dictionary<int, string> ThingTypePaths { get; set; }

        public ThingType Find(string ttName)
        {
            var tt = ThingTypes.Values.FirstOrDefault(e => e.Name == ttName);
            return tt;
        }
        
        public ThingType Find(int id)
        {
            return ThingTypes.TryGetValue(id, out var tt) ? tt : default;
        }

        public ThingTypeStore(IEnumerable<ThingType> thingTypes)
        {
            ThingTypes = thingTypes.ToDictionary(e => e.ID, e => e);
        }

        public string FindPath(int id)
        {
            return ThingTypePaths.TryGetValue(id, out var fileName) ? fileName : default;
        }

        public ThingType GetRandom(Func<KeyValuePair<int, ThingType>, bool> e)
        {
            var list = ThingTypes.Where(e).ToList();

            return list[Random.Range(0, list.Count)].Value;
        }
    }
}