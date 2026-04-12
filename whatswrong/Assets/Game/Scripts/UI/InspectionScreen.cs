using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class InspectionScreen : MonoBehaviour, IPointerClickHandler
    {
        public event Action InspectionScreenClicked;
        
        [SerializeField] private CanvasGroup contentCanvasGroup;
        [SerializeField] private GameObject root;
        [SerializeField] private Image image;

        [Header("Scale Settings")]
        [SerializeField] private Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private float scaleDuration = 0.25f;

        private Coroutine _scaleRoutine;

        public void Show(InspectionData data)
        {
            root.SetActive(true);

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

            if (_scaleRoutine != null)
                StopCoroutine(_scaleRoutine);

            image.rectTransform.localScale = startScale;
            contentCanvasGroup.alpha = 0f;

            _scaleRoutine = StartCoroutine(ScaleAndFadeIn());
        }

        public void Hide()
        {
            if (!gameObject.activeInHierarchy || !root.activeInHierarchy)
            {
                ApplyHiddenState();
                return;
            }

            if (!image.enabled)
            {
                ApplyHiddenState();
                return;
            }

            if (_scaleRoutine != null)
                StopCoroutine(_scaleRoutine);

            _scaleRoutine = StartCoroutine(ScaleAndFadeOut());
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
            _scaleRoutine = null;
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
            _scaleRoutine = null;
            ApplyHiddenState();
        }

        private void ApplyHiddenState()
        {
            if (_scaleRoutine != null)
            {
                StopCoroutine(_scaleRoutine);
                _scaleRoutine = null;
            }

            contentCanvasGroup.alpha = 0f;
            image.rectTransform.localScale = startScale;
            image.enabled = false;
            root.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InspectionScreenClicked?.Invoke();
        }
    }
}
