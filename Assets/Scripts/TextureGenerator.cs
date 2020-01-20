using System.Collections;
using System.Collections.Generic;
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
    /// <param name="noiseMap">our Noise Map</param>
    /// <returns>Completed Texture</returns>
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        
        // Creating finally texture with TextureFromColorMap method
        return TextureFromColorMap(colorMap, width, height);
    }
}
