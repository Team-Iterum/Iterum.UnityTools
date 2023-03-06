using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Iterum.Logs;

namespace Iterum.ThingTypes
{
    public interface IThingTypeSerializer
    {
        string FileExtension { get; }
        
        public ThingTypeStore DeserializeAll(string directory)
        {
            var things = new List<ThingType>();
            var thingsPaths = new Dictionary<int, string>();
            
            var files = Directory.EnumerateFiles(directory, $"*.{FileExtension}", SearchOption.AllDirectories).ToList();
            foreach (string fileName in files)
            {
                ThingType tt;
                
                try
                {
                    tt = Deserialize(fileName);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Filename: {fileName}", ex);
                }

                if(tt.Name == null) continue;
                
                things.Add(tt);
                thingsPaths.Add(tt.ID, fileName);
            }

            var store = new ThingTypeStore(things)
            {
                ThingTypePaths = thingsPaths
            };
            return store;
        }
        
        ThingType Deserialize(string fileName);
        void Serialize(string fileName, ThingType tt);
    }
}