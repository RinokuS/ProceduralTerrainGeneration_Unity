using UnityEngine;

public class Tree
{
    private GameObject treeObj;
    public Vector3 coord;
    

    public Tree(Vector3 coord, GameObject treeObj, float treeScale, Transform parent)
    {
        this.coord = coord;

        this.treeObj = treeObj;
        this.treeObj.transform.localScale = new Vector3(treeScale, treeScale, treeScale);
        this.treeObj.transform.parent = parent;

        SetVisible(false); // Default state of each point of interest
    }

    public void SetVisible(bool visible)
    {
        treeObj.SetActive(visible);
    }
}
