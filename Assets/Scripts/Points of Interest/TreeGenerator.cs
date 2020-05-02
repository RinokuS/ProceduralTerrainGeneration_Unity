using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField]
    private HeightMapSettings treeGenSettings;
    [SerializeField]
    private GameObject treePrefab;
    [SerializeField, Range(0,2)]
    private float treeScale;
    [SerializeField, Range(0, 10)] 
    private int neighborRadius;

    private const float Epsilone = 0.02f;

    public void GenerateTrees(int width, int height, TerrainChunk chunk, TextureData textureData, Mesh mesh, Transform parent)
    {
        float[,] treeMap = HeightMapGenerator.GenerateHeightMap(width, height, treeGenSettings, chunk.sampleCentre).values;
        
        Vector3[] meshVertices = mesh.vertices;
        
        for (int i = 0; i < width-5; i++)
        {
            for (int j = 0; j < height-5; j++)
            {
                int vertexIndex = i * width + j;

                float treeValue = treeMap[i, j];

                int neighborIBegin = (int) Mathf.Max(0, i - neighborRadius);
                int neighborIEnd = (int) Mathf.Min(width - 1, i + neighborRadius);
                int neighborJBegin = (int) Mathf.Max(0, j - neighborRadius);
                int neighborJEnd = (int) Mathf.Min(height - 1, j + neighborRadius);
                float maxValue = treeValue;
                
                for (int neighborI = neighborIBegin; neighborI <= neighborIEnd; neighborI++)
                {
                    for (int neighborJ = neighborJBegin; neighborJ <= neighborJEnd; neighborJ++)
                    {
                        float neighborValue = treeMap[neighborI, neighborJ];

                        if (neighborValue >= maxValue)
                            maxValue = neighborValue;
                    }
                }
                

                try
                {
                    float corrHeight =
                        Mathf.InverseLerp(textureData.SavedMinHeight, textureData.SavedMaxHeight, meshVertices[vertexIndex].y);
                    float corrHeat = Mathf.InverseLerp(TerrainGenerator.minHeat, TerrainGenerator.maxHeat,
                        chunk.heatMap.values[i, j]);
                    float corrMoisture = Mathf.InverseLerp(TerrainGenerator.minMoisture, TerrainGenerator.maxMoisture,
                        chunk.moistureMap.values[i, j]);
                    if (treeValue.CompareTo(maxValue) == 0 && corrHeight <= (textureData.layers[3].startHeight - Epsilone) && 
                        corrHeight >= (textureData.layers[2].startHeight + Epsilone) &&
                        corrHeat <= 1 && corrHeat >= 0.55f &&
                        corrMoisture <= 0.7f && corrMoisture >= 0.08f)  // Add epsilone to minimize borders of biome
                    {
                        Vector3 treePos = new Vector3(meshVertices[vertexIndex].x, meshVertices[vertexIndex].y, meshVertices[vertexIndex].z) + 
                                          new Vector3(chunk.bounds.center.x,0, chunk.bounds.center.y);
                        chunk.chunkTreesDict.Add(treePos, new Tree(treePos, Instantiate(treePrefab, treePos, Quaternion.identity), treeScale, parent));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
        }
    }
}
