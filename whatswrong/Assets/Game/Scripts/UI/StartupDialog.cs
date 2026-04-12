using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class StartupDialog : MonoBehaviour
    {
        [SerializeField] private DialogElement[] dialogLines;
        [SerializeField] private CanvasGroup contentCanvasGroup;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text descriptionText;

        private int index = 0;

        private void Start()
        {
            Continue();
        }

        public void Continue()
        {
            if (index < dialogLines.Length)
            {
                var currentLine = dialogLines[index].text;
                var currentColor = SpeakerColorMappings.GetColor(dialogLines[index].speakerColor);
            
                ShowLine(currentLine, currentColor);
                index++;
            }
            else
            {
                Hide();
            }
        }

        private void ShowLine(string text, Color color)
        {
            root.SetActive(true);

            descriptionText.text = text;
            descriptionText.color = color;
        }

        private void Hide()
        {
            contentCanvasGroup.alpha = 0f;
            root.SetActive(false);
        }
    }
}