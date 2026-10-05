using UnityEngine;

public class MechMotor : MonoBehaviour
{
    public float groundSpeed = 15f;
    public float flySpeed = 20f;
    public float thrusterForce = 30f;
    
    private Rigidbody _rb;
    private bool _isFlying;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        if (_isFlying)
        {
            _rb.MovePosition(_rb.position + direction * flySpeed * Time.fixedDeltaTime);
        }
        else
        {
            _rb.MovePosition(_rb.position + direction * groundSpeed * Time.fixedDeltaTime);
        }
    }

    public void ApplyThrusters()
    {
        _isFlying = true;
        _rb.AddForce(Vector3.up * thrusterForce, ForceMode.Acceleration);
    }
    
    public void StopThrusters()
    {
        _isFlying = false;
    }
}
