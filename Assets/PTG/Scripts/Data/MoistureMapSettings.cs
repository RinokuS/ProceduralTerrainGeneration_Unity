using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoistureMapSettings : UpdatableData
{
    public MoistureRate[] layers;
    public NoiseSettings moistureSettings;

    public float heightMultiplier;
    public AnimationCurve heightCurve;
    
    #if UNITY_EDITOR
    /// <summary>
    /// checking our bounds
    /// </summary>
    protected override void OnValidate()
    {
        moistureSettings.ValidateValues();
        base.OnValidate();
    }
    #endif
}

[System.Serializable]
public class MoistureRate
{
    public Color tint;
    [Range(0, 1)] 
    public float startHeight;
}
