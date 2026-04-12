using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class RoomInteractionController : MonoBehaviour
    {
        [SerializeField] private Camera roomCamera;
        [SerializeField] private LayerMask inspectableLayerMask;

        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D hoverCursor;
        [SerializeField] private Vector2 hotspot = Vector2.zero;

        public event Action<InspectionData> InspectRequested;
        public event Action OutspectRequested;

        private bool inspecting;
        private bool isHovering;

        private void Awake()
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }

        public void SetInteractionEnabled(bool enabled)
        {
            inspecting = !enabled;

            if (inspecting)
            {
                SetCursor(false);
            }
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            Vector2 screenPosition = mouse.position.ReadValue();
            Vector3 worldPosition3 = roomCamera.ScreenToWorldPoint(screenPosition);
            Vector2 worldPosition2 = new Vector2(worldPosition3.x, worldPosition3.y);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition2, inspectableLayerMask);

            bool hoveringInspectable = false;
            InspectableHotspot hotspotComponent = null;

            if (!inspecting && hit != null)
            {
                hotspotComponent = hit.GetComponent<InspectableHotspot>();
                hoveringInspectable = hotspotComponent != null;
            }

            SetCursor(hoveringInspectable);

            if (!mouse.leftButton.wasPressedThisFrame)
                return;

            if (inspecting)
            {
                OutspectRequested?.Invoke();
                return;
            }

            if (hotspotComponent != null)
            {
                InspectRequested?.Invoke(hotspotComponent.Data);
                return;
            }

            OutspectRequested?.Invoke();
        }

        private void SetCursor(bool hovering)
        {
            if (isHovering == hovering)
                return;

            isHovering = hovering;

            Cursor.SetCursor(
                hovering ? hoverCursor : defaultCursor,
                hotspot,
                CursorMode.Auto);
        }

        private void OnDisable()
        {
            SetCursor(false);
        }
    }
}