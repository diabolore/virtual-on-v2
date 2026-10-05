using UnityEngine;

namespace Combat
{
    public class LaserSword : MonoBehaviour
    {
        [SerializeField] private float damage = 25f;
        [SerializeField] private float lungeRange = 15f;
        [SerializeField] private float lungeSpeed = 50f;
        [SerializeField] private TargetingSystem targetingSystem;
        private Rigidbody rb;

        private bool isSwinging = false;
        private Transform currentTarget;

        private void Awake()
        {
            rb = GetComponentInParent<Rigidbody>();
        }

        public void Swing()
        {
            if (isSwinging) return;
            isSwinging = true;

            if (targetingSystem != null)
            {
                currentTarget = targetingSystem.GetTarget();
            }

            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);
                if (distance <= lungeRange)
                {
                    // Lunge towards target
                    Vector3 direction = (currentTarget.position - transform.position).normalized;
                    if (rb != null)
                    {
                        rb.AddForce(direction * lungeSpeed, ForceMode.VelocityChange);
                    }
                }
            }
            
            // Note: Visuals/Animation logic would trigger here
            Invoke(nameof(EndSwing), 0.5f);
        }

        private void EndSwing()
        {
            isSwinging = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isSwinging)
            {
                // Note: Handle damage application here
                // e.g. if (other.TryGetComponent(out MechHealth health)) { health.TakeDamage(damage); }
            }
        }
    }
}
