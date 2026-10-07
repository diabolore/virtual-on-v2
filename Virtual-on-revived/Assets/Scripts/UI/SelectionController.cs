using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VirtualOnRevived.Core;

namespace VirtualOnRevived.UI
{
    public class SelectionController : MonoBehaviour
    {
        [Header("Data References")]
        [SerializeField] private MechDatabase database;
        [SerializeField] private SessionData sessionData;
        [SerializeField] private string arenaSceneName = "SampleScene"; // Configurable scene name
        
        [Header("Roster UI")]
        [SerializeField] private Transform rosterContainer;
        [SerializeField] private GameObject mechButtonPrefab;

        [Header("Details Panel UI")]
        [SerializeField] private Text mechNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private Slider mobilitySlider;
        [SerializeField] private Button confirmButton;

        private MechProfile currentlySelectedMech;

        private void Start()
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = false;
                confirmButton.onClick.AddListener(ConfirmSelection);
            }

            PopulateRoster();
            ClearDetailsPanel();
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(ConfirmSelection);
            }
        }

        private void PopulateRoster()
        {
            if (database == null || rosterContainer == null || mechButtonPrefab == null)
            {
                Debug.LogError("SelectionController is missing necessary references to populate the roster.");
                return;
            }

            foreach (var mech in database.GetAllMechs())
            {
                GameObject btnObj = Instantiate(mechButtonPrefab, rosterContainer);
                if (btnObj.TryGetComponent(out MechSelectionButton btn))
                {
                    // Pass the callback to handle selection events
                    btn.Initialize(mech, OnMechSelected);
                }
            }
        }

        private void ClearDetailsPanel()
        {
            if (mechNameText != null) mechNameText.text = "Select a Mech";
            if (descriptionText != null) descriptionText.text = "";
            if (healthSlider != null) healthSlider.value = 0;
            if (speedSlider != null) speedSlider.value = 0;
            if (mobilitySlider != null) mobilitySlider.value = 0;
        }

        private void OnMechSelected(MechProfile profile)
        {
            currentlySelectedMech = profile;
            
            // Note: In a larger project, consider a dedicated View class to update these elements
            // (MVP/MVC pattern) instead of doing it directly in the controller.
            if (mechNameText != null) mechNameText.text = profile.mechName;
            if (descriptionText != null) descriptionText.text = profile.description;
            
            // Assuming sliders are set from 0 to some max value in the inspector
            if (healthSlider != null) healthSlider.value = profile.baseHealth;
            if (speedSlider != null) speedSlider.value = profile.baseSpeed;
            if (mobilitySlider != null) mobilitySlider.value = profile.mobilityRating;
            
            if (confirmButton != null) confirmButton.interactable = true;
            
            // Future feature: Trigger event to update the 3D RenderTexture preview here
        }

        private void ConfirmSelection()
        {
            if (currentlySelectedMech == null || sessionData == null) return;

            // Save choice to the persistent ScriptableObject
            sessionData.selectedPlayerMechId = currentlySelectedMech.mechId;
            
            // For now, auto-assign the same mech to the enemy (or random)
            // This is where a 2nd player/CPU state machine would take over
            sessionData.selectedEnemyMechId = currentlySelectedMech.mechId; 
            
            SceneManager.LoadScene(arenaSceneName);
        }
    }
}
