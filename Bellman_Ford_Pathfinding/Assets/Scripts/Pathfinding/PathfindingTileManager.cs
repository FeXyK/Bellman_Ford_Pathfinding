using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTileManager : MonoBehaviour
{
    public Transform Target;
    public PathfindingNode[,] Nodes = null;
    public int size = 1;
    float MaxDistance = 0;
    public List<LayerMask> layerMasks;
    public Transform Corner1;
    public Transform Corner2;

    void Start()
    {
        GameObject[] corners = GameObject.FindGameObjectsWithTag("Corner");
        if (corners.Length >= 1)
        {
            Corner1 = corners[0].transform;
            Corner2 = corners[1].transform;
        }
        if (Target != null)
        {
            RegenerateTiles();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegenerateTiles();
        }
    }
    public void RegenerateTiles()
    {
        SortCorners();
        GenerateTilesBetween(Corner1.position, Corner2.position, size);
        CalculateDistances(Target.position);
    }
    public void RecalculateDistances()
    {
        CalculateDistances(Target.position);
    }
    public void GenerateTilesBetween(Vector3 vec1, Vector3 vec2, int size)
    {
        int x = (int)(vec2.x - vec1.x) / size;
        int z = (int)(vec2.z - vec1.z) / size;
        Nodes = new PathfindingNode[z, x];
        for (int i = 0; i < z; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Nodes[i, j] = new PathfindingNode(new Vector3((j * size) + vec1.x, 0, (i * size) + vec1.z), new Vector3(size, size, size), j, i);
            }
        }
    }

    public void CalculateDistances(Vector3 startPos)
    {
        List<PathfindingNode> nodes = new List<PathfindingNode>();

        foreach (var node in Nodes)
        {
            if (node != null)
            {
                node.CheckWall(layerMasks);
                node.distance = float.PositiveInfinity;
                node.ParentNode = null;
                nodes.Add(node);
            }
        }
        bool isChanged = true;

        NodeFromWorldPosition(startPos).distance = 0;

        for (int i = 0; i < nodes.Count && isChanged; i++)
        {
            isChanged = false;
            foreach (var v in nodes)
            {
                if (MaxDistance < v.distance && v.distance < 10000)
                {
                    MaxDistance = v.distance;
                }
                foreach (var u in GetNeighboringNodes(v))
                {
                    if (u.distance + 1 < v.distance)
                    {
                        isChanged = true;
                        v.distance = u.distance + 1;
                        v.ParentNode = u;
                    }
                }
            }
        }
    }
    public List<PathfindingNode> GetNeighboringNodes(PathfindingNode node)
    {
        List<PathfindingNode> NeighborList = new List<PathfindingNode>();//Make a new list of all available neighbors.
        PathfindingNode CheckNode = null;
        int xN = node.xPos;
        int zN = node.zPos;
        //Check the right side of the current node.
        if (zN + 1 < Nodes.GetLength(0))
        {
            CheckNode = Nodes[zN + 1, xN];
            if (CheckNode.isWalkable)
                NeighborList.Add(CheckNode);//Add the grid to the available neighbors list
        }
        //Check the Left side of the current node.
        if (zN - 1 >= 0)
        {
            CheckNode = Nodes[zN - 1, xN];
            if (CheckNode.isWalkable)
                NeighborList.Add(CheckNode);//Add the grid to the available neighbors list
        }
        if (xN + 1 < Nodes.GetLength(1))
        {
            CheckNode = Nodes[zN, xN + 1];
            if (CheckNode.isWalkable)
                NeighborList.Add(CheckNode);//Add the grid to the available neighbors list
        }
        if (xN - 1 >= 0)
        {
            CheckNode = Nodes[zN, xN - 1];
            if (CheckNode.isWalkable)
                NeighborList.Add(CheckNode);//Add the grid to the available neighbors list
        }//Add the grid to the available neighbors list

        return NeighborList;//Return the neighbors list.
    }

    public void SortCorners()
    {
        float c1, c2;
        Vector3 cPosition1, cPosition2;

        cPosition1 = Corner1.position;
        cPosition2 = Corner2.position;
        if (cPosition1.x > cPosition2.x && cPosition1.z > cPosition2.z)
        {
            Transform temp = Corner1;
            Corner1 = Corner2;
            Corner2 = temp;
        }
        if (cPosition1.x > cPosition2.x && cPosition1.z < cPosition2.z)
        {
            c1 = Corner1.position.x;
            c2 = Corner2.position.x;
            Corner1.position = new Vector3(c2, 0, Corner1.position.z);
            Corner2.position = new Vector3(c1, 0, Corner2.position.z);
        }
        if (cPosition1.x < cPosition2.x && cPosition1.z > cPosition2.z)
        {
            c1 = Corner1.position.z;
            c2 = Corner2.position.z;
            Corner1.position = new Vector3(Corner1.position.x, 0, c2);
            Corner2.position = new Vector3(Corner2.position.x, 0, c1);
        }
    }
    public PathfindingNode NodeFromWorldPosition(Vector3 worldPosition)
    {
        return Nodes[((int)worldPosition.z - (int)Corner1.position.z) / size, ((int)worldPosition.x - (int)Corner1.position.x) / size];
    }
    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public void OnDrawGizmosSelected()
    {
        if (Nodes != null)
            foreach (var node in Nodes)
            {
                if (node.isWalkable)
                {
                    float intensity = Remap(node.distance, 500, 0, 0, 1);
                    Gizmos.color = new Color(0, intensity, 0, 1.0f);//Set the color of the node
                }
                else//If the current node is a wall node
                {
                    Gizmos.color = Color.yellow;

                }
                Gizmos.DrawCube(node.w_centerPosition, node.size);
            }
    }
}
