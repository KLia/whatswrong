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

        private bool _userIsWriting;

        private void OnEnable()
        {
            if (roomInput == null)
                return;

            roomInput.DaddyConversationRequested += HandleDaddyConversationRequested;
            dialogUI.ResponseSubmitted += HandleResponseSubmitted;
        }

        private void OnDisable()
        {
            if (roomInput == null)
                return;

            roomInput.DaddyConversationRequested -= HandleDaddyConversationRequested;
            dialogUI.ResponseSubmitted -= HandleResponseSubmitted;
        }

        private void HandleDaddyConversationRequested()
        {
            if (_userIsWriting)
                return;

            _userIsWriting = true;
            roomInput.SetInteractionEnabled(false);
            dialogUI.ShowTextInput();
        }

        private void HandleResponseSubmitted(string submittedText)
        {
            if (!_userIsWriting)
                return;

            if (string.IsNullOrWhiteSpace(submittedText))
                return;

            _userIsWriting = false;
            RespondToDaddy(submittedText);
        }

        private async void RespondToDaddy(string submittedText)
        {
            if (gameManager.CluesRevealed == 0)
            {
                ShowReplyAndWaitForContinue("... *nothing* ...");
                return;
            }

            string guess = "Daddy, what's wrong with you? Is it because " + submittedText;

            var reply = await diamond.InvokeReplyAsync(
                gameManager.DaddyPersonality,
                guess,
                gameManager.DaddyReason,
                $"{gameManager.CluesRevealed}/3");

            gameManager.EndSceneTriggered =
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "True" ||
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "Yes" ||
                reply[DiamondConstants.OUTPUT_GAME_OVER] == "yes";

            ShowReplyAndWaitForContinue(reply[DiamondConstants.OUTPUT_REPLY]);
        }

        private void ShowReplyAndWaitForContinue(string replyText)
        {
            dialogUI.ShowLine(replyText, SpeakerColorMapping.GetColor(SpeakerColor.Daddy));
            dialogUI.ContinueButtonClicked += HandleReplyAcknowledged;
        }

        private void HandleReplyAcknowledged()
        {
            dialogUI.ContinueButtonClicked -= HandleReplyAcknowledged;
            dialogUI.Hide();
            roomInput.SetInteractionEnabled(true);
        }
    }
}
