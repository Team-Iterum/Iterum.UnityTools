using UnityEngine;

namespace Iterum.ThingTypes
{
    public interface IPrefabRef
    {
        int ID { get; }
        GameObject gameObject { get; }
    }
}