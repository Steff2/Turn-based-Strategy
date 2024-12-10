using GridCombat;
using System;
using UnityEngine;

public class MeleeUnit : UnitGridCombat
{
    private void Start()
    {
        attackDamage = UnityEngine.Random.Range(3, 6);
        attackRange = 20;
        movement = 3;
        healthSystem.SetHealth(13);
    }
    public override void AttackUnit(UnitGridCombat unitGridCombat, Action onComplete)
    {
        Attack(unitGridCombat, () =>
        {
            //state = State.Idle;
            onComplete();
        });
    }
}
