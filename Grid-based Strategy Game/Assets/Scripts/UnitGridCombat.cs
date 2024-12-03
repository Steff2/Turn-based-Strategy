using System;
using UnityEngine;
using Utils.HealthSystemCM;

namespace GridCombat
{
    public class UnitGridCombat : MonoBehaviour
    {
        [SerializeField] private Team team;
        [SerializeField] private bool isRangedAttacker;

        private const float maxAttackRangeMelee = 20;
        private const float maxAttackRangeShot = 50;

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
        
        private HealthSystem healthSystem;
        private Usable_Bar healthBar;
        private CharacterMovementHandler characterMovementHandler;
        private float activeAttackRange;

        private void Awake()
        {
            characterMovementHandler = gameObject.GetComponent<CharacterMovementHandler>();
            healthSystem = new HealthSystem(10);
            healthBar = new Usable_Bar(transform, new Vector3(0, 10), new Vector3(10, 1.3f), Color.grey, Color.red, 1f, 10000, new Usable_Bar.Outline { color = Color.black, size = .5f });
            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        }
        private void Start()
        {
            SetWeaponProperties();
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
        public void AttackUnit(UnitGridCombat unitGridCombat, Action onComplete)
        {
            if(isRangedAttacker)
            {
                Shoot(unitGridCombat, () =>
                {
                    //state = State.Idle;
                    onComplete();
                });
            }
            else
            {
                Strike(unitGridCombat, () =>
                {
                    //state = State.Idle;
                    onComplete();
                });
            }
        }

        private void Shoot(UnitGridCombat unitGridCombat, Action onShotComplete)
        {
            unitGridCombat.Damage(UnityEngine.Random.Range(2, 6));
            onShotComplete();
        }

        private void Strike(UnitGridCombat unitGridCombat, Action onStrikeComplete)
        {
            unitGridCombat.Damage(UnityEngine.Random.Range(4, 8));
            onStrikeComplete();
        }

        public bool IsInAttackRange(UnitGridCombat target)
        {
            if (Vector3.Distance(gameObject.transform.position, target.gameObject.transform.position) < activeAttackRange)
            {
                return true;
            }

            return false;
        }
        public void SwitchWeapon()
        {
            isRangedAttacker = !isRangedAttacker;
            SetWeaponProperties();
        }

        private void SetWeaponProperties()
        {
            if (isRangedAttacker)
            {
                activeAttackRange = maxAttackRangeShot;
            }
            else
            {
                activeAttackRange = maxAttackRangeMelee;
            }
        }

        public void Damage(float damageTaken)
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
