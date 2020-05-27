using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingMovement : MonoBehaviour
{
    private PathfindingNode currentNode = null;
    private PathfindingTileManager PTM = null;
    private PathfindingNode targetNode;
    private int distanceMultiplier = 1;
    public bool RootMotionMovement = false;
    public int parentDepth = 200;
    public int maxRngPathFactor = 1;
    public float Speed = 7.0f;
    public float TurnSpeed = 4.0f;

    void Start()
    {
        PTM = FindObjectOfType<PathfindingTileManager>();
        if (RootMotionMovement)
        {
            GetComponent<Animator>().applyRootMotion = true;
        }
    }

    void FixedUpdate()
    {
        if (PTM != null)
        {
            distanceMultiplier = PTM.size;

            currentNode = PTM.NodeFromWorldPosition(this.transform.position);
            if (targetNode == null || isClose(targetNode.w_centerPosition, transform.position))
            {
                targetNode = FindFurthestNodeAvailable(currentNode, parentDepth);
            }

            var lookPos = Quaternion.LookRotation(targetNode.w_centerPosition - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookPos, Time.deltaTime * TurnSpeed);
            if (!RootMotionMovement)
                transform.position += transform.forward * Time.deltaTime * Speed;

        }
    }
    private bool isClose(Vector3 v1, Vector3 v2)
    {
        if (v1 != null && v2 != null)
            return Vector3.SqrMagnitude(v2 - v1) < 1.0f;
        return true;
    }
    private PathfindingNode GetParentOf(PathfindingNode node, float depth)
    {
        PathfindingNode parent = node;
        for (int i = 0; i < depth; i++)
        {
            if (parent.ParentNode == null)
                return parent;

            parent = parent.ParentNode;
        }
        return parent;
    }
    private bool isWallBlocking(PathfindingNode target, Vector3 start, float depth)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.w_centerPosition - this.transform.position, out hit, Vector3.Distance(target.w_centerPosition, transform.position)))
        {
            if (hit.collider.gameObject.layer == 9)
            {
                return true;
            }
        }
        return false;
    }
    public PathfindingNode FindFurthestNodeAvailable(PathfindingNode currentNode, int depth)
    {
        int rngPathFactor = Random.Range(0, depth / 2);
        if (rngPathFactor > maxRngPathFactor)
            rngPathFactor = maxRngPathFactor;

        PathfindingNode target = GetParentOf(currentNode, depth - rngPathFactor);
        if (isWallBlocking(target, this.transform.position, depth))
        {
            if (depth <= 2)
                return GetParentOf(currentNode, 2);
            else
                return target = FindFurthestNodeAvailable(currentNode, depth - distanceMultiplier);
        }

        return target;
    }
    void OnDrawGizmosSelected()
    {
        if (targetNode != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = targetNode.w_centerPosition - this.transform.position;
            Gizmos.DrawRay(transform.position + Vector3.up, direction);
        }
    }
}
