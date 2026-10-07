using UnityEngine;
using System;

namespace VirtualOnRevived.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action OnMatchStart;
        public event Action<int> OnMatchEnd; // int represents winning player ID

        [Header("Data Architecture")]
        [SerializeField] private SessionData sessionData;
        [SerializeField] private MechDatabase mechDatabase;

        [Header("Spawn Points")]
        [SerializeField] private Transform player1Spawn;
        [SerializeField] private Transform player2Spawn;
        
        private bool isMatchActive = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            StartMatch();
        }

        public void StartMatch()
        {
            SpawnMechs();
            isMatchActive = true;
            OnMatchStart?.Invoke();
        }

        private void SpawnMechs()
        {
            if (sessionData == null || mechDatabase == null)
            {
                Debug.LogWarning("SessionData or MechDatabase is missing. Mechs cannot be spawned.");
                return;
            }

            // Spawn Player 1
            if (!string.IsNullOrEmpty(sessionData.selectedPlayerMechId))
            {
                MechProfile p1Profile = mechDatabase.GetMechById(sessionData.selectedPlayerMechId);
                if (p1Profile != null && p1Profile.gameplayPrefab != null)
                {
                    Instantiate(p1Profile.gameplayPrefab, player1Spawn.position, player1Spawn.rotation);
                }
            }

            // Spawn Player 2 / Bot
            if (!string.IsNullOrEmpty(sessionData.selectedEnemyMechId))
            {
                MechProfile p2Profile = mechDatabase.GetMechById(sessionData.selectedEnemyMechId);
                if (p2Profile != null && p2Profile.gameplayPrefab != null)
                {
                    Instantiate(p2Profile.gameplayPrefab, player2Spawn.position, player2Spawn.rotation);
                }
            }
        }

        public void EndMatch(int winnerId)
        {
            if (!isMatchActive) return;
            
            isMatchActive = false;
            
            if (sessionData != null)
            {
                sessionData.winnerId = winnerId;
            }

            OnMatchEnd?.Invoke(winnerId);
        }
    }
}
