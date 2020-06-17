using System;
using System.IO;
using System.Linq;
using Iterum.ThingTypes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("ShapeMesh", nameof(Create))]
    public class ShapeMeshData : IDataBlock
    {
        public string Name;


        public static IDataBlock Create(GameObject go)
        {
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
                throw new Exception("Factory ShapeMeshData: MeshFilter not found");


            var settings = Object.FindObjectOfType<ThingTypeSettings>();
            var tt = ThingTypeSerializer.Find(settings.SavePath, go.GetComponent<ThingTypeRef>().ID);

            ShapeMeshData data = new ShapeMeshData
            {
                Name = $"{tt.Name}_Mesh"
            };


            var mesh = meshFilter.sharedMesh;

            var points = mesh.vertices.Select(e =>
                new Math.Vector3(e.x * go.transform.lossyScale.x,
                    e.y * go.transform.lossyScale.y,
                    e.z * go.transform.lossyScale.z));

            string vertices = string.Join("\n", points.Select(e => $"v {e.x}/{e.y}/{e.z}"));
            string indices  = string.Join("\n", mesh.triangles.Select(e => $"i {e}"));

            string dirPath = Path.Combine(settings.SavePath, "Mesh");
            Directory.CreateDirectory(dirPath);
            
            File.WriteAllText(Path.Combine(dirPath, $"{data.Name}.txt"), $"// Mesh data '{tt.Category}/{tt.Name}'\n{vertices}\n{indices}");
            
            return data;
        }


    }
}