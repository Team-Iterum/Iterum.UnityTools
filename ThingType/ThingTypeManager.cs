﻿#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Log;
using Magistr.Utils;
#else
using Debug = UnityEngine.Debug;
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

        public static ThingType GetThingType(int thingTypeId)
        {
            return HasThingType(thingTypeId) ? ThingTypes[thingTypeId] : default;
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

        public static void Save(Stream fs, string gameName, int version)
        {
            var thingsArchive = CreateArchive(gameName, version);

            try
            {
                var formatter = new BinaryFormatter();

                thingsArchive.ThingTypes = ThingTypes.Values.ToArray();
                formatter.Serialize(fs, thingsArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError("[ThingTypeManager] Failed to serializer ThingTypeArchive. Reason: " + e.Message);
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
                var formatter = new BinaryFormatter
                {
                    Binder = new Binder()
                };

                var thingsArchive = (ThingTypeArchive) formatter.Deserialize(fs);
                for (int i = 0; i < thingsArchive.ThingTypes.Length; i++)
                {
                    ThingTypes.Add(thingsArchive.ThingTypes[i].ThingTypeId, thingsArchive.ThingTypes[i]);
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError("[ThingTypeManager] Failed to deserialize ThingTypeArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }

    }

    
    [Serializable]
    public struct ThingTypeArchive
    {
        public DateTime Created;
        public int Version;
        public string Name;
        public ThingType[] ThingTypes;
    }

    internal sealed class Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            var executingAssemblyName = Assembly.GetExecutingAssembly().FullName;

            // The following line of code returns the type.
            var typeToDeserialize = Type.GetType($"{typeName}, {executingAssemblyName}");

            return typeToDeserialize;
        }
    }
}
