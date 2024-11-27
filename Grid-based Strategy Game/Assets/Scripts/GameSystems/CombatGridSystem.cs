using CodeMonkey.Utils;
using UnityEngine;
using Utils;
using Utils.HealthSystemCM;

namespace GridCombat
{
    public class CombatGridSystem : MonoBehaviour
    {
        private const int MAXMOVEMENTDISTANCE = 4;
        public enum State
        {
            Idle,
            Walking,
            Waiting
        }

        private State state;

        [SerializeField] UnitGridCombat unitGridCombat;
        // Start is called before the first frame update
        void Awake()
        {
            state = State.Idle;
        }
        private void Start()
        {
            ManageMovement();
        }
        private void ManageMovement()
        {
            GameGrid<CombatGridObject> grid = GameManager.Instance.GetGrid();
            Pathfinding gridPathfinding = GameManager.Instance.GetPathfinding();

            // Get Unit Grid Position X, Y
            grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);

            // Reset Entire Grid ValidMovePositions
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    grid.GetGridObject(x, y).SetTraversable(false);
                }
            }

            for (int x = unitX - MAXMOVEMENTDISTANCE; x <= unitX + MAXMOVEMENTDISTANCE; x++)
            {
                for (int y = unitY - MAXMOVEMENTDISTANCE; y <= unitY + MAXMOVEMENTDISTANCE; y++)
                {
                    if (!gridPathfinding.IsInBoundaries(x, y)) { continue; }

                    if (!gridPathfinding.IsWalkable(x, y)) { continue; }

                    var path = gridPathfinding.ShortcutPath(unitX, unitY, x, y);
                    if (path.vectorPathList == null || path.vectorPathList.Count > MAXMOVEMENTDISTANCE)
                    { continue; }

                    grid.GetGridObject(x, y).SetTraversable(true);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Idle:

                    if (Input.GetMouseButtonDown(0))
                    {
                        GameGrid<CombatGridObject> grid = GameManager.Instance.GetGrid();
                        CombatGridObject gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());

                        if (gridObject.GetUnitGridCombat() != null)
                        {
                            state = State.Waiting;

                            unitGridCombat.AttackUnit(unitGridCombat, () =>
                            {
                                state = State.Idle;
                                ManageMovement();
                            });
                        }

                        if (gridObject.GetTraversable())
                        {
                            state = State.Walking;
                            unitGridCombat.MoveTo(GameUtils.GetMouseWorldPosition(), () => {
                                state = State.Idle;
                                ManageMovement();
                            });
                        }
                    }
                    break;


                case State.Walking:
                    break;
            }
        }
        public void GetAttackTarget(Vector3 attackTargetPosition)
        {

        }
        public class CombatGridObject
        {
            private GameGrid<CombatGridObject> grid;
            private int x;
            private int y;
            private UnitGridCombat unitCombatObject;
            private bool isTraversable = false;

            public CombatGridObject(GameGrid<CombatGridObject> grid, int x, int y)
            {
                this.grid = grid;
                this.x = x;
                this.y = y;
            }
            public void SetUnitGridObject(UnitGridCombat gridObject)
            {
                this.unitCombatObject = gridObject;
            }
            public UnitGridCombat GetUnitGridCombat() 
            {
                return unitCombatObject; 
            }
            public void ClearUnitCombatObject()
            {
                unitCombatObject = null;
            }
            public void SetTraversable(bool set)
            {
                isTraversable = set;
            }
            public bool GetTraversable()
            {
                return isTraversable;
            }
        }
    }
}
