using UnityEngine;

namespace Game.Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        [SerializeField] private RoomInteractionController roomInteractionController;
        [SerializeField] private InspectionDatabase inspectionDatabase;
        [SerializeField] private InspectionScreen inspectionScreen;
        [SerializeField] private BackgroundBlurController blurController;

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

        private void OpenInspection(string objectId)
        {
            InspectionData data = inspectionDatabase.GetById(objectId);
            if (data == null)
            {
                Debug.LogWarning($"No inspection data found for id: {objectId}");
                return;
            }

           // blurController?.EnableBlur();
            inspectionScreen.Show(data);
        }

        private void CloseInspection()
        {
            inspectionScreen.Hide();
           // blurController?.DisableBlur();
        }
    }
}