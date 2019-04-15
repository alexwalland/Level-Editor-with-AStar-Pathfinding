using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace gridMaster
{
    public class GridGenerator : MonoBehaviour
    {


        public GameObject nodePrefab;

        public int sizeX = 15;
        public int sizeZ = 15;
        public int offset = 2;
        public List<Node> reset;

        public Node[,] grid;

        private static GridGenerator instance = null;
        public static GridGenerator GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
            CreateGrid();
            CreateMouseCollison();
        }

        void CreateGrid()
        {
            grid = new Node[sizeX, sizeZ];

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float posX = (x + 1) * offset;
                    float posZ = z * offset;

                    GameObject go = Instantiate(nodePrefab, new Vector3(posX, 0, posZ), Quaternion.identity) as GameObject;
                    go.transform.parent = transform.GetChild(1).transform;

                    Node_Object nodeObj = go.GetComponent<Node_Object>();
                    nodeObj.posX = x;
                    nodeObj.posZ = z;

                    Node node = new Node();
                    node.vis = go;
                    node.tileRenderer = node.vis.GetComponentInChildren<MeshRenderer>();
                    node.nodePosX = x;
                    node.nodePosZ = z;
                    grid[x, z] = node;
                    node.isWalkable = true;
                }
            }
        }

        void CreateMouseCollison()
        {
            GameObject go = new GameObject();
            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().size = new Vector3((sizeX * offset) / 2 - 1, 0, (sizeZ * offset) / 2 - 1);
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            float worldX = worldPosition.x;
            float worldZ = worldPosition.z;

            worldX /= offset;
            worldZ /= offset;

            //makes sure right node is linked
            int x = Mathf.RoundToInt(worldX) -1;
            int z = Mathf.RoundToInt(worldZ);

            if (x > sizeX)
                x = sizeX;
            if (z > sizeZ)
                z = sizeZ;
            if (x < 0)
                x = 0;
            if (z < 0)
                z = 0;

            return grid[x, z];
        }

        public bool start;
        void Update()
        {
            if (start)
            {
                start = false;

                Pathfinding.Pathfinder path = new Pathfinding.Pathfinder();

				int N1x = Random.Range (1, 11);
				int N1y = Random.Range (1, 3);
				int N2x = Random.Range (1, 11);
				int N2y = Random.Range (11, 13);

				Node startNode = grid [N1x,N1y];
				Node end = grid [N2x, N2y];

                path.start = startNode;
                path.end = end;

                List<Node> p = path.FindPath();

                reset = p;

                startNode.vis.SetActive(false);

                reset.Add(startNode);

                foreach (Node n in p)
                {
                    n.vis.SetActive(false);
                }
            }
        }
        

        public Node GetNode(int x, int z)
        {

            Node retVal = null;

            if (x < sizeX && x >= 0 &&
                z >= 0 && z < sizeZ)
            {
                retVal = grid[x, z];
            }

            return retVal;
        }

        public Node GetNodeFromVector3(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x);
            int z = Mathf.RoundToInt(pos.z);

            Node retVal = GetNode(x, z);
            return retVal;
        }

        public void onClick()
        {
            start = true;
        }

        public void Reset()
        {
            foreach (Node n in reset)
            {
                n.vis.SetActive(true);
            }
        }
    }
}