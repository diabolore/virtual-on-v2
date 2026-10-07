using UnityEngine;

public class DetachableArmor : MonoBehaviour
{
    [Tooltip("The health ratio (0.0 to 1.0) at which this armor piece detaches.")]
    [Range(0f, 1f)]
    public float detachThreshold = 0.5f;

    public void Detach()
    {
        transform.SetParent(null);
        
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.isKinematic = false;
        rb.AddForce(Random.onUnitSphere * 10f, ForceMode.Impulse);
        rb.AddTorque(Random.onUnitSphere * 10f, ForceMode.Impulse);

        // Optionally move to a 'Debris' layer so it doesn't collide with the mech
        int debrisLayer = LayerMask.NameToLayer("Debris");
        if (debrisLayer != -1)
        {
            gameObject.layer = debrisLayer;
        }

        // Clean up debris after 10 seconds to save performance
        Destroy(gameObject, 10f);
    }
}
