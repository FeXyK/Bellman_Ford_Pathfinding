using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{

    public Vector3 w_centerPosition;
    public Vector3 size;
    public bool isWalkable = true;
    public  float distance;
    public int xPos;
    public int zPos;
    public PathfindingNode ParentNode { get;  set; }

    public PathfindingNode()
    {

    }
    public PathfindingNode(Vector3 _centerPosition, Vector3 _size, int _xPos, int _zPos)
    {
        xPos = _xPos;
        zPos = _zPos;
        w_centerPosition = _centerPosition;
        size = _size;
    }


    public void CheckWall(List<LayerMask> _layerMasks)
    {
        isWalkable = true;
        foreach (var mask in _layerMasks)
        {
            if (Physics.CheckSphere(w_centerPosition, size.x, mask))
            {
                isWalkable = false;
            }
        }
    }
    public override string ToString()
    {
        return w_centerPosition.ToString() + "\tDistance: " + distance;
    }
}
