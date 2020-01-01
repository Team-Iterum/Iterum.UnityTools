using System.Linq;
using Magistr.WorldMap.Editor;

#if UNITY_EDITOR
using EasyButtons;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Vector3 = Magistr.Math.Vector3;

namespace Magistr.Things.Editor
{
    [ExecuteInEditMode]
    public class ThingTypeGroup : MonoBehaviour
    {
        private sealed class ThingTypeGroupEqualityComparer : IEqualityComparer<ThingTypeGroup>
        {
            public bool Equals(ThingTypeGroup x, ThingTypeGroup y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Category == y.Category && x.Title == y.Title && x.Description == y.Description && x.ShapeModel == y.ShapeModel && x.ShapeBox == y.ShapeBox && x.ShapeSphere == y.ShapeSphere && x.ShapeCapsule == y.ShapeCapsule && x.Static == y.Static && x.Dynamic == y.Dynamic && x.Kinematic == y.Kinematic && x.NoCollision == y.NoCollision && x.Invisible == y.Invisible && Equals(x.Data, y.Data);
            }

            public int GetHashCode(ThingTypeGroup obj)
            {
                unchecked
                {
                    var hashCode = (int) obj.Category;
                    hashCode = (hashCode * 397) ^ (obj.Title != null ? obj.Title.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Description != null ? obj.Description.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.ShapeModel.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ShapeBox.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ShapeSphere.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ShapeCapsule.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Static.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Dynamic.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Kinematic.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.NoCollision.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Invisible.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.Data != null ? obj.Data.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<ThingTypeGroup> ThingTypeGroupComparer { get; } = new ThingTypeGroupEqualityComparer();

        [Header("Main data")]
        public ThingCategory Category = ThingCategory.Item;
        public string Title = string.Empty;
        [Multiline]
        public string Description = string.Empty;
        
        [Header("Map data")]
        public bool Created;
        public UnityEngine.Quaternion MapRotation;
        public UnityEngine.Vector3 MapPosition;
        public int ThingTypeId = -1;

        [Header("Attributes")]
        public bool ShapeModel;
        public bool ShapeBox;
        public bool ShapeSphere;
        public bool ShapeCapsule;
        [Space]
        public bool Static;
        public bool Dynamic;
        public bool Kinematic;
        [Space]
        public bool NoCollision;
        public bool Invisible;

        [Header("Special")] 
        public float ModelScale;

        [Header("Data blocks")] 
        [SerializeField]
        public DataBlock[] Data;

        public void ClearGenerated()
        {
            Created = false;
            ThingTypeId = -1;
            MapPosition = UnityEngine.Vector3.zero;
            MapRotation = UnityEngine.Quaternion.identity;
        }

        public ThingType Create()
        {
            var tt = new ThingType()
            {
                Category = Category,
                ThingTypeId  = ThingTypeId,
                Title = Title,
                Description = Description,
            };

            if(ShapeModel)             tt.Attributes |= ThingAttr.ShapeModel;
            if(ShapeBox)               tt.Attributes |= ThingAttr.ShapeBox;
            if(ShapeSphere)            tt.Attributes |= ThingAttr.ShapeSphere;
            if(ShapeCapsule)            tt.Attributes |= ThingAttr.ShapeCapsule;

            if(Static)                 tt.Attributes |= ThingAttr.Static;
            if(Dynamic)                tt.Attributes |= ThingAttr.Dynamic;
            if(Kinematic)              tt.Attributes |= ThingAttr.Kinematic;

            if(NoCollision)            tt.Attributes |= ThingAttr.NoCollision;
            if(Invisible)              tt.Attributes |= ThingAttr.Invisible;
            
            tt.DataBlocks = Data;
            
            return tt;
        }

        private void OnEnable()
        {
            if (ModelScale == 0)
                ModelScale = 1;
            Data = new DataBlock[]
            {
                new SpawnData(),
                new ShapeBoxData(),
                new ShapeSphereData(),
                new ShapeModelData(),
                new ShapeCapsuleData()
            };

            if (GetComponent<MeshRenderer>())
            {
               

                var shapeBoxData = Data.FirstOrDefault(e => e is ShapeBoxData) as ShapeBoxData;
                if (shapeBoxData != null && ShapeBox)
                {
                    var size= GetComponent<BoxCollider>().size / 2;
                    shapeBoxData.HalfSize = new Vector3(size.x, size.y, size.z);
                }

                var shapeSphereData = Data.FirstOrDefault(e => e is ShapeSphereData) as ShapeSphereData;

                if (shapeSphereData != null && ShapeSphere)
                {
                    var bounds = GetComponent<MeshRenderer>().bounds;
                    shapeSphereData.Radius = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
                }

                var shapeModelData = Data.FirstOrDefault(e => e is ShapeModelData) as ShapeModelData;
                if (shapeModelData != null && ShapeModel)
                {
                    var mesh = GetComponent<MeshFilter>().sharedMesh;
                    shapeModelData.Points = mesh.vertices.Select(e => new Vector3(e.x * transform.lossyScale.x,
                        e.y * transform.lossyScale.y, e.z * transform.lossyScale.z) * ModelScale).ToArray();
                    shapeModelData.Triangles = mesh.triangles;
                }
            }


            if(GetComponent<CapsuleCollider>() && ShapeCapsule)
            {
                var capsuleCollider = GetComponent<CapsuleCollider>();
                var shapeCapsuleData = Data.FirstOrDefault(e => e is ShapeCapsuleData) as ShapeCapsuleData;

                if (shapeCapsuleData != null)
                {
                    shapeCapsuleData.Height = capsuleCollider.height;
                    shapeCapsuleData.Height = capsuleCollider.radius;
                }
            }

            if (string.IsNullOrEmpty(Title))
                Title = gameObject.name;
                    


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