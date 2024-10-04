using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GameGrid<PathNode> grid;
    private int x;
    private int y;

    private int gCost;
    private int hCost;
    private int fCost;

    public PathNode cameFromNode;

    public PathNode(GameGrid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

    }

    public override string ToString()
    {
        return x + " " + y;
    }
}
