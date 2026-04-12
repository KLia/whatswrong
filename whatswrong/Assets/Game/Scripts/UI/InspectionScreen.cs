using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class InspectionScreen : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image image;

        public void Show(InspectionData data)
        {
            root.SetActive(true);
            titleText.text = data.title;
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
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}