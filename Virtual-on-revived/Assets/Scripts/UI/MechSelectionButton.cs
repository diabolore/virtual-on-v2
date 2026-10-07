using UnityEngine;
using UnityEngine.UI;
using System;
using VirtualOnRevived.Core;

namespace VirtualOnRevived.UI
{
    /// <summary>
    /// UI Component attached to the prefab representing a single mech in the selection list.
    /// Uses an event-driven approach to communicate back to the SelectionController.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class MechSelectionButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text nameText;
        
        private Button button;
        private MechProfile profile;
        private Action<MechProfile> onSelected;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void Initialize(MechProfile mechProfile, Action<MechProfile> selectedCallback)
        {
            profile = mechProfile;
            onSelected = selectedCallback;
            
            if (iconImage != null && profile.menuIcon != null)
            {
                iconImage.sprite = profile.menuIcon;
            }
            
            if (nameText != null)
            {
                nameText.text = profile.mechName;
            }
                
            button.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            onSelected?.Invoke(profile);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }
    }
}
