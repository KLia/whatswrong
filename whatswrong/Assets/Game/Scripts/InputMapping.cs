using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class InputMapping : MonoBehaviour
    {
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
                Vector2 pos = Mouse.current.position.ReadValue();

                if (pos.x < Screen.width * 0.5f)
                    OnLeftClick();
                else
                    OnRightClick();
            }
        }

    }
}