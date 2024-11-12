using GridCombat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGridSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public class CombatGridObject
    {
        private GameGrid<CombatGridObject> grid;
        private int x;
        private int y;

        public CombatGridObject(GameGrid<CombatGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y; 
        }
    }
}
