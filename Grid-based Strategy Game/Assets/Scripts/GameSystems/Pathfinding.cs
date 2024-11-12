using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Pathfinding
{
    private int movementCost = 15;

    public const int WALL_WEIGHT = 80000;
    public static Pathfinding Instance { get; private set; }

    private GameGrid<PathNode> grid;
    private List<PathNode> openList;
    private HashSet<PathNode> closedList;
    private PathNode[][] existingNodes;

    private float cellSize;
    private int cellWidth;
    private int cellHeight;


    public Pathfinding(int width, int height, float cellSize)
    {
        Instance = this;

        this.cellWidth = width;
        this.cellSize = cellSize;
        this.cellHeight = height;

        Initialize(width, height);
    }

    public void Initialize(int mapWidth, int mapHeight)
    {
        // Creates PathNodes
        for (int x = 0; x < existingNodes.Length; x++)
        {
            for (int y = 0; y < existingNodes[x].Length; y++)
            {
                existingNodes[x][y] = new PathNode(x, y);
            }
        }
        UpdateNodeConnections();
    }

    private void UpdateNodeConnections()
    {
        for (int x = 0; x < existingNodes.Length; x++)
        {
            for (int y = 0; y < existingNodes[x].Length; y++)
            {
                if (y < existingNodes[x].Length - 1)
                    existingNodes[x][y].north = existingNodes[x][y + 1];
                if (y > 0)
                    existingNodes[x][y].south = existingNodes[x][y - 1];

                if (x < existingNodes.Length - 1)
                    existingNodes[x][y].east = existingNodes[x + 1][y];
                if (x > 0)
                    existingNodes[x][y].west = existingNodes[x - 1][y];
            }
        }
    }

    public GameGrid<PathNode> GetGrid()
    {
        return grid;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.xPos, pathNode.yPos) * grid.GetCellSize() + .5f * grid.GetCellSize() * new Vector3(1, 1));
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode>{ startNode };
        closedList = new HashSet<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        CalculateManhattanDistance(startNode, startNode.xPos, startNode.yPos, endNode.xPos, endNode.yPos);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + neighbourNode.weight + movementCost;
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    CalculateManhattanDistance(neighbourNode, neighbourNode.xPos, neighbourNode.yPos, endNode.xPos, endNode.yPos);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        //Out of nodes on the openList
        return null;
    }

    private void CalculateManhattanDistance(PathNode currentNode, int currX, int currY, int targetX, int targetY)
    {
        currentNode.parent = null;
        currentNode.hCost = (Mathf.Abs(currX - targetX) + Mathf.Abs(currY - targetY));
        currentNode.isOnOpenList = false;
        currentNode.isOnClosedList = false;
    }

    private List<PathNode> GetNeighourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();

        if(currentNode.xPos - 1 >= 0)
        {
            // Left
            neighbourList.Add(grid.GetGridObject(currentNode.xPos - 1, currentNode.yPos));
            // Left Down
            if (currentNode.yPos - 1 >= 0) neighbourList.Add(grid.GetGridObject(currentNode.xPos - 1, currentNode.yPos - 1));
            // Left Up
            if (currentNode.yPos + 1 < grid.GetHeight()) neighbourList.Add(grid.GetGridObject(currentNode.xPos - 1, currentNode.yPos + 1));
        }
        if(currentNode.xPos + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(grid.GetGridObject(currentNode.xPos + 1, currentNode.yPos));
            //Right Down
            if (currentNode.yPos - 1 >= 0) neighbourList.Add(grid.GetGridObject(currentNode.xPos + 1, currentNode.yPos - 1));
            //Right Up
            if (currentNode.yPos + 1 < grid.GetHeight()) neighbourList.Add(grid.GetGridObject(currentNode.xPos + 1, currentNode.yPos + 1));
        }
        // Down
        if (currentNode.yPos - 1 >= 0) neighbourList.Add(grid.GetGridObject(currentNode.xPos, currentNode.yPos - 1));
        // Up
        if (currentNode.yPos + 1 < grid.GetHeight()) neighbourList.Add(grid.GetGridObject(currentNode.xPos, currentNode.yPos + 1));

        return neighbourList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new() { endNode };
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
