using UnityEngine;

namespace VirtualOnRevived.Core
{
    [CreateAssetMenu(fileName = "New Mech Profile", menuName = "Virtual-On/Mech Profile")]
    public class MechProfile : ScriptableObject
    {
        public string mechName;
        public string mechId; // Unique identifier (e.g., "TEMJIN")
        [TextArea] public string description;
        
        [Header("Stats")]
        public float baseHealth;
        public float baseSpeed;
        public float mobilityRating;
        
        [Header("Assets")]
        public Sprite menuIcon; // 2D portrait for UI
        
        // Using GameObject for simplicity, can be updated to Addressables/AssetReference later
        public GameObject gameplayPrefab; 
        public GameObject previewPrefab; 
    }
}
