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
        public List<IThing> Entities = new List<IThing>();

        public Map()
        {
        }

        public void Add(IThing t)
        {
            Entities.Add(t);
        }

        public void Remove(IThing t)
        {
            t.Destroy();
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
                BinaryFormatter formatter = new BinaryFormatter();
                mapArchive.Objects = Entities.Select(e => new MapObject() { Position = (Vector3)e.Position, Rotation = (Quaternion)e.Rotation, Scale = (Vector3)e.Scale, ThingTypeId = e.ThingTypeId }).ToArray();
                formatter.Serialize(fs, mapArchive);
            }
            catch (SerializationException e)
            {
                UnityEngine.Debug.LogError("Failed to serializer MapArchive. Reason: " + e.Message);
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
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new Binder();
                var map = (MapArchive)formatter.Deserialize(fs);
                foreach (var item in map.Objects)
                {
                    if (!ThingTypeManager.HasThingType(item.ThingTypeId)) continue;
                    if (ThingTypeManager.GetTningType(item.ThingTypeId).HasAttr(ThingAttr.Static))
                        Add(new Thing(item.ThingTypeId, (UnityEngine.Vector3)item.Position, (UnityEngine.Quaternion)item.Rotation, (UnityEngine.Vector3)item.Scale));
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
        struct MapArchive
        {
            public DateTime Created;
            public string Name;
            public MapObject[] Objects;
        }

        [Serializable]
        struct MapObject
        {
            public int ThingTypeId;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }
    }
}
