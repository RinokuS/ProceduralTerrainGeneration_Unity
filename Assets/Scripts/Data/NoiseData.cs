using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode normilezeMode;
    
    public float noiseScale;

    public int octaves;
    // ползунок от 0 до 1
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    
    #if UNITY_EDITOR
    /// <summary>
    /// checking our bounds
    /// </summary>
    protected override void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
        
        base.OnValidate();
    }
    #endif
}
