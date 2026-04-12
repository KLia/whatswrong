using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DaddyMenuController : MonoBehaviour
{
    private const int UiLayer = 5;
    private const string StartupSceneName = "StartupScene";

    private readonly List<DaddyProfile> _profiles = new List<DaddyProfile>();

    private void Awake()
    {
        EnsureEventSystemExists();
        BuildProfiles();
        BuildUI();

        if (_profiles.Count > 0)
            GameSetup.Daddy = _profiles[0];
    }

    private void BuildProfiles()
    {
        AddProfile("Aries Daddy - Firefighter",
            "Personality: Acts first, thinks later, but somehow always lands on his feet. Protective in a way that feels like heat... intense, immediate, undeniable. | North Star: Action above all | Secondary Trait: Impatient with emotional nuance | Non-Negotiable: Never abandons someone in danger | Fatal Flaw: Rushes in without thinking, creates bigger disasters");

        AddProfile("Taurus Daddy - Luxury Chef",
            "Personality: Moves slow, chooses carefully, indulges deeply. Shows love through touch, food, and presence you can lean into. | North Star: Stability and comfort | Secondary Trait: Deep possessiveness | Non-Negotiable: Protects what he considers \"his\" | Fatal Flaw: Refuses change, even when everything is crumbling");

        AddProfile("Gemini Daddy - Writer",
            "Personality: Words are his playground and his weapon. Keeps you guessing whether hes joking, flirting... or both. | North Star: Curiosity and stimulation | Secondary Trait: Avoidance of emotional depth | Non-Negotiable: Freedom to express himself | Fatal Flaw: Deflects truth with humor until nothing feels real");

        AddProfile("Cancer Daddy - Therapist (Soft Dom)",
            "Personality: Reads you gently, holds you firmly, never lets you fall too far. Control wrapped in care, guidance that feels like home. | North Star: Emotional safety (for others) | Secondary Trait: Fear of vulnerability (for himself) | Non-Negotiable: Protects those he loves at all costs | Fatal Flaw: Over-gives, loses himself, then quietly resents it");

        AddProfile("Leo Daddy - Actor",
            "Personality: Thrives in attention but gives it back tenfold. Makes you feel chosen, seen, and a little bit worshipped. | North Star: To be adored and remembered | Secondary Trait: Genuine generosity | Non-Negotiable: Will not be ignored or disrespected | Fatal Flaw: Needs validation so badly it clouds his judgment");

        AddProfile("Virgo Daddy - Doctor Surgeon",
            "Personality: Observes everything, misses nothing, fixes whats broken. Quiet control, expressed through perfection and intention. | North Star: Perfection and order | Secondary Trait: Deep internal anxiety | Non-Negotiable: Mistakes are unacceptable | Fatal Flaw: Self-criticism so harsh it becomes paralyzing");

        AddProfile("Libra Daddy - Art Curator",
            "Personality: Lives for beauty, balance, and shared moments. Flirts like its an art form... and youre the masterpiece. | North Star: Harmony and connection | Secondary Trait: Fear of conflict | Non-Negotiable: Keeps the peace at all costs | Fatal Flaw: Avoids hard choices until everything collapses");

        AddProfile("Scorpio Daddy - Police Detective",
            "Personality: Sees through lies, silence, and surface-level truths. Intensity simmers beneath stillness, pulling you in without effort. | North Star: Truth and emotional depth | Secondary Trait: Extreme secrecy | Non-Negotiable: Betrayal is unforgivable | Fatal Flaw: Obsession that consumes him and others");

        AddProfile("Sagittarius Daddy - Travel Photographer",
            "Personality: Restless soul, always chasing the next horizon. Connection with him feels like freedom... not confinement. | North Star: Freedom and exploration | Secondary Trait: Difficulty committing | Non-Negotiable: Will never feel trapped | Fatal Flaw: Runs when things start to matter too much");

        AddProfile("Capricorn Daddy - Nordic CEO",
            "Personality: Disciplined, controlled, built himself from nothing. Softness exists, but only behind locked doors and earned trust. | North Star: Achievement and legacy | Secondary Trait: Emotional suppression | Non-Negotiable: Failure is not an option | Fatal Flaw: Sacrifices everything, including himself");

        AddProfile("Aquarius Daddy - Tech Innovator",
            "Personality: Thinks ahead of the world, lives outside convention. Detached at first... until he chooses to let you in. | North Star: Innovation and change | Secondary Trait: Emotional detachment | Non-Negotiable: Wont conform to expectations | Fatal Flaw: Disconnects from people in pursuit of ideas");

        AddProfile("Pisces Daddy - Musician",
            "Personality: Feels everything, expresses it without needing words. Being with him is like drifting through a dream you dont want to wake from. | North Star: Emotional expression | Secondary Trait: Escapism | Non-Negotiable: Protects his inner world | Fatal Flaw: Avoids reality until it crashes in");
    }

    private void AddProfile(string name, string description)
    {
        DaddyProfile profile = ScriptableObject.CreateInstance<DaddyProfile>();
        profile.daddyName = name;
        profile.description = description;
        _profiles.Add(profile);
    }

    private void BuildUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Background canvas (behind everything)
        GameObject bgCanvasObj = new GameObject("BackgroundCanvas", typeof(RectTransform));
        bgCanvasObj.layer = UiLayer;

        Canvas bgCanvas = bgCanvasObj.AddComponent<Canvas>();
        bgCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        bgCanvas.sortingOrder = 0;

        CanvasScaler bgScaler = bgCanvasObj.AddComponent<CanvasScaler>();
        bgScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        bgScaler.referenceResolution = new Vector2(1920f, 1080f);
        bgScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        bgScaler.matchWidthOrHeight = 0.5f;

        // Background image
        GameObject bgObj = new GameObject("Background", typeof(RectTransform));
        bgObj.layer = UiLayer;
        bgObj.transform.SetParent(bgCanvasObj.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        Sprite bgSprite = LoadSprite("blurred_bg");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.preserveAspect = false;
        }
        else
        {
            bgImage.color = new Color(0.1f, 0.1f, 0.15f);
        }

        RectTransform bgRect = (RectTransform)bgObj.transform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Main UI canvas
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform));
        canvasObject.layer = UiLayer;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        // Game title image (placeholder — expects game_title.png in Art/UI/)
        Sprite titleSprite = LoadSprite("game_title");
        if (titleSprite != null)
        {
            GameObject titleObj = new GameObject("GameTitle", typeof(RectTransform));
            titleObj.layer = UiLayer;
            titleObj.transform.SetParent(canvasObject.transform, false);

            Image titleImage = titleObj.AddComponent<Image>();
            titleImage.sprite = titleSprite;
            titleImage.preserveAspect = true;
            titleImage.raycastTarget = false;

            RectTransform titleRect = (RectTransform)titleObj.transform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -20f);
            titleRect.sizeDelta = new Vector2(1400f, 400f);
        }

        // Dropdown label
        Text label = CreateText("Label", canvasObject.transform, font, 42, Color.white, TextAnchor.MiddleCenter);
        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = new Vector2(0f, 80f);
        labelRect.sizeDelta = new Vector2(600f, 50f);
        label.text = "Choose Your Daddy";

        // Dropdown
        Dropdown dropdown = CreateDropdown(canvasObject.transform, font);
        RectTransform dropdownRect = (RectTransform)dropdown.transform;
        dropdownRect.anchorMin = new Vector2(0.5f, 0.5f);
        dropdownRect.anchorMax = new Vector2(0.5f, 0.5f);
        dropdownRect.pivot = new Vector2(0.5f, 0.5f);
        dropdownRect.anchoredPosition = new Vector2(0f, 10f);
        dropdownRect.sizeDelta = new Vector2(600f, 60f);

        List<string> names = new List<string>();
        foreach (DaddyProfile p in _profiles)
            names.Add(p.daddyName);

        dropdown.ClearOptions();
        dropdown.AddOptions(names);
        dropdown.onValueChanged.AddListener(index =>
        {
            if (index >= 0 && index < _profiles.Count)
                GameSetup.Daddy = _profiles[index];
        });

        // Start button with image
        Sprite startSprite = LoadSprite("start_button");
        GameObject startObj = new GameObject("StartButton", typeof(RectTransform));
        startObj.layer = UiLayer;
        startObj.transform.SetParent(canvasObject.transform, false);

        Image startImage = startObj.AddComponent<Image>();
        if (startSprite != null)
        {
            startImage.sprite = startSprite;
            startImage.preserveAspect = true;
            startImage.color = Color.white;
        }
        else
        {
            startImage.color = new Color(0.55f, 0.09f, 0.34f);
        }

        Button startButton = startObj.AddComponent<Button>();
        startButton.targetGraphic = startImage;
        startButton.transition = Selectable.Transition.ColorTint;

        ColorBlock btnColors = startButton.colors;
        btnColors.normalColor = Color.white;
        btnColors.highlightedColor = new Color(0.85f, 0.85f, 0.85f);
        btnColors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        startButton.colors = btnColors;

        RectTransform startRect = (RectTransform)startObj.transform;
        startRect.anchorMin = new Vector2(0.5f, 0.5f);
        startRect.anchorMax = new Vector2(0.5f, 0.5f);
        startRect.pivot = new Vector2(0.5f, 0.5f);
        startRect.anchoredPosition = new Vector2(0f, -180f);
        startRect.sizeDelta = new Vector2(1440f, 360f);

        // Only add text label if no sprite (fallback)
        if (startSprite == null)
        {
            Text btnText = CreateText("Text", startObj.transform, font, 38, Color.white, TextAnchor.MiddleCenter);
            RectTransform textRect = (RectTransform)btnText.transform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            btnText.text = "Start Game";
        }

        startButton.onClick.AddListener(() => SceneManager.LoadScene(StartupSceneName));
    }

    private Dropdown CreateDropdown(Transform parent, Font font)
    {
        // Root
        GameObject root = new GameObject("Dropdown", typeof(RectTransform));
        root.layer = UiLayer;
        root.transform.SetParent(parent, false);

        Image rootImage = root.AddComponent<Image>();
        rootImage.color = new Color(0.2f, 0.2f, 0.25f);

        Dropdown dropdown = root.AddComponent<Dropdown>();

        // Caption
        Text captionText = CreateText("CaptionText", root.transform, font, 32, Color.white, TextAnchor.MiddleLeft);
        RectTransform captionRect = (RectTransform)captionText.transform;
        captionRect.anchorMin = Vector2.zero;
        captionRect.anchorMax = Vector2.one;
        captionRect.offsetMin = new Vector2(16f, 0f);
        captionRect.offsetMax = new Vector2(-16f, 0f);
        captionText.raycastTarget = true;

        // Template
        GameObject templateObj = new GameObject("Template", typeof(RectTransform));
        templateObj.layer = UiLayer;
        templateObj.transform.SetParent(root.transform, false);

        Image templateBg = templateObj.AddComponent<Image>();
        templateBg.color = new Color(0.15f, 0.15f, 0.2f);

        ScrollRect scrollRect = templateObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        RectTransform templateRect = (RectTransform)templateObj.transform;
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchoredPosition = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0f, 400f);

        // Viewport
        GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform));
        viewportObj.layer = UiLayer;
        viewportObj.transform.SetParent(templateObj.transform, false);

        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = Color.white;

        Mask mask = viewportObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        RectTransform viewportRect = (RectTransform)viewportObj.transform;
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(0f, 6f);
        viewportRect.offsetMax = new Vector2(0f, -6f);

        // Content — sized to fit items exactly, no extra padding
        GameObject contentObj = new GameObject("Content", typeof(RectTransform));
        contentObj.layer = UiLayer;
        contentObj.transform.SetParent(viewportObj.transform, false);

        RectTransform contentRect = (RectTransform)contentObj.transform;
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;

        // Item
        GameObject itemObj = new GameObject("Item", typeof(RectTransform));
        itemObj.layer = UiLayer;
        itemObj.transform.SetParent(contentObj.transform, false);

        Image itemBg = itemObj.AddComponent<Image>();
        itemBg.color = new Color(0.15f, 0.15f, 0.2f);

        Toggle toggle = itemObj.AddComponent<Toggle>();
        toggle.targetGraphic = itemBg;
        toggle.transition = Selectable.Transition.ColorTint;

        ColorBlock toggleColors = toggle.colors;
        toggleColors.normalColor = Color.white;
        toggleColors.highlightedColor = new Color(0.6f, 0.6f, 0.7f);
        toggleColors.pressedColor = new Color(0.5f, 0.5f, 0.6f);
        toggleColors.selectedColor = new Color(0.6f, 0.6f, 0.7f);
        toggle.colors = toggleColors;

        Text itemLabel = CreateText("ItemLabel", itemObj.transform, font, 30, Color.white, TextAnchor.MiddleLeft);
        RectTransform itemLabelRect = (RectTransform)itemLabel.transform;
        itemLabelRect.anchorMin = Vector2.zero;
        itemLabelRect.anchorMax = Vector2.one;
        itemLabelRect.offsetMin = new Vector2(16f, 0f);
        itemLabelRect.offsetMax = Vector2.zero;

        RectTransform itemRect = (RectTransform)itemObj.transform;
        itemRect.anchorMin = new Vector2(0f, 0.5f);
        itemRect.anchorMax = new Vector2(1f, 0.5f);
        itemRect.sizeDelta = new Vector2(0f, 50f);

        // Wire up
        dropdown.captionText = captionText;
        dropdown.itemText = itemLabel;
        dropdown.template = templateRect;

        templateObj.SetActive(false);

        return dropdown;
    }

    private static Sprite LoadSprite(string name)
    {
        // Unity loads sprites from Resources folders. Since our images are in
        // Assets/Game/Art/UI/, we load via Resources.Load if a Resources folder
        // exists, otherwise fall back to streaming. For simplicity, we place a
        // "Resources" folder symlink or move assets there at build time.
        // For now: try loading from a Resources/UI/ path.
        Sprite sprite = Resources.Load<Sprite>("UI/" + name);
        return sprite;
    }

    private Text CreateText(string objectName, Transform parent, Font font, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(objectName, typeof(RectTransform));
        textObj.layer = UiLayer;
        textObj.transform.SetParent(parent, false);

        Text text = textObj.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;

        return text;
    }

    private void EnsureEventSystemExists()
    {
        if (EventSystem.current != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.layer = UiLayer;
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }
}
