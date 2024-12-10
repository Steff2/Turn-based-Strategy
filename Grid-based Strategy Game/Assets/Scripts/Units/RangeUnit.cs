using GridCombat;
using System;
using UnityEngine;

public class RangeUnit : UnitGridCombat
{
    private void Start()
    {
        attackDamage = UnityEngine.Random.Range(2, 4);
        attackRange = 50;
        movement = 4;
        healthSystem.SetHealth(10);
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
