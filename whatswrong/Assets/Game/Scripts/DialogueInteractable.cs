using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class DialogueInteractable : MonoBehaviour, IPointerClickHandler
{
    public enum SpeakerColor
    {
        Pink = 0,
        Blue = 1
    }

    [Serializable]
    public class DialogueLine
    {
        public SpeakerColor speakerColor = SpeakerColor.Pink;
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
    [SerializeField] private SpeakerColor responseSpeakerColor = SpeakerColor.Pink;
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
    [SerializeField] private ResponseSubmittedEvent onUserResponseSubmitted = new ResponseSubmittedEvent();

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
               line.speakerColor == SpeakerColor.Pink;
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
        onUserResponseSubmitted?.Invoke(lastSubmittedText);
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
