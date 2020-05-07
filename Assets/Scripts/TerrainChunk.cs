using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    private const float colliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    public GameObject meshObject;
    public Vector2 sampleCentre;
    public Bounds bounds;

    private HeightMap heightMap;
    public HeightMap heatMap;
    public HeightMap moistureMap;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;
    private int colliderLODIndex;

    private HeightMap _mapd;
    private bool heightMapRecieved;
    private bool heatMapRecieved;
    private bool moistureMapRecieved;

    private int previousLODIndex = -1;
    private bool hasSetCollider;
    private float maxViewDst;
    private bool hasSetTrees;
    private int treeLOD;

    private HeightMapSettings heightMapSettings;
    private HeatMapSettings heatMapSettings;
    private MoistureMapSettings moistureMapSettings;
    private MeshSettings meshSettings;
    private Transform viewer;
    private TreeGenerator treeGenerator;
    private GrassGenerator grassGenerator;
    private TextureData textureData;
    
    public Dictionary<Vector3, Tree> chunkTreesDict = new Dictionary<Vector3, Tree>();
    public Dictionary<Vector3, Grass> chunkGrassDict = new Dictionary<Vector3, Grass>();

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, HeatMapSettings heatMapSettings,
        MoistureMapSettings moistureMapSettings,
        MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex,
        Transform parent, Transform viewer, Material material, TreeGenerator treeGen, GrassGenerator grassGen, TextureData textureData, int treeLOD)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.heatMapSettings = heatMapSettings;
        this.moistureMapSettings = moistureMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        treeGenerator = treeGen;
        grassGenerator = grassGen;
        this.textureData = textureData;
        this.treeLOD = treeLOD;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        

        SetVisible(false); // default state of the terrain chunk

        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
    }

    /// <summary>
    /// Method which helps us to avoid situation when the height map gets received and we update the terrain chunk
    /// before OnTerrainChunkVisibilityChanges() method is subscribed to the event
    /// </summary>
    public void Load()
    {
        /*
         * Some magic with lamda:
         * We can cast Method with bunch of parameters to the method with NO parameters! WoW
         * And it will automatically cast the return type to object, yep
         */
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived); // Check
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeatMap(meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine, heightMapSettings, sampleCentre, heatMapSettings), OnHeatMapReceived);
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateMoistureMap(meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine, sampleCentre, heightMapSettings, moistureMapSettings), OnMoistureMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap) heightMapObject;
        heightMapRecieved = true;

        UpdateTerrainChunk();
    }

    void OnHeatMapReceived(object heatMapObject)
    {
        this.heatMap = (HeightMap) heatMapObject;
        heatMapRecieved = true;

        if (heatMap.minValue < TerrainGenerator.minHeat)
            TerrainGenerator.minHeat = heatMap.minValue;
        if (heatMap.maxValue < TerrainGenerator.maxHeat)
            TerrainGenerator.maxHeat = heatMap.maxValue;

        UpdateTerrainChunk();
    }

    void OnMoistureMapReceived(object moistureMapObject)
    {
        this.moistureMap = (HeightMap) moistureMapObject;
        moistureMapRecieved = true;

        if (moistureMap.minValue < TerrainGenerator.minMoisture)
            TerrainGenerator.minMoisture = moistureMap.minValue;
        if (moistureMap.maxValue < TerrainGenerator.maxMoisture)
            TerrainGenerator.maxMoisture = moistureMap.maxValue;

        UpdateTerrainChunk();
    }

    Vector2 viewerPosition => new Vector2(viewer.position.x, viewer.position.z);

    /// <summary>
    /// Finding the point on its perimeter that is the closest to the viewers position
    /// and it will find the distance between that point in the viewer and if that distance is less than
    /// the maximum view distance then it will make sure that the mesh object is enabled and if that distance
    /// exceeds the maximum view distance then it will disable the mesh object
    /// </summary>
    public void UpdateTerrainChunk()
    {
        if (heightMapRecieved && heatMapRecieved && moistureMapRecieved)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDistThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }
                    
                    if (chunkTreesDict.Count == 0 && lodIndex <= treeLOD)
                    {
                        treeGenerator.GenerateTrees(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, this,
                            textureData, meshFilter.mesh, meshObject.transform);
                    }
                    if (lodIndex <= treeLOD)
                    {
                        if (!hasSetTrees)
                        {
                            foreach (var item in chunkTreesDict)
                            {
                                item.Value.SetVisible(true);
                            }

                            hasSetTrees = true;
                        }
                    }
                    else
                    {
                        if (hasSetTrees)
                        {
                            foreach (var item in chunkTreesDict)
                            {
                                item.Value.SetVisible(false);
                            }

                            foreach (var item in chunkGrassDict)
                            {
                                item.Value.SetVisible(false);
                            }

                            hasSetTrees = false;
                        }
                    }
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);
                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    /// <summary>
    /// Method for updating our collision mesh, if player is close enough
    /// </summary>
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider) // we won`t do anything, if we had have the collider already
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;

                    grassGenerator.GenerateGrass(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, this, textureData, meshCollider.sharedMesh, meshObject.transform);
                    foreach (var item in chunkTreesDict)
                    {
                        item.Value.SetVisible(true);
                    }
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }
}

/// <summary>
/// Level of details mesh
/// </summary>
class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    private int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataRecieved(object meshDataObject)
    {
        mesh = ((MeshData) meshDataObject).CreateMesh();
        hasMesh = true;
        
        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod),
            OnMeshDataRecieved);
    }
}

