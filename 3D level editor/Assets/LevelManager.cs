using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using gridMaster;

	public class LevelManager : MonoBehaviour
	{
		GridGenerator gridGenerator;
		
		public List<GameObject> inSceneGameObjects = new List<GameObject>();
		public List<GameObject> inSceneStackObjects = new List<GameObject>();
        public List<GameObject> inSceneWalls = new List<GameObject>();

		private static LevelManager instance = null;
		public static LevelManager GetInstance()
	 	{
			return instance;
		}
		
		void Awake()
		{
			instance = this;
		}
		
		void Start()
		{
			gridGenerator = GridGenerator.GetInstance();
			
		}

		
		

	}

