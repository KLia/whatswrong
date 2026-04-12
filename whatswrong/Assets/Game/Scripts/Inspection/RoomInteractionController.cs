using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class RoomInteractionController : MonoBehaviour
    {
        [SerializeField] private Camera roomCamera;
        [SerializeField] private LayerMask inspectableLayerMask;

        public event Action<string> InspectRequested;
        public event Action OutspectRequested;

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            Vector2 screenPosition = mouse.position.ReadValue();
            Vector3 worldPosition3 = roomCamera.ScreenToWorldPoint(screenPosition);
            Vector2 worldPosition2 = new Vector2(worldPosition3.x, worldPosition3.y);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition2, inspectableLayerMask);

            if (hit == null)
            {
                OutspectRequested?.Invoke();
                return;
            }

            InspectableHotspot hotspot = hit.GetComponent<InspectableHotspot>();
            if (hotspot == null)
            {
                OutspectRequested?.Invoke();
                return;
            }

            InspectRequested?.Invoke(hotspot.ObjectId);
        }
    }
}