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
    
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, 
        float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        // Randomizer with seed
        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX,offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        
        if (scale <= 0)
        {
            scale = 0.00001f;
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
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octavesOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    // changing amplitude and frequency in the end of each loop
                    // frequency - частота
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                // changing our Max and Min values if it is necessary
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }
        // changing noiseMap[,] values to value from 0 to 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Если текущее значение ближе к максимуму, то оно приближается к 1, если же оно ближе к минимуму, то приближается к 0
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]); // don`t work with endless terrain
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
