using System;
using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class DialogUI : MonoBehaviour
    {
        public event Action ContinueButtonnClicked;
        public event Action<string> ResponseSubmitted;

        [SerializeField] private CanvasGroup contentCanvasGroup;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_InputField inputText;

        private void Awake()
        {
            inputText.onSubmit.AddListener(HandleSubmit);
        }

        private void HandleSubmit(string text)
        {
            ResponseSubmitted?.Invoke(text);
        }

        public void ContinueButtonClickHandler() //Called from within unity dialogUI prefab continue button
        {
            ContinueButtonnClicked?.Invoke();
        }

        public void ShowLine(string text, Color color = default)
        {
            if(color == default)
                color = SpeakerColorMapping.GetColor(SpeakerColor.Player);
            
            root.SetActive(true);
            inputText.gameObject.SetActive(false);
            descriptionText.gameObject.SetActive(true);
           
            descriptionText.text = text;
            descriptionText.color = color;
        }

        public void ShowTextInput(Color color = default)
        {
            if(color == default)
                color = SpeakerColorMapping.GetColor(SpeakerColor.Daddy);
            
            root.SetActive(true);
            inputText.gameObject.SetActive(true);
            descriptionText.gameObject.SetActive(false);

            inputText.text = string.Empty;
            inputText.ActivateInputField();
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}