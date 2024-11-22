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
            Shooting,
            Idle
        }
        //private HealthSystem healthSystem;
        private CharacterMovementHandler characterMovementHandler;

        private void Awake()
        {
            characterMovementHandler = gameObject.GetComponent<CharacterMovementHandler>();
        }
        public void MoveTo(Vector3 targetPos, Action onComplete)
        {
            state = State.Walking;
            characterMovementHandler.SetTargetPosition(targetPos + new Vector3(1, 1), () =>
            {
                state = State.Idle;
                onComplete();
            }); // Needs Action to set state back to normal after reaching destination)
        }

        public void TakeDamage(float damageTaken)
        {
            //healthSystem.Damage(damageTaken);
        }
    }
}
