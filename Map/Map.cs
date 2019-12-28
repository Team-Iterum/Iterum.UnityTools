#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Math;
#else
using UnityEngine;
#endif
using Magistr.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Magistr.WorldMap
{
    public class Map
    {
        public List<Thing> Entities = new List<Thing>();

        public void Add(Thing t)
        {
            Entities.Add(t);
        }

        public void Remove(Thing t)
        {
            Entities.Remove(t);
        }

        private static MapArchive CreateArchive(string mapName)
        {
            var mapArchive = new MapArchive
            {
                Name = mapName,
                Created = DateTime.UtcNow
            };
            return mapArchive;

        }

        public void Save(FileStream fs, string mapName)
        {
            var mapArchive = CreateArchive(mapName);

            try
            {
                var formatter = new BinaryFormatter();
                mapArchive.Objects = Entities.Select(e => new MapObject() { 
                    Position = (Magistr.Math.Vector3)e.Position,
                    Rotation = (Magistr.Math.Quaternion)e.Rotation,
                    Scale = (Magistr.Math.Vector3)e.Scale,
                    ThingTypeId = e.ThingTypeId 
                    }).ToArray();

                formatter.Serialize(fs, mapArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serializer MapArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        public void Load(FileStream fs)
        {
            try
            {
                var formatter = new BinaryFormatter {Binder = new Binder()};

                var map = (MapArchive)formatter.Deserialize(fs);
                foreach (var item in map.Objects)
                {
                    if (!ThingTypeManager.HasThingType(item.ThingTypeId)) continue;
                    if (ThingTypeManager.GetThingType(item.ThingTypeId).HasAttr(ThingAttr.Static))
                    {
                        Add(new Thing
                        {
                            ThingTypeId = item.ThingTypeId,
                            Position = (Vector3) item.Position,
                            Rotation = (Quaternion) item.Rotation,
                            Scale = (Vector3) item.Scale
                        });
                    }
                }
            }
            catch (SerializationException e)
            {
                UnityEngine.Debug.LogError("Failed to deserialize MapArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }

        [Serializable]
        private struct MapArchive
        {
            public DateTime Created;
            public string Name;
            public MapObject[] Objects;
        }

        [Serializable]
        private struct MapObject
        {
            public int ThingTypeId;
            public Magistr.Math.Vector3 Position;
            public Magistr.Math.Quaternion Rotation;
            public Magistr.Math.Vector3 Scale;
        }
    }
}
