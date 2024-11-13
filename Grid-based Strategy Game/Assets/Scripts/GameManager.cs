using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.HealthSystemCM;

namespace GridCombat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private CharacterMovementHandler characterPathfinding;
        private GameGrid<CombatGridSystem.CombatGridObject> grid;
        private Pathfinding pathfindingGrid;

        private static float speed = 5;

        private void Awake()
        {
            Instance = this;

            int mapWidth = 30;
            int mapHeight = 15;
            float cellSize = 8f;

            Vector3 origin = new Vector3(0, 0);

            grid = new GameGrid<CombatGridSystem.CombatGridObject>(mapWidth, mapHeight, cellSize, origin, (GameGrid<CombatGridSystem.CombatGridObject> g, int x, int y) => new CombatGridSystem.CombatGridObject(g, x, y));
            pathfindingGrid = new Pathfinding(mapWidth, mapHeight, cellSize);

        }

        // Start is called before the first frame update
        private void Start()
        {


            var gridSizeVector = grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight());
            var camera = Camera.main;

            camera.transform.position = new Vector3(gridSizeVector.x / 2, gridSizeVector.y / 2, -10);
            camera.orthographicSize = Mathf.Max(gridSizeVector.x, gridSizeVector.y) * 0.20f + 10;

        }

        public GameGrid<CombatGridSystem.CombatGridObject> GetGrid()
        { 
            return grid;
        }

        public Pathfinding GetPathfinding() 
        { 
            return pathfindingGrid;
        }
    }
}
