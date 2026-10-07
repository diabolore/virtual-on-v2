using UnityEngine;
using Combat;

namespace VirtualOnRevived.Environment
{
    [RequireComponent(typeof(Collider))]
    public class EnergyBarrier : MonoBehaviour
    {
        [Header("Barrier Visuals & Feedback")]
        [SerializeField] private GameObject impactVfxPrefab;
        [SerializeField] private Color barrierActiveColor = new Color(0f, 0.9f, 1f, 0.45f);
        [SerializeField] private MeshRenderer barrierRenderer;

        private MaterialPropertyBlock _propBlock;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            if (barrierRenderer != null)
            {
                barrierRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor(BaseColorId, barrierActiveColor);
                barrierRenderer.SetPropertyBlock(_propBlock);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollisionOrTrigger(collision.collider, collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollisionOrTrigger(other, other.ClosestPoint(transform.position));
        }

        private void HandleCollisionOrTrigger(Collider other, Vector3 hitPoint)
        {
            if (other.TryGetComponent(out Projectile projectile))
            {
                // Spawn deflection effect
                if (impactVfxPrefab != null)
                {
                    Instantiate(impactVfxPrefab, hitPoint, Quaternion.identity);
                }
            }
        }
    }
}
