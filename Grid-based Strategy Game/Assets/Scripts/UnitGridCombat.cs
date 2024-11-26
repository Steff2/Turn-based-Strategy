using System;
using UnityEngine;
using Utils.HealthSystemCM;

namespace GridCombat
{
    public class UnitGridCombat : MonoBehaviour
    {
        [SerializeField] private Team team;

        State state;
        public enum Team
        {
            Red,
            Blue
        }
        public enum State
        {
            Walking,
            Attacking,
            Idle
        }
        
        //private HealthSystem healthSystem;
        private CharacterMovementHandler characterMovementHandler;

        private void Awake()
        {
            characterMovementHandler = gameObject.GetComponent<CharacterMovementHandler>();
            //healthSystem = new HealthSystem(100);
            //healthBar = new World_Bar(transform, new Vector3(0, 10), new Vector3(10, 1.3f), Color.grey, Color.red, 1f, 10000, new World_Bar.Outline { color = Color.black, size = .5f });
            //healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        }
        public void MoveTo(Vector3 targetPos, Action onComplete)
        {
            state = State.Walking;
            characterMovementHandler.SetTargetPosition(targetPos + new Vector3(1, 1), () =>
            {
                state = State.Idle;
                onComplete();
            });
        }
        public void AttackUnit(UnitGridCombat unitGridCombat, Action onComplete)
        {
            state = State.Attacking;
            Strike(unitGridCombat, () =>
            {
                state = State.Idle;
                onComplete();
            });
        }
        private void Strike(UnitGridCombat unitGridCombat, Action onStrikeComplete)
        {
            TakeDamage(UnityEngine.Random.Range(2, 6));
            onStrikeComplete();
        }
        public void TakeDamage(float damageTaken)
        {
            //healthSystem.Damage(damageTaken);
        }
        /*public bool IsDead()
        {
            return healthSystem.IsDead();
        }*/
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        public Team GetTeam()
        {
            return team;
        }
    }
}
