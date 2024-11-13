using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;


// TODO - Make class for holding a found path to be used by CharacterMovementHandler
// current way is too scuffed

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
    private Vector3 worldOrigin;
    public List<PathNode> pathNodeList;
    public List<Vector3> pathVectorList;

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

    public PathNode[][] GetMapNodes()
    {
        return existingNodes;
    }

    public PathNode GetNode(int x, int y)
    {
        return existingNodes[x][y];
    }
    public List<PathNode> GetPath(Vector3 start, Vector3 end)
    {
        start = start - worldOrigin;
        end = end - worldOrigin;
        start = start / cellSize;
        end = end / cellSize;
        var startMapPos = GetClosestValidPos(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        var endMapPos = GetClosestValidPos(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));
        return FindPath((int)startMapPos.x, (int)startMapPos.y, (int)endMapPos.x, (int)endMapPos.y);
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

        PathNode currentNode = existingNodes[startX][startY];
        PathNode endNode = existingNodes[endX][endY];

        openList = new List<PathNode>{ currentNode };
        closedList = new HashSet<PathNode>();

        CalculateAllHeuristics(endX, endY);

        /*for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                //pathNode.gCost = 99999999;
                //pathNode.CalculateFCost();
            }
        }*/
        if (currentNode == endNode)
        {
            return new List<PathNode> { currentNode };
        }

        while (openList.Count > 0)
        {

            // Check the north node
            if (currentNode.moveNorth) DetermineNodeValues(currentNode, currentNode.north, endNode);
            // Check the east node
            if (currentNode.moveEast) DetermineNodeValues(currentNode, currentNode.east, endNode);
            // Check the south node
            if (currentNode.moveSouth) DetermineNodeValues(currentNode, currentNode.south, endNode);
            // Check the west node
            if (currentNode.moveWest) DetermineNodeValues(currentNode, currentNode.west, endNode);

            currentNode = GetLowestFCostNode(openList);

            openList.Remove(currentNode);
            currentNode.isOnOpenList = false;
            closedList.Add(currentNode);
            currentNode.isOnClosedList = true;

            if (currentNode == endNode)
            {
                openList.Add(currentNode);
                return CalculatePath(endNode);
            }
        }

        //Out of nodes on the openList
        return null;
    }
    private void DetermineNodeValues(PathNode currentNode, PathNode testNode, PathNode endNode)
    {
        if (!closedList.Contains(testNode))
        {
            if (!openList.Contains(testNode))
            {
                if (!testNode.isWalkable)
                {
                    closedList.Add(testNode);
                    return;
                }

                int tentativeGCost = currentNode.gCost + testNode.weight + movementCost;

                if (tentativeGCost < testNode.gCost)
                {
                    testNode.Parent = currentNode;
                    testNode.gCost = tentativeGCost;
                    testNode.CalculateFCost();
                    openList.Add(testNode);
                    testNode.isOnOpenList = true;
                }
            } else {
                testNode.Parent = currentNode;
                testNode.gCost = currentNode.gCost + testNode.weight + movementCost;
                openList.Remove(testNode);
                testNode.CalculateFCost();
                openList.Add(testNode);
            }
        }
    }
    private void CalculateManhattanDistance(PathNode currentNode, int currX, int currY, int targetX, int targetY)
    {
        currentNode.parent = null;
        currentNode.hCost = (Mathf.Abs(currX - targetX) + Mathf.Abs(currY - targetY));
        currentNode.isOnOpenList = false;
        currentNode.isOnClosedList = false;
    }
    private Vector3 GetClosestValidPos(int mapX, int mapY)
    {
        int width = cellWidth;
        int height = cellHeight;
        // Inside bounds
        while (mapX < 0) mapX++;
        while (mapY < 0) mapY++;
        while (mapX >= width) mapX--;
        while (mapY >= height) mapY--;

        return new Vector3(mapX, mapY);
    }
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new() { endNode };
        PathNode currentNode = endNode;
        while(currentNode.Parent != null)
        {
            path.Add(currentNode.Parent);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        PathRoute(path, worldOrigin, cellSize);
        return path;
    }
    private void CalculateAllHeuristics(int endX, int endY)
    {
        int rows = cellHeight;
        int cols = cellWidth;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                CalculateManhattanDistance(existingNodes[x][y], x, y, endX, endY);
            }
        }
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
    public void PathRoute(List<PathNode> pathNodeList, Vector3 worldOrigin, float cellSize)
    {
        this.pathNodeList = pathNodeList;
        pathVectorList = new List<Vector3>();
        foreach (PathNode pathNode in pathNodeList)
        {
            pathVectorList.Add(pathNode.GetWorldVector(worldOrigin, cellSize));
        }
    }
}
