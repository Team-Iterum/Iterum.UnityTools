using System;
using System.IO;
using System.Linq;
using System.Text;
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

            if (meshFilter.sharedMesh.vertices.Length == 0) 
                throw new Exception("Mesh empty");

            var settings = Object.FindObjectOfType<ThingTypeSettings>();
            var tt = ThingTypeSerializer.Find(settings.SavePath, go.GetComponent<ThingTypeRef>().ID);

            ShapeMeshData data = new ShapeMeshData
            {
                Name = $"{tt.Name}_Mesh"
            };

            string content = GetMeshContent(meshFilter.sharedMesh, tt, go.transform.lossyScale);
            
            string dirPath = Path.Combine(settings.SavePath, "Mesh");
            Directory.CreateDirectory(dirPath);
            
            File.WriteAllText(Path.Combine(dirPath, $"{data.Name}.txt"), $"{GetHeader(tt)}\n{content}");
            
            return data;
        }

        public static string GetHeader(ThingType tt)
        {
            return $"// Mesh data '{tt.Category}/{tt.Name}'";
        }

        public static string GetMeshContent(Mesh mesh, ThingType tt, Vector3 scale)
        {
            var meshVertices = mesh.vertices;    
            StringBuilder vertices = new StringBuilder(meshVertices.Length);

            for (int j = 0; j < meshVertices.Length; j++)
            {
                var e = meshVertices[j];
                
                e.x *= scale.x;
                e.y *= scale.y;
                e.z *= scale.z;
                
                vertices.AppendFormat("\nv {0}/{1}/{2}", e.x, e.y, e.z);

            }
            
            var meshTriangles = mesh.triangles;
            StringBuilder indices = new StringBuilder(meshTriangles.Length);
            
            for (int j = 0; j < meshTriangles.Length; j++)
            {
                int e = meshTriangles[j];
                vertices.AppendFormat("\ni {0}", e);
            }

            return $"\n{vertices}\n{indices}";
        }


    }
}