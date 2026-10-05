using UnityEngine;
using UnityEngine.UI;

namespace VirtualOnRevived.UI
{
    public class MechHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider fuelSlider;
        [SerializeField] private Text ammoText;

        // In a full implementation, these methods would be subscribed to events
        // exposed by MechHealth, MechFuel, and WeaponController in OnEnable()
        // Example: mechHealth.OnHealthChanged += UpdateHealth;

        public void UpdateHealth(float currentHealthPercentage)
        {
            if (healthSlider != null)
                healthSlider.value = currentHealthPercentage;
        }

        public void UpdateFuel(float currentFuelPercentage)
        {
            if (fuelSlider != null)
                fuelSlider.value = currentFuelPercentage;
        }

        public void UpdateAmmo(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}
