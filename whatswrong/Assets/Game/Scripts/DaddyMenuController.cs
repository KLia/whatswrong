using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DaddyMenuController : MonoBehaviour
{
    private const int UiLayer = 5;
    private const string StartupSceneName = "StartupScene";

    private List<string> _daddyNames = new List<string>();

    private void Awake()
    {
        EnsureEventSystemExists();
        LoadDaddies();
        BuildUI();

        if (_daddyNames.Count > 0)
            GameSetup.ChosenDaddyName = _daddyNames[0];
    }

    private void LoadDaddies()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "daddies.json");
        if (!File.Exists(path))
        {
            Debug.LogError("daddies.json not found at " + path);
            return;
        }

        string json = File.ReadAllText(path);
        JObject root = JObject.Parse(json);
        JObject daddiesObj = (JObject)root["daddies"];

        var daddies = new Dictionary<string, Daddy>();

        foreach (var kvp in daddiesObj)
        {
            string daddyName = kvp.Key;
            JObject entry = (JObject)kvp.Value;

            var daddy = new Daddy
            {
                Name = daddyName,
                Personality = entry["personality"]?.ToString() ?? "",
                Reason = new List<Reason>()
            };

            JArray dataArray = (JArray)entry["data"];
            if (dataArray != null)
            {
                foreach (JObject reasonObj in dataArray)
                {
                    var reason = new Reason
                    {
                        ReasonText = reasonObj["reason"]?.ToString() ?? ""
                    };

                    JArray objects = (JArray)reasonObj["objects"];
                    if (objects != null && objects.Count >= 3)
                    {
                        reason.Object1 = objects[0]["name"]?.ToString() ?? "";
                        reason.Hint1 = objects[0]["hint"]?.ToString() ?? "";
                        reason.Object2 = objects[1]["name"]?.ToString() ?? "";
                        reason.Hint2 = objects[1]["hint"]?.ToString() ?? "";
                        reason.Object3 = objects[2]["name"]?.ToString() ?? "";
                        reason.Hint3 = objects[2]["hint"]?.ToString() ?? "";
                    }

                    daddy.Reason.Add(reason);
                }
            }

            daddies[daddyName] = daddy;
        }

        GameSetup.Daddies = daddies;
        _daddyNames = new List<string>(daddies.Keys);
    }

    private void BuildUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Background canvas
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

        canvas.pixelPerfect = true;

        canvasObject.AddComponent<GraphicRaycaster>();

        // Game title image
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
            titleRect.sizeDelta = new Vector2(2100f, 600f);
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
        dropdownRect.anchoredPosition = new Vector2(0f, -20f);
        dropdownRect.sizeDelta = new Vector2(800f, 70f);

        dropdown.ClearOptions();
        dropdown.AddOptions(_daddyNames);
        dropdown.onValueChanged.AddListener(index =>
        {
            if (index >= 0 && index < _daddyNames.Count)
                GameSetup.ChosenDaddyName = _daddyNames[index];
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
        startRect.anchoredPosition = new Vector2(0f, -280f);
        startRect.sizeDelta = new Vector2(1080f, 270f);

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
        GameObject root = new GameObject("Dropdown", typeof(RectTransform));
        root.layer = UiLayer;
        root.transform.SetParent(parent, false);

        Image rootImage = root.AddComponent<Image>();
        rootImage.color = new Color(0.2f, 0.2f, 0.25f);

        Dropdown dropdown = root.AddComponent<Dropdown>();

        Text captionText = CreateText("CaptionText", root.transform, font, 32, Color.white, TextAnchor.MiddleLeft);
        RectTransform captionRect = (RectTransform)captionText.transform;
        captionRect.anchorMin = Vector2.zero;
        captionRect.anchorMax = Vector2.one;
        captionRect.offsetMin = new Vector2(20f, 4f);
        captionRect.offsetMax = new Vector2(-20f, -4f);
        captionText.raycastTarget = true;

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
        itemLabelRect.offsetMin = new Vector2(20f, 2f);
        itemLabelRect.offsetMax = new Vector2(-10f, -2f);

        RectTransform itemRect = (RectTransform)itemObj.transform;
        itemRect.anchorMin = new Vector2(0f, 0.5f);
        itemRect.anchorMax = new Vector2(1f, 0.5f);
        itemRect.sizeDelta = new Vector2(0f, 50f);

        dropdown.captionText = captionText;
        dropdown.itemText = itemLabel;
        dropdown.template = templateRect;

        templateObj.SetActive(false);

        return dropdown;
    }

    private static Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>("UI/" + name);
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
