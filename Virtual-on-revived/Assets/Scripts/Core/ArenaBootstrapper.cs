using UnityEngine;

namespace VirtualOnRevived.Core
{
    public class ArenaBootstrapper : MonoBehaviour
    {
        [Header("Gravity Settings")]
        [SerializeField] private bool overrideGravity = false;
        [SerializeField] private Vector3 customGravity = new Vector3(0f, -4.905f, 0f);

        [Header("Arena Boundaries")]
        [SerializeField] private Vector3 arenaBoundsSize = new Vector3(120f, 30f, 120f);
        [SerializeField] private Vector3 arenaBoundsCenter = new Vector3(0f, 15f, 0f);
        [SerializeField] private bool drawBoundaryGizmos = true;

        private Vector3 _defaultGravity;
        private bool _gravityOverridden = false;

        public bool OverrideGravity => overrideGravity;
        public Vector3 CustomGravity => customGravity;

        private void Awake()
        {
            _defaultGravity = Physics.gravity;

            if (overrideGravity)
            {
                Physics.gravity = customGravity;
                _gravityOverridden = true;
            }

            ConfigurePhysicsMatrix();
        }

        private void OnDestroy()
        {
            if (_gravityOverridden)
            {
                Physics.gravity = _defaultGravity;
            }
        }

        private void ConfigurePhysicsMatrix()
        {
            // Dynamically set physics matrix ignoring Debris vs Debris to save CPU
            int debrisLayer = LayerMask.NameToLayer("Debris");
            if (debrisLayer != -1)
            {
                Physics.IgnoreLayerCollision(debrisLayer, debrisLayer, true);
            }
            else
            {
                Debug.LogWarning("Debris layer not found. Please add a 'Debris' layer in Project Settings -> Tags and Layers.");
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawBoundaryGizmos) return;

            Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
            Gizmos.DrawWireCube(transform.position + arenaBoundsCenter, arenaBoundsSize);
        }
    }
}
