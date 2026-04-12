using System;
using Game.Scripts.DaddyInteraction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class RoomInput : MonoBehaviour
    {
        [SerializeField] private Camera roomCamera;
        [SerializeField] private LayerMask inspectableLayerMask;
        [SerializeField] private LayerMask daddyLayerMask;

        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D hoverCursor;
        [SerializeField] private Vector2 hotspot = Vector2.zero;

        public event Action<InspectionData> InspectRequested;
        public event Action DaddyConversationRequested;

        private bool _roomInputEnabled;
        private bool _isHovering;

        private void Awake()
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }

        public void SetInteractionEnabled(bool enabled)
        {
            _roomInputEnabled = enabled;

            if (!_roomInputEnabled)
            {
                SetCursor(false);
            }
        }

        private void Update()
        {
            if (_roomInputEnabled == false)
            {
                Debug.Log("RoomInput disabled.");
                return;
            }
            
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            var collider = GetCollider(mouse.position.ReadValue());
            SetCursor(collider != null);

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (collider == null)
                {
                    return;
                }

                var inspectionTrigger = collider.GetComponent<InspectionTrigger>();
                if (inspectionTrigger != null)
                {
                    InspectRequested?.Invoke(inspectionTrigger.Data);
                    return;
                }

                var daddyInteractionTrigger = collider.GetComponent<DaddyInteractionTrigger>();
                if (daddyInteractionTrigger != null)
                {
                    DaddyConversationRequested?.Invoke();
                }
            }
        }

        private Collider2D GetCollider(Vector2 mousePosition)
        {
            Vector3 mouseWorldPosition3 = roomCamera.ScreenToWorldPoint(mousePosition);
            Vector2 mouseWorldPosition2 = new Vector2(mouseWorldPosition3.x, mouseWorldPosition3.y);

            return Physics2D.OverlapPoint(mouseWorldPosition2, inspectableLayerMask);
        }

        private void SetCursor(bool hovering)
        {
            if (_isHovering == hovering)
                return;

            _isHovering = hovering;

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