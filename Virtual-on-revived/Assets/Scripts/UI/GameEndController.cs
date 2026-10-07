using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VirtualOnRevived.Core;

namespace VirtualOnRevived.UI
{
    public class GameEndController : MonoBehaviour
    {
        [Header("Data References")]
        [SerializeField] private SessionData sessionData;
        [SerializeField] private MechDatabase mechDatabase;

        [Header("Scene Navigation")]
        [SerializeField] private string arenaSceneName = "SampleScene";
        [SerializeField] private string mainMenuSceneName = "MainMenuScene";

        [Header("UI Elements")]
        [SerializeField] private Text winnerNameText;
        [SerializeField] private Button rematchButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            DisplayWinner();
            
            if (rematchButton != null) rematchButton.onClick.AddListener(OnRematch);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);
        }

        private void OnDestroy()
        {
            if (rematchButton != null) rematchButton.onClick.RemoveListener(OnRematch);
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenu);
        }

        private void DisplayWinner()
        {
            if (sessionData == null || mechDatabase == null) 
            {
                Debug.LogWarning("GameEndController missing SessionData or MechDatabase.");
                return;
            }

            string winningMechId = sessionData.winnerId == 1 ? sessionData.selectedPlayerMechId : sessionData.selectedEnemyMechId;
            
            if (!string.IsNullOrEmpty(winningMechId))
            {
                MechProfile winnerProfile = mechDatabase.GetMechById(winningMechId);
                if (winnerProfile != null && winnerNameText != null)
                {
                    winnerNameText.text = $"{winnerProfile.mechName} WINS!";
                    
                    // Future feature: Spawn winnerProfile.previewPrefab here to celebrate
                }
            }
            else
            {
                if (winnerNameText != null) winnerNameText.text = "DRAW";
            }
        }

        private void OnRematch()
        {
            // The SessionData still holds the previously selected mechs, 
            // so we can just reload the arena directly.
            SceneManager.LoadScene(arenaSceneName); 
        }

        private void OnMainMenu()
        {
            // Clear the session before going back to the menu
            if (sessionData != null)
            {
                sessionData.ResetSession();
            }
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
