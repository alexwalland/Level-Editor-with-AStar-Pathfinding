using UnityEngine;
using System.Collections;

	public class Node_Object : MonoBehaviour
	{
	public int posX;
	public int posZ;
	public int textueid;
		
	public void UpdateNodeObject(Node curNode, Node_ObjectSaveable Saveable)
	{
		posX = Saveable.posX;
		posZ = Saveable.posZ;
		textueid = Saveable.textureId;

		ChangeMaterial(curNode);
	}

	void ChangeMaterial(Node curNode)
	{
		Material getMaterial = LevelEditor.ResourceManager.GetInstance().GetMaterial(textueid);
		curNode.tileRenderer.material = getMaterial;
	}

	public Node_ObjectSaveable GetSaveable()
	{
		Node_ObjectSaveable saveable = new Node_ObjectSaveable();
		saveable.posX = this.posX;
		saveable.posZ = this.posZ;
		saveable.textureId = this.textueid;

		return saveable;
	}

}

[System.Serializable]
public class Node_ObjectSaveable
{
	public int posX;
	public int posZ;
	public int textureId;
}

