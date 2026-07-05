using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    public class DialogueInteractable : MonoBehaviour, IPointerClickHandler
    {
        public DaddyDiamond diamond;
        public GameManager gameManager;

        [Serializable]
        public class DialogueLine
        {
            public SpeakerColor speakerColor = SpeakerColor.Player;
            public bool requestManualResponseAfterLine = false;

            [TextArea(2, 6)]
            public string text = string.Empty;
        }

        [Serializable]
        public class ResponseSubmittedEvent : UnityEvent<string>
        {
        }

        [Header("Dialogue Content")]
        [SerializeField] private DialogueLine[] lines = Array.Empty<DialogueLine>();
        [SerializeField] private bool allowUserResponse = true;
        [SerializeField] private SpeakerColor responseSpeakerColor = SpeakerColor.Player;
        [SerializeField] private string responsePrompt = "Type your reply:";
        [SerializeField] private string responsePlaceholder = "Type your answer and press Enter";

        [Header("Presentation")]
        [SerializeField] private Sprite dialogueBoxSprite;
        [SerializeField] private Sprite continueBoxSprite;
        [SerializeField] private string continueMessage = "Continue";
        [Min(0f)]
        [SerializeField] private float charactersPerSecond = 72f;

        [Header("Debug")]
        [SerializeField] private string lastSubmittedText = string.Empty;
        [SerializeField] public ResponseSubmittedEvent onUserResponseSubmitted = new ResponseSubmittedEvent();

        private int _lastActivationFrame = -1;

        public DialogueLine[] Lines => lines;
        public bool AllowUserResponse => allowUserResponse;
        public SpeakerColor ResponseSpeakerColor => responseSpeakerColor;
        public string ResponsePrompt => responsePrompt;
        public string ResponsePlaceholder => responsePlaceholder;
        public Sprite DialogueBoxSprite => dialogueBoxSprite;
        public Sprite ContinueBoxSprite => continueBoxSprite;
        public string ContinueMessage => continueMessage;
        public float CharactersPerSecond => charactersPerSecond;

        public bool ShouldRequestManualResponse(DialogueLine line)
        {
            return allowUserResponse &&
                   line != null &&
                   line.requestManualResponseAfterLine &&
                   line.speakerColor == SpeakerColor.Player;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TryOpenDialogue();  
        }

        private void OnMouseDown()
        {
            TryOpenDialogue();
        }

        public bool HasPlayableDialogue()
        {
            if (lines == null || lines.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                DialogueLine line = lines[i];

                if (line != null && !string.IsNullOrWhiteSpace(line.text))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanOpenDialogue()
        {
            return HasPlayableDialogue() || allowUserResponse;
        }

        internal void RegisterSubmittedResponse(string submittedText)
        {
            lastSubmittedText = submittedText ?? string.Empty;
        
            GameObject go = GameObject.Find("GameManager");
            GameManager gameManager = go.GetComponent<GameManager>();
        
            if (string.IsNullOrWhiteSpace(submittedText))
                return;

            if (gameManager.CluesRevealed == 0)
            {
                onUserResponseSubmitted?.Invoke("... *nothing* ...");
                return;
            }

            //TODO: get the reason and daddy from the static class
            var reply =  diamond.InvokeReply(
                gameManager.DaddyPersonality, 
                "Daddy, what's wrong with you? Is it because " + lastSubmittedText,
                gameManager.DaddyReason, 
                $"{gameManager.CluesRevealed}/3");
        
            gameManager.EndSceneTriggered = reply[DiamondConstants.OUTPUT_GAME_OVER] == "True" || 
                                            reply[DiamondConstants.OUTPUT_GAME_OVER] == "Yes" || 
                                            reply[DiamondConstants.OUTPUT_GAME_OVER] == "yes";
            onUserResponseSubmitted?.Invoke(reply[DiamondConstants.OUTPUT_REPLY]);
        }

        private void TryOpenDialogue()
        {
            if (_lastActivationFrame == Time.frameCount)
            {
                return;
            }

            _lastActivationFrame = Time.frameCount;

            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!CanOpenDialogue())
            {
                Debug.LogWarning($"DialogueInteractable on '{name}' does not contain any valid dialogue lines or response input.");
                return;
            }

            RuntimeDialogueUI.ShowDialogue(this);
        }

    }
}
