using System;
using UnityEngine;
using Utils.HealthSystemCM;

namespace GridCombat
{
    public abstract class UnitGridCombat : MonoBehaviour
    {
        [SerializeField] private Team team;
        public int attackRange { get; protected set; }
        public int attackDamage { get; protected set; }
        public int movement { get; protected set; }

        //State state;
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
        
        protected HealthSystem healthSystem;
        private Usable_Bar healthBar;
        private CharacterMovementHandler characterMovementHandler;


        private void Awake()
        {
            characterMovementHandler = gameObject.GetComponent<CharacterMovementHandler>();
            healthSystem = new HealthSystem(10);
            healthBar = new Usable_Bar(transform, new Vector3(0, 10), new Vector3(10, 1.3f), Color.grey, Color.red, 1f, 10000, new Usable_Bar.Outline { color = Color.black, size = .5f });
            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        }
        private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
        {
            healthBar.SetSize(healthSystem.GetHealthNormalized());
        }

        public void MoveTo(Vector3 targetPos, Action onComplete)
        {
            //state = State.Walking;
            characterMovementHandler.SetTargetPosition(targetPos + new Vector3(1, 1), () =>
            {
                //state = State.Idle;
                onComplete();
            });
        }
        public abstract void AttackUnit(UnitGridCombat unitGridCombat, Action onShotComplete);

        public void Attack(UnitGridCombat unitGridCombat, Action onShotComplete)
        {
            unitGridCombat.Damage(UnityEngine.Random.Range(2, 6));
            onShotComplete();
        }
        public bool IsInAttackRange(UnitGridCombat target)
        {
            if (Vector3.Distance(gameObject.transform.position, target.gameObject.transform.position) < attackRange)
            {
                return true;
            }

            return false;
        }
        protected void Damage(float damageTaken)
        {
            healthSystem.Damage(damageTaken);
            if (IsDead())
            {
                Destroy(gameObject);
            }
        }
        public bool IsDead()
        {
            return healthSystem.IsDead();
        }
        public bool CheckForEnemy(UnitGridCombat testSubject)
        {
            return testSubject.GetTeam() != team;
        }
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
