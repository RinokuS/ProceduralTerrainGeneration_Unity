using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator 
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) 
    {
        float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(width, height); // Generating falloffMap, too
        
        AnimationCurve heightCurve_threadsafe = new AnimationCurve (settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                if (settings.useFalloff) // Clamping our NoiseMap Values with FalloffMap Values if useFalloff is selected
                {
                    values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]); // So we can made single island in water
                }
                values [i, j] *= heightCurve_threadsafe.Evaluate (values [i, j]) * settings.heightMultiplier;

                if (values [i, j] > maxValue)
                    maxValue = values [i, j];
                if (values [i, j] < minValue)
                    minValue = values [i, j];
            }
        }
        
        return new HeightMap (values, minValue, maxValue);
    }
    
    public static HeightMap GenerateHeatMap(int width, int height, HeightMapSettings heightSettings, Vector2 sampleCentre, HeatMapSettings heatSettings, out float[,] heatMap)
    {
        float[,] heightMap = Noise.GenerateNoiseMap (width, height, heightSettings.noiseSettings, sampleCentre);
        float[,] uniformHeatMap = Noise.GenerateHeatNoiseMap(width, height, heatSettings.heatSettings, sampleCentre);
        heatMap = new float[width,height];

        for (int yIndex = 0; yIndex < height; yIndex++)
        {
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                heatMap[yIndex, xIndex] = uniformHeatMap[yIndex, xIndex] * heightMap[yIndex, xIndex];
                heatMap[yIndex, xIndex] += heightMap[yIndex, xIndex] * heightMap[yIndex, xIndex];
            }
        }
        AnimationCurve heightCurve_threadsafe = new AnimationCurve (heightSettings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                heatMap [i, j] *= heightCurve_threadsafe.Evaluate (heatMap [i, j]) * heightSettings.heightMultiplier;

                if (heatMap [i, j] > maxValue)
                    maxValue = heatMap [i, j];
                if (heatMap [i, j] < minValue)
                    minValue = heatMap [i, j];
            }
        }

        heatMap = uniformHeatMap;
        return new HeightMap(heatMap, minValue, maxValue);
    }
}

public struct HeightMap 
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap (float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

