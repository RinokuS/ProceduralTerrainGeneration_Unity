using System;
using UnityEngine;
using Random = System.Random;

public class GrassGenerator : MonoBehaviour
{
    private const float Epsilone = 0.02f;
    
    [SerializeField]
    private HeightMapSettings grassGenSettings;
    [SerializeField]
    private GameObject grassPrefab;
    [SerializeField]
    private GameObject winterGrassPrefab;
    [SerializeField]
    private GameObject savannaGrassPrefab;
    [SerializeField, Range(0,20)]
    private float grassScale;
    [SerializeField, Range(0,1)]
    private float grassFrequency;
    
    public void GenerateGrass(int width, int height, TerrainChunk chunk, TextureData textureData, Mesh mesh, Transform parent)
    {
        Vector3[] meshVertices = mesh.vertices;
        HeightMap grassMap = HeightMapGenerator.GenerateHeightMap(width, height, grassGenSettings, chunk.sampleCentre);

        if (grassPrefab is null && winterGrassPrefab is null && savannaGrassPrefab is null)
            return;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int vertexIndex = i * width + j;

                try
                {
                    float corrHeight =
                        Mathf.InverseLerp(textureData.SavedMinHeight, textureData.SavedMaxHeight, meshVertices[vertexIndex].y);
                    float corrHeat = Mathf.InverseLerp(TerrainGenerator.minHeat, TerrainGenerator.maxHeat,
                        chunk.heatMap.values[i, j]);
                    float corrMoisture = Mathf.InverseLerp(TerrainGenerator.minMoisture, TerrainGenerator.maxMoisture,
                        chunk.moistureMap.values[i, j]);

                    if (corrHeight <= (textureData.layers[3].startHeight - Epsilone) &&
                        corrHeight >= (textureData.layers[2].startHeight + Epsilone) && 
                        (Mathf.Lerp(grassMap.minValue, grassMap.maxValue, grassMap.values[i,j]) <= grassFrequency))
                    {
                        if (corrHeat < 0.7f && corrHeat >= 0.2f &&
                            corrMoisture <= 0.7f && corrMoisture >= 0)
                        {
                            Vector3 grassPos = new Vector3(meshVertices[vertexIndex].x, meshVertices[vertexIndex].y, meshVertices[vertexIndex].z) + 
                                               new Vector3(chunk.bounds.center.x,0, chunk.bounds.center.y);
                            if (!(grassPrefab is null))
                                chunk.chunkGrassDict.Add(grassPos, new Grass(Instantiate(grassPrefab, grassPos, Quaternion.identity), grassScale, parent));
                        }
                        else if (corrHeat <= 1f && corrHeat >= 0.7f &&
                                 corrMoisture <= 0.7f && corrMoisture >= 0)
                        {
                            Vector3 grassPos = new Vector3(meshVertices[vertexIndex].x, meshVertices[vertexIndex].y, meshVertices[vertexIndex].z) + 
                                               new Vector3(chunk.bounds.center.x,0, chunk.bounds.center.y);
                            if (!(winterGrassPrefab is null))
                                chunk.chunkGrassDict.Add(grassPos, new Grass(Instantiate(winterGrassPrefab, grassPos, Quaternion.identity), grassScale, parent));
                        }
                        else if (corrHeat < 0.61f && corrHeat >= 0.16f &&
                                 corrMoisture <= 0.95f && corrMoisture > 0.7f)
                        {
                            Vector3 grassPos = new Vector3(meshVertices[vertexIndex].x, meshVertices[vertexIndex].y, meshVertices[vertexIndex].z) + 
                                               new Vector3(chunk.bounds.center.x,0, chunk.bounds.center.y);
                            if (!(savannaGrassPrefab is null))
                                chunk.chunkGrassDict.Add(grassPos, new Grass(Instantiate(savannaGrassPrefab, grassPos, Quaternion.identity), grassScale, parent));
                        }
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
