using UnityEngine;
using Utils;

namespace GridCombat
{
    public class CombatGridSystem : MonoBehaviour
    {
        public enum State
        {
            Walking,
            Idle
        }

        private State state;

        [SerializeField] UnitGridCombat unitGridCombat;
        // Start is called before the first frame update
        void Awake()
        {
            state = State.Idle;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Idle:

                    if (Input.GetMouseButtonDown(0))
                    {
                        state = State.Walking;
                        unitGridCombat.MoveTo(GameUtils.GetMouseWorldPosition());
                    }
                    break;

                case State.Walking:
                    break;
            }
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
}
