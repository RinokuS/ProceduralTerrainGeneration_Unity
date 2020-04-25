using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BiomesSettings : UpdatableData
{
    public TerrainType[] heightType;
    public TerrainType[] heatType;
    public TerrainType[] moistureType;
    
    public static Biome[] ourBiomes =
    {
        new Biome("ice", Color.gray), new Biome("water", Color.cyan), new Biome("sea", Color.blue), 
        new Biome("savanna", new Color(183,171,59)), new Biome("desert", Color.yellow), new Biome("grassland", Color.green),
        new Biome("forest", new Color(43,55,0)), new Biome("rocky mount", Color.magenta),
        new Biome("snowy mount", Color.red), new Biome("salt sea", Color.white)
    };
    
    public Biome[,,] biomes = {{{ourBiomes[2], ourBiomes[2], ourBiomes[9]},{ourBiomes[1], ourBiomes[1], ourBiomes[2]},
            {ourBiomes[1], ourBiomes[0], ourBiomes[0]}},
        {{ourBiomes[4], ourBiomes[3], ourBiomes[3]},{ourBiomes[6], ourBiomes[5], ourBiomes[4]},
            {ourBiomes[6], ourBiomes[6], ourBiomes[6]}},
        {{ourBiomes[7], ourBiomes[7], ourBiomes[7]},{ourBiomes[7], ourBiomes[7], ourBiomes[7]},
            {ourBiomes[8],ourBiomes[8],ourBiomes[8]}}};
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
    public string name;
    [Range(0,1)]
    public float threshold;
    public Color color;
    public int index;
}
