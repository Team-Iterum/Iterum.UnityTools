

using System;
using UnityEngine;

namespace Magistr.Things
{
    public interface IThing
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }
        int ThingTypeId { get; set; }
        void Create();
        void Destroy();
    }

    public interface ICreature : IThing
    {
        void Move(MoveDirection directions);
    }


    [Flags]
    public enum MoveDirection
    {
        None = 0,
        Forward = 1 << 0,
        Backward = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Up = 1 << 4,
        Down = 1 << 5
    }
}
