using TMPro;
using UnityEngine;

namespace Utils.HealthSystemCM
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject getHealthSystemGameObject;

        [SerializeField] private TextMeshProUGUI textMesh;


        private HealthSystem healthSystem;

        private void Start()
        {
            if(HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out healthSystem))
            {
                SetHealthSystem(healthSystem);
            }
        }
        private void SetHealthSystem(HealthSystem healthSystem)
        {
            if(this.healthSystem != null)
            {
                this.healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            }
            this.healthSystem = healthSystem;

            UpdateHealthBar();

            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        }
        private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
        {
            UpdateHealthBar();
        }
        private void UpdateHealthBar()
        {
            textMesh.text = healthSystem.GetHealth() + "/" + healthSystem.GetMaxHealth();
        }

    }


}
