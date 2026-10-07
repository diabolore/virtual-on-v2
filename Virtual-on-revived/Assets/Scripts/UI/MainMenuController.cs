using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace VirtualOnRevived.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        [Header("Scene Navigation")]
        [SerializeField] private string selectionSceneName = "SelectionScene";

        private void Start()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            // Best Practice: Always unregister events to prevent memory leaks
            if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
            if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(selectionSceneName);
        }

        private void OnQuitClicked()
        {
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
