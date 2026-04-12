using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class RuntimeDialogueUI : MonoBehaviour
{
    private enum DialoguePhase
    {
        Hidden,
        Typing,
        WaitingForAdvance,
        WaitingForResponse
    }

    private const int UiLayer = 5;
    private const float PanelWidth = 1350f;
    private const float PanelHeight = 900f;
    private const float PanelVerticalOffset = -130f;
    private const string DefaultContinueLabel = "Continue";
    private const string DefaultResponsePrompt = "Type your reply:";
    private const string DefaultResponsePlaceholder = "Type your answer and press Enter";
    private static readonly Color PinkSpeakerColor = new Color(1f, 0.47f, 0.78f);
    private static readonly Color BlueSpeakerColor = new Color(0.45f, 0.77f, 1f);
    private static readonly Color ContinueLabelColor = new Color(0.55f, 0.09f, 0.34f);

    private static RuntimeDialogueUI _instance;

    private readonly List<DialogueInteractable.DialogueLine> _activeLines = new List<DialogueInteractable.DialogueLine>();

    private Canvas _canvas;
    private RectTransform _rootRect;
    private RectTransform _panelRect;
    private RectTransform _textAreaRect;
    private RectTransform _continueBoxRect;
    private GameObject _responseRoot;
    private Image _panelImage;
    private Image _continueBoxImage;
    private Button _continueButton;
    private Text _dialogueText;
    private Text _continueBoxText;
    private Text _responsePromptText;
    private Text _responseInputText;
    private Text _responsePlaceholderText;
    private InputField _responseInputField;
    private Font _font;
    private DialogueInteractable _currentSource;
    private Coroutine _typingCoroutine;
    private DialoguePhase _phase = DialoguePhase.Hidden;
    private int _currentLineIndex = -1;
    private int _ignoreAdvanceFrame = -1;

    public static bool IsBlockingInput => _instance != null && _instance._phase != DialoguePhase.Hidden;

    public static void ShowDialogue(DialogueInteractable source)
    {
        if (source == null)
        {
            return;
        }

        EnsureInstance().Open(source);
    }

    private static RuntimeDialogueUI EnsureInstance()
    {
        if (_instance != null)
        {
            return _instance;
        }

        _instance = FindFirstObjectByType<RuntimeDialogueUI>();

        if (_instance != null)
        {
            _instance.InitializeIfNeeded();
            return _instance;
        }

        GameObject runtimeUi = new GameObject("RuntimeDialogueUI");
        runtimeUi.layer = UiLayer;
        _instance = runtimeUi.AddComponent<RuntimeDialogueUI>();
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        gameObject.layer = UiLayer;
        DontDestroyOnLoad(gameObject);
        InitializeIfNeeded();
    }

    private void Update()
    {
        if (_phase == DialoguePhase.Hidden)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            HideDialogue();
            return;
        }

        if (Time.frameCount == _ignoreAdvanceFrame)
        {
            return;
        }

        if (_phase == DialoguePhase.WaitingForResponse)
        {
            if (_responseInputField != null && !_responseInputField.isFocused)
            {
                _responseInputField.ActivateInputField();
            }

            if (keyboard != null &&
                (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
            {
                SubmitResponse();
            }

            return;
        }
    }

    private void InitializeIfNeeded()
    {
        if (_canvas != null)
        {
            return;
        }

        EnsureEventSystemExists();

        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (_font == null)
        {
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 500;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        _rootRect = CreateRectTransform("DialogueRoot", transform);
        StretchRect(_rootRect);

        _panelRect = CreateRectTransform("DialoguePanel", _rootRect);
        _panelRect.anchorMin = new Vector2(0.5f, 0f);
        _panelRect.anchorMax = new Vector2(0.5f, 0f);
        _panelRect.pivot = new Vector2(0.5f, 0f);
        _panelRect.anchoredPosition = new Vector2(0f, PanelVerticalOffset);
        _panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        _panelImage = _panelRect.gameObject.AddComponent<Image>();
        _panelImage.preserveAspect = true;
        _panelImage.color = Color.white;

        _textAreaRect = CreateRectTransform("TextArea", _panelRect);
        StretchRect(_textAreaRect, new Vector2(220f, 130f), new Vector2(-180f, -345f));

        _dialogueText = CreateText("DialogueText", _textAreaRect, 42, PinkSpeakerColor, TextAnchor.UpperLeft);
        _dialogueText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _dialogueText.verticalOverflow = VerticalWrapMode.Truncate;
        StretchRect((RectTransform)_dialogueText.transform, Vector2.zero, new Vector2(0f, -250f));

        _continueBoxRect = CreateRectTransform("ContinueBox", _panelRect);
        _continueBoxRect.anchorMin = new Vector2(0.5f, 0f);
        _continueBoxRect.anchorMax = new Vector2(0.5f, 0f);
        _continueBoxRect.pivot = new Vector2(0.5f, 0f);
        _continueBoxRect.anchoredPosition = new Vector2(0f, 200f);
        _continueBoxRect.sizeDelta = new Vector2(310f, 78f);

        _continueBoxImage = _continueBoxRect.gameObject.AddComponent<Image>();
        _continueBoxImage.preserveAspect = true;
        _continueBoxImage.color = Color.white;

        _continueButton = _continueBoxRect.gameObject.AddComponent<Button>();
        _continueButton.transition = Selectable.Transition.None;
        _continueButton.targetGraphic = _continueBoxImage;
        _continueButton.onClick.AddListener(HandleAdvanceRequest);

        _continueBoxText = CreateText("ContinueText", _continueBoxRect, 28, ContinueLabelColor, TextAnchor.MiddleCenter);
        StretchRect((RectTransform)_continueBoxText.transform, new Vector2(68f, 6f), new Vector2(-18f, -8f));

        _responseRoot = CreateRectTransform("ResponseRoot", _textAreaRect).gameObject;
        RectTransform responseRect = (RectTransform)_responseRoot.transform;
        StretchRect(responseRect);

        _responsePromptText = CreateText("ResponsePrompt", responseRect, 36, Color.white, TextAnchor.UpperLeft);
        RectTransform promptRect = (RectTransform)_responsePromptText.transform;
        promptRect.anchorMin = new Vector2(0f, 1f);
        promptRect.anchorMax = new Vector2(1f, 1f);
        promptRect.pivot = new Vector2(0.5f, 1f);
        promptRect.anchoredPosition = Vector2.zero;
        promptRect.sizeDelta = new Vector2(0f, 70f);

        RectTransform inputRect = CreateRectTransform("ResponseInputField", responseRect);
        inputRect.anchorMin = new Vector2(0f, 0f);
        inputRect.anchorMax = new Vector2(1f, 1f);
        inputRect.offsetMin = new Vector2(0f, 0f);
        inputRect.offsetMax = new Vector2(0f, -92f);

        _responseInputField = inputRect.gameObject.AddComponent<InputField>();
        _responseInputField.lineType = InputField.LineType.SingleLine;
        _responseInputField.characterLimit = 200;
        _responseInputField.selectionColor = new Color(1f, 0.66f, 0.85f, 0.35f);
        _responseInputField.transition = Selectable.Transition.None;

        _responseInputText = CreateText("ResponseInputText", inputRect, 42, Color.white, TextAnchor.UpperLeft);
        StretchRect((RectTransform)_responseInputText.transform);

        _responsePlaceholderText = CreateText("ResponsePlaceholder", inputRect, 38, new Color(1f, 0.82f, 0.91f, 0.7f), TextAnchor.UpperLeft);
        _responsePlaceholderText.fontStyle = FontStyle.Italic;
        StretchRect((RectTransform)_responsePlaceholderText.transform);

        _responseInputField.textComponent = _responseInputText;
        _responseInputField.placeholder = _responsePlaceholderText;

        HideDialogue();
    }

    private void Open(DialogueInteractable source)
    {
        if (source == null)
        {
            return;
        }

        InitializeIfNeeded();
        StopTyping();

        _activeLines.Clear();

        foreach (DialogueInteractable.DialogueLine line in source.Lines)
        {
            if (line != null && !string.IsNullOrWhiteSpace(line.text))
            {
                _activeLines.Add(line);
            }
        }

        if (_activeLines.Count == 0)
        {
            Debug.LogWarning($"DialogueInteractable on '{source.name}' does not contain any playable lines.");
            return;
        }

        _currentSource = source;
        _currentLineIndex = -1;
        _ignoreAdvanceFrame = Time.frameCount;

        _panelRect.gameObject.SetActive(true);
        _panelImage.sprite = source.DialogueBoxSprite;
        _panelImage.enabled = source.DialogueBoxSprite != null;
        _continueBoxImage.sprite = source.ContinueBoxSprite;
        _continueBoxImage.enabled = source.ContinueBoxSprite != null;
        _continueBoxText.text = string.IsNullOrWhiteSpace(source.ContinueMessage) ? DefaultContinueLabel : source.ContinueMessage;

        _responseRoot.SetActive(false);
        _dialogueText.gameObject.SetActive(true);
        SetContinueBoxVisible(false);

        ShowNextLine();
    }

    private void HandleAdvanceRequest()
    {
        if (_phase == DialoguePhase.Hidden || Time.frameCount == _ignoreAdvanceFrame)
        {
            return;
        }

        if (_phase == DialoguePhase.Typing)
        {
            CompleteCurrentLine();
            return;
        }

        if (_phase == DialoguePhase.WaitingForAdvance)
        {
            if (CurrentLineRequestsManualResponse())
            {
                ShowResponseInput();
                return;
            }

            if (_currentLineIndex < _activeLines.Count - 1)
            {
                ShowNextLine();
                return;
            }

            HideDialogue();
            return;
        }

        if (_phase == DialoguePhase.WaitingForResponse && _responseInputField != null)
        {
            _responseInputField.ActivateInputField();
        }
    }

    private void ShowNextLine()
    {
        _currentLineIndex++;

        if (_currentLineIndex < 0 || _currentLineIndex >= _activeLines.Count)
        {
            HideDialogue();
            return;
        }

        DialogueInteractable.DialogueLine line = _activeLines[_currentLineIndex];
        ApplyDialogueColor(line.speakerColor);

        _responseRoot.SetActive(false);
        _dialogueText.gameObject.SetActive(true);
        SetContinueBoxVisible(false);

        StopTyping();
        _typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private void ShowResponseInput()
    {
        ApplyResponseColors(_currentSource != null ? _currentSource.ResponseSpeakerColor : DialogueInteractable.SpeakerColor.Pink);

        _dialogueText.gameObject.SetActive(false);
        SetContinueBoxVisible(false);
        _responseRoot.SetActive(true);

        _responsePromptText.text = _currentSource != null ? _currentSource.ResponsePrompt : DefaultResponsePrompt;
        _responsePlaceholderText.text = _currentSource != null ? _currentSource.ResponsePlaceholder : DefaultResponsePlaceholder;

        _responseInputField.text = string.Empty;
        _responseInputField.caretPosition = 0;
        _phase = DialoguePhase.WaitingForResponse;
        _ignoreAdvanceFrame = Time.frameCount;

        EventSystem currentEventSystem = EventSystem.current;
        if (currentEventSystem != null)
        {
            currentEventSystem.SetSelectedGameObject(_responseInputField.gameObject);
        }

        _responseInputField.ActivateInputField();
    }

    private IEnumerator TypeLine(string fullText)
    {
        string safeText = fullText ?? string.Empty;
        float charactersPerSecond = _currentSource != null ? _currentSource.CharactersPerSecond : 72f;

        if (charactersPerSecond <= 0f)
        {
            _dialogueText.text = safeText;
            _phase = DialoguePhase.WaitingForAdvance;
            SetContinueBoxVisible(true);
            _typingCoroutine = null;
            yield break;
        }

        _phase = DialoguePhase.Typing;
        _dialogueText.text = string.Empty;
        SetContinueBoxVisible(false);

        StringBuilder builder = new StringBuilder(safeText.Length);
        float delay = 1f / charactersPerSecond;

        for (int i = 0; i < safeText.Length; i++)
        {
            builder.Append(safeText[i]);
            _dialogueText.text = builder.ToString();
            yield return new WaitForSeconds(delay);
        }

        _dialogueText.text = safeText;
        _phase = DialoguePhase.WaitingForAdvance;
        SetContinueBoxVisible(true);
        _typingCoroutine = null;
    }

    private void CompleteCurrentLine()
    {
        if (_currentLineIndex < 0 || _currentLineIndex >= _activeLines.Count)
        {
            return;
        }

        StopTyping();
        _dialogueText.text = _activeLines[_currentLineIndex].text ?? string.Empty;
        _phase = DialoguePhase.WaitingForAdvance;
        SetContinueBoxVisible(true);
    }

    private void SubmitResponse()
    {
        if (_phase != DialoguePhase.WaitingForResponse || _currentSource == null)
        {
            return;
        }

        string submittedText = _responseInputField.text?.Trim() ?? string.Empty;

        if (submittedText.Length == 0)
        {
            _responseInputField.ActivateInputField();
            return;
        }

        _currentSource.RegisterSubmittedResponse(submittedText);

        if (_currentLineIndex < _activeLines.Count - 1)
        {
            _responseRoot.SetActive(false);
            _dialogueText.gameObject.SetActive(true);
            SetContinueBoxVisible(false);
            _ignoreAdvanceFrame = Time.frameCount;
            ShowNextLine();
            return;
        }

        HideDialogue();
    }

    private void HideDialogue()
    {
        StopTyping();

        _phase = DialoguePhase.Hidden;
        _currentLineIndex = -1;
        _currentSource = null;
        _activeLines.Clear();

        if (_panelRect != null)
        {
            _panelRect.gameObject.SetActive(false);
        }

        SetContinueBoxVisible(false);

        if (_responseInputField != null)
        {
            _responseInputField.text = string.Empty;
        }
    }

    private void StopTyping()
    {
        if (_typingCoroutine == null)
        {
            return;
        }

        StopCoroutine(_typingCoroutine);
        _typingCoroutine = null;
    }

    private bool CurrentLineRequestsManualResponse()
    {
        if (_currentSource == null || _currentLineIndex < 0 || _currentLineIndex >= _activeLines.Count)
        {
            return false;
        }

        return _currentSource.ShouldRequestManualResponse(_activeLines[_currentLineIndex]);
    }

    private void SetContinueBoxVisible(bool isVisible)
    {
        if (_continueBoxRect == null)
        {
            return;
        }

        _continueBoxRect.gameObject.SetActive(isVisible);
    }

    private void ApplyDialogueColor(DialogueInteractable.SpeakerColor speakerColor)
    {
        _dialogueText.color = ResolveSpeakerColor(speakerColor);
    }

    private void ApplyResponseColors(DialogueInteractable.SpeakerColor speakerColor)
    {
        Color speakerColorValue = ResolveSpeakerColor(speakerColor);
        Color placeholderColor = speakerColorValue;
        placeholderColor.a = 0.6f;

        _responsePromptText.color = speakerColorValue;
        _responseInputText.color = speakerColorValue;
        _responsePlaceholderText.color = placeholderColor;
    }

    private static Color ResolveSpeakerColor(DialogueInteractable.SpeakerColor speakerColor)
    {
        return speakerColor == DialogueInteractable.SpeakerColor.Blue
            ? BlueSpeakerColor
            : PinkSpeakerColor;
    }

    private void EnsureEventSystemExists()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.layer = UiLayer;
        DontDestroyOnLoad(eventSystemObject);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private RectTransform CreateRectTransform(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.layer = UiLayer;
        uiObject.transform.SetParent(parent, false);
        return uiObject.GetComponent<RectTransform>();
    }

    private Text CreateText(string objectName, Transform parent, int fontSize, Color color, TextAnchor alignment)
    {
        RectTransform textRect = CreateRectTransform(objectName, parent);
        Text text = textRect.gameObject.AddComponent<Text>();
        text.font = _font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.supportRichText = false;
        text.raycastTarget = false;
        return text;
    }

    private static void StretchRect(RectTransform rectTransform)
    {
        StretchRect(rectTransform, Vector2.zero, Vector2.zero);
    }

    private static void StretchRect(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }
}
