using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class NavigationControls : MonoBehaviour
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;

        public event Action NextButtonPressed;
        public event Action PreviousButtonPressed;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            nextButton.onClick.AddListener(NextButtonClicked);
            previousButton.onClick.AddListener(PreviousButtonClicked);
        }

        private void PreviousButtonClicked()
        {
            PreviousButtonPressed?.Invoke();
        }

        private void NextButtonClicked()
        {
            NextButtonPressed?.Invoke();
        }
    }
}