using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    /// <summary>
    /// Mods for drawing map
    /// </summary>
    public enum DrawMode
    {
        NoiseMap,
        Mesh,
        FalloffMap
    }
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPrevLOD;

    public bool autoUpdate;

    /// <summary>
    /// Method for drawing our map with current mode
    /// </summary>
    public void DrawMapInEditor()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, 
            meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
        
        if (drawMode == DrawMode.NoiseMap)
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        else if (drawMode == DrawMode.Mesh)
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPrevLOD));
        else if (drawMode == DrawMode.FalloffMap)
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine), 0, 1)));
    }

    /// <summary>
    /// Method for applying our texture to object
    /// </summary>
    /// <param name="texture">texture for our map</param>
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width,1,texture.height) / 10f;
        
        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Method for applying our mesh to object
    /// </summary>
    /// <param name="meshData"></param>
    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        
        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }
    
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }
    
    /// <summary>
    /// checking our bounds
    /// </summary>
    void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated; // unsubsribe to the event, if we already had have subscription
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
