using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using LevelEditor;
using System;
using gridMaster;

	public class LevelSaveLoad : MonoBehaviour
	{
	private List<SaveableLevelObject> saveLevelObjects_List = new List<SaveableLevelObject>();
	private List<SaveableLevelObject> saveStackableLevelObjects_List = new List<SaveableLevelObject>();
	private List<Node_ObjectSaveable> saveNodeObjectsList = new List<Node_ObjectSaveable>();

	public static string saveFolderName = "LevelObjects";

	public void SaveLevelButton()
	{
		SaveLevel("level1");
	}

	public void LoadLevelButton()
	{
		LoadLevel("level1");
	}

	static string SaveLocation(string LevelName)
	{
		string saveLocation = Application.persistentDataPath + "/" + saveFolderName + "/";

		if (!Directory.Exists(saveLocation))
		{
			Directory.CreateDirectory(saveLocation);
		}

		return saveLocation + LevelName;
	}

	void SaveLevel(string saveName)
	{
		CreateLevel[] levelObjects = FindObjectsOfType<CreateLevel>();

		saveLevelObjects_List.Clear();
		saveNodeObjectsList.Clear();
		saveStackableLevelObjects_List.Clear();

		foreach(CreateLevel lvlObj in levelObjects)
		{
			if (!lvlObj.isStackableObj)
			{
				saveLevelObjects_List.Add(lvlObj.GetSaveableObject());
			}
			else
			{
				saveStackableLevelObjects_List.Add(lvlObj.GetSaveableObject());
			}
		}

		Node_Object[] nodeObjects = FindObjectsOfType<Node_Object>();
		saveNodeObjectsList.Clear();

		foreach(Node_Object nodeObject in nodeObjects)
		{
			saveNodeObjectsList.Add(nodeObject.GetSaveable());
		}

		LevelSaveable levelSave = new LevelSaveable();
		levelSave.saveLevelObjects_List = saveLevelObjects_List;
		levelSave.saveStackableObjects_List = saveStackableLevelObjects_List;
		levelSave.saveNodeObjectsList = saveNodeObjectsList;

		string saveLocation = SaveLocation(saveName);

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write,FileShare.None);
		formatter.Serialize(stream, levelSave);
		stream.Close();

		Debug.Log(saveLocation);
	}

	bool LoadLevel(string saveName)
	{
		bool retVal = true;

		string SaveFile = SaveLocation(saveName);

		if (!File.Exists(SaveFile))
		{
			retVal = false;
		}
		else
		{
			IFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(SaveFile, FileMode.Open);

			LevelSaveable save = (LevelSaveable)formatter.Deserialize(stream);

			stream.Close();
			LoadLevelActual(save);
		}
		return retVal;
	}

	void LoadLevelActual(LevelSaveable levelSaveable)
	{
		for (int i =0; i < levelSaveable.saveLevelObjects_List.Count; i++)
		{
			SaveableLevelObject s_obj = levelSaveable.saveLevelObjects_List[i];

			Node nodeToPlace = GridGenerator.GetInstance().grid[s_obj.posX,s_obj.posZ];

			GameObject go = Instantiate(
				ResourceManager.GetInstance().GetObjBase(s_obj.obj_Id).objPrefab,
				nodeToPlace.vis.transform.position,
				Quaternion.Euler(
				s_obj.rotX,
				s_obj.rotY,
				s_obj.rotZ))
				as GameObject;

			nodeToPlace.placedObj = go.GetComponent<CreateLevel>();
			nodeToPlace.placedObj.gridPosX = nodeToPlace.nodePosX;
			nodeToPlace.placedObj.gridPosZ = nodeToPlace.nodePosZ;
			nodeToPlace.placedObj.worldRotation = nodeToPlace.placedObj.transform.localEulerAngles;

		}

		for (int i=0; i< levelSaveable.saveStackableObjects_List.Count; i++)
		{
			SaveableLevelObject s_obj = levelSaveable.saveStackableObjects_List[i];

			Node nodeToPlace = GridGenerator.GetInstance().grid[s_obj.posX,s_obj.posZ];

			GameObject go = Instantiate(
				ResourceManager.GetInstance().GetStackObjBase(s_obj.obj_Id).objPrefab,
				nodeToPlace.vis.transform.position,
				Quaternion.Euler(
				s_obj.rotX,
				s_obj.rotY,
				s_obj.rotZ))
				as GameObject;

			CreateLevel stack_obj = go.GetComponent<CreateLevel>();
			stack_obj.gridPosX = nodeToPlace.nodePosX;
			stack_obj.gridPosZ = nodeToPlace.nodePosZ;

			nodeToPlace.stackedObjs.Add(stack_obj);
		}

		for (int i= 0; i<levelSaveable.saveNodeObjectsList.Count; i++)
		{
			Node node = 
				GridGenerator.GetInstance().grid[levelSaveable.saveNodeObjectsList[i].posX,
				                                 levelSaveable.saveNodeObjectsList[i].posZ];

			node.vis.GetComponent<Node_Object>().UpdateNodeObject(node, levelSaveable.saveNodeObjectsList[i]);
		}

	}
	[Serializable]
	public class LevelSaveable
	{
		public List<SaveableLevelObject> saveLevelObjects_List;
		public List<SaveableLevelObject> saveStackableObjects_List;
		public List<Node_ObjectSaveable> saveNodeObjectsList;
	}
}
