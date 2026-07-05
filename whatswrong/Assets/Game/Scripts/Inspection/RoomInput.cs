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

        public event Action<InspectionData> InspectRequested;
        public event Action DaddyConversationRequested;

        private CursorManager _cursorManager;
        private bool _roomInputEnabled;
        private bool _isHovering;

        private void Awake()
        {
            _cursorManager = FindAnyObjectByType<CursorManager>();
            _cursorManager.SetDefaultCursor();
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

            if (_isHovering)
                _cursorManager.SetHoverCursor();
            else
                _cursorManager.SetDefaultCursor();

        }

        private void OnDisable()
        {
            SetCursor(false);
        }
    }
}