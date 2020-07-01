using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Iterum.DataBlocks;
using Iterum.ThingTypes;
using UnityEngine;

namespace Iterum.Game
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
            
            var tt = ThingTypeSerializer.Find(settings.SavePath, go.GetComponent<ThingTypeRef>().ID);
            
            string dirPath = Path.Combine(settings.SavePath, "Mesh");
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
                    string content = ShapeMeshData.GetMeshContent(mesh, tt, go.transform.lossyScale);
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