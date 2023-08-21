using System;
using System.IO;
using System.Linq;
using System.Text;
using Iterum.ThingTypes;
using UnityEngine;
using Object = UnityEngine.Object;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("ShapeMesh", nameof(Create))]
    public class ShapeMeshData : IDataBlock
    {
        public static bool Skip = false;

        public string Name;


        public static IDataBlock Create(GameObject go)
        {
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
                throw new Exception("Factory ShapeMeshData: MeshFilter not found");

            Mesh sharedMesh = meshFilter.sharedMesh;

            var meshCollider = go.GetComponent<MeshCollider>();

            if (meshCollider != null && meshCollider.sharedMesh != meshFilter.sharedMesh)
                sharedMesh = meshCollider.sharedMesh;

            if (sharedMesh.vertices.Length == 0)
                throw new Exception("Mesh empty");


            var settings = ThingTypeSettings.instance;
            var tt = TTStore.Find(go.GetComponent<ThingTypeRef>().ID);

            ShapeMeshData data = new ShapeMeshData
            {
                Name = $"{tt.Name}_Mesh"
            };

            if (!Skip)
            {
                Debug.Log($"Run GetMeshContent");

                string content = GetMeshContent(sharedMesh, Vector3.zero, go.transform.lossyScale, Quaternion.identity);

                string dirPath = Path.Combine(settings.SaveDataPath, "Mesh");
                Directory.CreateDirectory(dirPath);

                File.WriteAllText(Path.Combine(dirPath, $"{data.Name}.txt"), $"{GetHeader(tt)}\n{content}");

            }

            return data;
        }

        public static string GetHeader(ThingType tt)
        {
            return $"// Mesh data '{tt.Category}/{tt.Name}'";
        }

        public static string GetMeshContent(Mesh mesh, Vector3 translate, Vector3 scale, Quaternion rot)
        {
            var meshVertices = mesh.vertices;
            StringBuilder vertices = new StringBuilder(meshVertices.Length);

            for (int j = 0; j < meshVertices.Length; j++)
            {
                var e = meshVertices[j];

                e = rot * e;

                e.x += translate.x * (1 / scale.x);
                e.y += translate.y * (1 / scale.y);
                e.z += translate.z * (1 / scale.z);

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
