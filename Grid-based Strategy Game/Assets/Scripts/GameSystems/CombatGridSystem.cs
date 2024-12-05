using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

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

        [SerializeField] private Grid tileMapGrid;
        [SerializeField] private UnitGridCombat[] unitGridCombatArray;

        private State state;

        private List<UnitGridCombat> redTeamList;
        private List<UnitGridCombat> blueTeamList;

        private UnitGridCombat activeUnit;

        private int redTeamIndex = -1;
        private int blueTeamIndex = -1;

        private bool hasAttacked = false;
        private bool hasMoved = false;

        private GameGrid<CombatGridObject> grid;
        private Pathfinding gridPathfinding;
        private TilemapManager tilemapManager;

        private Vector3Int[] movementTilePositions;
        private TileBase[] movementTileArray;

        // Start is called before the first frame update
        void Awake()
        {
            state = State.Idle;

        }
        private void Start()
        {
            redTeamList = new List<UnitGridCombat>();
            blueTeamList = new List<UnitGridCombat>();
            grid = GameManager.Instance.GetGrid();
            gridPathfinding = GameManager.Instance.GetPathfinding();
            tilemapManager = GameManager.Instance.GetTileMaps();

            movementTilePositions = new Vector3Int[(MAXMOVEMENTDISTANCE * 2 + 1) * (MAXMOVEMENTDISTANCE * 2 + 1)];
            movementTileArray = new TileBase[movementTilePositions.Length];

            foreach (var unit in unitGridCombatArray)
            {
                GameManager.Instance.GetGrid().GetGridObject(unit.GetPosition()).SetUnitGridObject(unit);

                if (unit.GetTeam() == UnitGridCombat.Team.Red) 
                { 
                    redTeamList.Add(unit);
                }
                else 
                { 
                    blueTeamList.Add(unit);
                }
            }

            SelectNextActive(UnitGridCombat.Team.Blue);
            ManageMovement();
        }
        private void SelectNextActive(UnitGridCombat.Team team)
        {
            // Check current unit team and take one from the opposite
            if (team == UnitGridCombat.Team.Red)
            {
                Debug.Log("Blue Turn");
                blueTeamIndex = (blueTeamIndex + 1) % blueTeamList.Count;

                if (blueTeamList[blueTeamIndex] == null || blueTeamList[blueTeamIndex].IsDead())
                {
                    SelectNextActive(team);
                }
                else
                {
                    activeUnit = blueTeamList[blueTeamIndex];
                }
            }
            else
            {
                Debug.Log("Red Turn");
                redTeamIndex = (redTeamIndex + 1) % redTeamList.Count;

                if (redTeamList[redTeamIndex] == null || redTeamList[redTeamIndex].IsDead())
                {
                    SelectNextActive(team);
                }
                else
                {
                    activeUnit = redTeamList[redTeamIndex]; 
                }
            }

            hasAttacked = false;
            hasMoved = false;
        }
        private void ManageMovement()
        {

            // Get Unit Grid Position X, Y
            grid.GetXY(activeUnit.GetPosition(), out int unitX, out int unitY);

            //Debug.Log("Unit Coords: " + unitX +  ", " + unitY);

            // Reset Entire Grid Traversables
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    grid.GetGridObject(x, y).SetTraversable(false);
                    tilemapManager.tilemaps[2].SetTile(new Vector3Int(x, y, 0), null);
                }
            }

            int i = 0;
            // Mark all eligible tiles in range as traversable
            for (int x = unitX - MAXMOVEMENTDISTANCE; x <= unitX + MAXMOVEMENTDISTANCE; x++)
            {
                //Debug.Log("iteration on X: " + i);
                //Debug.Log("X: " + x);


                for (int y = unitY - MAXMOVEMENTDISTANCE; y <= unitY + MAXMOVEMENTDISTANCE; y++)
                {
                    //Debug.Log("iteration on Y: " + i);
                    //Debug.Log("Y: " + y);

                    if (!gridPathfinding.IsInBoundaries(x, y)) { continue; }

                    // Cache the change in movement tiles position for further use
                    movementTilePositions[i] = new Vector3Int(x, y);
                    i++;

                    if (!gridPathfinding.IsWalkable(x, y)) { continue; }

                    var path = gridPathfinding.GetPathStructure(activeUnit.GetPosition(), grid.GetWorldPosition(x, y));
                    if (path.vectorPathList == null || path.vectorPathList.Count > MAXMOVEMENTDISTANCE)
                    { continue; }

                    grid.GetGridObject(x, y).SetTraversable(true);

                    // Set movement indicator tiles
                    tilemapManager.tilemaps[2].SetTile(new Vector3Int(x, y, 0), tilemapManager.movementTiles[0]);

                    movementTileArray[i - 1] = null;
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

                        if (gridObject == null) 
                        { 
                            Debug.Log("Out of Bounds"); 
                            return;
                        }
                        // If the Space has a unit
                        if (gridObject.GetUnitGridCombat() != null)
                        {

                            // If the unit is an enemy
                            if (!activeUnit.CheckForEnemy(gridObject.GetUnitGridCombat())) return;

                            // If the enemy is in range
                            if (!activeUnit.IsInAttackRange(gridObject.GetUnitGridCombat())) return;

                            // If active unit has attacked
                            if (hasAttacked) return;

                            state = State.Waiting;

                            activeUnit.AttackUnit(gridObject.GetUnitGridCombat(), () =>
                            {
                                state = State.Idle;
                                hasAttacked = true;
                                ManageMovement();
                                TestTurnOver();
                            });
                            break;
                        }
                        // If you can move to the tile
                        if (gridObject.GetTraversable())
                        {
                            // If the unit has already moved
                            if (hasMoved) return;

                            state = State.Waiting;

                            ResetMovementTiles();

                            // Remove Unit from current Grid Object
                            grid.GetGridObject(activeUnit.GetPosition()).ClearUnitCombatObject();
                            // Set Unit on target Grid Object
                            gridObject.SetUnitGridObject(activeUnit);

                            activeUnit.MoveTo(GameUtils.GetMouseWorldPosition(), () => {
                                state = State.Idle;
                                hasMoved = true;
                                TestTurnOver();
                            });
                        }
                    }
                    // End turn manually
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        TurnOver();
                    }
                    break;

                case State.Walking:
                    break;
            }
        }
        private void ResetMovementTiles()
        {
            tilemapManager.SetTilemapSprites(2, movementTilePositions, movementTileArray);

            for (int i = 0; i < movementTilePositions.Length; i++)
            {
                movementTilePositions[i] = default;
                movementTileArray[i] = null;
            }
        }

        private void TestTurnOver()
        {
            if(hasAttacked && hasMoved)
            {
                TurnOver();
            }
        }
        private void TurnOver()
        {
            SelectNextActive(activeUnit.GetTeam());
            ManageMovement();
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
