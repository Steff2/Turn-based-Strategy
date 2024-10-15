using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Utils;

public class TestGrid : MonoBehaviour
{
    public static GameGrid<bool> grid;
    private bool counter = true;
    Vector3 gridSizeVector;


    public void Start()
    {
        grid = new GameGrid<bool>(5, 3, 4f, Vector3.zero, (GameGrid<bool> g, int x, int y) => new bool());
        gridSizeVector = grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight());

        var camera = Camera.main;

        camera.transform.position = new Vector3(gridSizeVector.x / 2, gridSizeVector.y / 2, -10);
        camera.orthographicSize = Mathf.Max(gridSizeVector.x, gridSizeVector.y) * 0.25f + 10;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.SetGridObject(GameUtils.GetMouseWorldPosition(), counter);
        }
        
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetGridObject(GameUtils.GetMouseWorldPosition()));
        }
    }
}
