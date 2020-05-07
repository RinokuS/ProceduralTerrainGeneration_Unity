using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoistureMapSettings : UpdatableData
{
    public MoistureRate[] layers;
    public MoistureSettings moistureSettings;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight => heightMultiplier * heightCurve.Evaluate(0);

    public float maxHeight => heightMultiplier * heightCurve.Evaluate(1);
    
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
    [Range(0,1)]
    public float tintStrength;
    [Range(0, 1)]
    public float startHeight;
    [Range(0,1)]
    public float blendStrength;
    public float textureScale;
}
