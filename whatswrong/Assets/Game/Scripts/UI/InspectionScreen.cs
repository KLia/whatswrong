using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class InspectionScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup contentCanvasGroup;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image image;

        [Header("Scale Settings")]
        [SerializeField] private Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private float scaleDuration = 0.25f;

        private Coroutine scaleRoutine;

        public void Show(InspectionData data)
        {
            root.SetActive(true);

            descriptionText.text = data.Description;

            if (data.image != null)
            {
                image.sprite = data.image;
                image.enabled = true;
            }
            else
            {
                image.sprite = null;
                image.enabled = false;
            }

            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            image.rectTransform.localScale = startScale;
            contentCanvasGroup.alpha = 0f;

            scaleRoutine = StartCoroutine(ScaleAndFadeIn());
        }

        public void Hide()
        {
            if (!image.enabled)
            {
                root.SetActive(false);
                return;
            }

            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            scaleRoutine = StartCoroutine(ScaleAndFadeOut());
        }

        private IEnumerator ScaleAndFadeIn()
        {
            Vector3 targetScale = Vector3.one;
            Vector3 initialScale = image.rectTransform.localScale;

            float t = 0f;
            while (t < scaleDuration)
            {
                t += Time.deltaTime;
                float lerp = t / scaleDuration;

                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, lerp);
                contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, lerp);

                yield return null;
            }

            image.rectTransform.localScale = targetScale;
            contentCanvasGroup.alpha = 1f;
        }

        private IEnumerator ScaleAndFadeOut()
        {
            Vector3 initialScale = image.rectTransform.localScale;
            Vector3 targetScale = startScale;

            float t = 0f;
            while (t < scaleDuration)
            {
                t += Time.deltaTime;
                float lerp = t / scaleDuration;

                image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, lerp);
                contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, lerp);

                yield return null;
            }

            image.rectTransform.localScale = targetScale;
            contentCanvasGroup.alpha = 0f;

            image.enabled = false;
            root.SetActive(false);
        }
    }
}