using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class InspectionScreen : MonoBehaviour
    {
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
            descriptionText.text = data.description;

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

            // Start scale animation
            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            image.rectTransform.localScale = startScale;
            scaleRoutine = StartCoroutine(ScaleToFull());
        }

        private IEnumerator ScaleToFull()
        {
            Vector3 target = Vector3.one;
            Vector3 initial = image.rectTransform.localScale;

            float t = 0f;
            while (t < scaleDuration)
            {
                t += Time.deltaTime;
                float lerp = t / scaleDuration;
                image.rectTransform.localScale = Vector3.Lerp(initial, target, lerp);
                yield return null;
            }

            image.rectTransform.localScale = target;
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}