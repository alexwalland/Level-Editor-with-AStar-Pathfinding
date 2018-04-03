using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {

	public int nodePosX;
	public int nodePosZ;
	public GameObject vis;
	public MeshRenderer tileRenderer;
    public bool isWalkable;
    public Node parentNode;
    public LevelEditor.CreateLevel placedObj;
	public List<LevelEditor.CreateLevel> stackedObjs = new List<LevelEditor.CreateLevel>();
    public float hCost;
    public float gCost;
    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public GameObject obj;
}