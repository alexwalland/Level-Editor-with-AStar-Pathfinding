using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using gridMaster;

namespace Pathfinding
{
    class Pathfinder
    {
        GridGenerator gridGenerator;
        public Node start;
        public Node end;

        public List<Node> FindPath()
        {
            gridGenerator = GridGenerator.GetInstance();

            return FindPathActual(start, end);
        }

        private List<Node> FindPathActual(Node startPos, Node target)
        {
            //uses A* pathfinding to  find the shortest route 
            List<Node> foundPath = new List<Node>();

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost &&
                        openSet[i].hCost < currentNode.hCost))
                    {
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }
                
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

               if (currentNode.Equals(target))
                {
                  foundPath = RetracePath(start, currentNode);
                    break;
                }

               foreach (Node neighbour in GetNeighbours(currentNode, true))
                {
                    if (!closedSet.Contains(neighbour))
                    {
                        float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, target);
                            neighbour.parentNode = currentNode;
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }
            return foundPath;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)

        {
            //adds nodes to path from back to front then reverses the list
            List<Node> path = new List<Node>();

            Node currentNode = endNode;
            
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parentNode;

            }
            path.Reverse();
            return path;

        }

        private List<Node> GetNeighbours(Node node, bool getVerticalneighbours = false)
        {
            List<Node> retList = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int y = yIndex;

                       if (!getVerticalneighbours)
                        {
                            y = 0;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                        }
                        else
                        {
                            Node searchPos = new Node();

                           searchPos.nodePosX = node.nodePosX + x;
                           searchPos.nodePosZ = node.nodePosZ + z;

                            Node newNode = GetNeighbourNode(searchPos, true, node);

                            if (newNode != null)
                            {
                                retList.Add(newNode);
                            }
                        }
                    }
                }
            }

            return retList;

        }

        private Node GetNeighbourNode(Node adjPos, bool searchTopDown, Node currentNodePos)
        {

            Node retVal = null;

            searchTopDown = false;

            Node node = GetNode(adjPos.nodePosX, adjPos.nodePosZ);
            
            if (node != null && node.isWalkable)
            {
                retVal = node;
            }
            else if (searchTopDown)
            {
                Node bottomBlock = GetNode(adjPos.nodePosX, adjPos.nodePosZ);
                
                if (bottomBlock != null && bottomBlock.isWalkable)
                {
                    retVal = bottomBlock;
                }
                else
                {
                    Node topBlock = GetNode(adjPos.nodePosX, adjPos.nodePosZ);
                    if (topBlock != null && topBlock.isWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }
            
            int originalX = adjPos.nodePosX - currentNodePos.nodePosX;
            int originalZ = adjPos.nodePosZ - currentNodePos.nodePosZ;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                Node neighbour1 = GetNode(currentNodePos.nodePosX + originalX, currentNodePos.nodePosZ);
                if (neighbour1 == null || !neighbour1.isWalkable)
                {
                    retVal = null;
                }

                Node neighbour2 = GetNode(currentNodePos.nodePosX, currentNodePos.nodePosZ + originalZ);
                if (neighbour2 == null || !neighbour2.isWalkable)
                {
                    retVal = null;
                }
            }
            
            if (retVal != null)
            {
            }

            return retVal;
        }

        private Node GetNode(int x, int z)
        {
            Node n = null;

            lock (gridGenerator)
            {
                n = gridGenerator.GetNode(x, z);
            }
            return n;
        }

        private int GetDistance(Node posA, Node posB)
        {
            int distX = Mathf.Abs(posA.nodePosX - posB.nodePosX);
            int distZ = Mathf.Abs(posA.nodePosZ - posB.nodePosZ);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * 1;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * 1;
        }

    }

}