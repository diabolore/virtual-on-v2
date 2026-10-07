using System;
using UnityEngine;
using UnityEngine.Events;

public class MechHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    
    // For code subscribers
    public event Action<float> OnHealthChanged;
    
    // For inspector subscribers (like ModularArmorDetacher)
    public UnityEvent<float> OnDamageTakenRatio;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);

        float ratio = _currentHealth / maxHealth;
        
        OnHealthChanged?.Invoke(ratio);
        OnDamageTakenRatio?.Invoke(ratio);
    }
}
