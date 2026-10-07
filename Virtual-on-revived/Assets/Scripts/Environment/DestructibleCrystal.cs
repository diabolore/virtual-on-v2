using UnityEngine;

namespace VirtualOnRevived.Environment
{
    public class DestructibleCrystal : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 80f;
        [SerializeField] private GameObject crystalMesh;
        [SerializeField] private Collider intactCollider;
        [SerializeField] private Collider stumpCollider;

        [Header("Explosion Mechanics")]
        [SerializeField] private float explosionRadius = 8f;
        [SerializeField] private float explosionDamage = 35f;
        [SerializeField] private float explosionForce = 15f;
        [SerializeField] private LayerMask damageableLayers = ~0;
        [SerializeField] private GameObject explosionVfxPrefab;

        private float _currentHealth;
        private bool _isDestroyed = false;

        public float CurrentHealth => _currentHealth;
        public bool IsDestroyed => _isDestroyed;

        private void Start()
        {
            _currentHealth = maxHealth;
            if (stumpCollider != null)
            {
                stumpCollider.enabled = false;
            }
        }

        public void TakeDamage(float amount)
        {
            if (_isDestroyed) return;

            _currentHealth -= amount;
            if (_currentHealth <= 0f)
            {
                ExplodeAndDestroy();
            }
        }

        public void ExplodeAndDestroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            // Trigger AoE damage
            TriggerAoEExplosion();

            // Disable main mesh and swap colliders
            if (crystalMesh != null) crystalMesh.SetActive(false);
            if (intactCollider != null) intactCollider.enabled = false;
            if (stumpCollider != null) stumpCollider.enabled = true;

            // Spawn VFX
            if (explosionVfxPrefab != null)
            {
                Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            }
        }

        private void TriggerAoEExplosion()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                // Damage mechs
                if (hit.TryGetComponent(out MechHealth health))
                {
                    health.TakeDamage(explosionDamage);
                }
                else if (hit.GetComponentInParent<MechHealth>() is MechHealth parentHealth)
                {
                    parentHealth.TakeDamage(explosionDamage);
                }

                // Add physics impulse
                if (hit.attachedRigidbody != null && !hit.attachedRigidbody.isKinematic)
                {
                    hit.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.8f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
