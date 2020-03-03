using System;
using System.IO;
using System.Linq;
using Magistr.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Magistr.New.ThingTypes
{
    public static class BaseDataBlocksFactory
    {
        public static void Register()
        {
            DataBlockFactory.Add("ShapeBox", ShapeBox);
            DataBlockFactory.Add("ShapeSphere", ShapeSphere);
            DataBlockFactory.Add("ShapeMesh", ShapeMesh);
            DataBlockFactory.Add("ShapeCapsule", ShapeCapsule);
            DataBlockFactory.Add("HasPivots", PivotData);
            DataBlockFactory.Add("IsSpawn", SpawnData);
        }

        private static IDataBlock SpawnData(GameObject go)
        {
            var spawnData = new SpawnData() {
                Position = (Math.Vector3)go.transform.position,
                RadiusX = 0,
                RadiusY = 0,
            };

            if (go.GetComponent<SphereCollider>())
            {
                var bounds = go.GetComponent<SphereCollider>().bounds;
                spawnData.RadiusX = spawnData.RadiusY = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            }

            return spawnData;
        }

        private static IDataBlock PivotData(GameObject go)
        {
            var pivotData = new PivotData();

            var pivots = go.GetComponentsInChildren<Transform>(true)
                .Where(e => e.name.Contains("Pivot"))
                .Select(e=> 
                    new PivotData.Pivot
                    {
                        Position = (Math.Vector3)e.transform.localPosition,
                        Name = e.name
                    }
                ).ToArray();
            
            pivotData.Pivots = pivots;

            return pivotData;
        }

        private static IDataBlock ShapeCapsule(GameObject go)
        {
            if (!go.GetComponent<CapsuleCollider>()) 
                throw new Exception("Factory ShapeCapsuleData: CapsuleCollider not found");

            var capsuleCollider = go.GetComponent<CapsuleCollider>();

            ShapeCapsuleData data = new ShapeCapsuleData 
            {
                Height = capsuleCollider.height, 
                Radius = capsuleCollider.radius
            };

            return data;
        }

        private static IDataBlock ShapeMesh(GameObject go)
        {
            if (!go.GetComponent<MeshFilter>()) 
                throw new Exception("Factory ShapeMeshData: MeshFilter not found");


            var settings = Object.FindObjectOfType<ThingTypeSettings>();
            var tt = ThingTypeSerializer.Find(settings.SavePath, go.GetComponent<ThingTypeRef>().ID);

            ShapeMeshData data = new ShapeMeshData
            {
                Name = $"{tt.Name}_MeshData"
            };


            var mesh = go.GetComponent<MeshFilter>().sharedMesh;

            var points = mesh.vertices.Select(e =>
                new Math.Vector3(e.x * go.transform.lossyScale.x,
                    e.y * go.transform.lossyScale.y,
                    e.z * go.transform.lossyScale.z));

            string vertices = string.Join("\n", points.Select(e => $"v {e.x}/{e.y}/{e.z}"));
            string indices  = string.Join("\n", mesh.triangles.Select(e => $"i {e}"));

            File.WriteAllText(Path.Combine(settings.GetPath(tt, $"{data.Name}.txt")), $"// Mesh data '{tt.Category}/{tt.Name}'\n{vertices}\n{indices}");
            return data;
        }

        private static IDataBlock ShapeSphere(GameObject go)
        {
            if (!go.GetComponent<SphereCollider>()) 
                throw new Exception("Factory ShapeSphereData: Sphere collider not found");

            ShapeSphereData data = new ShapeSphereData {Radius = go.GetComponent<SphereCollider>().radius};
            return data;
        }

        private static IDataBlock ShapeBox(GameObject go)
        {
            if (!go.GetComponent<MeshRenderer>()) 
                throw new Exception("Factory ShapeBoxData: MeshRenderer not found");

            var bounds = go.GetComponent<MeshRenderer>().bounds;
            ShapeBoxData data = new ShapeBoxData
            {
                HalfSize = new Math.Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z)
            };
            return data;
        }
    }
}