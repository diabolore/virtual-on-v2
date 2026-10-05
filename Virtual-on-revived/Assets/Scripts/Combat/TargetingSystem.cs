using UnityEngine;

namespace Combat
{
    public class TargetingSystem : MonoBehaviour
    {
        [SerializeField] private float targetingRadius = 50f;
        [SerializeField] private LayerMask enemyLayer;
        
        private Transform currentTarget;

        private void Update()
        {
            FindTarget();
        }

        private void FindTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, targetingRadius, enemyLayer);
            
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;

            foreach (var col in colliders)
            {
                if (col.transform == transform.root) continue; // Ignore self

                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = col.transform;
                }
            }

            currentTarget = closestTarget;
        }

        public Transform GetTarget()
        {
            return currentTarget;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetingRadius);
        }
    }
}
