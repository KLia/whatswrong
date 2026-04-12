using DefaultNamespace;
using UnityEngine;

namespace Game.Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        [SerializeField] private RoomInteractionController roomInteractionController;
        [SerializeField] private InspectionDatabase inspectionDatabase;
        [SerializeField] private InspectionScreen inspectionScreen;
        [SerializeField] private GameManager gameManager;

        private void OnEnable()
        {
            if (roomInteractionController == null)
                return;

            roomInteractionController.InspectRequested += OpenInspection;
            roomInteractionController.OutspectRequested += CloseInspection;
        }

        private void OnDisable()
        {
            if (roomInteractionController == null)
                return;

            roomInteractionController.InspectRequested -= OpenInspection;
            roomInteractionController.OutspectRequested -= CloseInspection;
        }

        private void OpenInspection(InspectionData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"No inspection data attached to object.");
                return;
            }

            roomInteractionController.SetInteractionEnabled(false);

            data.Description = gameManager.RevealClue(data.id);
            inspectionScreen.Show(data);
        }

        private void CloseInspection()
        {
            inspectionScreen.Hide();
            roomInteractionController.SetInteractionEnabled(true);
        }
    }
}