using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class NavigationInput : MonoBehaviour
    {
        [SerializeField] private NavigationControls navigationControls;
        public event Action NextRoom;
        public event Action PreviousRoom;

        public event Action MoveLeftStarted;
        public event Action MoveLeftStopped;

        public event Action MoveRightStarted;
        public event Action MoveRightStopped;

        public event Action DragStarted;
        public event Action<float> DragDelta;
        public event Action DragEnded;


        private CursorManager _cursorManager;
        
        private bool _leftWasPressed;
        private bool _rightWasPressed;
        private bool _isDragging;
        private Vector2 _lastPointerPosition;
        private const float DragThreshold = 5f;

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

        private void OnDragStarted()
        {
            DragStarted?.Invoke();
        }

        private void OnDragDelta(float deltaX)
        {
            _cursorManager.SetDragCursor();
            DragDelta?.Invoke(deltaX);
        }

        private void OnDragEnded()
        {
            _cursorManager.SetDefaultCursor();
            DragEnded?.Invoke();
        }

        private void Awake()
        {
            _cursorManager = FindObjectOfType<CursorManager>();
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
            HandleKeyboardInput();
            HandlePointerDrag();
        }

        private void HandlePointerDrag()
        {
            Pointer pointer = Pointer.current;
            if (pointer == null)
                return;

            bool pressed = pointer.press.isPressed;
            Vector2 position = pointer.position.ReadValue();

            if (pressed)
            {
                if (!_isDragging)
                {
                    _isDragging = true;
                    _lastPointerPosition = position;
                    OnDragStarted();
                    return;
                }

                float deltaX = position.x - _lastPointerPosition.x;

                if (Mathf.Abs(deltaX) > DragThreshold)
                {
                    OnDragDelta(deltaX);
                }

                _lastPointerPosition = position;
            }
            else if (_isDragging)
            {
                _isDragging = false;
                OnDragEnded();
            }
        }

        private void HandleKeyboardInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
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
            }
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