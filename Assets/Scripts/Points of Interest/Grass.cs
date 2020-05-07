using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass
{
    private GameObject grassObj;
    public Vector3 coord;
    

    public Grass(Vector3 coord, GameObject grassObj, float grassScale, Transform parent)
    {
        this.coord = coord;

        this.grassObj = grassObj;
        this.grassObj.transform.localScale = new Vector3(grassScale, grassScale, grassScale);
        this.grassObj.transform.parent = parent;

        SetVisible(true); // Default state of each point of interest
    }

    public void SetVisible(bool visible)
    {
        grassObj.SetActive(visible);
    }
}
