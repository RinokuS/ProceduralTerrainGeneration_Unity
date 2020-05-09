using System;
using UnityEngine;

[CreateAssetMenu]
public class BiomesSettings : UpdatableData
{
    public TerrainType[] heightType;
    public TerrainType[] heatType;
    public TerrainType[] moistureType;
    
    public static Biome[] ourBiomes =
    {
        new Biome("ice", new Color(0.737f, 0.98f, 0.984f)), new Biome("water", new Color(0.584f,0.737f,0.976f)), new Biome("sea", new Color(0.352f, 0.376f, 0.929f)), 
        new Biome("savanna", new Color(0.698f, 0.701f, 0.239f)), new Biome("desert", new Color(0.886f, 0.784f, 0.415f)), new Biome("grassland", new Color(0.113f, 0.764f, 0.203f)),
        new Biome("forest", new Color(0.235f, 0.545f, 0.239f)), new Biome("rocky mount", new Color(0.372f,0.294f,0.184f)),
        new Biome("snowy mount", Color.white), new Biome("salt sea", new Color(0.866f, 0.85f, 0.835f))
    };
    
    public Biome[,,] biomes = {{{ourBiomes[1], ourBiomes[1], ourBiomes[1]},{ourBiomes[1], ourBiomes[1], ourBiomes[1]},
            {ourBiomes[1], ourBiomes[1], ourBiomes[1]}},
        {{ourBiomes[1],ourBiomes[1],ourBiomes[9]},{ourBiomes[1],ourBiomes[1],ourBiomes[1]},{ourBiomes[0],ourBiomes[0],ourBiomes[0]}},
        {{ourBiomes[4],ourBiomes[3],ourBiomes[3]},{ourBiomes[6],ourBiomes[5],ourBiomes[4]},{ourBiomes[6],ourBiomes[6],ourBiomes[6]}},
        {{ourBiomes[3], ourBiomes[3], ourBiomes[3]},{ourBiomes[6], ourBiomes[5], ourBiomes[3]},
            {ourBiomes[6], ourBiomes[6], ourBiomes[6]}},
        {{ourBiomes[7], ourBiomes[7], ourBiomes[7]},{ourBiomes[7], ourBiomes[7], ourBiomes[7]},
            {ourBiomes[7],ourBiomes[7],ourBiomes[7]}},
        {{ourBiomes[7],ourBiomes[7],ourBiomes[7]},{ourBiomes[8],ourBiomes[8],ourBiomes[8]},{ourBiomes[8],ourBiomes[8],ourBiomes[8]}}
    };
}
[Serializable]
public class Biome
{
    // Место для трехмерного массива типов (первый индекс - тип высоты, второй - тип температуры, третий - тип влажности) [По индексам выбираем биом]
    public string name;
    public Color color;

    public Biome(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }
}

[Serializable]
public class TerrainType
{
    [Range(0,1)]
    public float threshold;
    public int index;
}
