using UnityEngine;
using UnityEngine.Pool;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 100f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float damage = 10f;
        
        private float currentLifetime;
        private IObjectPool<Projectile> pool;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void SetPool(IObjectPool<Projectile> pool)
        {
            this.pool = pool;
        }

        private void OnEnable()
        {
            currentLifetime = lifetime;
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }

        private void Update()
        {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Note: Apply damage to target here
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Release(this);
            }
            else
            {
                gameObject.SetActive(false); // Fallback
            }
        }
    }
}
