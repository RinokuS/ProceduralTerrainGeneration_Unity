using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
// persistance - постоянство,  lacunarity - лакунарность
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        // Randomizer with seed
        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];
        
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) + offset.y;
            octavesOffsets[i] = new Vector2(offsetX,offsetY);
        }
        
        if (scale <= 0)
        {
            scale = 0.00001f;
        }
        // Maximum and Minimum values of Height
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        // Width and Height divided by 2
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        // Y loop
        for (int y = 0; y < mapHeight; y++)
        {
            // X loop
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                // Octaves loop
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octavesOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octavesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    // changing amplitude and frequency in the end of each loop
                    // frequency - частота
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                // changing our Max and Min values 
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }
        // changing noiseMap[,] values to value from 0 to 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Если текущее значение ближе к максимуму, то оно приближается к 1, если же оно ближе к минимуму, то приближается к 0
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
