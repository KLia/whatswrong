using UnityEngine;

namespace Game.Scripts.DaddyInteraction
{
    public class IntroDialogController : MonoBehaviour
    {
        [SerializeField] private RoomInput roomInput;
        [SerializeField] private DialogLine[] dialogLines;
        [SerializeField] private DialogUI dialogUI;

        private int _index = 0;
        private bool _endReached = false;

        private void Start()
        {
            roomInput.SetInteractionEnabled(false);
            Continue();
        }
        
        private void OnEnable()
        {
            if (dialogUI == null)
                return;
            
            dialogUI.ContinueButtonClicked += Continue;
        }


        public void Continue()
        {
            if (_endReached) return;

            if (_index < dialogLines.Length)
            {
                var currentLine = dialogLines[_index].text;
                var currentColor = SpeakerColorMapping.GetColor(dialogLines[_index].speakerColor);

                dialogUI.ShowLine(currentLine, currentColor);
                _index++;
            }
            else
            {
                _endReached = true;
                EndStartFlow();
            }
        }

        private void EndStartFlow()
        {
            dialogUI.Hide();
            roomInput.SetInteractionEnabled(true);
        }

        public void Restart()
        {
            _index = 0;
            _endReached = false;
            Continue();
        }
        
        private void OnDisable()
        {
            if (dialogUI == null)
                return;

            dialogUI.ContinueButtonClicked -= Continue;
        }
    }
}