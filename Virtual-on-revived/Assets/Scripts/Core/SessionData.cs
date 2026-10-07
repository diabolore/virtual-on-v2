using UnityEngine;

namespace VirtualOnRevived.Core
{
    [CreateAssetMenu(fileName = "Session Data", menuName = "Virtual-On/Session Data")]
    public class SessionData : ScriptableObject
    {
        [Header("Match Settings")]
        public string selectedPlayerMechId;
        public string selectedEnemyMechId;

        [Header("Match Results")]
        public int winnerId; // 1 for Player 1, 2 for Player 2/Enemy
        
        public void ResetSession()
        {
            selectedPlayerMechId = string.Empty;
            selectedEnemyMechId = string.Empty;
            winnerId = 0;
        }
    }
}
