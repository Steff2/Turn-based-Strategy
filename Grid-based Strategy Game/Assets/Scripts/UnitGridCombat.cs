using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.HealthSystemCM;

public class UnitGridCombat : MonoBehaviour
{
    public Team team;
    public HealthSystem healthSystem;

    public enum Team
    {
        Red,
        Blue
    }

    public void MoveTo(int x, int y)
    {

    }

    public void TakeDamage(float damageTaken)
    {
        healthSystem.Damage(damageTaken);
    }
}
