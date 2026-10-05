using System;
using UnityEngine;
using UnityEngine.Events;

public class MechHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public event Action<float> OnHealthChanged;
    public UnityEvent<float> OnHealthThresholdReached;

    private float _currentHealth;
    
    private bool _passed75 = false;
    private bool _passed50 = false;
    private bool _passed25 = false;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);

        OnHealthChanged?.Invoke(_currentHealth / maxHealth);
        CheckThresholds();
    }
    
    private void CheckThresholds()
    {
        float ratio = _currentHealth / maxHealth;
        
        if (ratio <= 0.75f && !_passed75)
        {
            _passed75 = true;
            OnHealthThresholdReached?.Invoke(0.75f);
        }
        if (ratio <= 0.50f && !_passed50)
        {
            _passed50 = true;
            OnHealthThresholdReached?.Invoke(0.50f);
        }
        if (ratio <= 0.25f && !_passed25)
        {
            _passed25 = true;
            OnHealthThresholdReached?.Invoke(0.25f);
        }
    }
}
