using DefaultNamespace;
using UnityEngine;

namespace Game.Scripts.DaddyInteraction
{
    public class DaddyDialogController : MonoBehaviour
    {
        [SerializeField] private RoomInput roomInput;
        [SerializeField] private DialogUI dialogUI;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private DaddyDiamond diamond;

        private bool _isUIOpen;

        private void OnEnable()
        {
            if (roomInput == null)
                return;

            roomInput.DaddyConversationRequested += HandleDaddyConversationRequested;
            dialogUI.ResponseSubmitted += HandleResponseSubmitted;
            dialogUI.ContinueButtonClicked += HandleContinueClicked;
        }

        private void OnDisable()
        {
            if (roomInput == null)
                return;

            roomInput.DaddyConversationRequested -= HandleDaddyConversationRequested;
            dialogUI.ResponseSubmitted -= HandleResponseSubmitted;
            dialogUI.ContinueButtonClicked -= HandleContinueClicked;
        }

        private void HandleDaddyConversationRequested()
        {
            if (_isUIOpen)
                return;

            _isUIOpen = true;
            roomInput.SetInteractionEnabled(false);
            dialogUI.ShowTextInput();
        }

        private void HandleResponseSubmitted(string submittedText)
        {
            if (!_isUIOpen)
                return;

            if (string.IsNullOrWhiteSpace(submittedText))
                return;

            RespondToDaddy(submittedText);
        }

        private async void RespondToDaddy(string submittedText)
        {
            if (gameManager.CluesRevealed == 0)
            {
                dialogUI.ShowLine("... *nothing* ...", SpeakerColorMapping.GetColor(SpeakerColor.Daddy));
                return;
            }

            string guess = "Daddy, what's wrong with you? Is it because " + submittedText;

            var reply = await diamond.InvokeReplyAsync(
                gameManager.DaddyPersonality,
                guess,
                gameManager.DaddyReason,
                $"{gameManager.CluesRevealed}/3");

            if (!_isUIOpen)
                return; // player already closed the conversation while the backend was thinking

            gameManager.EndSceneTriggered =
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "True" ||
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "Yes" ||
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "yes";

            dialogUI.ShowLine(reply[DiamondConstants.OUTPUT_REPLY], SpeakerColorMapping.GetColor(SpeakerColor.Daddy));
        }

        private void HandleContinueClicked()
        {
            if (!_isUIOpen)
                return;

            _isUIOpen = false;
            dialogUI.Hide();
            roomInput.SetInteractionEnabled(true);
        }
    }
}
