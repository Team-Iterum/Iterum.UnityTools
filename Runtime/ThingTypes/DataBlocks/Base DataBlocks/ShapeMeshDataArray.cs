using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Iterum.DataBlocks;
using Iterum.ThingTypes;
using UnityEngine;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.DataBlocks
{
    [AutoRegisterDataBlock("ShapeMeshArray", nameof(Create))]
    public class ShapeMeshDataArray : IDataBlock
    {
        public static bool Skip = false;

        public string[] Name;

        public static IDataBlock Create(GameObject go)
        {
            var settings = ThingTypeSettings.instance;

            var list = new List<string>();
            var meshes = go.GetComponent<MeshArray>().Meshes;
            var meshFilters = new List<MeshFilter>();

            if (meshes.Length == 0)
            {
                meshFilters = go.GetComponent<MeshArray>().MesheFilters.ToList();
                meshes = go.GetComponent<MeshArray>().MesheFilters.Select(e => e.sharedMesh).ToArray();

            }

            var tt = TTStore.Find(go.GetComponent<ThingTypeRef>().ID);

            string dirPath = Path.Combine(settings.SaveDataPath, "Mesh");
            Directory.CreateDirectory(dirPath);

            for (int i = 0; i < meshes.Length; i++)
            {
                var mesh = meshes[i];
                // skip empty
                if (mesh.vertexCount == 0) continue;

                string name = $"{tt.Name}_Mesh_{i}";

                if (!Skip)
                {
                    Debug.Log($"Run GetMeshContent {i}");
                    var rot = Quaternion.identity;
                    if (meshFilters.Count > 0)
                    {
                        rot = meshFilters[i].transform.localRotation;
                    }
                    var localPos = Vector3.zero;
                    if (meshFilters.Count > 0)
                    {
                        localPos = meshFilters[i].transform.localPosition;
                    }
                    var localScale = go.transform.lossyScale;
                    if (meshFilters.Count > 0)
                    {
                        localScale = meshFilters[i].transform.localScale;
                    }

                    string content = ShapeMeshData.GetMeshContent(mesh, localPos, localScale, rot);
                    File.WriteAllText(Path.Combine(dirPath, $"{name}.txt"),
                        $"{ShapeMeshData.GetHeader(tt)}\n{content}");
                }

                list.Add(name);
            }

            return new ShapeMeshDataArray
            {
                Name = list.ToArray()
            };
        }
    }
}
