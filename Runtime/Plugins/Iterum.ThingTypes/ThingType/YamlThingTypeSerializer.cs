using System;
using System.IO;
using System.Linq;
using Iterum.Logs;
using YamlDotNet.Serialization;

namespace Iterum.ThingTypes
{
    public class YamlThingTypeSerializer : IThingTypeSerializer
    {
        private readonly DeserializerBuilder deserBuilder;
        private readonly SerializerBuilder serBuilder;

        public string FileExtension => "yml";

        public YamlThingTypeSerializer()
        {
            deserBuilder = new DeserializerBuilder();

            serBuilder = new SerializerBuilder();
            serBuilder.DisableAliases();
            serBuilder.WithEventEmitter(e => new FlowFloatSequences(e));
            serBuilder.WithEventEmitter(e => new FlowIntSequences(e));

            var dataBlocks = DataBlockUtils.GetDataBlocksTypes().ToList();

            foreach (var (flagKeyword, dataBlock) in dataBlocks)
                serBuilder.WithTagMapping($"!{flagKeyword}", dataBlock);

            foreach (var (flagKeyword, dataBlock) in dataBlocks)
                deserBuilder.WithTagMapping($"!{flagKeyword}", dataBlock);
        }

        public ThingType Deserialize(string fileName)
        {
            var serializer = deserBuilder.Build();

            fileName = Path.ChangeExtension(fileName, FileExtension);

            return serializer.Deserialize<ThingType>(File.ReadAllText(fileName));
        }

        public void Serialize(string fileName, ThingType tt)
        {

            var serializer = serBuilder.Build();

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            fileName = Path.ChangeExtension(fileName, FileExtension);

            using StreamWriter w = File.AppendText(fileName);

            w.Write($"---\n" +
                    $"# ThingType #{tt.ID} {tt.Category}/{tt.Name}\n" +
                    $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                    $"\n");

            serializer.Serialize(w, tt);
        }
    }
}
