using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int xPos;
    public int yPos;
    public PathNode parent;
    public PathNode north;
    public PathNode south;
    public PathNode west;
    public PathNode east;
    public bool moveNorth;
    public bool moveSouth;
    public bool moveWest;
    public bool moveEast;
    public bool isOnOpenList = false;
    public bool isOnClosedList = false;

    public int weight = 0;
    public int gCost = 0;
    public int hCost;
    public int fCost;

    public Boolean isWalkable = true;
    public PathNode Parent;

    public PathNode(int _xPos, int _yPos)
    {
        xPos = _xPos;
        yPos = _yPos;

        moveNorth = true;
        moveSouth = true;
        moveWest = true;
        moveEast = true;
    }
    public void ResetRestrictions()
    {
        moveNorth = true;
        moveSouth = true;
        moveWest = true;
        moveEast = true;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void SetWalkable(bool walkable)
    {
        weight = walkable ? 0 : Pathfinding.WALL_WEIGHT;
    }
    public void SetWeight(int weight)
    {
        this.weight = weight;
    }
    public Vector3 GetWorldVector(Vector3 worldOrigin, float nodeSize)
    {
        return worldOrigin + new Vector3(xPos * nodeSize, yPos * nodeSize);
    }
    public override string ToString()
    {
        return xPos + " " + yPos;
    }
}
