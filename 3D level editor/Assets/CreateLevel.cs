using UnityEngine;
using System.Collections;

namespace LevelEditor
{
	public class CreateLevel : MonoBehaviour
	{
		public string obj_id;
		public int gridPosX;
		public int gridPosZ;
		public GameObject modelVisualisation;
		public Vector3 worldPositionOffset;
		public Vector3 worldRotation;

		public bool isStackableObj = false;

		public float rotateDegrees = 60;

		public void UpdateNode(Node[,] grid)
		{
			Node node = grid [gridPosX, gridPosZ];
	
			Vector3 worldPosition = node.vis.transform.position;
			worldPosition += worldPositionOffset;
			transform.rotation = Quaternion.Euler(worldRotation);
			transform.position = worldPosition;
		}

		public void ChangeRotation()
		{
			Vector3 eulerAngles = transform.eulerAngles;
			eulerAngles += new Vector3 (0, rotateDegrees, 0);
			transform.localRotation = Quaternion.Euler (eulerAngles);
		}

		public SaveableLevelObject GetSaveableObject()
		{
			SaveableLevelObject savedObj = new SaveableLevelObject ();
			savedObj.obj_Id = obj_id;
			savedObj.posX = gridPosX;
			savedObj.posZ = gridPosZ;

			worldRotation = transform.localEulerAngles;

			savedObj.rotX = worldRotation.x;
			savedObj.rotY = worldRotation.y;
			savedObj.rotZ = worldRotation.z;
			savedObj.isStackable = isStackableObj;

			return savedObj;
		}
	}

	[System.Serializable]
	public class SaveableLevelObject
	{
		public string obj_Id;
		public int posX;
		public int posZ;

		public float rotX;
		public float rotY;
		public float rotZ;

		public bool isStackable = false;
	}
}
