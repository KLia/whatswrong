using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Game.Scripts
{
    public class InputMapping : MonoBehaviour
    {
        [SerializeField] private NavigationControls navigationControls;
        public event Action NextRoom;
        public event Action PreviousRoom;

        public event Action MoveLeftStarted;
        public event Action MoveLeftStopped;

        public event Action MoveRightStarted;
        public event Action MoveRightStopped;

        private bool _leftWasPressed;
        private bool _rightWasPressed;

        private void OnLeftClick()
        {
            PreviousRoom?.Invoke();
        }

        private void OnRightClick()
        {
            NextRoom?.Invoke();
        }
        
        private void RightArrowReleased()
        {
            MoveRightStopped?.Invoke();
        }

        private void RightArrowPressed()
        {
            MoveRightStarted?.Invoke();
        }

        private void LeftArrowReleased()
        {
            MoveLeftStopped?.Invoke();
        }

        private void LeftArrowPressed()
        {
            MoveLeftStarted?.Invoke();
        }

        private void Start()
        {
            if (navigationControls == null)
                return;

            navigationControls.NextButtonPressed += OnRightClick;
            navigationControls.PreviousButtonPressed += OnLeftClick;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            bool leftPressed = keyboard.leftArrowKey.isPressed;
            bool rightPressed = keyboard.rightArrowKey.isPressed;

            if (leftPressed && !_leftWasPressed)
                LeftArrowPressed();

            if (!leftPressed && _leftWasPressed)
                LeftArrowReleased();

            if (rightPressed && !_rightWasPressed)
                RightArrowPressed();

            if (!rightPressed && _rightWasPressed)
                RightArrowReleased();

            _leftWasPressed = leftPressed;
            _rightWasPressed = rightPressed;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!ShouldHandleRoomNavigationClick())
                    return;

                Vector2 pos = Mouse.current.position.ReadValue();

                if (pos.x < Screen.width * 0.5f)
                    OnLeftClick();
                else
                    OnRightClick();
            }
        }

        private bool ShouldHandleRoomNavigationClick()
        {
            if (RuntimeDialogueUI.IsBlockingInput)
                return false;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return false;

            Camera targetCamera = ResolveGameplayCamera();
            if (targetCamera == null || Mouse.current == null)
                return true;

            Ray ray = targetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return true;

            if (hit.collider.GetComponentInParent<DialogueInteractable>() != null)
                return false;

            if (hit.collider.GetComponentInParent<ClickableObject>() != null)
                return false;

            return true;
        }

        private Camera ResolveGameplayCamera()
        {
            CameraZoomController zoomController = FindFirstObjectByType<CameraZoomController>();
            if (zoomController != null)
            {
                Camera zoomCamera = zoomController.GetComponent<Camera>();
                if (zoomCamera != null && zoomCamera.isActiveAndEnabled)
                    return zoomCamera;
            }

            if (Camera.main != null && Camera.main.isActiveAndEnabled)
                return Camera.main;

            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Camera sceneCamera in cameras)
            {
                if (sceneCamera.isActiveAndEnabled)
                    return sceneCamera;
            }

            return null;
        }

        private void OnDestroy()
        {
            if (navigationControls == null)
                return;

            navigationControls.NextButtonPressed -= OnRightClick;
            navigationControls.PreviousButtonPressed -= OnLeftClick;
        }

    }
}
