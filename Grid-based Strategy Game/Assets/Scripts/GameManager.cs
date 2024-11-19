using UnityEngine;

namespace GridCombat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private GameGrid<CombatGridSystem.CombatGridObject> grid;
        private Pathfinding pathfindingGrid;

        private void Awake()
        {
            Instance = this;

            int mapWidth = 30;
            int mapHeight = 15;
            float cellSize = 10f;

            Vector3 origin = new Vector3(0, 0);

            grid = new GameGrid<CombatGridSystem.CombatGridObject>(mapWidth, mapHeight, cellSize, origin, (GameGrid<CombatGridSystem.CombatGridObject> g, int x, int y) => new CombatGridSystem.CombatGridObject(g, x, y));
            pathfindingGrid = new Pathfinding(mapWidth, mapHeight, cellSize, Vector3.zero);

        }

        // Start is called before the first frame update
        private void Start()
        {
        }
        private void HandleCameraMovement()
        {
            var camera = Camera.main;

            var gridSizeVector = grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight());
            camera.transform.position = new Vector3(gridSizeVector.x / 2, gridSizeVector.y / 2, -10);
            camera.orthographicSize = Mathf.Max(gridSizeVector.x, gridSizeVector.y) * 0.20f + 10;

            Vector3 moveDir = new Vector3(0, 0);
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                moveDir.y = +1;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                moveDir.y = -1;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir.x = -1;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveDir.x = +1;
            }
            moveDir.Normalize();

            camera.transform.position += moveDir;
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
