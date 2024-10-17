using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.HealthSystemCM;

public class TestUnitMovement : MonoBehaviour, IGetHealthSystem
{
    [SerializeField] private CharacterMovementHandler characterPathfinding;
    private Pathfinding pathfinding;
    private GameGrid<PathNode> grid;

    private static float speed = 5;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = new HealthSystem(100);
        pathfinding = new Pathfinding(20, 10);
    }

    // Start is called before the first frame update
    private void Start()
    {

        grid = pathfinding.GetGrid();

        var gridSizeVector = grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight());
        var camera = Camera.main;

        camera.transform.position = new Vector3(gridSizeVector.x / 2, gridSizeVector.y / 2, -10);
        camera.orthographicSize = Mathf.Max(gridSizeVector.x, gridSizeVector.y) * 0.20f + 10;

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(transform.position, out int startX, out int startY);
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int endX, out int endY);

            if (!pathfinding.GetNode(endX, endY).isWalkable) return;

            List<PathNode> path = pathfinding.FindPath(startX, startY, endX, endY);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.white, 5f);
                }
            }
            characterPathfinding.SetTargetPosition(mouseWorldPosition);
        }
        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }

    public void TakeDamage(float damageTaken)
    {
        healthSystem.Damage(damageTaken);
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
