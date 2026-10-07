using UnityEngine;

namespace VirtualOnRevived.Environment
{
    public class OscillatingObstacle : MonoBehaviour
    {
        [Header("Oscillation Movement")]
        [SerializeField] private Vector3 movementOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private float frequency = 0.5f;
        [SerializeField] private float phaseShift = 0f;

        [Header("Drift Rotation")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 10f, 5f);

        private Vector3 _startPosition;
        private Rigidbody _rb;

        private void Awake()
        {
            _startPosition = transform.position;
            _rb = GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.isKinematic = true;
            }
        }

        private void Update()
        {
            float sine = Mathf.Sin((Time.time * frequency * Mathf.PI * 2f) + phaseShift);
            Vector3 targetPos = _startPosition + (movementOffset * sine);

            if (_rb != null)
            {
                _rb.MovePosition(targetPos);
                _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotationSpeed * Time.deltaTime));
            }
            else
            {
                transform.position = targetPos;
                transform.Rotate(rotationSpeed * Time.deltaTime);
            }
        }
    }
}
