using UnityEngine;

public class ModularArmorDetacher : MonoBehaviour
{
    [System.Serializable]
    public struct ArmorPiece
    {
        public GameObject armorObject;
        public float detachThreshold;
    }

    public ArmorPiece[] armorPieces;

    public void OnHealthThresholdReached(float threshold)
    {
        foreach (var piece in armorPieces)
        {
            if (piece.armorObject != null && Mathf.Approximately(piece.detachThreshold, threshold))
            {
                DetachArmor(piece.armorObject);
            }
        }
    }

    private void DetachArmor(GameObject armor)
    {
        armor.transform.SetParent(null);
        var rb = armor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Random.onUnitSphere * 10f, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * 10f, ForceMode.Impulse);
        }
    }
}
