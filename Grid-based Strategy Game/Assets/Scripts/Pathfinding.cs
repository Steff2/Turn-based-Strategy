using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GameGrid<PathNode> grid;

    private List<PathNode> OpenList;
    private HashSet<PathNode> ClosedList;

    public Pathfinding(int width, int height)
    {
        //grid = new GameGrid<PathNode> (width, height, 10f, Vector3.zero, (GameGrid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }
}
