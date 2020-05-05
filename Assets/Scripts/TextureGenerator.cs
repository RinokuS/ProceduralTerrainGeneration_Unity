using UnityEngine;

public class TextureGenerator
{
    /// <summary>
    /// creating texture of Color Map
    /// </summary>
    /// <param name="colorMap">our Color Map</param>
    /// <param name="width">Width of texture</param>
    /// <param name="height">Height of texture</param>
    /// <returns>Completed Texture</returns>
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        // Removing blur
        texture.filterMode = FilterMode.Point;
        // Fixing wrapping
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        
        return texture;
    }
    /// <summary>
    /// creating texture of Noise Map
    /// </summary>
    /// <param name="heightMap">our Noise Map</param>
    /// <returns>Completed Texture</returns>
    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);
        

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
            }
        }
        
        // Creating finally texture with TextureFromColorMap method
        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D BiomeTexture(HeightMap heightMap, HeightMap heatMap,
        HeightMap moistureMap, BiomesSettings settings)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int yIndex = 0; yIndex < height; yIndex++)
        {
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                int colorIndex = xIndex * width + yIndex;
                float currentHeight = Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[yIndex, xIndex]);
                float currentHeat = Mathf.InverseLerp(0, 1, heatMap.values[yIndex, xIndex]);
                float currentMoisture = Mathf.InverseLerp(moistureMap.minValue, moistureMap.maxValue, moistureMap.values[yIndex, xIndex]);
                

                int heightIndex = 0;
                int heatIndex = 0;
                int moistureIndex = 0;

                for (int i = 0; i < settings.heightType.Length; i++)
                {
                    if (currentHeight <= settings.heightType[i].threshold)
                    {
                        heightIndex = settings.heightType[i].index;
                        break; // Чтобы не проходил весь цикл
                    }
                }
                
                for (int i = 0; i < settings.heatType.Length; i++)
                {
                    if (currentHeat <= settings.heatType[i].threshold)
                    {
                        heatIndex = settings.heatType[i].index;
                        break; // Чтобы не проходил весь цикл
                    }
                }
                
                for (int i = 0; i < settings.moistureType.Length; i++)
                {
                    if (currentMoisture <= settings.moistureType[i].threshold)
                    {
                        moistureIndex = settings.moistureType[i].index;
                        break; // Чтобы не проходил весь цикл
                    }
                }

                colorMap[colorIndex] = settings.biomes[heightIndex, heatIndex, moistureIndex].color;
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
