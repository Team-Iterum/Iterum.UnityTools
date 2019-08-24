#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Math;
using Magistr.Log;
#else
using UnityEngine;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Magistr.Things
{

    public static class ThingTypeManager
    {
        public static int Count => ThingTypes.Count;
        public static Dictionary<int, ThingType> ThingTypes;
        public static Dictionary<int, ThingType>.ValueCollection All => ThingTypes.Values;

       static ThingTypeManager()
        {
            ThingTypes = new Dictionary<int, ThingType>();
        }

        public static bool HasThingType(int thingTypeId)
        {
            return ThingTypes.ContainsKey(thingTypeId);
        }

        public static ThingType GetTningType(int thingTypeId)
        {
            if (HasThingType(thingTypeId))
            {
                return ThingTypes[thingTypeId];
            }
            return default;
        }

        private static ThingTypeArchive CreateArchive(string gameName, int version)
        {
            var thingsArchive = new ThingTypeArchive
            {
                Version = version,
                Name = gameName,
                Created = DateTime.UtcNow
            };
            return thingsArchive;

        }
        public static void Save(Stream fs, string gameName, int version, bool removeModels)
        {
            var thingsArchive = CreateArchive(gameName, version);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                thingsArchive.ThingTypes = ThingTypes.Values.Select(e =>
                {
                    e.Model = removeModels ? default : e.Model;
                    return e;
                }
                ).ToArray();
                formatter.Serialize(fs, thingsArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serializer ThingTypeArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        public static void Load(Stream fs)
        {
            try
            {
                ThingTypes = new Dictionary<int, ThingType>();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new Binder();
                var thingsArchive = (ThingTypeArchive)formatter.Deserialize(fs);
                for (int i = 0; i < thingsArchive.ThingTypes.Length; i++)
                {
                    thingsArchive.ThingTypes[i].ThingTypeId = i;
                    ThingTypes.Add(i, thingsArchive.ThingTypes[i]);
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to deserialize ThingTypeArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }

        [Serializable]
        struct ThingTypeArchive
        {
            public DateTime Created;
            public int Version;
            public string Name;
            public ThingType[] ThingTypes;
        }

    }

        sealed class Binder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type.
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));

                return typeToDeserialize;
            }
        }
}
