using System.Collections.Generic;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public struct ThingType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public string[] Flags { get; set; }
        public Dictionary<string, string> Attrs { get; set; }

        public IDataBlock[] DataBlocks { get; set; }
        
        public override string ToString()
        {
            return $"ThingType {Category}/{Name}";
        }

    }
    
}