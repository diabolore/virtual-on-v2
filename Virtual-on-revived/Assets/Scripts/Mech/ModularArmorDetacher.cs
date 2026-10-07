using UnityEngine;
using System.Collections.Generic;

public class ModularArmorDetacher : MonoBehaviour
{
    private List<DetachableArmor> armorPieces = new List<DetachableArmor>();

    private void Awake()
    {
        // Automatically discover all cosmetic armor pieces on the mech at startup
        armorPieces.AddRange(GetComponentsInChildren<DetachableArmor>(true));
    }

    public void OnHealthChanged(float currentHealthRatio)
    {
        // Iterate backwards since we are removing elements from the list as they detach
        for (int i = armorPieces.Count - 1; i >= 0; i--)
        {
            var piece = armorPieces[i];
            if (piece != null && currentHealthRatio <= piece.detachThreshold)
            {
                piece.Detach();
                armorPieces.RemoveAt(i);
            }
        }
    }
}
