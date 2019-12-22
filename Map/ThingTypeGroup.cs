
using Magistr.WorldMap.Editor;
#if UNITY_EDITOR
using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Magistr.Things.Editor
{
    public class ThingTypeGroupComparer : IEqualityComparer<ThingTypeGroup>
    {
        public bool Equals(ThingTypeGroup self, ThingTypeGroup other)
        {
            // Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return self.Description.Equals(other.Description) && self.Category.Equals(other.Category) && self.Ground.Equals(other.Ground) && self.Container.Equals(other.Container) && self.Writable.Equals(other.Writable) && self.WritableOnce.Equals(other.WritableOnce) && self.Walkable.Equals(other.Walkable) && self.Moveable.Equals(other.Moveable) && self.NotBlockProjectile.Equals(other.NotBlockProjectile) && self.Pickupable.Equals(other.Pickupable) && self.Rotateable.Equals(other.Rotateable) && self.Usable.Equals(other.Usable) && self.Teleport.Equals(other.Teleport) && self.Model.Equals(other.Model) && self.BoxGeometry.Equals(other.BoxGeometry) && self.SphereGeometry.Equals(other.SphereGeometry) && self.Static.Equals(other.Static) && self.Dynamic.Equals(other.Dynamic) && self.Size.Equals(other.Size) && self.Radius.Equals(other.Radius);
        }

        public int GetHashCode(ThingTypeGroup self)
        {
            unchecked
            {
                int hash = (int)2166136261;
                if (self.Description != null) hash = (hash * 16777619) ^ self.Description.GetHashCode();
                hash = (hash * 16777619) ^ self.Category.GetHashCode();
                hash = (hash * 16777619) ^ self.Ground.GetHashCode();
                hash = (hash * 16777619) ^ self.Container.GetHashCode();
                hash = (hash * 16777619) ^ self.Writable.GetHashCode();
                hash = (hash * 16777619) ^ self.WritableOnce.GetHashCode();
                hash = (hash * 16777619) ^ self.Walkable.GetHashCode();
                hash = (hash * 16777619) ^ self.Moveable.GetHashCode();
                hash = (hash * 16777619) ^ self.NotBlockProjectile.GetHashCode();
                hash = (hash * 16777619) ^ self.Pickupable.GetHashCode();
                hash = (hash * 16777619) ^ self.Rotateable.GetHashCode();
                hash = (hash * 16777619) ^ self.Usable.GetHashCode();
                hash = (hash * 16777619) ^ self.Teleport.GetHashCode();
                hash = (hash * 16777619) ^ self.Model.GetHashCode();
                hash = (hash * 16777619) ^ self.BoxGeometry.GetHashCode();
                hash = (hash * 16777619) ^ self.SphereGeometry.GetHashCode();
                hash = (hash * 16777619) ^ self.Static.GetHashCode();
                hash = (hash * 16777619) ^ self.Dynamic.GetHashCode();
                hash = (hash * 16777619) ^ self.Size.GetHashCode();
                hash = (hash * 16777619) ^ self.Radius.GetHashCode();
                return hash;
            }
        }

    }
    [ExecuteInEditMode]
    public class ThingTypeGroup : MonoBehaviour
    {

        [Header("Main data")]
        public ThingCategory Category = ThingCategory.Item;
        public string Description = string.Empty;
        
        [Header("Generated data")]
        public bool Created;
        public Quaternion MapRotation;
        public Vector3 MapPosition;
        public int ThingTypeId = -1;

        [Header("Attributes")]
        public bool Ground;
        public bool Container;
        public bool Writable;
        public bool WritableOnce;
        public bool Walkable;
        public bool Moveable;
        public bool NotBlockProjectile;
        public bool Pickupable;
        public bool Rotateable;
        public bool Usable;
        public bool Teleport;
        public bool Model;
        public bool BoxGeometry;
        public bool SphereGeometry;
        public bool Static = true;
        public bool Dynamic;

        [Header("Additional data")]
        public Math.Vector3 Size;
        public float Radius;


        public void ClearGenerated()
        {
            Created = false;
            ThingTypeId = -1;
            Created = false;
            MapPosition = Vector3.zero;
            MapRotation = Quaternion.identity;
        }

        public ThingType Create()
        {
            var tt = new ThingType()
            {
                Category = Category,
                Description = Description,
            };
            if (Ground) tt.Attributes |= ThingAttr.Ground;
            if (Container) tt.Attributes |= ThingAttr.Container;
            if (Writable) tt.Attributes |= ThingAttr.Writable;
            if (WritableOnce) tt.Attributes |= ThingAttr.WritableOnce;
            if (Walkable) tt.Attributes |= ThingAttr.Walkable;
            if (Moveable) tt.Attributes |= ThingAttr.Moveable;
            if (NotBlockProjectile) tt.Attributes |= ThingAttr.NotBlockProjectile;
            if (Pickupable) tt.Attributes |= ThingAttr.Pickupable;
            if (Rotateable) tt.Attributes |= ThingAttr.Rotateable;
            if (Usable) tt.Attributes |= ThingAttr.Usable;
            if (Teleport) tt.Attributes |= ThingAttr.Teleport;
            if (Model) tt.Attributes |= ThingAttr.Model;
            if (BoxGeometry) tt.Attributes |= ThingAttr.BoxGeometry;
            if (SphereGeometry) tt.Attributes |= ThingAttr.SphereGeometry;
            if (Static) tt.Attributes |= ThingAttr.Static;
            if (Dynamic) tt.Attributes |= ThingAttr.Dynamic;

            tt.Size = Size;
            tt.Radius = Radius;

            return tt;
        }

        private void OnEnable()
        {
            if (GetComponent<MeshRenderer>())
            {
                var bounds = GetComponent<MeshRenderer>().bounds;

                if (Size.magnitude == 0)
                    Size = (Math.Vector3)bounds.size;
                if (Radius == 0)
                    Radius = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            }
            if(GetComponent<CapsuleCollider>())
            {
                var capsuleCollider = GetComponent<CapsuleCollider>();
                Size = (Math.Vector3)new Vector3(0, capsuleCollider.height, 0);
                Radius = capsuleCollider.radius;
            }

            if (string.IsNullOrEmpty(Description))
                Description = gameObject.name;
        }


        [Button("Update", ButtonMode.DisabledInPlayMode, ButtonSpacing.After)]
        public void UpdateThingType()
        {
            if (ThingTypeId < 0) return;

            var tt = Create();
            ThingTypeManager.ThingTypes[ThingTypeId] = tt;
            
            UpdatePrefab();
        }

        public GameObject UpdatePrefab()
        {
            if (ThingTypeId < 0) return null;

            var prefab = PrefabUtility.SaveAsPrefabAsset(this.gameObject, PacketGenerateMenu.AssetThings + ThingTypeId + ".prefab");
            DestroyImmediate(prefab.GetComponent<ThingTypeGroup>(), true);

            return prefab;

        }

        
    }
}

#endif