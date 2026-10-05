using UnityEngine;

namespace Camera
{
    public class MechCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Combat.TargetingSystem targetingSystem;
        
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private Vector3 offset = new Vector3(0, 3, -5);
        [SerializeField] private float rotationSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

            Transform enemyTarget = targetingSystem != null ? targetingSystem.GetTarget() : null;
            if (enemyTarget != null)
            {
                // Lock-on tracking
                Vector3 direction = (enemyTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Free look / follow target forward
                Quaternion targetRotation = Quaternion.LookRotation(target.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
