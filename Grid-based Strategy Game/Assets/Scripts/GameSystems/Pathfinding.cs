using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;


// TODO - Make class for holding a found path to be used by CharacterMovementHandler
// current way is too scuffed

public class Pathfinding
{
    public static Pathfinding Instance;

    public const int WALL_WEIGHT = 80000;

    private BinaryTree openListTree;
    private int openListCount;
    private HashSet<PathNode> closedList;
    private PathNode[][] existingNodes;


    private float cellSize;
    private int width, height;
    private Vector3 worldOrigin;

    private int movementCost = 15;
    private bool targetFound;

    public Pathfinding(Vector3 worldMinimum, Vector3 worldMaximum, float cellSize)
    {
        Instance = this;

        worldOrigin = worldMinimum;
        this.cellSize = cellSize;

        float worldWidth = worldMaximum.x - worldMaximum.x;
        float worldHeight = worldMaximum.y - worldMaximum.y;

        int width = Mathf.RoundToInt(worldWidth / cellSize);
        int height = Mathf.RoundToInt(worldHeight / cellSize);
        existingNodes = new PathNode[width][];
        for (int i = 0; i < width; i++)
        {
            existingNodes[i] = new PathNode[height];
        }
        Initialize();
    }

    public Pathfinding(int mapWidth, int mapHeight, float nodeSize, Vector3 worldOrigin)
    {
        Instance = this;

        this.cellSize = nodeSize;
        this.worldOrigin = worldOrigin;

        existingNodes = new PathNode[mapWidth][];
        for (int i = 0; i < mapWidth; i++)
        {
            existingNodes[i] = new PathNode[mapHeight];
        }
        width = mapWidth;
        height = mapHeight;

        Initialize();
    }
    public void RaycastWalkable()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 nodeWorldPosition = existingNodes[i][j].GetWorldVector(worldOrigin, cellSize);
                RaycastHit2D raycastHit = Physics2D.Raycast(nodeWorldPosition, Vector2.zero, 0f);
                if (raycastHit.collider != null)
                {
                    existingNodes[i][j].SetWalkable(false);
                }
            }
        }
    }
    public void Initialize()
    {
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

    public PathNode[][] GetExistingNodes()
    {
        return existingNodes;
    }

    public PathNode GetNode(int x, int y)
    {
        return existingNodes[x][y];
    }
    public List<PathNode> GetPath(Vector3 start, Vector3 end)
    {
        start -= worldOrigin;
        end -= worldOrigin;
        start /= cellSize;
        end /= cellSize;
        var startVec = GetClosestValidPos(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        var endVec = GetClosestValidPos(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));
        return FindPath((int)startVec.x, (int)startVec.y, (int)endVec.x, (int)endVec.y);

    }
    public List<PathNode> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        ConvertVectorPositionValidate(startWorldPosition, out int startX, out int startY);
        ConvertVectorPositionValidate(endWorldPosition, out int endX, out int endY);

        var path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            return path;
        }
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = existingNodes[startX][startY];
        PathNode endNode = existingNodes[endX][endY];

        currentNode.gCost = 0;

        CalculateAllHeuristics(endX, endY);

        openListTree = new BinaryTree();
        openListCount = 1;
        openListTree.AddNode(currentNode);
        closedList = new HashSet<PathNode>();
        targetFound = false;

        // If the beginning is the target
        if (currentNode == endNode)
        {
            return new List<PathNode> { currentNode };
        }

        while (openListCount > 0)
        {

            // Check the north node
            if (currentNode.moveNorth) DetermineNodeValues(currentNode, currentNode.north, endNode);
            // Check the east node
            if (currentNode.moveEast) DetermineNodeValues(currentNode, currentNode.east, endNode);
            // Check the south node
            if (currentNode.moveSouth) DetermineNodeValues(currentNode, currentNode.south, endNode);
            // Check the west node
            if (currentNode.moveWest) DetermineNodeValues(currentNode, currentNode.west, endNode);

            if (!targetFound)
            {
                openListTree.RemoveNode(currentNode);
                openListCount--;
                currentNode.isOnOpenList = false;
                closedList.Add(currentNode);
                currentNode.isOnClosedList = true;

                currentNode = openListTree.GetLowestValue();
            // Target found
            } else {
                return CalculatePath(endNode);
            }
        }

        // Out of nodes on the openList
        return null;
    }
    private void DetermineNodeValues(PathNode currentNode, PathNode testNode, PathNode endNode)
    {
        // Dont work on null nodes
        if (testNode == null)
            return;

        // Check to see if the node is the target
        if (testNode == endNode)
        {
            endNode.parent = testNode;
            targetFound = true;
            return;
        }
        // If it is not traversable
        if (!testNode.isWalkable)
        {
            return;
        }

        if (!testNode.isOnClosedList)
        {
            if (testNode.isOnOpenList)
            {
                int tentativeGCost = currentNode.gCost + testNode.weight + movementCost;

                // Take the lower gCost between test and current node
                if (tentativeGCost < testNode.gCost)
                {
                    testNode.Parent = currentNode;
                    testNode.gCost = tentativeGCost;
                    openListTree.RemoveNode(testNode);
                    testNode.CalculateFCost();
                    openListTree.AddNode(testNode);

                }
            } else {
                testNode.Parent = currentNode;
                testNode.gCost = currentNode.gCost + currentNode.weight + movementCost;
                testNode.CalculateFCost();
                openListTree.AddNode(testNode);
                testNode.isOnOpenList = true;
                openListCount++;
            }
        }
    }
    public PathStructure GetPathStructure(Vector3 start, Vector3 end)
    {
        List<PathNode> pathNodeList = GetPath(start, end);
        return new PathStructure(pathNodeList, worldOrigin, cellSize);
    }
    public Vector3 GetClosestValidPosition(Vector3 position)
    {
        int mapX, mapY;
        ConvertVectorPositionValidate(position, out mapX, out mapY);
        var closestValidMapPos = GetClosestValidPos(mapX, mapY);
        PathNode pathNode = existingNodes[(int)closestValidMapPos.x][(int)closestValidMapPos.y];
        return pathNode.GetWorldVector(worldOrigin, cellSize);
    }
    // Check out of bounds and correct if needed
    private Vector3 GetClosestValidPos(int mapX, int mapY)
    {
        // Inside bounds
        while (mapX < 0) mapX++;
        while (mapY < 0) mapY++;
        while (mapX >= width) mapX--;
        while (mapY >= height) mapY--;

        return new Vector3(mapX, mapY);
    }
    // Construct the path from all nodes
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode > pathNodes = new List<PathNode>();
        PathNode currentNode = endNode;
        while(currentNode.Parent != null)
        {
            pathNodes.Add(currentNode.Parent);
            currentNode = currentNode.Parent;
        }
        pathNodes.Reverse();
        return pathNodes;
    }
    // Initialize the pathnodes hCost
    private void CalculateAllHeuristics(int endX, int endY)
    {
        int rows = height;
        int cols = width;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                CalculateManhattanDistance(existingNodes[x][y], x, y, endX, endY);
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
    private void ConvertVectorPosition(Vector3 position, out int x, out int y)
    {
        x = (int)((position.x - worldOrigin.x) / cellSize);
        y = (int)((position.y - worldOrigin.y) / cellSize);
    }
    private void ConvertVectorPositionValidate(Vector3 position, out int x, out int y)
    {
        ConvertVectorPosition(position, out x, out y);

        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x >= width) x = width - 1;
        if (y >= height) y = height - 1;
    }
}
