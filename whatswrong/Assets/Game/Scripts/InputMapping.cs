using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    public class InputMapping : MonoBehaviour
    {
        public event Action NextRoom;
        public event Action PreviousRoom;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard.leftArrowKey.wasPressedThisFrame)
            {
                PreviousRoom?.Invoke();
            }
            else if (keyboard.rightArrowKey.wasPressedThisFrame)
            {
                NextRoom?.Invoke();
            }
        }

        public void ResolveDependency()
        {
            throw new NotImplementedException();
        }
    }
}