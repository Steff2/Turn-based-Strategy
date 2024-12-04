using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.HealthSystemCM
{
    public class HealthSystem
    {

        public event EventHandler OnHealthChanged;
        public event EventHandler OnMaxHealthChanged;
        public event EventHandler OnDamaged;
        public event EventHandler OnHealed;
        public event EventHandler OnDeath;

        private float maxHealth;
        private float health;

        public HealthSystem(float maxHealth)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public float GetHealthNormalized()
        {
            return health / maxHealth;
        }

        public void Damage(float damage)
        {
            if (damage > 0)
            {
                health -= damage;

                if (health < 0) health = 0;
            }

            OnHealthChanged.Invoke(this, EventArgs.Empty);
            //OnDamaged.Invoke(this, EventArgs.Empty);

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            //OnDeath.Invoke(this, EventArgs.Empty);
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public void SetHealth(float health)
        {
            if (health > maxHealth)
            {
                this.maxHealth = health;
            }

            if (health < 0)
            {
                health = 0;
            }

            this.health = health;
            OnHealthChanged?.Invoke(this, EventArgs.Empty);

            if (health <= 0)
            {
                Die();
            }
        }

        public static bool TryGetHealthSystem(GameObject getHealthSystemGameObject, out HealthSystem healthSystem)
        {
            healthSystem = null;
            if(getHealthSystemGameObject != null){
                if(getHealthSystemGameObject.TryGetComponent(out IGetHealthSystem getHealthSystem))
                {
                    healthSystem = getHealthSystem.GetHealthSystem();
                }
                if(healthSystem != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
