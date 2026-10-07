using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualOnRevived.Environment
{
    public class HazardVent : MonoBehaviour
    {
        public enum VentState
        {
            Idle,
            Warning,
            Active
        }

        [Header("Cycle Timings")]
        [SerializeField] private float initialDelay = 0f;
        [SerializeField] private float idleDuration = 5f;
        [SerializeField] private float warningDuration = 2f;
        [SerializeField] private float activeDuration = 3f;

        [Header("Damage Settings")]
        [SerializeField] private float damagePerSecond = 40f;
        [SerializeField] private float tickInterval = 0.25f;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject warningVisual;
        [SerializeField] private GameObject activeHazardVisual;
        [SerializeField] private Renderer indicatorRenderer;
        [SerializeField] private Color idleColor = Color.green;
        [SerializeField] private Color warningColor = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color activeColor = Color.red;

        private VentState _currentState = VentState.Idle;
        private readonly List<MechHealth> _overlappingMechs = new List<MechHealth>();
        private Coroutine _cycleCoroutine;
        private Coroutine _damageCoroutine;

        public VentState CurrentState => _currentState;

        private void Start()
        {
            SetState(VentState.Idle);
            _cycleCoroutine = StartCoroutine(VentCycleRoutine());
        }

        private void OnDisable()
        {
            if (_cycleCoroutine != null) StopCoroutine(_cycleCoroutine);
            if (_damageCoroutine != null) StopCoroutine(_damageCoroutine);
            _overlappingMechs.Clear();
        }

        private IEnumerator VentCycleRoutine()
        {
            if (initialDelay > 0f)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            while (true)
            {
                // Idle Phase
                SetState(VentState.Idle);
                yield return new WaitForSeconds(idleDuration);

                // Warning / Telegraph Phase
                SetState(VentState.Warning);
                yield return new WaitForSeconds(warningDuration);

                // Active Hazard Phase
                SetState(VentState.Active);
                yield return new WaitForSeconds(activeDuration);
            }
        }

        private void SetState(VentState newState)
        {
            _currentState = newState;

            if (warningVisual != null)
                warningVisual.SetActive(newState == VentState.Warning);

            if (activeHazardVisual != null)
                activeHazardVisual.SetActive(newState == VentState.Active);

            if (indicatorRenderer != null)
            {
                Color targetColor = newState switch
                {
                    VentState.Idle => idleColor,
                    VentState.Warning => warningColor,
                    VentState.Active => activeColor,
                    _ => idleColor
                };
                indicatorRenderer.material.color = targetColor;
            }

            if (newState == VentState.Active)
            {
                if (_damageCoroutine == null)
                {
                    _damageCoroutine = StartCoroutine(DamageTickRoutine());
                }
            }
            else
            {
                if (_damageCoroutine != null)
                {
                    StopCoroutine(_damageCoroutine);
                    _damageCoroutine = null;
                }
            }
        }

        private IEnumerator DamageTickRoutine()
        {
            float damagePerTick = damagePerSecond * tickInterval;

            while (_currentState == VentState.Active)
            {
                for (int i = _overlappingMechs.Count - 1; i >= 0; i--)
                {
                    MechHealth mech = _overlappingMechs[i];
                    if (mech != null)
                    {
                        mech.TakeDamage(damagePerTick);
                    }
                    else
                    {
                        _overlappingMechs.RemoveAt(i);
                    }
                }

                yield return new WaitForSeconds(tickInterval);
            }

            _damageCoroutine = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MechHealth health) || (health = other.GetComponentInParent<MechHealth>()) != null)
            {
                if (!_overlappingMechs.Contains(health))
                {
                    _overlappingMechs.Add(health);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out MechHealth health) || (health = other.GetComponentInParent<MechHealth>()) != null)
            {
                _overlappingMechs.Remove(health);
            }
        }
    }
}
