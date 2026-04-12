using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    [Header("Zoom Settings")]
    [Tooltip("Quanto ingrandire l'oggetto (es. 2 = doppia dimensione)")]
    public float zoomScale = 2f;

    [Tooltip("Quanto la camera si avvicina (valori piccoli = più vicino)")]
    [FormerlySerializedAs("zoomDistance")]
    public float cameraZoomDistance = 3f;

    [Tooltip("Offset della posizione quando zoomato")]
    [FormerlySerializedAs("cameraOffset")]
    public Vector3 zoomOffset = Vector3.zero;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private int originalSortingOrder;

    private void Start()
    {
        // Salva valori originali
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        Camera zoomCamera = ResolveZoomCamera();
        if (zoomCamera == null)
        {
            Debug.LogWarning("Camera per lo zoom non trovata!");
            return;
        }

        InspectionTransition zoomController = zoomCamera.GetComponent<InspectionTransition>();

        if (zoomController != null)
        {
            // Blocca il click se c'è QUALSIASI zoom attivo (anche in corso) su un altro oggetto
            if (zoomController.CurrentTarget != null && zoomController.CurrentTarget != transform)
            {
                return;
            }

            // Se già zoomato su questo stesso oggetto, ignora
            if (zoomController.IsZoomed && zoomController.CurrentTarget == transform)
            {
                return;
            }

            // Porta l'oggetto davanti alla camera sul centro dello schermo.
            // Senza una differenza di profondità reale, il Depth Of Field non ha nulla da sfocare.
            float targetDistance = Mathf.Clamp(
                cameraZoomDistance,
                zoomCamera.nearClipPlane + 0.25f,
                zoomCamera.farClipPlane - 0.25f);

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, targetDistance);
            Vector3 worldCenter = zoomCamera.ScreenToWorldPoint(screenCenter) + zoomOffset;

            zoomController.ZoomToObject(this, worldCenter, zoomScale, targetDistance);
        }
        else
        {
            Debug.LogWarning("CameraZoomController non trovato sulla camera selezionata per lo zoom.");
        }
    }

    private Camera ResolveZoomCamera()
    {
        Camera[] sceneCameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Camera sceneCamera in sceneCameras)
        {
            if (!sceneCamera.isActiveAndEnabled)
            {
                continue;
            }

            if (sceneCamera.gameObject.scene != gameObject.scene)
            {
                continue;
            }

            if (sceneCamera.TryGetComponent<InspectionTransition>(out _))
            {
                return sceneCamera;
            }
        }

        if (Camera.main != null && Camera.main.TryGetComponent<InspectionTransition>(out _))
        {
            return Camera.main;
        }

        InspectionTransition fallbackController = FindAnyObjectByType<InspectionTransition>();
        return fallbackController != null ? fallbackController.GetComponent<Camera>() : null;
    }

    public Vector3 OriginalPosition => originalPosition;
    public Vector3 OriginalScale => originalScale;
}
