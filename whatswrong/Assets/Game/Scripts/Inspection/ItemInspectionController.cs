using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    public class ItemInspectionController : MonoBehaviour
    {
        [FormerlySerializedAs("roomInteractionController")] [SerializeField] private RoomInput roomInput;
        [SerializeField] private InspectionScreen inspectionScreen;
        [SerializeField] private GameManager gameManager;
        [FormerlySerializedAs("dialog")] [SerializeField] private DialogUI dialogUI;
        
        
        private bool _inspecting;

        private void OnEnable()
        {
            if (roomInput == null)
                return;

            roomInput.InspectRequested += OnInspectItem;
            inspectionScreen.InspectionScreenClicked += CloseInspection;
            dialogUI.ContinueButtonnClicked += CloseInspection;
        }

        private void OnDisable()
        {
            if (roomInput == null)
                return;

            roomInput.InspectRequested -= OnInspectItem;
            inspectionScreen.InspectionScreenClicked -= CloseInspection;
            dialogUI.ContinueButtonnClicked -= CloseInspection;
        }

        private void OnInspectItem(InspectionData data)
        {
            if(_inspecting) 
                return;
            
            if (data == null)
            {
                Debug.LogWarning($"No inspection data attached to object.");
                return;
            }

            OpenInspection(data);
        }

        private void OpenInspection(InspectionData data)
        {
            DisableInput();
            var description = gameManager.RevealClue(data.id);
            _inspecting = true;
            inspectionScreen.Show(data);
            dialogUI.ShowLine(description, SpeakerColorMapping.GetColor(SpeakerColor.Pink));
        }

        private void CloseInspection()
        {
            inspectionScreen.Hide();
            dialogUI.Hide();
            _inspecting = false;
            EnableInput();
        }
        
        private void DisableInput()
        {
            roomInput.SetInteractionEnabled(false);
        }
        
        private void EnableInput()
        {
            roomInput.SetInteractionEnabled(true);
        }
    }
}