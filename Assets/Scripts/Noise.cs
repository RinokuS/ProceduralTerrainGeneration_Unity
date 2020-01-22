using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// persistance - постоянство,  lacunarity - лакунарность
public static class Noise
{
    public enum NormalizeMode
    {
        Local,
        Global
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        // Randomizer with seed
        System.Random prng = new System.Random(settings.seed);
        Vector2[] octavesOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        // Maximum and Minimum values of Height
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;
        // Width and Height divided by 2
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        // Y loop
        for (int y = 0; y < mapHeight; y++)
        {
            // X loop
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                // Octaves loop
                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octavesOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    // changing amplitude and frequency in the end of each loop
                    // frequency - частота
                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                // changing our Max and Min values if it is necessary
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        // changing noiseMap[,] values to value from 0 to 1
        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] =
                        Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight,
                            noiseMap[x, y]); // don`t work with endless terrain
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;
    
    public float scale = 50;

    public int octaves = 6;
    // ползунок от 0 до 1
    [Range(0,1)]
    public float persistance = 0.6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}
