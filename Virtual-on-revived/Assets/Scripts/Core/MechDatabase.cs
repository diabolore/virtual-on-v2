using UnityEngine;
using System.Collections.Generic;

namespace VirtualOnRevived.Core
{
    [CreateAssetMenu(fileName = "Mech Database", menuName = "Virtual-On/Mech Database")]
    public class MechDatabase : ScriptableObject
    {
        [SerializeField] private List<MechProfile> allMechs = new List<MechProfile>();

        public MechProfile GetMechById(string mechId)
        {
            foreach (var mech in allMechs)
            {
                if (mech != null && mech.mechId == mechId)
                {
                    return mech;
                }
            }
            Debug.LogWarning($"Mech with ID {mechId} not found in Database.");
            return null;
        }

        public IReadOnlyList<MechProfile> GetAllMechs()
        {
            return allMechs;
        }
    }
}
