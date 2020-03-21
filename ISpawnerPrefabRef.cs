using UnityEngine;

namespace Magistr.New.ThingTypes
{
    public interface IPrefabRef
    {
        int ID { get; }
        GameObject gameObject { get; }
    }
}