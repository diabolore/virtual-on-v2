using UnityEngine;
using System;

namespace VirtualOnRevived.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action OnMatchStart;
        public event Action<int> OnMatchEnd; // int represents winning player ID

        [SerializeField] private Transform player1Spawn;
        [SerializeField] private Transform player2Spawn;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject botPrefab;
        
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
            // Spawn mechs and initialize match
            isMatchActive = true;
            OnMatchStart?.Invoke();
        }

        public void EndMatch(int winnerId)
        {
            if (!isMatchActive) return;
            
            isMatchActive = false;
            OnMatchEnd?.Invoke(winnerId);
            
            // Example of match loop restart
            // Invoke(nameof(StartMatch), 5f);
        }
    }
}
