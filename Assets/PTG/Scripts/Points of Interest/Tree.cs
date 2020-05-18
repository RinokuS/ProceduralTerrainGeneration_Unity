using UnityEngine;

public class Tree
{
    private GameObject treeObj;
    
    public Tree(GameObject treeObj, float treeScale, Transform parent)
    {
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
