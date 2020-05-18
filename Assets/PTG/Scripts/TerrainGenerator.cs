using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private const float viewerMoveThresholdForChunkUpdate = 50f;

    private const float sqrViewerMoveThresholdForChunkUpdate =
        viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int treeLOD;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public HeatMapSettings heatMapSettings;
    public MoistureMapSettings moistureMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material mapMaterial;

    public TreeGenerator treeGen;
    public GrassGenerator grassGen;
    public GameObject water;
    [Range(0, 30)] public float waterHeight;

    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;
    private float meshWorldSize;
    private int chunksVisibleInViewDst;

    public static float minHeat = 0.5f;
    public static float maxHeat = 1;
    public static float minMoisture = 0.5f;
    public static float maxMoisture = 1;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    void Start()
    {
        try
        {
            textureSettings.ApplyToMaterial(mapMaterial);
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

            try
            {
                viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("Please add viewer prefab into Map Generator.");
                return;
            }

            float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
            meshWorldSize = meshSettings.meshWorldSize;
            chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

            UpdateVisibleChunks();
        }
        catch (NullReferenceException)
        {
            Debug.Log("You have to insert all missing elements into your Map Generator.");
        }
        catch (UnassignedReferenceException)
        {
            Debug.Log("Please add mapMaterial prefab into Map Generator.");
        }
    }

    void Update()
    {
        try
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z); 
        }
        catch (UnassignedReferenceException e)
        {
            return;
        }

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    /// <summary>
    /// Method for Updating visibility of our Chunks
    /// </summary>
    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)
                ) // only if we have not already updated this coord, we will bother running code below
                {
                    if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                        terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    else
                    {
                        if (!(heatMapSettings is null) && !(moistureMapSettings is null))
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings,
                                heatMapSettings,
                                moistureMapSettings,
                                meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial, treeGen,
                                grassGen, textureSettings, treeLOD);
                            terrainChunkDict.Add(viewedChunkCoord, newChunk);
                            newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanges;
                            newChunk.Load();
                            if (!(water is null))
                            {
                                GameObject waterObj = Instantiate(water,
                                    new Vector3(newChunk.bounds.center.x, 0, newChunk.bounds.center.y),
                                    Quaternion.identity);
                                waterObj.transform.parent = newChunk.meshObject.transform;
                                waterObj.transform.position += new Vector3(0, waterHeight, 0);
                                waterObj.transform.localScale = new Vector3(3, 1, 3) * meshSettings.meshScale;
                            }
                        }
                        else
                            Debug.Log("You have to insert all missing elements into your Map Generator.");
                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanges(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleTerrainChunks.Add(chunk);
        else
            visibleTerrainChunks.Remove(chunk);
    }
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;

    public float visibleDistThreshold;

    public float sqrVisibleDstThreshold
    {
        get => visibleDistThreshold * visibleDistThreshold;
    }
}