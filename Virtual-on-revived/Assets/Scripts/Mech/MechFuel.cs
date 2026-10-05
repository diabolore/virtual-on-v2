using System;
using UnityEngine;

public class MechFuel : MonoBehaviour
{
    public float maxFuel = 100f;
    public float fuelDrainRate = 20f;
    public float rechargeDelay = 3f;
    public float fuelRechargeRate = 40f;

    public event Action<float> OnFuelChanged;

    private float _currentFuel;
    private bool _isUsingFuel;
    private float _lastUsedTime;

    private void Awake()
    {
        _currentFuel = maxFuel;
    }

    private void Update()
    {
        if (_isUsingFuel)
        {
            float oldFuel = _currentFuel;
            _currentFuel -= fuelDrainRate * Time.deltaTime;
            _currentFuel = Mathf.Clamp(_currentFuel, 0, maxFuel);
            
            if (_currentFuel != oldFuel)
                OnFuelChanged?.Invoke(_currentFuel / maxFuel);
                
            _lastUsedTime = Time.time;
        }
        else if (Time.time - _lastUsedTime >= rechargeDelay)
        {
            if (_currentFuel < maxFuel)
            {
                float oldFuel = _currentFuel;
                _currentFuel += fuelRechargeRate * Time.deltaTime;
                _currentFuel = Mathf.Clamp(_currentFuel, 0, maxFuel);
                
                if (_currentFuel != oldFuel)
                    OnFuelChanged?.Invoke(_currentFuel / maxFuel);
            }
        }
    }

    public void UseFuel()
    {
        _isUsingFuel = true;
    }

    public void StopUsingFuel()
    {
        _isUsingFuel = false;
    }
    
    public bool HasFuel()
    {
        return _currentFuel > 0;
    }
}
