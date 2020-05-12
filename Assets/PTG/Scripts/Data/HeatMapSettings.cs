using UnityEngine;

[CreateAssetMenu()]
public class HeatMapSettings : UpdatableData
{
    public HeatRate[] layers;
    public NoiseSettings heatSettings;
    
    public AnimationCurve heightCurve;

#if UNITY_EDITOR
    /// <summary>
    /// checking our bounds
    /// </summary>
    protected override void OnValidate()
    {
        heatSettings.ValidateValues();
        base.OnValidate();
    }
    #endif
}

[System.Serializable]
public class HeatRate
{
    public Color tint;
    [Range(0, 1)]
    public float startHeight;
}