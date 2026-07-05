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
        }

        private void OnDisable()
        {
            if (roomInput == null)
                return;

            roomInput.InspectRequested -= OnInspectItem;
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
            _inspecting = true;
            
            dialogUI.ContinueButtonClicked += CloseInspection;
            inspectionScreen.InspectionScreenClicked += CloseInspection;
            roomInput.SetInteractionEnabled(false);
            
            var description = gameManager.RevealClue(data.id);
            inspectionScreen.Show(data);
            dialogUI.ShowLine(description, SpeakerColorMapping.GetColor(SpeakerColor.Player));
        }

        private void CloseInspection()
        {
            if (!_inspecting)
                return;

            _inspecting = false;
            
            dialogUI.ContinueButtonClicked -= CloseInspection;
            inspectionScreen.InspectionScreenClicked -= CloseInspection;
            roomInput.SetInteractionEnabled(true);
                        
            inspectionScreen.Hide();
            dialogUI.Hide();
        }
    }
}