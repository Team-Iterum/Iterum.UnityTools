using System;
using System.IO;
using System.Runtime.InteropServices;
using Iterum.ThingTypes;
using NetStack.Serialization;
using UnityEngine;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Base Data block
    /// </summary>
    [AutoRegisterDataBlock("TerrainShape", nameof(Create))]
    public class TerrainShapeData : IDataBlock
    {
        public static bool Skip { get; set; }
        
        public string Heightmap;
        public float HeightmapResoultion;
        public float[] HeightmapScale;

        public static IDataBlock Create(GameObject go)
        {
            var settings = ThingTypeSettings.instance;
            
            var td = go.GetComponent<Terrain>().terrainData;
            if (td == null)
                td = go.GetComponentInChildren<Terrain>().terrainData;
            
            if (!td) 
                throw new Exception("Factory TerrainShape: TerrainData not found");
            
            var tt = TTStore.Find(go.GetComponent<ThingTypeRef>().ID);
            
            string name = $"{tt.Name}_TerrainHeightmap";

            if (!Skip)
            {
                string dirPath = Path.Combine(settings.SaveDataPath, "Terrain");
                Directory.CreateDirectory(dirPath);

                var buffer = GetTerrainBuffer(td);
                
                File.WriteAllBytes(Path.Combine(dirPath, $"{name}.bin"), buffer);
            }

            
            TerrainShapeData data = new TerrainShapeData
            {
                Heightmap = name,
                HeightmapResoultion = td.heightmapResolution,
                HeightmapScale = new[] { td.heightmapScale.x, td.heightmapScale.y, td.heightmapScale.z }
            };
            
            return data;
            
        }

        private static byte[] GetTerrainBuffer(TerrainData td)
        {
            // Write data
           int heightmapRes = td.heightmapResolution;
           float[,] heights = td.GetHeights(0, 0, heightmapRes, heightmapRes);
           byte[] data = new byte[heightmapRes * heightmapRes * sizeof(short)];
           
           const float normalize = (1 << 16);

           for (int y = 0; y < heightmapRes; ++y)
           {
               for (int x = 0; x < heightmapRes; ++x)
               {
                   int index = x + y * heightmapRes;

                   int height = Mathf.RoundToInt(heights[y, x] * normalize);
                   short compressedHeight = (short)Mathf.Clamp(height, 0, short.MaxValue);
                   
                   byte[] byteData = BitConverter.GetBytes(compressedHeight);
                   
                   data[index * 2 + 0] = byteData[0];
                   data[index * 2 + 1] = byteData[1];
               }
           }

           return data;
        }

        
    }
}