using UnityEngine;

namespace VirtualOnRevived.Environment
{
    public class DestructibleBuilding : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 500f;
        [SerializeField] private GameObject mainMesh;
        [SerializeField] private Collider foundationCollider;
        [SerializeField] private GameObject debrisPrefab; // Pooled or instantiated debris
        
        private float currentHealth;
        private bool isDestroyed = false;

        private void Start()
        {
            currentHealth = maxHealth;
            if (foundationCollider != null)
                foundationCollider.enabled = false;
        }

        public void TakeDamage(float amount)
        {
            if (isDestroyed) return;

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                DestroyBuilding();
            }
        }

        private void DestroyBuilding()
        {
            isDestroyed = true;
            
            if (mainMesh != null) mainMesh.SetActive(false);
            if (foundationCollider != null) foundationCollider.enabled = true;

            if (debrisPrefab != null)
            {
                // Standard practice: spawn debris on a designated "Debris" layer to avoid self-collisions
                GameObject debris = Instantiate(debrisPrefab, transform.position, transform.rotation);
                // The ArenaBootstrapper or Unity Physics settings will handle ignoring Debris vs Debris collisions
            }
        }
    }
}
