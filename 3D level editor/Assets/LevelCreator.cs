using UnityEngine;
using System.Collections.Generic;
using gridMaster;

namespace LevelEditor
{
	public class LevelCreator : MonoBehaviour
	{
		LevelManager manager;
		GridGenerator gridGenerator;
		InterfaceManager ui;

		bool hasObj;
		GameObject objToPlace;
		GameObject cloneObj;
		CreateLevel objProperties;
		Vector3 mousePosition;
		Vector3 worldPosition;
		bool deleteObj;

		bool hasMaterial;
		bool paintTile;
		public Material matToPlace;
		Node previousNode;
		Material previousMaterial;
		Quaternion targetRot;
		Quaternion prevRotation;

		bool placeStackObj;
		GameObject stackObjToPlace;
		GameObject stackCloneObj;
		CreateLevel stackObjProperties;
		bool deleteStackObj;
        
		void Start()
		{
			gridGenerator = GridGenerator.GetInstance();
			manager = LevelManager.GetInstance();
			ui = InterfaceManager.GetInstance();

			PaintAll ();
		}

		void Update()
		{
			PlaceObject();
			paintTiles();
			DeleteObjs();
			PlaceStackedObjs();
			DeleteStackedObjs();
		}

		void UpdateMousePosition()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, Mathf.Infinity))
			{
				mousePosition = hit.point;
			}
		}

		#region Place Objects
		void PlaceObject()
		{
		if (hasObj) {
			UpdateMousePosition ();

			Node curNode = gridGenerator.NodeFromWorldPosition (mousePosition);

			worldPosition = curNode.vis.transform.position;

			if (cloneObj == null) {
				cloneObj = Instantiate (objToPlace, worldPosition, Quaternion.identity) as GameObject;
				objProperties = cloneObj.GetComponent<CreateLevel> ();
			} else { 
				cloneObj.transform.position = worldPosition;

				if (Input.GetMouseButton (0) && !ui.mouseOverUIElement) {
					if (curNode.placedObj != null) {
						manager.inSceneGameObjects.Remove (curNode.placedObj.gameObject);
						Destroy (curNode.placedObj.gameObject);
						curNode.placedObj = null;
					}

					GameObject actualObjPlaced = Instantiate (objToPlace, worldPosition, cloneObj.transform.rotation) as GameObject;
					CreateLevel placedObjProperties = actualObjPlaced.GetComponent<CreateLevel> ();

					placedObjProperties.gridPosX = curNode.nodePosX;
					placedObjProperties.gridPosZ = curNode.nodePosZ;
					curNode.placedObj = placedObjProperties;
					manager.inSceneGameObjects.Add (actualObjPlaced);
                        curNode.isWalkable = false;
				}

				if (Input.GetMouseButtonUp (1)) {
					objProperties.ChangeRotation ();
				}
			}
		} 
		else 
		{
			if (cloneObj != null) 
			{
				Destroy (cloneObj);
			}
		}
	}

		public void PassGameObjectToPlace(string objId)
		{
			if (cloneObj != null)
			{
				Destroy (cloneObj);
			}

			CloseAll();
			hasObj = true;
			objToPlace = ResourceManager.GetInstance().GetObjBase(objId).objPrefab;
		}

		void DeleteObjs()
		{
			if (deleteObj) 
			{
				UpdateMousePosition ();

				Node curNode = gridGenerator.NodeFromWorldPosition (mousePosition);

				if (Input.GetMouseButton (0) && !ui.mouseOverUIElement) {
					if (curNode.placedObj != null) {
						if (manager.inSceneGameObjects.Contains (curNode.placedObj.gameObject)) {
							manager.inSceneGameObjects.Remove (curNode.placedObj.gameObject);
							Destroy (curNode.placedObj.gameObject);
						}

						curNode.placedObj = null;
					}
				}
			}
		}

		public void DeleteObj()
		{
			CloseAll ();
			deleteObj = true;
		}
		#endregion

		#region TilePainting

		void paintTiles()
		{
			if (hasMaterial)
			{
				UpdateMousePosition();
				
				Node curNode = gridGenerator.NodeFromWorldPosition(mousePosition);
				
				if (previousNode == null)
				{
					previousNode = curNode;
					previousMaterial = previousNode.tileRenderer.material;
					prevRotation = previousNode.vis.transform.rotation;
				}
				else
				{
					if (previousNode != curNode)
					{
						if (paintTile)
						{
							int matId = ResourceManager.GetInstance().GetMaterialId(matToPlace);
							curNode.vis.GetComponent<Node_Object>().textueid = matId;
							paintTile = false;
						}
						else
						{
							previousNode.tileRenderer.material = previousMaterial;
							previousNode.vis.transform.rotation = prevRotation;
						}
						
						previousNode = curNode;
						previousMaterial = curNode.tileRenderer.material;
						prevRotation = curNode.vis.transform.rotation;
					}
				}
				
				curNode.tileRenderer.material = matToPlace;
				curNode.vis.transform.localRotation = targetRot;
				
				if (Input.GetMouseButton(0) && !ui.mouseOverUIElement)
				{
					paintTile = true;
				}
				
				if (Input.GetMouseButtonUp(1))
				{
					Vector3 eulerAngles = curNode.vis.transform.eulerAngles;
					eulerAngles += new Vector3(0,90,0);
					targetRot = Quaternion.Euler(eulerAngles);
				}
			}
		}

		public void PassMaterialToPaint(int matId)
		{
			deleteObj = false;
			placeStackObj = false;
			hasObj = false;
			matToPlace = ResourceManager.GetInstance().GetMaterial(matId);
			hasMaterial = true;
		}

		public void PaintAll()
		{
			for (int x=0; x < gridGenerator.sizeX; x++)
			{
				for (int z=0; z < gridGenerator.sizeZ; z++)
				{
					gridGenerator.grid[x,z].tileRenderer.material = matToPlace;
					int matId = ResourceManager.GetInstance().GetMaterialId(matToPlace);
					gridGenerator.grid[x,z].vis.GetComponent<Node_Object>().textueid = matId;
				}
			}

			previousNode = null;
		}
		#endregion

		#region StackedObjects
		public void PassStackedObjectToPlace(string objId)
		{
			if (stackCloneObj != null)
			{
				Destroy(stackCloneObj);
			}

			CloseAll();
			placeStackObj = true;
			stackCloneObj = null;
			stackObjToPlace = ResourceManager.GetInstance().GetStackObjBase(objId).objPrefab;
		}

		void PlaceStackedObjs()
		{
			if (placeStackObj)
			{
				UpdateMousePosition();

				Node curNode = gridGenerator.NodeFromWorldPosition(mousePosition);

				worldPosition = curNode.vis.transform.position;

				if (stackCloneObj == null)
				{
					stackCloneObj = Instantiate(stackObjToPlace,worldPosition,Quaternion.identity) as GameObject;
					stackObjProperties = stackCloneObj.GetComponent<CreateLevel>();
				}
				else
				{
					stackCloneObj.transform.position = worldPosition;

					if (Input.GetMouseButtonUp(0) && !ui.mouseOverUIElement)
					{
						GameObject actualObjPlaced = Instantiate(stackObjToPlace, worldPosition, stackCloneObj.transform.rotation) as GameObject;
						CreateLevel placedObjProperties = actualObjPlaced.GetComponent<CreateLevel>();

						placedObjProperties.gridPosX = curNode.nodePosX;
						placedObjProperties.gridPosZ = curNode.nodePosZ;
						curNode.stackedObjs.Add(placedObjProperties);
						manager.inSceneGameObjects.Add(actualObjPlaced);
                        curNode.isWalkable = false;
					}

						if (Input.GetMouseButtonUp(1))
						{
							stackObjProperties.ChangeRotation();
						}
					}
			}
			else
			{
				if (stackCloneObj != null)
				{
					Destroy(stackCloneObj);
				}
			}
		}

		public void DeleteStackObj()
		{
			CloseAll();
			deleteStackObj = true;
		}

		void DeleteStackedObjs()
		{
			if (deleteStackObj)
			{
				UpdateMousePosition();

				Node curNode = gridGenerator.NodeFromWorldPosition(mousePosition);

				if (Input.GetMouseButton(0) && !ui.mouseOverUIElement)
				{
					if (curNode.stackedObjs.Count > 0)
					{
						for (int i =0; i < curNode.stackedObjs.Count; i++)
						{
							if (manager.inSceneStackObjects.Contains(curNode.stackedObjs[i].gameObject))
							{
								manager.inSceneStackObjects.Remove(curNode.stackedObjs[i].gameObject);
								Destroy(curNode.stackedObjs[i].gameObject);
							}
						}
						curNode.stackedObjs.Clear();
					}
				}
			}
		}
		#endregion

		void CloseAll()
		{
			hasObj = false;
			deleteObj = false;
			paintTile = false;
			placeStackObj = false;
			hasMaterial = false;
			deleteStackObj = false;
		}
	}
}

