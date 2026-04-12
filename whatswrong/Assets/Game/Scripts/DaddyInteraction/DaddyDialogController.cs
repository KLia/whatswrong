using UnityEngine;

namespace Game.Scripts.DaddyInteraction
{
    public class DaddyDialogController : MonoBehaviour
    {
        [SerializeField] private RoomInput roomInput;
        [SerializeField] private DialogUI dialogUI;

        private void OnEnable()
        {
            if (roomInput == null)
                return;
            
            roomInput.DaddyConversationRequested += HandleDaddyConversationRequested;
        }

        private void HandleDaddyConversationRequested()
        {
            dialogUI.ShowTextInput();
        }
        
        private void OnDisable()
        {
            if (roomInput == null)
                return;

            roomInput.DaddyConversationRequested -= HandleDaddyConversationRequested;
        }
    }
}