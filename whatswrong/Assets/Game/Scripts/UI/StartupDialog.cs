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

        // [Header("Scale Settings")] [SerializeField]
        // private float scaleDuration = 0.25f;

        // private Coroutine scaleRoutine;
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
                var currentColor = dialogLines[index].color;
            
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

        // private IEnumerator ScaleAndFadeIn()
        // {
        //     float t = 0f;
        //     while (t < scaleDuration)
        //     {
        //         t += Time.deltaTime;
        //         float lerp = t / scaleDuration;
        //
        //         contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, lerp);
        //
        //         yield return null;
        //     }
        //
        //     contentCanvasGroup.alpha = 1f;
        //     scaleRoutine = null;
        // }
        //
        // private IEnumerator ScaleAndFadeOut()
        // {
        //     float t = 0f;
        //     while (t < scaleDuration)
        //     {
        //         t += Time.deltaTime;
        //         float lerp = t / scaleDuration;
        //
        //         contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, lerp);
        //
        //         yield return null;
        //     }
        //
        //     contentCanvasGroup.alpha = 0f;
        //     scaleRoutine = null;
        //     ApplyHiddenState();
        // }

        private void ApplyHiddenState()
        {
            // if (scaleRoutine != null)
            // {
            //     StopCoroutine(scaleRoutine);
            //     scaleRoutine = null;
            // }

           
        }
    }
}